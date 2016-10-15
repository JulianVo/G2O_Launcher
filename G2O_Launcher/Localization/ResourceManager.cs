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

    using G2O_Launcher.Annotations;

    internal class ResourceManager
    {
        private readonly Dictionary<CultureInfo, Dictionary<string, string>> LocalizationDictionary =
            new Dictionary<CultureInfo, Dictionary<string, string>>();

        private readonly Dictionary<string, DynamicStringResource> ResourceObjs =
            new Dictionary<string, DynamicStringResource>();

        private CultureInfo currentCulture;

        private CultureInfo fallbackCulture;

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

            string[] definedCultures = { "EN-us" };
            foreach (var definedCulture in definedCultures)
            {
                this.ReadLocalization(CultureInfo.GetCultureInfo(definedCulture));
            }
        }

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
                foreach (var key in this.ResourceObjs.Keys)
                {
                    string locValue;
                    if (!this.LocalizationDictionary[this.currentCulture].TryGetValue(key, out locValue))
                    {
                        // Try to get the fallback value for the resource key.
                        if (!this.LocalizationDictionary[this.fallbackCulture].TryGetValue(key, out locValue))
                        {
                            throw new ArgumentException(
                                @"The a string resource with the given key does not exist for the given culture",
                                nameof(locValue));
                        }
                    }

                    this.ResourceObjs[key].Value = locValue;
                }
            }
        }

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

        public DynamicStringResource this[string key]
        {
            get
            {
                DynamicStringResource result;
                if (!this.ResourceObjs.TryGetValue(key, out result))
                {
                    string value;

                    if (this.LocalizationDictionary.ContainsKey(this.currentCulture) && !this.LocalizationDictionary[this.currentCulture].TryGetValue(key, out value))
                    {
                        // Try to get the fallback value for the resource key.
                        if (!this.LocalizationDictionary[this.fallbackCulture].TryGetValue(key, out value))
                        {
                            throw new ArgumentException(
                                @"The a string resource with the given key does not exist",
                                nameof(value));
                        }
                    }
                    //Current culture does not exist.
                    else
                    {
                        // Try to get the fallback value for the resource key.
                        if (!this.LocalizationDictionary[this.fallbackCulture].TryGetValue(key, out value))
                        {
                            throw new ArgumentException(
                                @"The a string resource with the given key does not exist",
                                nameof(value));
                        }
                    }

                    result = new DynamicStringResource(key, value);
                    this.ResourceObjs.Add(key, result);
                }

                return result;
            }
        }

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

                if (this.LocalizationDictionary.ContainsKey(cultureInfo))
                {
                    throw new ArgumentException(@"The given culture is already loaded", nameof(cultureInfo));
                }

                this.LocalizationDictionary.Add(cultureInfo, new Dictionary<string, string>());

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

                    //There should not be duplicate key. But lets check this just in case.
                    if (!this.LocalizationDictionary[cultureInfo].ContainsKey(key))
                    {
                        this.LocalizationDictionary[cultureInfo].Add(key, value);
                    }
                }
            }
        }
    }
}