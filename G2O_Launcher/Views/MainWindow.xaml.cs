// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="MainWindow.xaml.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Views
{
    #region

    using System.Windows;
    using System.Windows.Input;

    using G2O_Launcher.ViewModels;

    #endregion

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}