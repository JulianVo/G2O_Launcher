// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="FavoritesView.xaml.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Views
{
    #region

    using System.Windows.Controls;

    using G2O.Launcher.ServerRequests;

    using G2O_Launcher.ViewModels;

    #endregion

    /// <summary>
    ///     Interaction logic for the <see cref="FavoritesView"/>.
    /// </summary>
    public partial class FavoritesView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FavoritesView"/> class.
        /// </summary>
        public FavoritesView()
        {
            this.InitializeComponent();
            this.DataContext = new FavoritesViewViewModel(new ServerWatcher(28970, 1000, 5000, 5000));
        }
    }
}