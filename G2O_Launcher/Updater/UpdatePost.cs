//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="UpdatePost.cs" company="Gothic Online Project">
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
    /// <summary>
    ///     Class used for serializing the update info request.
    /// </summary>
    internal class UpdatePost
    {
        /// <summary>
        ///     Current build number.
        /// </summary>
        public int build { get; set; } = 0;

        /// <summary>
        ///     Current major version.
        /// </summary>
        public int major { get; set; } = 0;

        /// <summary>
        ///     Current minor version.
        /// </summary>
        public int minor { get; set; } = 0;

        /// <summary>
        ///     Current patch number.
        /// </summary>
        public int patch { get; set; } = 0;
    }
}