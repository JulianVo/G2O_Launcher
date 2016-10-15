//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ViewModelBase.cs" company="Gothic Online Project">
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
namespace G2O_Launcher.ViewModels
{
    #region

    using System;
    using System.ComponentModel;

    using G2O_Launcher.Localization;
    using G2O_Launcher.Properties;

    #endregion

    /// <summary>
    ///     Base class for all observable types.
    /// </summary>
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class. 
        /// </summary>
        /// <param name="res">The instance of the resource manager that should be used to provide resource strings for the view.</param>
        protected ViewModelBase(ResourceManager res)
        {
            if (res == null)
            {
                throw new ArgumentNullException(nameof(res));
            }
            this.Res = res;
        }

        /// <summary>
        ///     Called when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Invokes the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property that has changed.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(propertyName));
            }

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the instance of the resource manager that should be used to provide resource strings for the view.
        /// </summary>
        public ResourceManager Res { get; }
    }
}