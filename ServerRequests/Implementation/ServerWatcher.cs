using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace G2O.Launcher.ServerRequests
{
    /// <summary>
    ///     A class that manages the ping and info requests that are send to a list of servers.
    /// </summary>
    public class ServerWatcher
    {
        #region static fields and constants

        /// <summary>
        ///     The identifier string of a G2 ping.
        /// </summary>
        private const string PingIdentifier = "GOp";

        /// <summary>
        ///     The identifier string of a G2 info packets(poll).
        /// </summary>
        private const string PollIdentifier = "GOi";

        #endregion

        #region fields

        /// <summary>
        ///     Lock object used to synchronize the access to object fields.
        /// </summary>
        private readonly object _Lock = new object();

        /// <summary>
        ///     Stores the byte of a GO ping.
        /// </summary>
        private readonly byte[] _PingBytes;


        /// <summary>
        ///     Stores the bytes of a info(poll) packet.
        /// </summary>
        private readonly byte[] _PollBytes;

        /// <summary>
        ///     List of server states that a keept refreshed by <see cref="ServerWatcher" />.
        /// </summary>
        private readonly List<ServerState> _Polls = new List<ServerState>();

        /// <summary>
        ///     Regex for finding the port number in a connection string.
        /// </summary>
        private readonly Regex _PortRegex = new Regex(@"[:][0-9]{1,5}", RegexOptions.None);

        /// <summary>
        ///     The port to which the requests should be send if no port is contained in connection string.
        /// </summary>
        private ushort _DefaultPort;

        /// <summary>
        ///     Last time the pings where send to the servers.
        /// </summary>
        private DateTime _LastPingTime = DateTime.Now;

        /// <summary>
        ///     The interval[in ms] at which ping should be send to the servers.
        /// </summary>
        private int _PingInterval;

        /// <summary>
        ///     The interval[in ms] at which info serverStates should be send to the servers.
        /// </summary>
        private int _PollInterval;

        /// <summary>
        ///     The time in after which a server should be detected as not available[in ms].
        /// </summary>
        private int _ServerTimeout;

        /// <summary>
        ///     The socket used to send and receive.
        /// </summary>
        private Socket _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        /// <summary>
        ///     The background thread used for polling and sending pings.
        /// </summary>
        private Thread _WatcherThread;

        /// <summary>
        ///     Indicates wheter the PollThread should run or not.
        /// </summary>
        private bool _WatcherThreadRun;

        #endregion

        #region constructors

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
                throw new ArgumentOutOfRangeException(nameof(pollInterval), "The poll interval needs to be greater than the ping interval");
            }

            _PingInterval = pingInterval;
            _PollInterval = pollInterval;
            _ServerTimeout = serverTimeout;
            _DefaultPort = defaultPort;
            _PollBytes = Encoding.ASCII.GetBytes(PollIdentifier);
            _PingBytes = Encoding.ASCII.GetBytes(PingIdentifier);
            _Socket.DontFragment = false;
            _Socket.Blocking = false;
        }

        #endregion

        #region properties

        /// <summary>
        ///     Gets or sets the port to which the requests should be send if no port is contained in connection string.
        /// </summary>
        public ushort DefaultPort
        {
            get
            {
                lock (_Lock)
                {
                    return _DefaultPort;
                }
            }
            set
            {
                lock (_Lock)
                {
                    _DefaultPort = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the interval[in ms] at which pings should be send to the servers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value set is 0 or lower.</exception>
        public int PingIntervall
        {
            get
            {
                lock (_Lock)
                {
                    return _PingInterval;
                }
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                if (_PollInterval <= value)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The ping interval needs to be lower than the poll interval");
                }
                lock (_Lock)
                {
                    _PingInterval = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the interval[in ms] at which info serverStates should be send to the servers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value set is 0 or lower.</exception>
        public int PollInterval
        {
            get { return _PollInterval; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                if (value <= _PingInterval)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The poll interval needs to be greater than the ping interval");
                }
                lock (_Lock)
                {
                    _PollInterval = value;
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
                lock (_Lock)
                {
                    return _ServerTimeout;
                }
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                lock (_Lock)
                {
                    _ServerTimeout = value;
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
                lock (_Lock)
                {
                    return _Polls.ToArray();
                }
            }
        }

        #endregion

        #region events

        /// <summary>
        ///     Called whenever the status of a in the list changes.
        /// </summary>
        public event EventHandler<ServerStatusChangedEventArgs> OnServerStatusChanged;

        #endregion

        #region public methods

        /// <summary>
        ///     Adds a server to the <see cref="ServerWatcher" />.
        /// </summary>
        /// <param name="serverAddress">The address of the server that should be watched.</param>
        /// <exception cref="IOException">Thrown if the given address can not be resolved.</exception>
        /// <exception cref="ArgumentException">Thrown if the serverAddress argument is null,empty or whitespace.</exception>
        public void AddServer(string serverAddress)
        {
            if (string.IsNullOrEmpty(serverAddress))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(serverAddress));
            }
            if (string.IsNullOrWhiteSpace(serverAddress))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serverAddress));
            }
            string portMatch = _PortRegex.Match(serverAddress).ToString();

            ushort port;
            if (string.IsNullOrEmpty(portMatch) || !ushort.TryParse(portMatch.Remove(0, 1), out port))
            {
                port = DefaultPort;
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


            AddServer(new IPEndPoint(serverIp, port));
        }

        /// <summary>
        ///     Adds a server to the <see cref="ServerWatcher" />.
        /// </summary>
        /// <param name="serverEndPoint">The <see cref="IPEndPoint" /> of the server that should be watched.</param>
        /// <exception cref="ArgumentException">Thrown if the serverEndPoint parameter is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the given endpoint is no valid IPv4 endpoint.</exception>
        public void AddServer(IPEndPoint serverEndPoint)
        {
            if (serverEndPoint == null)
            {
                throw new ArgumentNullException(nameof(serverEndPoint));
            }
            if (serverEndPoint.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException("The argument does not contain a valid IPv4 address", nameof(serverEndPoint));
            }

            lock (_Lock)
            {
                _Polls.Add(new ServerState(serverEndPoint.Address, (ushort)serverEndPoint.Port));
            }
        }

        /// <summary>
        ///     Removes all servers from the watchlist.
        /// </summary>
        public void Reset()
        {
            lock (_Lock)
            {
                _Polls.Clear();
            }
        }

        /// <summary>
        ///     Starts the <see cref="ServerWatcher" />.
        /// </summary>
        public void Start()
        {
            if (!_WatcherThreadRun)
            {
                _WatcherThreadRun = true;
                _WatcherThread = new Thread(WatcherThreadProc) { IsBackground = true };
                _WatcherThread.Start();
            }
        }

        /// <summary>
        ///     Stops the <see cref="ServerWatcher" />.
        /// </summary>
        public void Stop()
        {
            if (_WatcherThreadRun)
            {
                _WatcherThreadRun = false;
                _WatcherThread.Join();
            }
        }

        #endregion

        #region private methods

        /// <summary>
        ///     Handles a reveived packet if its a ping reply.
        /// </summary>
        /// <param name="sender">The sender of the packet.</param>
        /// <param name="data">The reveived data.</param>
        /// <returns>True if the packet could be handled by this method.</returns>
        private bool CheckHandlePingReply(IPEndPoint sender, byte[] data)
        {
            //Invalid ping reply
            if (data.Length <= 3)
            {
                return false;
            }
            string msg = Encoding.ASCII.GetString(data);

            //Invalid identfier
            if (!msg.StartsWith(PingIdentifier))
            {
                return false;
            }

            ServerState state;
            lock (_Lock)
            {
                state = _Polls.FirstOrDefault(p => sender.Address.Equals(p.ServerIp) && sender.Port == p.ServerPort);
            }
            //Calcualte new ping
            if (state != null)
            {
                bool changed = !state.PingSuccessfull;
                var newPing = (int)(DateTime.Now - state.LastPingTime).TotalMilliseconds;
                changed = changed || (state.LastPing != newPing);

                state.LastSuccessfullPingTime = DateTime.Now;
                state.LastPing = newPing;
                state.PingSuccessfull = true;
                //Call the event on change.
                if (changed)
                {
                    InvokeOnServerStatusChanged(state);
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
            var buffer = new byte[_Socket.Available];

            try
            {
                _Socket.ReceiveFrom(buffer, buffer.Length, SocketFlags.None, ref sender);
            }
            catch (SocketException ex)
            {
                //For some reason this error is thrown.
                if (ex.ErrorCode != 10054)
                {
                    _Socket.Dispose();
                    _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    _Socket.DontFragment = false;
                    _Socket.Blocking = false;
                }
            }

            //If its no ping its maybe a state reply.
            if (!CheckHandlePingReply(sender as IPEndPoint, buffer))
            {
                HandlePollReply(sender as IPEndPoint, buffer);
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
                if (serverPoll.PingSuccessfull &&
                    (DateTime.Now - serverPoll.LastSuccessfullPingTime).TotalMilliseconds >
                    PollInterval + ServerTimeout)
                {
                    //Server timed out.
                    serverPoll.LastPing = (int)(DateTime.Now - serverPoll.LastSuccessfullPingTime).TotalMilliseconds;
                    serverPoll.PingSuccessfull = false;
                    serverPoll.Info = null;
                    InvokeOnServerStatusChanged(serverPoll);
                }
                SendPing(new IPEndPoint(serverPoll.ServerIp, serverPoll.ServerPort), _Socket);
                serverPoll.LastPingTime = DateTime.Now;
                if (sendPoll)
                {
                    SendPoll(new IPEndPoint(serverPoll.ServerIp, serverPoll.ServerPort), _Socket);
                }
            }
            _LastPingTime = DateTime.Now;
        }

        /// <summary>
        ///     Handles a reveived packet if its a info poll reply.
        /// </summary>
        /// <param name="sender">The sender of the packet.</param>
        /// <param name="data">The reveived data.</param>
        private void HandlePollReply(IPEndPoint sender, byte[] data)
        {
            //Invalid reply.
            if (data.Length < 9)
            {
                return;
            }

            string msg = Encoding.ASCII.GetString(data);

            //Invalid identfier
            if (!msg.StartsWith(PollIdentifier))
            {
                return;
            }

            //Invalid version
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
            lock (_Lock)
            {
                state = _Polls.FirstOrDefault(p => sender.Address.Equals(p.ServerIp) && sender.Port == p.ServerPort);
            }
            if (state != null)
            {
                var info = new ServerInfo(major, minor, patch, players, maxPlayers, hostName);
                if (state.Info == null || !state.Info.Equals(info))
                {
                    state.Info = info;
                    InvokeOnServerStatusChanged(state);
                }
            }
        }

        /// <summary>
        ///     Invokes the OnServerStatusChanged event using the <see cref="ThreadPool" />.
        /// </summary>
        /// <param name="state">The changed server state</param>
        private void InvokeOnServerStatusChanged(ServerState state)
        {
            //Invoke the event on the theadpool
            ThreadPool.QueueUserWorkItem(
                delegate { OnServerStatusChanged?.Invoke(this, new ServerStatusChangedEventArgs(state)); });
        }

        /// <summary>
        ///     Sends a ping to the given server endpoint using a given socket.
        /// </summary>
        /// <param name="destination">The <see cref="IPEndPoint" /> to which the ping hould be send.</param>
        /// <param name="socket">The socket that should be used to send the ping.</param>
        private void SendPing(IPEndPoint destination, Socket socket)
        {
            socket.SendTo(_PingBytes, _PingBytes.Length, SocketFlags.None, destination);
        }

        /// <summary>
        ///     Sends a info poll to the given server endpoint using a given socket.
        /// </summary>
        /// <param name="destination">The <see cref="IPEndPoint" /> to which the info poll should be send.</param>
        /// <param name="socket">The socket that should be used to send the info poll.</param>
        private void SendPoll(IPEndPoint destination, Socket socket)
        {
            socket.SendTo(_PollBytes, _PollBytes.Length, SocketFlags.None, destination);
        }

        /// <summary>
        ///     The watcher thread.
        /// </summary>
        private void WatcherThreadProc()
        {
            while (_WatcherThreadRun)
            {
                List<ServerState> polls;
                lock (_Lock)
                {
                    polls = new List<ServerState>(_Polls);
                }

                bool sendPoll = (DateTime.Now - _LastPingTime).TotalMilliseconds >= PingIntervall;

                //Send the serverStates
                DoSending(polls, sendPoll);


                //Wait for answers until the time for the next state has come.
                while ((DateTime.Now - _LastPingTime).TotalMilliseconds < PingIntervall)
                {
                    if (_Socket.Available > 1)
                    {
                        DoReceiving();
                    }
                    else
                    {
                        //Prevent unnecessary processor usage.
                        Thread.Sleep(1);
                    }
                }
            }
        }

        #endregion
    }
}