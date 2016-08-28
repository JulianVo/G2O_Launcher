// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="RegistryConfig.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
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