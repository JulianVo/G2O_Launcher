//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MainWindowViewModel.cs" company="Gothic Online Project">
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
    using System.Windows;

    using G2O_Launcher.Commands;
    using G2O_Launcher.Config;
    using G2O_Launcher.G2O;

    #endregion

    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        ///     The current launcher config.
        /// </summary>
        private readonly ILauncherConfig config;

        /// <summary>
        ///     Object used to access the gothic 2 online registry values.
        /// </summary>
        private readonly RegistryConfig registry;

        /// <summary>
        ///     The exit command
        /// </summary>
        private ProxyCommand exitCommand;

        /// <summary>
        ///     The minimize command.
        /// </summary>
        private ProxyCommand minimizeCommand;

        /// <summary>
        ///     The selected tab index.
        /// </summary>
        private int selectedTabIndex;

        /// <summary>
        ///     The current window state.
        /// </summary>
        private WindowState windowState;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <param name="config">The current launcher config,</param>
        /// <param name="registry">The registry config object.</param>
        public MainWindowViewModel(ILauncherConfig config, RegistryConfig registry)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            this.config = config;
            this.registry = registry;
            this.SelectedTabIndex = config.SelectedTabIndex > 0 ? config.SelectedTabIndex : 0;
        }

        /// <summary>
        ///     Gets the exit command.
        /// </summary>
        public ProxyCommand ExitCommand => this.exitCommand ?? (this.exitCommand = new ProxyCommand(ExecuteExitCommand))
            ;

        /// <summary>
        ///     Gets the minimizes command.
        /// </summary>
        public ProxyCommand MinimizeCommand
            => this.minimizeCommand ?? (this.minimizeCommand = new ProxyCommand(this.ExecuteMinimizeCommand));

        /// <summary>
        ///     Gets or sets the Nickname value.
        /// </summary>
        public string Nickname
        {
            get
            {
                return this.registry.Nickname;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.registry.Nickname = value;
                this.OnPropertyChanged(nameof(this.Nickname));
            }
        }

        /// <summary>
        ///     Gets or sets the selected tab index.
        /// </summary>
        public int SelectedTabIndex
        {
            get
            {
                return this.selectedTabIndex;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                this.selectedTabIndex = value;
                this.OnPropertyChanged(nameof(this.SelectedTabIndex));
                this.config.SelectedTabIndex = value;
            }
        }

        /// <summary>
        ///     Gets or sets the current window state.
        /// </summary>
        public WindowState WindowState
        {
            get
            {
                return this.windowState;
            }

            set
            {
                if (!Enum.IsDefined(typeof(WindowState), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(WindowState));
                }

                this.windowState = value;
                this.OnPropertyChanged(nameof(this.WindowState));
            }
        }

        /// <summary>
        ///     Executes the exit command.
        /// </summary>
        /// <param name="param">not used</param>
        private static void ExecuteExitCommand(object param)
        {
            Environment.Exit(0);
        }

        /// <summary>
        ///     Executes the minimize command.
        /// </summary>
        /// <param name="param">not used.</param>
        private void ExecuteMinimizeCommand(object param)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}