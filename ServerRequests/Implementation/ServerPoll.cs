using System;
using System.Net;

namespace G2O.Launcher.ServerRequests
{
    public class ServerPoll : IServerPoll
    {
        public ServerPoll(IPAddress serverIp, ushort serverPort)
        {
            if (serverIp == null)
            {
                throw new ArgumentNullException(nameof(serverIp));
            }
            ServerIp = serverIp;
            ServerPort = serverPort;
        }

        public IPAddress ServerIp { get; }
        public ushort ServerPort { get; }
        public int LastPing { get; internal set; }
        public bool PollSuccessfull { get; internal set; }
        public DateTime LastPollTime { get; internal set; }
        public ServerInfo Info { get; internal set; }
    }
}