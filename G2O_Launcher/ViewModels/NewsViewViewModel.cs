//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="NewsViewViewModel.cs" company="Gothic Online Project">
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
    using System;

    using G2O_Launcher.Localization;
    using G2O_Launcher.Properties;

    /// <summary>
    ///     ViewModel for the news view.
    /// </summary>
    internal class NewsViewViewModel : ViewModelBase
    {
        /// <summary>
        ///     The news text.
        /// </summary>
        private string newsText;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NewsViewViewModel" /> class.
        /// </summary>
        /// <param name="defaultNewsText">The default news text.</param>
        /// <param name="resourceManager">
        ///     The instance of the resource manager that should be used to provide resource strings for
        ///     the view.
        /// </param>
        public NewsViewViewModel([NotNull] string defaultNewsText, ResourceManager resourceManager)
            : base(resourceManager)
        {
            if (string.IsNullOrEmpty(defaultNewsText))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(defaultNewsText));
            }

            this.newsText = defaultNewsText;
        }

        /// <summary>
        ///     Gets or sets the news text.
        /// </summary>
        public string NewsText
        {
            get
            {
                return this.newsText;
            }

            set
            {
                this.newsText = value;
                this.OnPropertyChanged(nameof(this.NewsText));
            }
        }
    }
}