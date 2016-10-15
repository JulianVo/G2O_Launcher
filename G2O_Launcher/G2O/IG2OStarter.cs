//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IG2OStarter.cs" company="Gothic Online Project">
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
        /// <param name="versionMajor">
        ///     The target server major version number.
        /// </param>
        /// <param name="versionMinor">
        ///     The target server minor version number.
        /// </param>
        /// <param name="patchNr">
        ///     The target server patch number.
        /// </param>
        /// <param name="ipPort">
        ///     The server ip and port string.
        /// </param>
        /// <returns>
        ///     The <see cref="G2OProxy.RunResult" />.
        /// </returns>
        G2OProxy.RunResult Start(int versionMajor, int versionMinor, int patchNr, string ipPort);
    }
}