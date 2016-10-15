//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DynamicStringResource.cs" company="Gothic Online Project">
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
    using System.ComponentModel;

    using G2O_Launcher.Properties;

    /// <summary>
    ///     Class that encapsulates a string value and provides a PropertyChanged
    ///     event which allows the to bind and react to changes of the encapsulated value.
    /// </summary>
    internal class DynamicStringResource : INotifyPropertyChanged
    {
        /// <summary>
        ///     Backing field for the <see cref="Value" /> property.
        /// </summary>
        private string value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicStringResource" /> class.
        /// </summary>
        /// <param name="value"></param>
        public DynamicStringResource([NotNull] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(value));
            }

            this.value = value;
        }

        /// <summary>
        ///     Invokes all registered handlers if the value of the <see cref="DynamicStringResource" /> changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets or sets the value of the <see cref="DynamicStringResource" />.
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Value)));
            }
        }
    }
}