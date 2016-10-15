using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G2O_Launcher.Localization
{
    using System.ComponentModel;

    using G2O_Launcher.Annotations;

    internal class DynamicStringResource:INotifyPropertyChanged
    {
        private string value;

        public DynamicStringResource([NotNull] string name, [NotNull] string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            }
            this.Name = name;
            this.value=value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(this.Value)));
            }
        }
    }
}
