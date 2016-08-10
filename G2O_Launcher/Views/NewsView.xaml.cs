using System.Windows.Controls;
using G2O_Launcher.ViewModels;

namespace G2O_Launcher.Views
{
    /// <summary>
    /// Interaction logic for NewsView.xaml
    /// </summary>
    public partial class NewsView : UserControl
    {
        public NewsView()
        {
            InitializeComponent();
            DataContext = new NewsViewViewModel(Properties.Resources.resNewsNotLoaded);
        }
    }
}
