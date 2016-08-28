// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="MainWindow.xaml.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Views
{
    #region

    using System;
    using System.Windows.Input;

    using G2O_Launcher.ViewModels;

    #endregion

    /// <summary>
    ///     The main window.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        /// <param name="viewModel">The view model of the main window.</param>
        /// <param name="newsViewViewModel">The view model of the news view.</param>
        /// <param name="favoritesViewViewModel">The view model of the favorites view.</param>
        public MainWindow(
            MainWindowViewModel viewModel, 
            NewsViewViewModel newsViewViewModel, 
            FavoritesViewViewModel favoritesViewViewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (newsViewViewModel == null)
            {
                throw new ArgumentNullException(nameof(newsViewViewModel));
            }

            if (favoritesViewViewModel == null)
            {
                throw new ArgumentNullException(nameof(favoritesViewViewModel));
            }

            this.InitializeComponent();
            this.DataContext = viewModel;
            this.NewsView.DataContext = newsViewViewModel;
            this.FavoritesView.DataContext = favoritesViewViewModel;
        }

        /// <summary>
        ///     Called when mouse is moved while the left mouse button is down. Moving the window.
        /// </summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}