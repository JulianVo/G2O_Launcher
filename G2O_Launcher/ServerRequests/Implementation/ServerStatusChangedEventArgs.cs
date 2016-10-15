//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ServerStatusChangedEventArgs.cs" company="Gothic Online Project">
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
namespace G2O_Launcher.ServerRequests.Implementation
{
    #region

    using System;

    using G2O_Launcher.ServerRequests.Interface;

    #endregion

    /// <summary>
    ///     Class that contains information about the changed status of a listened server.
    /// </summary>
    public class ServerStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerStatusChangedEventArgs" />.
        /// </summary>
        /// <param name="changedStatus">The <see cref="IServerState" /> that has changed.</param>
        public ServerStatusChangedEventArgs(IServerState changedStatus)
        {
            this.ChangedStatus = changedStatus;
        }

        /// <summary>
        ///     Gets the changed <see cref="IServerState" />.
        /// </summary>
        public IServerState ChangedStatus { get; }
    }
}