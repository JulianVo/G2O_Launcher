// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ServerInfo.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O.Launcher.ServerRequests
{
    #region

    using System;

    #endregion

    /// <summary>
    ///     Provides information about a server.
    /// </summary>
    public class ServerInfo : IServerInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerInfo" /> class.
        /// </summary>
        /// <param name="major">The minor part of the server version.</param>
        /// <param name="minor">The minor part of the server version.</param>
        /// <param name="patch"> The patch part of the server version.</param>
        /// <param name="players">The current count of players that are currently connected to the server.</param>
        /// <param name="maxPlayers">The maximum of players that can join the server at the same time.</param>
        /// <param name="serverName"> The host name of the server.</param>
        public ServerInfo(int major, int minor, int patch, int players, int maxPlayers, string serverName)
        {
            if (serverName == null)
            {
                throw new ArgumentNullException(nameof(serverName));
            }

            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Players = players;
            this.MaxPlayers = maxPlayers;
            this.ServerName = serverName;
        }

        /// <summary>
        ///     Gets the host name of the server.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        ///     Gets the major part of the server version.
        /// </summary>
        public int Major { get; }

        /// <summary>
        ///     Gets the maximum of players that can join the server at the same time.
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        ///     Gets the minor part of the server version.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        ///     Gets the patch part of the server version.
        /// </summary>
        public int Patch { get; }

        /// <summary>
        ///     Gets the current count of players that are currently connected to the server.
        /// </summary>
        public int Players { get; }

        /// <summary>
        ///     Gets the full server version string.
        /// </summary>
        public string VersionString => $"{this.Major}.{this.Minor}.{this.Patch}";

        /// <summary>
        /// Gets the players count string.
        /// </summary>
        public string PlayersString => $"{this.Players}/{this.MaxPlayers}";

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />;
        ///     otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((ServerInfo)obj);
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Major;
                hashCode = (hashCode * 397) ^ this.Minor;
                hashCode = (hashCode * 397) ^ this.Patch;
                hashCode = (hashCode * 397) ^ this.Players;
                hashCode = (hashCode * 397) ^ this.MaxPlayers;
                hashCode = (hashCode * 397) ^ (this.ServerName?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"{this.ServerName}[{this.Players}/{this.MaxPlayers}]";
        }

        /// <summary>Determines whether the specified <see cref="ServerInfo" /> is equal to the current <see cref="ServerInfo" />.</summary>
        /// <param name="other"></param>
        /// <returns>True if the objects are equal.</returns>
        protected bool Equals(ServerInfo other)
        {
            return this.Major == other.Major && this.Minor == other.Minor && this.Patch == other.Patch
                   && this.Players == other.Players && this.MaxPlayers == other.MaxPlayers
                   && string.Equals(this.ServerName, other.ServerName);
        }
    }
}