using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace G2O_Launcher
{
    /// <summary>
    /// Interaktionslogik für GothicProgressBar.xaml
    /// </summary>
    public partial class GothicProgressBar : UserControl
    {
        // Using a DependencyProperty as the backing store for Property1.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty= DependencyProperty.Register("Value", typeof(int), typeof(GothicProgressBar), new PropertyMetadata(0,PropertyChangedCallback));
        private Timer _Time;

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var bar = dependencyObject as GothicProgressBar;
            if (bar != null)
            {
                bar.Value = (int)dependencyPropertyChangedEventArgs.NewValue;
            }
        }


        public GothicProgressBar()
        {
            InitializeComponent();
            _Time = new Timer();
            _Time.Elapsed += _Time_Elapsed;
            _Time.AutoReset = true;
            _Time.Interval = 50;
            _Time.Start();
        }

        private void _Time_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => _Time_Elapsed(sender, e));
            }
            else
            {
                if (Value >= 100)
                {
                    Value = 0;
                }
                else
                {
                    Value++;
                }
            }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > 100)
                {
                    value = 100;
                }
                imgBar.Width = (Width / 100) * value;
                imgBar.Height = Height;
                SetValue(ValueProperty, value);
            }
        }
    }
}
