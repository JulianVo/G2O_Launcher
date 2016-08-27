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

    internal class FavoritesViewViewModel : NotifyPropertyChangedBase
    {
        private readonly IServerWatcher serverWatcher;

        public FavoritesViewViewModel(IServerWatcher serverWatcher)
        {
            if (serverWatcher == null) throw new ArgumentNullException(nameof(serverWatcher));
            this.serverWatcher = serverWatcher;
            this.serverWatcher.ServerStatusChanged += this.ServerWatcherServerStatusChanged;
            this.Servers = new ObservableCollection<ObservableServerEnty>();
        }

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

        public ProxyCommand AddServerCommand => new ProxyCommand(this.ExecuteAddServerCommand);

        public ObservableCollection<ObservableServerEnty> Servers { get; }

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
                throw;
            }

        }
    }
}