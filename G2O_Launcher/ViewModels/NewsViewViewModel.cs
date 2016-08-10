namespace G2O_Launcher.ViewModels
{
    internal class NewsViewViewModel:NotifyPropertyChangedBase
    {
        private string _NewsText;

        public NewsViewViewModel(string newsText)
        {
            _NewsText = newsText;
        }

        public string NewsText
        {
            get { return _NewsText; }
            set
            {
                _NewsText = value; 
                OnPropertyChanged();
            }
        }
    }
}
