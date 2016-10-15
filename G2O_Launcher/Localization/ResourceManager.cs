//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ResourceManager.cs" company="Gothic Online Project">
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
namespace G2O_Launcher.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    using G2O_Launcher.Properties;

    /// <summary>
    ///     A class that loads an provides localized resource strings.
    /// </summary>
    internal class ResourceManager
    {
        /// <summary>
        ///     A dictionary that stores all loaded cultures resources grouped by their culture name and resource key.
        /// </summary>
        private readonly Dictionary<CultureInfo, Dictionary<string, string>> localizationDictionary =
            new Dictionary<CultureInfo, Dictionary<string, string>>();

        /// <summary>
        ///     A dictionary that stores all <see cref="DynamicStringResource" /> created by this <see cref="ResourceManager" />
        ///     together with the belonging resource key.
        /// </summary>
        private readonly Dictionary<string, DynamicStringResource> resourceObjs =
            new Dictionary<string, DynamicStringResource>();

        /// <summary>
        ///     The backing field of the <see cref="CurrentCulture" /> property.
        /// </summary>
        private CultureInfo currentCulture;

        /// <summary>
        ///     Backing field of the <see cref="FallbackCulture" /> property.
        /// </summary>
        private CultureInfo fallbackCulture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResourceManager" /> class.
        /// </summary>
        /// <param name="culture">The starting culture of the <see cref="ResourceManager" />.</param>
        /// <param name="fallBack">The fallback culture.</param>
        public ResourceManager(CultureInfo culture, [NotNull] CultureInfo fallBack)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            if (fallBack == null)
            {
                throw new ArgumentNullException(nameof(fallBack));
            }

            this.CurrentCulture = culture;
            this.FallbackCulture = fallBack;

            string[] definedCultures = { "en-US", "de-DE" };
            foreach (var definedCulture in definedCultures)
            {
                this.ReadLocalization(CultureInfo.GetCultureInfo(definedCulture));
            }
        }

        /// <summary>
        ///     Gets or sets the current culture of the <see cref="ResourceManager" />.
        ///     <remarks>If this value changes all <see cref="DynamicStringResource" /> get updated with the new culture.</remarks>
        /// </summary>
        [NotNull]
        public CultureInfo CurrentCulture
        {
            get
            {
                return this.currentCulture;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (Equals(value, this.currentCulture))
                {
                    return;
                }

                this.currentCulture = value;
                foreach (var key in this.resourceObjs.Keys)
                {
                    string locValue;
                    if (!this.localizationDictionary[this.currentCulture].TryGetValue(key, out locValue))
                    {
                        // Try to get the fallback value for the resource key.
                        if (!this.localizationDictionary[this.fallbackCulture].TryGetValue(key, out locValue))
                        {
                            throw new ArgumentException(
                                @"The a string resource with the given key does not exist for the given culture", 
                                nameof(locValue));
                        }
                    }

                    this.resourceObjs[key].Value = locValue;
                }
            }
        }

        /// <summary>
        ///     Gets or set the fallback culture.
        ///     <remarks>This culture will be used if there is no localized value for the <see cref="CurrentCulture" />.</remarks>
        /// </summary>
        [NotNull]
        public CultureInfo FallbackCulture
        {
            get
            {
                return this.fallbackCulture;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (Equals(value, this.fallbackCulture))
                {
                    return;
                }

                this.fallbackCulture = value;
            }
        }

        /// <summary>
        ///     Gets the <see cref="DynamicStringResource" /> object for the resource with the given key.
        /// </summary>
        /// <param name="key">The resource key for which the resource should be returned.</param>
        /// <returns>The <see cref="DynamicStringResource" /> object.</returns>
        public DynamicStringResource this[string key]
        {
            get
            {
                DynamicStringResource result;
                if (!this.resourceObjs.TryGetValue(key, out result))
                {
                    string value;

                    if (!this.localizationDictionary.ContainsKey(this.currentCulture)
                        || !this.localizationDictionary[this.currentCulture].TryGetValue(key, out value))
                    {
                        // Try to get the fallback value for the resource key.
                        if (!this.localizationDictionary[this.fallbackCulture].TryGetValue(key, out value))
                        {
                            throw new ArgumentException(
                                @"The a string resource with the given key does not exist", 
                                nameof(value));
                        }
                    }

                    result = new DynamicStringResource(value);
                    this.resourceObjs.Add(key, result);
                }

                return result;
            }
        }

        /// <summary>
        ///     Reads a localization file.
        /// </summary>
        /// <param name="cultureInfo">Tje culture info of the localization file(also its file name)</param>
        private void ReadLocalization(CultureInfo cultureInfo)
        {
            using (
                var stream =
                    Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream($"G2O_Launcher.Localization.{cultureInfo.Name}.xml"))
            {
                if (stream == null)
                {
                    throw new ArgumentException(
                        @"A localization file with for the given language code was not found", 
                        nameof(cultureInfo));
                }

                if (this.localizationDictionary.ContainsKey(cultureInfo))
                {
                    throw new ArgumentException(@"The given culture is already loaded", nameof(cultureInfo));
                }

                this.localizationDictionary.Add(cultureInfo, new Dictionary<string, string>());

                // Read the localization file.
                TextReader tr = new StreamReader(stream);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(tr.ReadToEnd());
                XmlNode rootNode = doc.SelectSingleNode("./Localization");

                // Check if the root node does exist.
                if (rootNode == null)
                {
                    throw new XmlException("Root node 'Localization' was not found");
                }

                string culture = rootNode.Attributes?.GetNamedItem("culture")?.Value;
                if (string.IsNullOrEmpty(culture) || culture != cultureInfo.Name)
                {
                    throw new XmlException("The culure attribute is null or does not equal the file name.");
                }

                // Check if there are any enties.
                XmlNodeList entries = rootNode.SelectNodes("./Entry");
                if (entries == null)
                {
                    throw new XmlException("No entries found.");
                }

                // Read the entries.
                foreach (XmlNode entry in entries)
                {
                    string key = entry.SelectSingleNode("./Key")?.InnerText;
                    string value = entry.SelectSingleNode("./Value")?.InnerText;

                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                    {
                        throw new XmlException("The entry does not contain a valid key value pair");
                    }

                    // There should not be duplicate key. But lets check this just in case.
                    if (!this.localizationDictionary[cultureInfo].ContainsKey(key))
                    {
                        this.localizationDictionary[cultureInfo].Add(key, value);
                    }
                }
            }
        }
    }
}