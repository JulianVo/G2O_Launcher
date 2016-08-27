// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="NewsViewViewModel.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.ViewModels
{
    /// <summary>
    ///     ViewModel for the news view.
    /// </summary>
    internal class NewsViewViewModel : NotifyPropertyChangedBase
    {
        /// <summary>
        ///     The news text.
        /// </summary>
        private string newsText;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NewsViewViewModel" /> class.
        /// </summary>
        /// <param name="defaultNewsText">The default news text.</param>
        public NewsViewViewModel(string defaultNewsText)
        {
            this.newsText = defaultNewsText;
        }

        /// <summary>
        ///     Gets or sets the news text.
        /// </summary>
        public string NewsText
        {
            get
            {
                return this.newsText;
            }

            set
            {
                this.newsText = value;
                this.OnPropertyChanged(nameof(this.NewsText));
            }
        }
    }
}