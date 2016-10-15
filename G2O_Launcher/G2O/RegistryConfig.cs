//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="RegistryConfig.cs" company="Gothic Online Project">
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

    using Microsoft.Win32;

    #endregion

    /// <summary>
    ///     Provides access to the gothic 2 online registry config values.
    /// </summary>
    public class RegistryConfig
    {
        /// <summary>
        ///     Gets the ip port.
        /// </summary>
        public string IpPort
        {
            get
            {
                return (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\G2O", "ip_port", null);
            }

            set
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\G2O", "ip_port", value);
            }
        }

        /// <summary>
        ///     Gets the nickname.
        /// </summary>
        public string Nickname
        {
            get
            {
                return (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\G2O", "nickname", "Nickname");
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Registry.SetValue("HKEY_CURRENT_USER\\Software\\G2O", "nickname", value);
            }
        }
    }
}