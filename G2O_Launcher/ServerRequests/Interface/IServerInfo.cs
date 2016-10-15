//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IServerInfo.cs" company="Gothic Online Project">
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
    /// <summary>
    ///     Provides information about a server.
    /// </summary>
    public interface IServerInfo
    {
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
        ///     Gets the players count string.
        /// </summary>
        string PlayersString { get; }

        /// <summary>
        ///     Gets the host name of the server.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        ///     Gets the full server version string.
        /// </summary>
        string VersionString { get; }
    }
}