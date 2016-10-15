//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="LauncherConfig.cs" company="Gothic Online Project">
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
namespace G2O_Launcher.Config
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    #endregion

    /// <summary>
    ///     The serializable launcher config.
    /// </summary>
    [Serializable]
    public class LauncherConfig : ILauncherConfig
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LauncherConfig" /> class.
        /// </summary>
        public LauncherConfig()
        {
            if (this.FavoriteServers == null)
            {
                this.FavoriteServers = new List<string>();
            }
        }

        /// <summary>
        ///     Gets or sets the favorite servers list.
        /// </summary>
        public List<string> FavoriteServers { get; set; }

        /// <summary>
        ///     Gets or sets the selected language.
        /// </summary>
        public string SelectedLanguage { get; set; }

        /// <summary>
        ///     Gets or sets the selected tab index.
        /// </summary>
        public int SelectedTabIndex { get; set; }

        /// <summary>
        ///     Loads a config instance from a xml file.
        /// </summary>
        /// <param name="path">The path to the config file.</param>
        /// <returns>The loaded <see cref="ILauncherConfig" /> instance.</returns>
        public static ILauncherConfig LoadConfig(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The launcher config file could not be found", path);
            }

            using (Stream file = new FileStream(path, FileMode.Open))
            {
                TextReader textReader = new StreamReader(file, Encoding.Unicode);
                XmlReader xmlReader = new XmlTextReader(textReader);
                var serializer = new XmlSerializer(typeof(LauncherConfig));
                return serializer.Deserialize(xmlReader) as ILauncherConfig;
            }
        }

        /// <summary>
        ///     Saves the current config instance to a file.
        /// </summary>
        /// <param name="path">The path of the file to which the config should be written.</param>
        public void SaveConfig(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(path));
            }

            // Create the config directory if it doesnt exist yet.
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (Stream file = new FileStream(path, FileMode.Create))
            {
                using (TextWriter textWriter = new StreamWriter(file, Encoding.Unicode))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings() { Indent = true })
                        )
                    {
                        var serializer = new XmlSerializer(typeof(LauncherConfig));
                        serializer.Serialize(xmlWriter, this);
                    }
                }
            }
        }
    }
}