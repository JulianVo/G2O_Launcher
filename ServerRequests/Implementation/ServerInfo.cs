using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G2O.Launcher.ServerRequests
{
    public class ServerInfo : IServerInfo
    {
        public ServerInfo(int major, int minor, int patch, int players, int maxPlayers, string hostName)
        {
            if (hostName == null)
            {
                throw new ArgumentNullException(nameof(hostName));
            }

            Major = major;
            Minor = minor;
            Patch = patch;
            Players = players;
            MaxPlayers = maxPlayers;
            HostName = hostName;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Players { get; }
        public int MaxPlayers { get; }
        public string HostName { get; }


        public string VersionString => $"{Major}.{Minor}.{Patch}";
    }
}
