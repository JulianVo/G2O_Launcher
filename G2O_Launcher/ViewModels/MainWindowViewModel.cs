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
    using System.Globalization;
    using System.Net;
    using System.Windows;

    using G2O_Launcher.Commands;
    using G2O_Launcher.Config;
    using G2O_Launcher.G2O;
    using G2O_Launcher.Localization;
    using G2O_Launcher.Updater;

    #endregion

    /// <summary>
    ///     ViewModel class for the main window view.
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
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
        ///     The used instance of the updater.
        /// </summary>
        private readonly Updater updater;

        /// <summary>
        ///     Backing field for the <see cref="ChangeLanguageCommand" /> property.
        /// </summary>
        private RelayCommand changeLanguageCommand;

        /// <summary>
        ///     The exit command
        /// </summary>
        private RelayCommand exitCommand;

        /// <summary>
        ///     The minimize command.
        /// </summary>
        private RelayCommand minimizeCommand;

        /// <summary>
        ///     Backing field for the <see cref="ResetCheckUpdateCommand" /> property.
        /// </summary>
        private RelayCommand resetCheckUpdateCommand;

        /// <summary>
        ///     The selected tab index.
        /// </summary>
        private int selectedTabIndex;

        /// <summary>
        ///     Backing field for the <see cref="UpdateProgress" /> property.
        /// </summary>
        private int updateProgress;

        /// <summary>
        ///     Backing field for the <see cref="UpdateProgressText" /> property.
        /// </summary>
        private string updateProgressText;

        /// <summary>
        ///     Backing field for the <see cref="UpdateRunning" /> property.
        /// </summary>
        private bool updateRunning;

        /// <summary>
        ///     The current window state.
        /// </summary>
        private WindowState windowState;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <param name="config">The current launcher config,</param>
        /// <param name="registry">The registry config object.</param>
        /// <param name="updater">The instance of the <see cref="Updater" /> class that is used to check for and execute updates.</param>
        /// <param name="resourceManager">
        ///     The instance of the resource manager that should be used to provide resource strings for
        ///     the view.
        /// </param>
        public MainWindowViewModel(
            ILauncherConfig config, 
            RegistryConfig registry, 
            Updater updater, 
            ResourceManager resourceManager)
            : base(resourceManager)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (updater == null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            this.config = config;
            this.registry = registry;
            this.SelectedTabIndex = config.SelectedTabIndex > 0 ? config.SelectedTabIndex : 0;
            updater.AvailableUpdateDetected += (obj, args) => updater.Update();
            updater.ErrorOccured += this.UpdaterErrorOccured;
            updater.DownloadStarted += (obj, args) => this.UpdateRunning = true;
            updater.UpdateCompleted += (obj, args) =>
                {
                    this.UpdateRunning = false;
                    updater.Check();
                };

            updater.DownloadProgress += this.UpdaterDownloadProgress;
            updater.Check();
            this.updater = updater;
        }

        /// <summary>
        ///     Gets the command for changing the language of the application
        /// </summary>
        public RelayCommand ChangeLanguageCommand
            =>
                this.changeLanguageCommand
                ?? (this.changeLanguageCommand = new RelayCommand(this.ExecuteChangeLanguageCommand));

        /// <summary>
        ///     Gets the exit command.
        /// </summary>
        public RelayCommand ExitCommand => this.exitCommand ?? (this.exitCommand = new RelayCommand(ExecuteExitCommand))
            ;

        /// <summary>
        ///     Gets the minimizes command.
        /// </summary>
        public RelayCommand MinimizeCommand
            => this.minimizeCommand ?? (this.minimizeCommand = new RelayCommand(this.ExecuteMinimizeCommand));

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
        ///     Gets a command that resets all updates and starts the redownload.
        /// </summary>
        public RelayCommand ResetCheckUpdateCommand
        {
            get
            {
                if (this.resetCheckUpdateCommand == null)
                {
                    this.resetCheckUpdateCommand = new RelayCommand(
                        (obj) => !this.updateRunning, 
                        (obj) =>
                            {
                                if (MessageBox.Show(
                                    "Do you really want to reset the all updates?", 
                                    "Reset updates?", 
                                    MessageBoxButton.YesNo, 
                                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                                {
                                    this.updater.CheckReset();
                                }
                            });
                }

                return this.resetCheckUpdateCommand;
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
        ///     Gets the current Progress of the update in percent.
        /// </summary>
        public int UpdateProgress
        {
            get
            {
                return this.updateProgress;
            }

            set
            {
                this.updateProgress = value;
                this.OnPropertyChanged(nameof(this.UpdateProgress));
            }
        }

        /// <summary>
        ///     Gets a text that describes the current update progress.
        /// </summary>
        public string UpdateProgressText
        {
            get
            {
                return this.updateProgressText;
            }

            set
            {
                this.updateProgressText = value;
                this.OnPropertyChanged(nameof(this.updateProgressText));
            }
        }

        /// <summary>
        ///     Gets a value indicating whether a update is running or not.
        /// </summary>
        public bool UpdateRunning
        {
            get
            {
                return this.updateRunning;
            }

            private set
            {
                this.updateRunning = value;
                this.OnPropertyChanged(nameof(this.UpdateRunning));
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
        ///     Execute the ChangeLanguageCommand command.
        /// </summary>
        /// <param name="language">Language code of the language to wich the application should be changed.</param>
        public void ExecuteChangeLanguageCommand(object language)
        {
            if (language != null)
            {
                var culture = CultureInfo.GetCultureInfo(language.ToString());
                this.Res.CurrentCulture = culture;
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

        private void UpdaterDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            this.UpdateProgress = e.ProgressPercentage;
            this.UpdateProgressText = $"{e.BytesReceived / 1000}KB / {e.TotalBytesToReceive / 1000}Kb";
        }

        private void UpdaterErrorOccured(object sender, UpdateErrorEventArgs e)
        {
            MessageBox.Show(e.ErrorMessage, this.Res["resMsgBoxUpdateError"].Value);
        }
    }
}