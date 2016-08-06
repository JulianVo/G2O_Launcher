using System;
using System.Net;

namespace G2O.Launcher.ServerRequests
{
    /// <summary>
    ///     Provides data about the status of a GO server.
    /// </summary>
    public class ServerState : IServerState
    {
        #region fields

        /// <summary>
        ///     Lock object used for synchronizing the access to the object members.
        /// </summary>
        private readonly object _Lock = new object();

        /// <summary>
        ///     Backing field for the ServerIp property.
        /// </summary>
        private readonly IPAddress _ServerIp;

        /// <summary>
        ///     Backing field for the ServerPort property.
        /// </summary>
        private readonly ushort _ServerPort;

        /// <summary>
        ///     Backing field for the Info property.
        /// </summary>
        private ServerInfo _Info;

        /// <summary>
        ///     Backing field for the LastPing property.
        /// </summary>
        private int _LastPing;

        /// <summary>
        ///     Backing field for the LastPingTime property.
        /// </summary>
        private DateTime _LastPingTime;

        /// <summary>
        ///     Backing field for the LastSuccessfullPingTime property.
        /// </summary>
        private DateTime _LastSuccessfullPingTime;

        /// <summary>
        ///     Backing field for the PingSuccessfull property.
        /// </summary>
        private bool _PingSuccessfull;

        #endregion

        #region constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerState" />.
        /// </summary>
        /// <param name="serverIp">The ip the server which should be watched.</param>
        /// <param name="serverPort">The port of the server that should be watched.</param>
        public ServerState(IPAddress serverIp, ushort serverPort)
        {
            if (serverIp == null)
            {
                throw new ArgumentNullException(nameof(serverIp));
            }
            _ServerIp = serverIp;
            _ServerPort = serverPort;
        }

        #endregion

        /// <summary>
        ///     Get the <see cref="IPAddress" /> of the server host.
        /// </summary>
        public IPAddress ServerIp
        {
            get
            {
                lock (_Lock)
                {
                    return _ServerIp;
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
                lock (_Lock)
                {
                    return _ServerPort;
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
                lock (_Lock)
                {
                    return _LastPing;
                }
            }
            internal set
            {
                lock (_Lock)
                {
                    _LastPing = value;
                }
            }
        }

        /// <summary>
        ///     Gets a value that indicates if a valid response was received from the server.
        /// </summary>
        public bool PingSuccessfull
        {
            get
            {
                lock (_Lock)
                {
                    return _PingSuccessfull;
                }
            }
            internal set
            {
                lock (_Lock)
                {
                    _PingSuccessfull = value;
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
                lock (_Lock)
                {
                    return _LastPingTime;
                }
            }
            internal set
            {
                lock (_Lock)
                {
                    _LastPingTime = value;
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
                lock (_Lock)
                {
                    return _LastSuccessfullPingTime;
                }
            }
            internal set
            {
                lock (_Lock)
                {
                    _LastSuccessfullPingTime = value;
                }
            }
        }

        /// <summary>
        ///     Gets the <see cref="ServerInfo" /> object for the target server, if available
        ///     <remarks>Returns null if no response was received from the server.</remarks>
        /// </summary>
        public ServerInfo Info
        {
            get
            {
                lock (_Lock)
                {
                    return _Info;
                }
            }
            internal set
            {
                lock (_Lock)
                {
                    _Info = value;
                }
            }
        }
    }
}