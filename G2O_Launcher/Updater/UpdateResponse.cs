//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="UpdateResponse.cs" company="Gothic Online Project">
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
    ///     Class that is used for deserializing the response to the update request.
    /// </summary>
    public class UpdateResponse
    {
        /// <summary>
        ///     Status code.
        /// </summary>
        public int Code { get; set; } = -1;

        /// <summary>
        ///     Link to the new update file.
        /// </summary>
        public string Link { get; set; }
    }
}