using System.Windows.Controls;
using G2O_Launcher.ViewModels;

namespace G2O_Launcher.Views
{
    /// <summary>
    /// Interaction logic for FavoritesView.xaml
    /// </summary>
    public partial class FavoritesView : UserControl
    {
        public FavoritesView()
        {
            InitializeComponent();
            DataContext = new FavoritesViewViewModel();
        }
    }
}
