//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ServerState.cs" company="Gothic Online Project">
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
    using System.Net;

    using G2O_Launcher.ServerRequests.Interface;

    #endregion

    /// <summary>
    ///     Provides data about the status of a GO server.
    /// </summary>
    public class ServerState : IServerState
    {
        /// <summary>
        ///     Lock object used for synchronizing the access to the object members.
        /// </summary>
        private readonly object lockObject = new object();

        /// <summary>
        ///     The server <see cref="IPAddress" />.
        /// </summary>
        private readonly IPAddress serverIpAddress;

        /// <summary>
        ///     Backing field for the ServerPort property.
        /// </summary>
        private readonly ushort serverPort;

        /// <summary>
        ///     Backing field for the Info property.
        /// </summary>
        private IServerInfo info;

        /// <summary>
        ///     Backing field for the LastPing property.
        /// </summary>
        private int lastPing;

        /// <summary>
        ///     Backing field for the LastPingTime property.
        /// </summary>
        private DateTime lastPingTime;

        /// <summary>
        ///     The last successful ping time.
        /// </summary>
        private DateTime lastSuccessfullPingTime;

        /// <summary>
        ///     The last ping was successful.
        /// </summary>
        private bool pingSuccessful;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerState" /> class.
        /// </summary>
        /// <param name="serverIp">The <see cref="IPAddress" /> which should be watched.</param>
        /// <param name="serverPort">The port of the server that should be watched.</param>
        /// <param name="originalAddress">The string that was used to add the server to the server watcher.</param>
        public ServerState(IPAddress serverIp, ushort serverPort, string originalAddress)
        {
            if (serverIp == null)
            {
                throw new ArgumentNullException(nameof(serverIp));
            }

            if (string.IsNullOrEmpty(originalAddress))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(originalAddress));
            }

            this.serverIpAddress = serverIp;
            this.serverPort = serverPort;
            this.OriginalAddress = originalAddress;
        }

        /// <summary>
        ///     Gets the <see cref="ServerInfo" /> object for the target server, if available
        ///     <remarks>Returns null if no response was received from the server.</remarks>
        /// </summary>
        public IServerInfo Info
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.info;
                }
            }

            internal set
            {
                lock (this.lockObject)
                {
                    this.info = value;
                }
            }
        }

        /// <summary>
        ///     Gets the last ping value.
        ///     <remarks>Returns -1 if no response was received from the server.</remarks>
        /// </summary>
        public int LastPing
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.lastPing;
                }
            }

            internal set
            {
                lock (this.lockObject)
                {
                    this.lastPing = value;
                }
            }
        }

        /// <summary>
        ///     Gets the last time a ping was send to the server.
        /// </summary>
        public DateTime LastPingTime
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.lastPingTime;
                }
            }

            internal set
            {
                lock (this.lockObject)
                {
                    this.lastPingTime = value;
                }
            }
        }

        /// <summary>
        ///     Gets the last time a ping reply was received from the server.
        /// </summary>
        public DateTime LastSuccessfullPingTime
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.lastSuccessfullPingTime;
                }
            }

            internal set
            {
                lock (this.lockObject)
                {
                    this.lastSuccessfullPingTime = value;
                }
            }
        }

        /// <summary>
        ///     Gets the string that was used to add the server to the server watcher.
        /// </summary>
        public string OriginalAddress { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether a valid response was received from the server.
        /// </summary>
        public bool PingSuccessfull
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.pingSuccessful;
                }
            }

            internal set
            {
                lock (this.lockObject)
                {
                    this.pingSuccessful = value;
                }
            }
        }

        /// <summary>
        ///     Gets the <see cref="IPAddress" /> of the server host.
        /// </summary>
        public IPAddress ServerIp
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.serverIpAddress;
                }
            }
        }

        /// <summary>
        ///     Gets the server port.
        /// </summary>
        public ushort ServerPort
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.serverPort;
                }
            }
        }
    }
}