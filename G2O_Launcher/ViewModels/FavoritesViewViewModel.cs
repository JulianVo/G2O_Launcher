// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="FavoritesViewViewModel.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.ViewModels
{
    #region

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;

    using G2O_Launcher.Commands;
    using G2O_Launcher.G2O;
    using G2O_Launcher.ServerRequests.Implementation;

    #endregion

    /// <summary>
    ///     ViewModel class for the favorites view.
    /// </summary>
    public class FavoritesViewViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        ///     The used <see cref="IServerWatcher" /> instance.
        /// </summary>
        private readonly IServerWatcher serverWatcher;

        /// <summary>
        /// The used instance of <see cref="IG2OStarter"/>.
        /// </summary>
        private readonly IG2OStarter starter;

        /// <summary>
        /// The selected entry of the servers collection.
        /// </summary>
        private ObservableServerEnty selectedEntry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FavoritesViewViewModel" /> class.
        /// </summary>
        /// <param name="serverWatcher">The used server watcher instance.</param>
        /// <param name="starter">The <see cref="IG2OStarter"/> instance that should be used to start the client.</param>
        public FavoritesViewViewModel(IServerWatcher serverWatcher, IG2OStarter starter)
        {
            if (serverWatcher == null)
            {
                throw new ArgumentNullException(nameof(serverWatcher));
            }
            if (starter == null)
            {
                throw new ArgumentNullException(nameof(starter));
            }
            this.serverWatcher = serverWatcher;
            this.starter = starter;
            this.serverWatcher.ServerStatusChanged += this.ServerWatcherServerStatusChanged;
            this.Servers = new ObservableCollection<ObservableServerEnty>();

            foreach (var watchedServer in serverWatcher.WatchedServers)
            {
                this.Servers.Add(new ObservableServerEnty(watchedServer));
            }
        }

        /// <summary>
        ///     Gets the add server command.
        /// </summary>
        public ProxyCommand AddServerCommand => new ProxyCommand(this.ExecuteAddServerCommand);

        /// <summary>
        ///     Gets the connect to server command.
        /// </summary>
        public ProxyCommand ConnectToServerCommand
            => new ProxyCommand(this.CanExecuteConnectToServerCommand, this.ExecuteConnectToServerCommand);

        /// <summary>
        ///     Gets the remove server command.
        /// </summary>
        public ProxyCommand RemoveServerCommand
            => new ProxyCommand(this.CanExecuteRemoveServerCommand, this.ExecuteRemoveServerCommand);

        /// <summary>
        ///     Gets or sets the selected entry.
        /// </summary>
        public ObservableServerEnty SelectedEntry
        {
            get
            {
                return this.selectedEntry;
            }

            set
            {
                this.selectedEntry = value;
                this.OnPropertyChanged(nameof(this.SelectedEntry));
            }
        }

        /// <summary>
        ///     Gets the observable server list.
        /// </summary>
        public ObservableCollection<ObservableServerEnty> Servers { get; }

        /// <summary>
        ///     Checks if the connect to server command can be executed.
        /// </summary>
        /// <param name="arg">command parameter.(not used)</param>
        /// <returns>True if the command can be executed.</returns>
        private bool CanExecuteConnectToServerCommand(object arg)
        {
            return this.SelectedEntry != null;
        }

        /// <summary>
        ///     Checks if the remove server command can be executed.
        /// </summary>
        /// <param name="arg">command parameter.(unused)</param>
        /// <returns>True if the remove server command can be executed.</returns>
        private bool CanExecuteRemoveServerCommand(object arg)
        {
            return this.SelectedEntry != null;
        }

        /// <summary>
        ///     Executes the add server command.
        /// </summary>
        /// <param name="value">[string] address of the server.</param>
        private void ExecuteAddServerCommand(object value)
        {
            try
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                var serverState = this.serverWatcher.AddServer(value.ToString());
                this.Servers.Add(new ObservableServerEnty(serverState));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Executes the connect to server command
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteConnectToServerCommand(object obj)
        {
            if (this.SelectedEntry.ServerState.Info != null)
            {
                var info = this.selectedEntry.ServerState.Info;
                var result = this.starter.Start(info.Major, info.Minor, info.Patch, $"{this.SelectedEntry.ServerState.ServerIp}:{this.SelectedEntry.ServerState.ServerPort}");
                switch (result)
                {
                    case G2OProxy.RunResult.Success:
                        break;
                    case G2OProxy.RunResult.WrongVersion:
                        MessageBox.Show(
                            Properties.Resources.resMessageBoxCanNotJoin,
                            Properties.Resources.resMessageBoxTitelG2O,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        break;
                    case G2OProxy.RunResult.GothicNotFound:
                        MessageBox.Show(
                                Properties.Resources.resMessageBoxGothicInstalled,
                                Properties.Resources.resMessageBoxTitelG2O,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                        break;
                    case G2OProxy.RunResult.Unknown:
                        MessageBox.Show($"{Properties.Resources.resMessageBoxCouldNotStartG2O} {info.VersionString}", Properties.Resources.resMessageBoxTitelG2O, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
            }
        }

        /// <summary>
        ///     Executes the remove server command.
        /// </summary>
        /// <param name="obj">command parameter(unused)</param>
        private void ExecuteRemoveServerCommand(object obj)
        {
            var entry = this.SelectedEntry;
            this.serverWatcher.RemoveServer(entry.ServerState);
            this.Servers.Remove(entry);
        }

        /// <summary>
        ///     Called when the status of a watched server changes.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="IServerWatcher" />
        /// </param>
        /// <param name="e">
        ///     <see cref="ServerStatusChangedEventArgs" />
        /// </param>
        private void ServerWatcherServerStatusChanged(object sender, ServerStatusChangedEventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Delegate del = new Action(() => this.ServerWatcherServerStatusChanged(sender, e));
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, del);
            }
            else
            {
                this.Servers.FirstOrDefault(serverEnty => serverEnty.ServerState == e.ChangedStatus)?.UpdateFromState();
            }
        }
    }
}