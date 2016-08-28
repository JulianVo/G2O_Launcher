// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="G2OStarter.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.G2O
{
    #region

    using System;

    #endregion

    /// <summary>
    /// Default implementation of the <see cref="IG2OStarter"/> interface.
    /// </summary>
    public class G2OStarter : IG2OStarter, IDisposable
    {
        /// <summary>
        /// Interface to the g2o proxy module.
        /// </summary>
        private readonly G2OProxy proxy;

        /// <summary>
        /// Object used to access the g2o registry value.
        /// </summary>
        private readonly RegistryConfig registry;

        /// <summary>
        /// Indicates whether this object is disposed or not.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="G2OStarter"/> class.
        /// </summary>
        /// <param name="proxy">The <see cref="G2OProxy"/> instance used to start the client.</param>
        /// <param name="registry">The object used to access the g2o registry values.</param>
        public G2OStarter(G2OProxy proxy, RegistryConfig registry)
        {
            if (proxy == null)
            {
                throw new ArgumentNullException(nameof(proxy));
            }
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }
            this.proxy = proxy;
            this.registry = registry;
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="versionMajor">
        /// The version major.
        /// </param>
        /// <param name="versionMinor">
        /// The version minor.
        /// </param>
        /// <param name="patchNr">
        /// The patch nr.
        /// </param>
        /// <param name="ipAndPort">
        /// The ip port.
        /// </param>
        /// <returns>
        /// The <see cref="G2OProxy.RunResult"/>.
        /// </returns>
        public G2OProxy.RunResult Start(int versionMajor, int versionMinor, int patchNr, string ipAndPort)
        {
            if (versionMajor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(versionMajor));
            }
            if (versionMinor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(versionMinor));
            }
            if (patchNr < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(patchNr));
            }
            if (string.IsNullOrEmpty(ipAndPort))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(ipAndPort));
            }
            this.registry.IpPort = ipAndPort;
            return this.proxy.Run(versionMajor, versionMinor, patchNr);
        }

        /// <summary>
        /// Releases all unmanaged resources related to this object.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.proxy.Dispose();
                this.disposed = true;
            }
        }
    }
}