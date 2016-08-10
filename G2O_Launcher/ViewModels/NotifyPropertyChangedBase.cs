using System.ComponentModel;
using System.Runtime.CompilerServices;
using G2O_Launcher.Annotations;

namespace G2O_Launcher.ViewModels
{
    internal class NotifyPropertyChangedBase:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
