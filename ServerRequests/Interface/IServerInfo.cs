// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="IServerInfo.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O.Launcher.ServerRequests
{
    /// <summary>
    ///     Provides information about a server.
    /// </summary>
    public interface IServerInfo
    {
        /// <summary>
        ///     Gets the host name of the server.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        ///     Gets the major part of the server version.
        /// </summary>
        int Major { get; }

        /// <summary>
        ///     Gets the maximum of players that can join the server at the same time.
        /// </summary>
        int MaxPlayers { get; }

        /// <summary>
        ///     Gets the minor part of the server version.
        /// </summary>
        int Minor { get; }

        /// <summary>
        ///     Gets the patch part of the server version.
        /// </summary>
        int Patch { get; }

        /// <summary>
        ///     Gets the current count of players that are currently connected to the server.
        /// </summary>
        int Players { get; }

        /// <summary>
        ///     Gets the full server version string.
        /// </summary>
        string VersionString { get; }

        /// <summary>
        /// Gets the players count string.
        /// </summary>
        string PlayersString { get; }
    }
}