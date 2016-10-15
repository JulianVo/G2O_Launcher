//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ZipExtractor.cs" company="Gothic Online Project">
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
namespace G2O_Launcher.Updater
{
    using System;
    using System.IO;

    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    ///     Provides for extracting zip files.
    ///     <remarks>Uses the ICSharpCode.SharpZipLib library.</remarks>
    /// </summary>
    internal class ZipExtractor
    {
        /// <summary>
        ///     Extracts a zip file to the given directory path.
        /// </summary>
        /// <param name="archiveFilename">The path of the zip file.</param>
        /// <param name="path">Path of the directory to which the zip file should be extracted</param>
        public void Extract(string archiveFilename, string path)
        {
            if (string.IsNullOrEmpty(archiveFilename))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(archiveFilename));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(path));
            }

            if (!File.Exists(archiveFilename))
            {
                throw new FileNotFoundException("The archive could not be found", archiveFilename);
            }

            ZipFile zipFile = new ZipFile(archiveFilename);
            try
            {
                foreach (ZipEntry entry in zipFile)
                {
                    if (entry.IsFile)
                    {
                        string outputName = Path.Combine(path, entry.Name);

                        using (FileStream fileStream = File.Create(outputName))
                        {
                            zipFile.GetInputStream(entry).CopyTo(fileStream);
                        }
                    }
                    else if (entry.IsDirectory)
                    {
                        string newDictionary = Path.GetDirectoryName(entry.Name);
                        if (newDictionary != null)
                        {
                            Directory.CreateDirectory(Path.Combine(path, newDictionary));
                        }
                    }
                }
            }
            finally
            {
                zipFile.Close();
            }
        }
    }
}