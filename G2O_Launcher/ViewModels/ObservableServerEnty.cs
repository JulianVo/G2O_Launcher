// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ObservableServerEnty.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.ViewModels
{
    using System;

    using G2O.Launcher.ServerRequests;

    /// <summary>
    ///     ViewModel class for server list entries.
    /// </summary>
    internal class ObservableServerEnty : NotifyPropertyChangedBase
    {
        /// <summary>
        ///     The host name.
        /// </summary>
        private string hostName;

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
        /// Initializes a new instance of the <see cref="ObservableServerEnty"/> class.
        /// </summary>
        /// <param name="serverState">The state object for the displayed server.</param>
        public ObservableServerEnty(IServerState serverState)
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
        /// Gets the ServerState.
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
        /// Updates the server enty values from the related state object.
        /// </summary>
        public void UpdateFromState()
        {
            this.HostName = $"{this.ServerState.ServerIp}:{this.ServerState.ServerPort}";
            this.ServerName = this.ServerState.Info?.ServerName ?? "-";
            this.Version = this.ServerState.Info?.VersionString ?? "-";
            this.PlayerCount = this.ServerState.Info?.PlayersString ?? "-";
        }
    }
}