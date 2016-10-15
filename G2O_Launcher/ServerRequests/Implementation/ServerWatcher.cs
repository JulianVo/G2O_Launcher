// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ServerWatcher.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.ServerRequests.Implementation
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    using G2O_Launcher.ServerRequests.Interface;

    #endregion

    /// <summary>
    ///     A class that manages the ping and info requests that are send to a list of servers.
    /// </summary>
    public class ServerWatcher : IServerWatcher
    {
        /// <summary>
        ///     The identifier string of a G2 ping.
        /// </summary>
        private const string PingIdentifier = "GOp";

        /// <summary>
        ///     The identifier string of a G2 info packets(poll).
        /// </summary>
        private const string PollIdentifier = "GOi";

        /// <summary>
        ///     Lock object used to synchronize the access to object fields.
        /// </summary>
        private readonly object instanceLock = new object();

        /// <summary>
        ///     Stores the byte of a GO ping.
        /// </summary>
        private readonly byte[] pingBytes;

        /// <summary>
        ///     Stores the bytes of a info(poll) packet.
        /// </summary>
        private readonly byte[] pollBytes;

        /// <summary>
        ///     List of server states that a kept refreshed by <see cref="ServerWatcher" />.
        /// </summary>
        private readonly List<ServerState> watchedServers = new List<ServerState>();

        /// <summary>
        ///     Regex for finding the port number in a connection string.
        /// </summary>
        private readonly Regex portRegex = new Regex(@"[:][0-9]{1,5}", RegexOptions.None);

        /// <summary>
        ///     The port to which the requests should be send if no port is contained in connection string.
        /// </summary>
        private ushort defaultPort;

        /// <summary>
        ///     Last time the pings where send to the servers.
        /// </summary>
        private DateTime lastPingTime = DateTime.Now;

        /// <summary>
        ///     The interval[in ms] at which ping should be send to the servers.
        /// </summary>
        private int pingInterval;

        /// <summary>
        ///     The interval[in ms] at which info serverStates should be send to the servers.
        /// </summary>
        private int pollInterval;

        /// <summary>
        ///     The time in after which a server should be detected as not available[in ms].
        /// </summary>
        private int serverTimeout;

        /// <summary>
        ///     The usedSocket used to send and receive.
        /// </summary>
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        /// <summary>
        ///     The background thread used for polling and sending pings.
        /// </summary>
        private Thread watcherThread;

        /// <summary>
        ///     Indicates wheter the PollThread should run or not.
        /// </summary>
        private bool watcherThreadRun;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerWatcher" /> class.
        /// </summary>
        /// <param name="defaultPort">The port to which the requests should be send if no port is contained in connection string.</param>
        /// <param name="pingInterval">The interval[in ms] at which ping should be send to the servers.</param>
        /// <param name="pollInterval">The interval[in ms] at which info serverStates should be send to the servers.</param>
        /// <param name="serverTimeout">The time in after which a server should be detected as not available[in ms].</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value of a parameter.</exception>
        public ServerWatcher(ushort defaultPort, int pingInterval, int pollInterval, int serverTimeout)
        {
            if (pingInterval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pingInterval));
            }

            if (pollInterval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pollInterval));
            }

            if (serverTimeout <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(serverTimeout));
            }

            if (pollInterval <= pingInterval)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pollInterval),
                    "The poll interval needs to be greater than the ping interval");
            }

            this.pingInterval = pingInterval;
            this.pollInterval = pollInterval;
            this.serverTimeout = serverTimeout;
            this.defaultPort = defaultPort;
            this.pollBytes = Encoding.ASCII.GetBytes(PollIdentifier);
            this.pingBytes = Encoding.ASCII.GetBytes(PingIdentifier);
            this.socket.DontFragment = false;
            this.socket.Blocking = false;
        }

        /// <summary>
        ///     Called whenever the status of a in the list changes.
        /// </summary>
        public event EventHandler<ServerStatusChangedEventArgs> ServerStatusChanged;

        /// <summary>
        ///     Gets or sets the default port.
        /// </summary>
        public ushort DefaultPort
        {
            get
            {
                lock (this.instanceLock)
                {
                    return this.defaultPort;
                }
            }

            set
            {
                lock (this.instanceLock)
                {
                    this.defaultPort = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the interval[in ms] at which pings should be send to the servers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value set is 0 or lower.</exception>
        public int PingInternal
        {
            get
            {
                lock (this.instanceLock)
                {
                    return this.pingInterval;
                }
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (this.pollInterval <= value)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        "The ping interval needs to be lower than the poll interval");
                }

                lock (this.instanceLock)
                {
                    this.pingInterval = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the interval[in ms] at which info serverStates should be send to the servers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value set is 0 or lower.</exception>
        public int PollInterval
        {
            get
            {
                return this.pollInterval;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (value <= this.pingInterval)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        "The poll interval needs to be greater than the ping interval");
                }

                lock (this.instanceLock)
                {
                    this.pollInterval = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the time in after which a server should be detected as not available[in ms].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value set is 0 or lower.</exception>
        public int ServerTimeout
        {
            get
            {
                lock (this.instanceLock)
                {
                    return this.serverTimeout;
                }
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                lock (this.instanceLock)
                {
                    this.serverTimeout = value;
                }
            }
        }

        /// <summary>
        ///     Gets a enumerable of <see cref="IServerState" /> that provide information about the watched servers and their
        ///     status.
        /// </summary>
        public IEnumerable<IServerState> WatchedServers
        {
            get
            {
                lock (this.instanceLock)
                {
                    return this.watchedServers.ToArray();
                }
            }
        }

        /// <summary>
        ///     Adds a server to the <see cref="ServerWatcher" />.
        /// </summary>
        /// <param name="serverAddress">The address of the server that should be watched.</param>
        /// <returns> The state object of the new server.</returns>
        /// <exception cref="IOException">Thrown if the given address can not be resolved.</exception>
        /// <exception cref="ArgumentException">Thrown if the serverAddress argument is null,empty or whitespace.</exception>
        public IServerState AddServer(string serverAddress)
        {
            if (string.IsNullOrEmpty(serverAddress))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(serverAddress));
            }

            if (string.IsNullOrWhiteSpace(serverAddress))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serverAddress));
            }
            serverAddress = serverAddress.Trim();
            string portMatch = this.portRegex.Match(serverAddress).ToString();

            ushort port;
            if (string.IsNullOrEmpty(portMatch) || !ushort.TryParse(portMatch.Remove(0, 1), out port))
            {
                port = this.DefaultPort;
            }
            else
            {
                serverAddress = serverAddress.Replace($":{port}", string.Empty);
            }

            IPAddress serverIp =
                Dns.GetHostAddresses(serverAddress).FirstOrDefault(ad => ad.AddressFamily == AddressFamily.InterNetwork);

            if (serverIp == null)
            {
                throw new IOException($"Can not resolve host name '{serverAddress}' to a IPv4 address.");
            }

            var state = this.AddServer(new IPEndPoint(serverIp, port));
            ((ServerState)state).OriginalAddress = serverAddress;
            return state;
        }

        /// <summary>
        ///     Adds a server to the <see cref="ServerWatcher" />.
        /// </summary>
        /// <param name="serverEndPoint">The <see cref="IPEndPoint" /> of the server that should be watched.</param>
        /// <returns> The state object of the new server.</returns>
        /// <exception cref="ArgumentException">Thrown if the serverEndPoint parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the given endpoint is no valid IPv4 endpoint.</exception>
        public IServerState AddServer(IPEndPoint serverEndPoint)
        {
            if (serverEndPoint == null)
            {
                throw new ArgumentNullException(nameof(serverEndPoint));
            }

            if (serverEndPoint.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException(
                    "The argument does not contain a valid IPv4 address",
                    nameof(serverEndPoint));
            }

            lock (this.instanceLock)
            {
                var newServer = new ServerState(serverEndPoint.Address, (ushort)serverEndPoint.Port, serverEndPoint.ToString());
                this.watchedServers.Add(newServer);
                return newServer;
            }
        }

        /// <summary>
        /// Removes a server from the <see cref="IServerWatcher"/>.
        /// </summary>
        /// <param name="server">The server state object that describes the server that should be removed.</param>
        public void RemoveServer(IServerState server)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }
            lock (this.instanceLock)
            {
                ServerState state = server as ServerState;
                if (state != null && this.watchedServers.Contains(state))
                {
                    this.watchedServers.Remove(state);
                }
            }
        }

        /// <summary>
        ///     Removes all servers from the watchlist.
        /// </summary>
        public void Reset()
        {
            lock (this.instanceLock)
            {
                this.watchedServers.Clear();
            }
        }

        /// <summary>
        ///     Starts the <see cref="ServerWatcher" />.
        /// </summary>
        public void Start()
        {
            if (!this.watcherThreadRun)
            {
                this.watcherThreadRun = true;
                this.watcherThread = new Thread(this.WatcherThreadFunction) { IsBackground = true };
                this.watcherThread.Start();
            }
        }

        /// <summary>
        ///     Stops the <see cref="ServerWatcher" />.
        /// </summary>
        public void Stop()
        {
            if (this.watcherThreadRun)
            {
                this.watcherThreadRun = false;
                this.watcherThread.Join();
            }
        }

        /// <summary>
        ///     Handles a received packet if its a ping reply.
        /// </summary>
        /// <param name="sender">The sender of the packet.</param>
        /// <param name="data">The received data.</param>
        /// <returns>True if the packet could be handled by this method.</returns>
        private bool CheckHandlePingReply(IPEndPoint sender, byte[] data)
        {
            // Invalid ping reply
            if (data.Length <= 3)
            {
                return false;
            }

            string msg = Encoding.ASCII.GetString(data);

            // Invalid identifier
            if (!msg.StartsWith(PingIdentifier))
            {
                return false;
            }

            ServerState state;
            lock (this.instanceLock)
            {
                state = this.watchedServers.FirstOrDefault(p => sender.Address.Equals(p.ServerIp) && sender.Port == p.ServerPort);
            }

            // Calcualte new ping
            if (state != null)
            {
                bool changed = !state.PingSuccessfull;
                var newPing = (int)(DateTime.Now - state.LastPingTime).TotalMilliseconds;
                changed = changed || (state.LastPing != newPing);

                state.LastSuccessfullPingTime = DateTime.Now;
                state.LastPing = newPing;
                state.PingSuccessfull = true;

                // Call the event on change.
                if (changed)
                {
                    this.InvokeOnServerStatusChanged(state);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Does everything related tho receiving the response packets.
        /// </summary>
        private void DoReceiving()
        {
            EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            var buffer = new byte[this.socket.Available];

            try
            {
                this.socket.ReceiveFrom(buffer, buffer.Length, SocketFlags.None, ref sender);
            }
            catch (SocketException ex)
            {
                // For some reason this error is thrown.
                if (ex.ErrorCode != 10054)
                {
                    this.socket.Dispose();
                    this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    this.socket.DontFragment = false;
                    this.socket.Blocking = false;
                }
            }

            // If its no ping its maybe a state reply.
            if (!this.CheckHandlePingReply(sender as IPEndPoint, buffer))
            {
                this.HandlePollReply(sender as IPEndPoint, buffer);
            }
        }

        /// <summary>
        ///     Does everything related to sending serverStates and pings.
        /// </summary>
        /// <param name="serverStates">The list of server states.</param>
        /// <param name="sendPoll">True if serverStates should be send. false if only ping should be send.</param>
        private void DoSending(IEnumerable<ServerState> serverStates, bool sendPoll)
        {
            foreach (ServerState serverPoll in serverStates)
            {
                if (serverPoll.PingSuccessfull
                    && (DateTime.Now - serverPoll.LastSuccessfullPingTime).TotalMilliseconds
                    > this.PollInterval + this.ServerTimeout)
                {
                    // Server timed out.
                    serverPoll.LastPing = (int)(DateTime.Now - serverPoll.LastSuccessfullPingTime).TotalMilliseconds;
                    serverPoll.PingSuccessfull = false;
                    serverPoll.Info = null;
                    this.InvokeOnServerStatusChanged(serverPoll);
                }

                this.SendPing(new IPEndPoint(serverPoll.ServerIp, serverPoll.ServerPort), this.socket);
                serverPoll.LastPingTime = DateTime.Now;
                if (sendPoll)
                {
                    this.SendPoll(new IPEndPoint(serverPoll.ServerIp, serverPoll.ServerPort), this.socket);
                }
            }

            this.lastPingTime = DateTime.Now;
        }

        /// <summary>
        ///     Handles a received packet if its a info poll reply.
        /// </summary>
        /// <param name="sender">The sender of the packet.</param>
        /// <param name="data">The received data.</param>
        private void HandlePollReply(IPEndPoint sender, byte[] data)
        {
            // Invalid reply.
            if (data.Length < 9)
            {
                return;
            }

            string msg = Encoding.ASCII.GetString(data);

            // Invalid identifier
            if (!msg.StartsWith(PollIdentifier))
            {
                return;
            }

            // Invalid version
            if (msg[3] != 0x1)
            {
                return;
            }

            int major = msg[4];
            int minor = msg[5];
            int patch = msg[6];
            int players = msg[7];
            int maxPlayers = msg[8];
            string hostName = msg.Substring(9, data.Length - 9);
            hostName = hostName.Replace("\0", string.Empty);

            ServerState state;
            lock (this.instanceLock)
            {
                state = this.watchedServers.FirstOrDefault(p => sender.Address.Equals(p.ServerIp) && sender.Port == p.ServerPort);
            }

            if (state != null)
            {
                var info = new ServerInfo(major, minor, patch, players, maxPlayers, hostName);
                if (state.Info == null || !state.Info.Equals(info))
                {
                    state.Info = info;
                    this.InvokeOnServerStatusChanged(state);
                }
            }
        }

        /// <summary>
        ///     Invokes the ServerStatusChanged event using the <see cref="ThreadPool" />.
        /// </summary>
        /// <param name="state">The changed server state</param>
        private void InvokeOnServerStatusChanged(ServerState state)
        {
            // Invoke the event on the thread pool
            ThreadPool.QueueUserWorkItem(
                delegate { this.ServerStatusChanged?.Invoke(this, new ServerStatusChangedEventArgs(state)); });
        }

        /// <summary>
        ///     Sends a ping to the given server endpoint using a given usedSocket.
        /// </summary>
        /// <param name="destination">The <see cref="IPEndPoint" /> to which the ping hould be send.</param>
        /// <param name="usedSocket">The usedSocket that should be used to send the ping.</param>
        private void SendPing(IPEndPoint destination, Socket usedSocket)
        {
            usedSocket.SendTo(this.pingBytes, this.pingBytes.Length, SocketFlags.None, destination);
        }

        /// <summary>
        ///     Sends a info poll to the given server endpoint using a given usedSocket.
        /// </summary>
        /// <param name="destination">The <see cref="IPEndPoint" /> to which the info poll should be send.</param>
        /// <param name="usedSocket">The usedSocket that should be used to send the info poll.</param>
        private void SendPoll(IPEndPoint destination, Socket usedSocket)
        {
            usedSocket.SendTo(this.pollBytes, this.pollBytes.Length, SocketFlags.None, destination);
        }

        /// <summary>
        ///     The watcher thread.
        /// </summary>
        private void WatcherThreadFunction()
        {
            while (this.watcherThreadRun)
            {
                List<ServerState> serverStates;
                lock (this.instanceLock)
                {
                    serverStates = new List<ServerState>(this.watchedServers);
                }

                bool sendPoll = (DateTime.Now - this.lastPingTime).TotalMilliseconds >= this.PingInternal;

                // Send the serverStates
                this.DoSending(serverStates, sendPoll);

                // Wait for answers until the time for the next state has come.
                while ((DateTime.Now - this.lastPingTime).TotalMilliseconds < this.PingInternal)
                {
                    if (this.socket.Available > 1)
                    {
                        this.DoReceiving();
                    }
                    else
                    {
                        // Prevent unnecessary processor usage.
                        Thread.Sleep(1);
                    }
                }
            }
        }
    }
}