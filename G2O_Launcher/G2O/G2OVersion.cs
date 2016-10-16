//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="G2OVersion.cs" company="Gothic Online Project">
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
    ///     Describes the local version of the gothic 2 online multiplayer.
    /// </summary>
    public struct G2OVersion
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="G2OVersion" /> struct.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch number.</param>
        /// <param name="build">The build number.</param>
        public G2OVersion(int major, int minor, int patch, int build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Build = build;
        }

        /// <summary>
        ///     Gets the major version number.
        /// </summary>
        public int Major { get; }

        /// <summary>
        ///     Gets the minor version number.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        ///     Gets the patch number.
        /// </summary>
        public int Patch { get; }

        /// <summary>
        ///     Gets the Build number.
        /// </summary>
        public int Build { get; }

        /// <summary>
        ///     Gets a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return $"{this.Major}.{this.Minor}.{this.Patch}.{this.Build}";
        }
    }
}