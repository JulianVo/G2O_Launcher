// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="App.xaml.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher
{
    #region

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows;

    using global::G2O.Launcher.ServerRequests;

    using G2O_Launcher.Config;
    using G2O_Launcher.G2O;
    using G2O_Launcher.ViewModels;
    using G2O_Launcher.Views;

    #endregion

    /// <summary>
    ///     The app main class.
    /// </summary>
    public partial class App
    {
        /// <summary>
        ///     The loaded launcher configuration.
        /// </summary>
        private readonly LauncherConfig config;

        /// <summary>
        ///     The path to the launcher config.
        /// </summary>
        private readonly string configPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Gothic2OnlineLauncher",
                "config.xml");

        /// <summary>
        ///     Mutex. Used to ensure that only one instance of the launcher exists.
        /// </summary>
        private readonly Mutex mutex = new Mutex(false, typeof(App).Namespace);

        /// <summary>
        /// The server watcher for the favorite servers.
        /// </summary>
        private readonly IServerWatcher favoritesServerWatcher;

        /// <summary>
        ///     Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomainUnhandledException;
            AppDomain.CurrentDomain.ProcessExit += this.CurrentDomainProcessExit;
            if (!this.mutex.WaitOne())
            {
                // The application is already running.
                Environment.Exit(1);
            }

            this.config = this.LoadConfig();
            if (!string.IsNullOrEmpty(this.config.SelectedLanguage))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(this.config.SelectedLanguage);
            }
            else
            {
                this.config.SelectedLanguage = Thread.CurrentThread.CurrentCulture.Name;
            }
            this.favoritesServerWatcher = new ServerWatcher(28970, 100, 1000, 2000);
            this.favoritesServerWatcher.Start();
            //Load the saved favorites servers.
            try
            {
                foreach (var favoriteServer in this.config.FavoriteServers)
                {
                    this.favoritesServerWatcher.AddServer(favoriteServer);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    G2O_Launcher.Properties.Resources.resErrorInvalidServerInConfig,
                    "Invalid server address loaded",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            var registry = new RegistryConfig();
            using (var starter = new G2OStarter(new G2OProxy(), registry))
            {
                MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(this.config, registry);
                NewsViewViewModel newsViewViewModel = new NewsViewViewModel(G2O_Launcher.Properties.Resources.resNewsNotLoaded);
                FavoritesViewViewModel favoritesViewViewModel = new FavoritesViewViewModel(this.favoritesServerWatcher, starter);
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                while (true)
                {
                    MainWindow window = new MainWindow(mainWindowViewModel, newsViewViewModel, favoritesViewViewModel);
                    window.ShowDialog();
                }
            }
        }

        /// <summary>
        ///     Called when the application is closed.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="AppDomain" />
        /// </param>
        /// <param name="e">
        ///     <see cref="EventArgs" />
        /// </param>
        private void CurrentDomainProcessExit(object sender, EventArgs e)
        {
            this.favoritesServerWatcher.Stop();
            this.config.FavoriteServers.Clear();
            foreach (var watchedServer in this.favoritesServerWatcher.WatchedServers)
            {
                this.config.FavoriteServers.Add(watchedServer.OriginalAddress);
            }

            this.config.SaveConfig(this.configPath);
        }

        /// <summary>
        ///     Called when a exception occures and is not handled anywhere.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="AppDomain" />
        /// </param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance.</param>
        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Show a messagebox with the exception text and stacktrace.
            MessageBox.Show(
                $"A unhandled error occured: {Environment.NewLine}{((Exception)e.ExceptionObject).Message}",
                "Unhandled error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        ///     Loads the launcher config.
        /// </summary>
        /// <returns></returns>
        private LauncherConfig LoadConfig()
        {
            ILauncherConfig launcherConfig = null;
            try
            {
                if (File.Exists(this.configPath))
                {
                    launcherConfig = LauncherConfig.LoadConfig(this.configPath);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                // ToDo add logging here.
            }
            finally
            {
                if (launcherConfig == null)
                {
                    launcherConfig = new LauncherConfig();
                }
            }

            return launcherConfig as LauncherConfig;
        }
    }
}