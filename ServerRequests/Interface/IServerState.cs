// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="IServerState.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O.Launcher.ServerRequests
{
    #region

    using System;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;

    #endregion

    /// <summary>
    ///     Provides data about the status of a GO server.
    /// </summary>
    public interface IServerState
    {
        /// <summary>
        ///     Gets the <see cref="ServerInfo" /> object for the target server, if available
        ///     <remarks>Returns null if no response was received from the server.</remarks>
        /// </summary>
        IServerInfo Info { get; }

        /// <summary>
        ///     Gets the last ping value.
        ///     <remarks>Returns -1 if no response was received from the server.</remarks>
        /// </summary>
        int LastPing { get; }

        /// <summary>
        ///     Gets the last time a ping was send to the server.
        /// </summary>
        DateTime LastPingTime { get; }

        /// <summary>
        ///     Gets the last time a ping reply was received from the server.
        /// </summary>
        DateTime LastSuccessfullPingTime { get; }

        /// <summary>
        ///     Gets a value indicating whether a valid response was received from the server.
        /// </summary>
        bool PingSuccessfull { get; }

        /// <summary>
        ///     Gets the <see cref="IPAddress" /> of the server host.
        /// </summary>
        IPAddress ServerIp { get; }

        /// <summary>
        ///     Gets the server port.
        /// </summary>
        ushort ServerPort { get; }
    }
}