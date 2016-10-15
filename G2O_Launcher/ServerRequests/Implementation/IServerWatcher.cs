// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="IServerWatcher.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.ServerRequests.Implementation
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Net;

    using G2O_Launcher.ServerRequests.Interface;

    #endregion

    /// <summary>
    ///     The ServerWatcher interface.
    /// </summary>
    public interface IServerWatcher
    {
        /// <summary>
        ///     Calls all registered handlers if the status of a watched server has changed.
        /// </summary>
        event EventHandler<ServerStatusChangedEventArgs> ServerStatusChanged;

        /// <summary>
        ///     Gets or sets the default port.
        /// </summary>
        ushort DefaultPort { get; set; }

        /// <summary>
        ///     Gets or sets the ping interval.
        /// </summary>
        int PingInternal { get; set; }

        /// <summary>
        ///     Gets or sets the poll interval.
        /// </summary>
        int PollInterval { get; set; }

        /// <summary>
        ///     Gets or sets the server timeout.
        /// </summary>
        int ServerTimeout { get; set; }

        /// <summary>
        ///     Gets the watched servers.
        /// </summary>
        IEnumerable<IServerState> WatchedServers { get; }

        /// <summary>
        /// Adds a server to the <see cref="IServerWatcher"/>.
        /// </summary>
        /// <param name="serverAddress">
        /// The server address.
        /// </param>
        /// <returns>
        /// The state object of the new server.
        /// </returns>
        IServerState AddServer(string serverAddress);

        /// <summary>
        /// Adds a server to the <see cref="IServerWatcher"/>.
        /// </summary>
        /// <param name="serverEndPoint">
        /// The server end point.
        /// </param>
        /// <returns>
        ///  The state object of the new server.
        /// </returns>
        IServerState AddServer(IPEndPoint serverEndPoint);

        /// <summary>
        /// Removes a server from the <see cref="IServerWatcher"/>.
        /// </summary>
        /// <param name="server">The server state object that describes the server that should be removed.</param>
        void RemoveServer(IServerState server);

        /// <summary>
        ///     Removes all watched servers from the <see cref="IServerWatcher" />.
        /// </summary>
        void Reset();

        /// <summary>
        ///     Starts the <see cref="IServerWatcher" />.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops the <see cref="IServerWatcher" />.
        /// </summary>
        void Stop();
    }
}