//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="UpdateErrorEventArgs.cs" company="Gothic Online Project">
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
    #region

    using System;

    #endregion

    /// <summary>
    ///     Stores information about an error that occured while updating.
    /// </summary>
    public class UpdateErrorEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateErrorEventArgs" /> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public UpdateErrorEventArgs(string errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        ///     Gets the error message.
        /// </summary>
        public string ErrorMessage { get; }
    }
}