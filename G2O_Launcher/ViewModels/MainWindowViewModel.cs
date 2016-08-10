using System;
using System.ComponentModel;
using System.Windows;
using G2O_Launcher.Commands;

namespace G2O_Launcher.ViewModels
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ProxyCommand _ExitCommand;
        private ProxyCommand _MinimizeCommand;
        private int _SelectedTabIndex;
        private WindowState _WindowWindowState;

        public ProxyCommand ExitCommand
        {
            get { return _ExitCommand ?? (_ExitCommand = new ProxyCommand(ExecuteExitCommand)); }
        }

        public ProxyCommand MinimizeCommand
        {
            get { return _MinimizeCommand ?? (_MinimizeCommand = new ProxyCommand(ExecuteMinimizeCommand)); }
        }

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _SelectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        public WindowState WindowState
        {
            get { return _WindowWindowState; }
            set
            {
                if (!Enum.IsDefined(typeof(WindowState), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int) value, typeof(WindowState));
                }

                _WindowWindowState = value;
                OnPropertyChanged();
            }
        }

        private void ExecuteExitCommand(object param)
        {
            Environment.Exit(0);
        }

        private void ExecuteMinimizeCommand(object param)
        {
            WindowState = WindowState.Minimized;
        }
    }
}