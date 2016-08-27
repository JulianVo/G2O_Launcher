// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="NewsView.xaml.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Views
{
    #region

    using System.Windows.Controls;

    using G2O_Launcher.ViewModels;

    #endregion

    /// <summary>
    ///     Interaction logic for NewsView.xaml
    /// </summary>
    public partial class NewsView : UserControl
    {
        public NewsView()
        {
            this.InitializeComponent();
            this.DataContext = new NewsViewViewModel(Properties.Resources.resNewsNotLoaded);
        }
    }
}