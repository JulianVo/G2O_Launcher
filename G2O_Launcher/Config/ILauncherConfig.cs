// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ILauncherConfig.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Config
{
    #region

    using System.Collections.Generic;

    #endregion

    /// <summary>
    ///     The LauncherConfig interface.
    /// </summary>
    public interface ILauncherConfig
    {
        /// <summary>
        ///     Gets the favorite servers list.
        /// </summary>
        List<string> FavoriteServers { get; }

        /// <summary>
        ///     Gets or sets the selected language.
        /// </summary>
        string SelectedLanguage { get; set; }

        /// <summary>
        ///     Gets or sets the selected tab index.
        /// </summary>
        int SelectedTabIndex { get; set; }
    }
}