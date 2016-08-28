﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="MainWindowViewModel.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.ViewModels
{
    #region

    using System;
    using System.ComponentModel;
    using System.Windows;

    using G2O_Launcher.Commands;
    using G2O_Launcher.Config;

    #endregion

    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private readonly ILauncherConfig config;

        /// <summary>
        ///     The exit command
        /// </summary>
        private ProxyCommand exitCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="config">The current launcher config,</param>
        public MainWindowViewModel(ILauncherConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            this.config = config;
            this.SelectedTabIndex = config.SelectedTabIndex > 0 ? config.SelectedTabIndex : 0;
        }

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