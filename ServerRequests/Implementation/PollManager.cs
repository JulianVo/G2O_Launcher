using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace G2O.Launcher.ServerRequests
{
    public class PollManager
    {
        private UdpClient _Client = new UdpClient();
        private readonly Regex _PortRegex = new Regex(@"[:][0-9]{1,5}", RegexOptions.None);
        private readonly List<ServerPoll> _Polls = new List<ServerPoll>();
        private readonly object _Lock = new object();

        public ushort DefaultPort { get; }

        public PollManager(ushort defaultPort)
        {
            DefaultPort = defaultPort;
        }

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
            if (!ushort.TryParse(portMatch, out port))
            {
                port = DefaultPort;
            }

            AddServer(new IPEndPoint(Dns.GetHostAddresses(serverAddress).First(), port));

        }

        public void AddServer(IPEndPoint serverEndPoint)
        {
            if (serverEndPoint == null)
            {
                throw new ArgumentNullException(nameof(serverEndPoint));
            }

            lock (_Lock)
            {
                _Polls.Add(new ServerPoll(serverEndPoint.Address, (ushort)serverEndPoint.Port));
            }
        }

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

        public void Reset()
        {
            lock (_Lock)
            {
                _Polls.Clear();
            }
        }

        public void PollAll()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                List<ServerPoll> polls;
                lock (_Lock)
                {
                    polls= new List<ServerPoll>(_Polls);
                }

                UdpClient client = new UdpClient(IPAddress.Any.AddressFamily);

                foreach (ServerPoll serverPoll in polls)
                {
                 


                }
            });
        }

        private void SendPoll(IPEndPoint destination)
        {
            
        }
    }
}
