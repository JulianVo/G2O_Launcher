using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G2O_Launcher.Commands;

namespace G2O_Launcher.ViewModels
{
    class FavoritesViewViewModel: NotifyPropertyChangedBase
    {
        public ProxyCommand AddServerCommand => new ProxyCommand(ExecuteAddServerCommand);

        private void ExecuteAddServerCommand(object value)
        {
            
        }
    }
}
