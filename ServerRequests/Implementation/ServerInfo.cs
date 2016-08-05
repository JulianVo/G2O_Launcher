using System;

namespace G2O.Launcher.ServerRequests
{
    public class ServerInfo : IServerInfo
    {
        #region constructors

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

        #endregion

        #region public methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ServerInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Major;
                hashCode = (hashCode*397) ^ Minor;
                hashCode = (hashCode*397) ^ Patch;
                hashCode = (hashCode*397) ^ Players;
                hashCode = (hashCode*397) ^ MaxPlayers;
                hashCode = (hashCode*397) ^ (HostName?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        #endregion

        #region private methods

        protected bool Equals(ServerInfo other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch && Players == other.Players &&
                   MaxPlayers == other.MaxPlayers && string.Equals(HostName, other.HostName);
        }

        #endregion

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Players { get; }
        public int MaxPlayers { get; }
        public string HostName { get; }


        public string VersionString => $"{Major}.{Minor}.{Patch}";

        public override string ToString()
        {
            return $"{HostName}[{Players}/{MaxPlayers}]";
        }
    }
}