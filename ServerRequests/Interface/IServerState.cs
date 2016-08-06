using System;
using System.Net;

namespace G2O.Launcher.ServerRequests
{
    /// <summary>
    ///     Provides data about the status of a GO server.
    /// </summary>
    public interface IServerState
    {
        /// <summary>
        ///     Gets the <see cref="ServerInfo" /> object for the target server, if available
        ///     <remarks>Returns null if no response was received from the server.</remarks>
        /// </summary>
        ServerInfo Info { get; }

        /// <summary>
        ///     Gets the last ping value.
        ///     <remarks>Returns -1 if no response was received from the server.</remarks>
        /// </summary>
        int LastPing { get; }

        /// <summary>
        ///     Gets the last time a ping reply was received from the server.
        /// </summary>
        DateTime LastSuccessfullPingTime { get; }

        /// <summary>
        ///     Gets the last time a ping was send to the server.
        /// </summary>
        DateTime LastPingTime { get; }

        /// <summary>
        ///     Gets a value that indicates if a valid response was received from the server.
        /// </summary>
        bool PingSuccessfull { get; }

        /// <summary>
        ///     Get the <see cref="IPAddress" /> of the server host.
        /// </summary>
        IPAddress ServerIp { get; }

        /// <summary>
        ///     Gets the server port.
        /// </summary>
        ushort ServerPort { get; }
    }
}