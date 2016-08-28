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

    using G2O.Launcher.ServerRequests;

    using G2O_Launcher.Commands;

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

        private ObservableServerEnty selectedEntry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FavoritesViewViewModel" /> class.
        /// </summary>
        /// <param name="serverWatcher">The used server watcher instance.</param>
        public FavoritesViewViewModel(IServerWatcher serverWatcher)
        {
            if (serverWatcher == null) throw new ArgumentNullException(nameof(serverWatcher));
            this.serverWatcher = serverWatcher;
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

        public ProxyCommand RemoveServerCommand => new ProxyCommand(this.CanExecuteRemoveServerCommand, this.ExecuteRemoveServerCommand);

        /// <summary>
        /// Executes the remove server command.
        /// </summary>
        /// <param name="obj">command parameter(unused)</param>
        private void ExecuteRemoveServerCommand(object obj)
        {
            var entry = this.SelectedEntry;
            this.serverWatcher.RemoveServer(entry.ServerState);
            this.Servers.Remove(entry);
        }

        /// <summary>
        /// Checks if the remove server command can be executed.
        /// </summary>
        /// <param name="arg">command parameter.(unused)</param>
        /// <returns>True if the remove server command can be executed.</returns>
        private bool CanExecuteRemoveServerCommand(object arg)
        {
            return this.SelectedEntry != null;
        }

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