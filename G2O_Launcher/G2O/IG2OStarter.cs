// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="IG2OStarter.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.G2O
{
    /// <summary>
    ///     Interface for classes that allow starting the gothic 2 online multiplayer client.
    /// </summary>
    public interface IG2OStarter
    {
        /// <summary>
        ///     Starts the client and connects to specific server.
        /// </summary>
        /// <param name="versionMajor">The target server major version number.</param>
        /// <param name="versionMinor">The target server minor version number.</param>
        /// <param name="patchNr">The target server patch number.</param>
        /// <param name="ipPort">The server ip and port string.</param>
        void Start(int versionMajor, int versionMinor, int patchNr, string ipPort);
    }
}