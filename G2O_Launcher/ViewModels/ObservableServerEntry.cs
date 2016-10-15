//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ObservableServerEntry.cs" company="Gothic Online Project">
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
namespace G2O_Launcher.ViewModels
{
    using System;
    using System.ComponentModel;

    using G2O_Launcher.Annotations;
    using G2O_Launcher.Localization;
    using G2O_Launcher.ServerRequests.Interface;

    /// <summary>
    ///     ViewModel class for server list entries.
    /// </summary>
    internal class ObservableServerEntry : INotifyPropertyChanged
    {
        /// <summary>
        ///     The host name.
        /// </summary>
        private string hostName;

        /// <summary>
        ///     The ping
        /// </summary>
        private int ping;

        /// <summary>
        ///     The player count.
        /// </summary>
        private string playerCount;

        /// <summary>
        ///     The server name.
        /// </summary>
        private string serverName;

        /// <summary>
        ///     The version.
        /// </summary>
        private string version;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableServerEntry" /> class.
        /// </summary>
        /// <param name="serverState">The state object for the displayed server.</param>
        public ObservableServerEntry(IServerState serverState)
        {
            if (serverState == null)
            {
                throw new ArgumentNullException(nameof(serverState));
            }

            this.ServerState = serverState;
            this.UpdateFromState();
        }

        /// <summary>
        ///     Gets or sets the host name.
        /// </summary>
        public string HostName
        {
            get
            {
                return this.hostName;
            }

            set
            {
                this.OnPropertyChanged(nameof(this.HostName));
                this.hostName = value;
            }
        }

        /// <summary>
        ///     Gets or sets the ping.
        /// </summary>
        public int Ping
        {
            get
            {
                return this.ping;
            }

            set
            {
                this.OnPropertyChanged(nameof(this.Ping));
                this.ping = value;
            }
        }

        /// <summary>
        ///     Gets or sets the player count.
        /// </summary>
        public string PlayerCount
        {
            get
            {
                return this.playerCount;
            }

            set
            {
                this.OnPropertyChanged(nameof(this.PlayerCount));
                this.playerCount = value;
            }
        }

        /// <summary>
        ///     Gets or sets the server name.
        /// </summary>
        public string ServerName
        {
            get
            {
                return this.serverName;
            }

            set
            {
                this.serverName = value;
                this.OnPropertyChanged(nameof(this.ServerName));
            }
        }

        /// <summary>
        ///     Gets the ServerState.
        /// </summary>
        public IServerState ServerState { get; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        public string Version
        {
            get
            {
                return this.version;
            }

            set
            {
                this.OnPropertyChanged(nameof(this.Version));
                this.version = value;
            }
        }

        /// <summary>
        ///     Updates the server enty values from the related state object.
        /// </summary>
        public void UpdateFromState()
        {
            this.HostName = this.ServerState.OriginalAddress;
            this.ServerName = this.ServerState.Info?.ServerName ?? "-";
            this.Version = this.ServerState.Info?.VersionString ?? "-";
            this.PlayerCount = this.ServerState.Info?.PlayersString ?? "-";
            this.Ping = this.ServerState.LastPing;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}