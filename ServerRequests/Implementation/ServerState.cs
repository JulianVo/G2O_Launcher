// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ServerState.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O.Launcher.ServerRequests
{
    #region

    using System;
    using System.Net;

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
        /// The server <see cref="IPAddress"/>.
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
        /// The last successful ping time.
        /// </summary>
        private DateTime lastSuccessfullPingTime;

        /// <summary>
        /// The last ping was successful.
        /// </summary>
        private bool pingSuccessful;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerState" /> class.
        /// </summary>
        /// <param name="serverIp">The <see cref="IPAddress"/> which should be watched.</param>
        /// <param name="serverPort">The port of the server that should be watched.</param>
        public ServerState(IPAddress serverIp, ushort serverPort)
        {
            if (serverIp == null)
            {
                throw new ArgumentNullException(nameof(serverIp));
            }

            this.serverIpAddress = serverIp;
            this.serverPort = serverPort;
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