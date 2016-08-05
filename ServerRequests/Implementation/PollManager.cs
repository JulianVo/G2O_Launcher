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
    public class PollManager
    {
        #region static fields and constants

        private const int ServerTimeout = 5000;

        #endregion

        #region fields

        private readonly Socket _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private readonly object _Lock = new object();
        private readonly byte[] _PollBytes;
        private readonly int _PollIntervall;
        private readonly List<ServerPoll> _Polls = new List<ServerPoll>();
        private readonly Regex _PortRegex = new Regex(@"[:][0-9]{1,5}", RegexOptions.None);
        private readonly string _ReplyIdentifier = "GOi";
        private DateTime _LastPoll = DateTime.Now;
        private Thread _PollThread;
        private bool _PollThreadRun;

        #endregion

        #region constructors

        public PollManager(ushort defaultPort, int pollIntervall)
        {
            if (pollIntervall <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pollIntervall));
            }

            _PollIntervall = pollIntervall;
            DefaultPort = defaultPort;
            _PollBytes = Encoding.ASCII.GetBytes("GOi");
            _Socket.DontFragment = false;
            _Socket.Blocking = false;
        }

        #endregion

        #region properties

        public ushort DefaultPort { get; }

        public IEnumerable<IServerPoll> Polls
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

        public event EventHandler<ServerStatusChangedEventArgs> OnServerStatusChanged;

        #endregion

        #region public methods

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
                string temp = serverAddress.Replace($":{port}", string.Empty);

                serverAddress = serverAddress.Replace($":{port}", string.Empty);
            }

            IPAddress serverIp = Dns.GetHostAddresses(serverAddress).FirstOrDefault(ad => ad.AddressFamily == AddressFamily.InterNetwork);

            if (serverIp == null)
            {
                throw new IOException($"Can not resolve host name '{serverAddress}' to a IPv4 address.");
            }


            AddServer(new IPEndPoint(serverIp, port));
        }

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
                _Polls.Add(new ServerPoll(serverEndPoint.Address, (ushort)serverEndPoint.Port));
            }
        }

        public void Reset()
        {
            lock (_Lock)
            {
                _Polls.Clear();
            }
        }

        public void Start()
        {
            if (!_PollThreadRun)
            {
                _PollThreadRun = true;
                _PollThread = new Thread(PollThreadProc) { IsBackground = true };
                _PollThread.Start();
            }
        }

        public void Stop()
        {
            if (_PollThreadRun)
            {
                _PollThreadRun = false;
                _PollThread.Join();
            }
        }

        #endregion

        #region private methods

        private void HandlePollReply(IPEndPoint sender, byte[] data)
        {
            //Invalid reply.
            if (data.Length < 9)
            {
                return;
            }

            string msg = Encoding.ASCII.GetString(data);

            //Invalid identfier
            if (!msg.StartsWith(_ReplyIdentifier))
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

            ServerPoll poll;
            lock (_Lock)
            {
                poll = _Polls.FirstOrDefault(p => sender.Address.Equals(p.ServerIp) && sender.Port == p.ServerPort);
            }
            if (poll != null)
            {
                //Calculate Ping
                var newPing = (int)(DateTime.Now- poll.LastPollTime).TotalMilliseconds;
                bool changed = newPing != poll.LastPing;
                poll.LastPing = newPing;
                poll.PollSuccessfull = true;

                var info = new ServerInfo(major, minor, patch, players, maxPlayers, hostName);


                if (poll.Info == null || !poll.Info.Equals(info))
                {
                    poll.Info = info;
                    changed = true;
                }

                if (changed)
                {
                    InvokeOnServerStatusChanged(poll);
                }
            }
        }

        private void InvokeOnServerStatusChanged(ServerPoll poll)
        {
            //Invoke the event on the theadpool
            ThreadPool.QueueUserWorkItem(
                delegate { OnServerStatusChanged?.Invoke(this, new ServerStatusChangedEventArgs(poll)); });
        }

        private void PollThreadProc()
        {
            while (_PollThreadRun)
            {
                List<ServerPoll> polls;
                lock (_Lock)
                {
                    polls = new List<ServerPoll>(_Polls);
                }

                //Send the polls
                foreach (ServerPoll serverPoll in polls)
                {
                    if (serverPoll.PollSuccessfull &&
                        (DateTime.Now - serverPoll.LastPollTime).TotalMilliseconds > _PollIntervall + ServerTimeout)
                    {
                        //Server timed out.
                        serverPoll.PollSuccessfull = false;
                        InvokeOnServerStatusChanged(serverPoll);
                    }

                    serverPoll.LastPollTime = DateTime.Now;
                    SendPoll(new IPEndPoint(serverPoll.ServerIp, serverPoll.ServerPort), _Socket);
                }

                _LastPoll = DateTime.Now;

                //Wait for answers until the time for the next poll has come.
                while ((DateTime.Now - _LastPoll).TotalMilliseconds < _PollIntervall)
                {
                    if (_Socket.Available > 1)
                    {
                        EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                        byte[] buffer = new byte[_Socket.Available];
                        _Socket.ReceiveFrom(buffer, buffer.Length, SocketFlags.None, ref sender);
                        HandlePollReply(sender as IPEndPoint, buffer);
                    }
                    else
                    {
                        //Prevent unnecessary processor usage.
                        Thread.Sleep(1);
                    }
                }
            }
        }

        private void SendPoll(IPEndPoint destination, Socket client)
        {
            client.SendTo(_PollBytes, _PollBytes.Length, SocketFlags.None, destination);
        }

        #endregion
    }
}