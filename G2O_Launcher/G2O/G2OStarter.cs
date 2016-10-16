//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="G2OStarter.cs" company="Gothic Online Project">
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
    #region

    using System;

    #endregion

    /// <summary>
    ///     Default implementation of the <see cref="IG2OStarter" /> interface.
    /// </summary>
    public class G2OStarter : IG2OStarter
    {
        /// <summary>
        ///     Object used to access the g2o registry value.
        /// </summary>
        private readonly RegistryConfig registry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="G2OStarter" /> class.
        /// </summary>
        /// <param name="registry">The object used to access the g2o registry values.</param>
        public G2OStarter(RegistryConfig registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            this.registry = registry;
        }

        /// <summary>
        ///     The start.
        /// </summary>
        /// <param name="versionMajor">
        ///     The version major.
        /// </param>
        /// <param name="versionMinor">
        ///     The version minor.
        /// </param>
        /// <param name="patchNr">
        ///     The patch nr.
        /// </param>
        /// <param name="ipAndPort">
        ///     The ip port.
        /// </param>
        /// <returns>
        ///     The <see cref="G2OProxy.RunResult" />.
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
            using (G2OProxy proxy = new G2OProxy())
            {
                return proxy.Run(versionMajor, versionMinor, patchNr);
            }
        }
    }
}