//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IServerState.cs" company="Gothic Online Project">
//  Copyright (C) <2016>  <Julian Vogel>
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  -
//  This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
// -
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http:www.gnu.org/licenses/>.
//  </copyright>
//  -------------------------------------------------------------------------------
namespace G2O_Launcher.ServerRequests.Interface
{
    #region

    using System;
    using System.Net;

    using G2O_Launcher.ServerRequests.Implementation;

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
        ///     Gets the string that was used to add the server to the server watcher.
        /// </summary>
        string OriginalAddress { get; }

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