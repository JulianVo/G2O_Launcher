// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="GothicProgressBar.xaml.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Controls
{
    #region

    using System;
    using System.Timers;
    using System.Windows;

    #endregion

    /// <summary>
    ///     Interaction logic for the <see cref="GothicProgressBar" />
    /// </summary>
    public partial class GothicProgressBar
    {
        /// <summary>
        ///     Using a DependencyProperty as the backing store for Property1.This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", 
            typeof(int), 
            typeof(GothicProgressBar), 
            new PropertyMetadata(0, PropertyChangedCallback));

        /// <summary>
        ///     Initializes a new instance of the <see cref="GothicProgressBar" /> class.
        /// </summary>
        public GothicProgressBar()
        {
            this.InitializeComponent();
            var timer = new Timer();
            timer.Elapsed += this.TimerElapsed;
            timer.AutoReset = true;
            timer.Interval = 50;
            timer.Start();
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        public int Value
        {
            get
            {
                return (int)this.GetValue(ValueProperty);
            }

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

                this.imgBar.Width = (this.Width / 100) * value;
                this.imgBar.Height = this.Height;
                this.SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        ///     The property changed callback.
        /// </summary>
        /// <param name="dependencyObject">
        ///     The dependency object.
        /// </param>
        /// <param name="dependencyPropertyChangedEventArgs">
        ///     The dependency property changed event args.
        /// </param>
        private static void PropertyChangedCallback(
            DependencyObject dependencyObject, 
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var bar = dependencyObject as GothicProgressBar;
            if (bar != null)
            {
                bar.Value = (int)dependencyPropertyChangedEventArgs.NewValue;
            }
        }

        /// <summary>
        ///     The timer elapsed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new Action(() => this.TimerElapsed(sender, e)));
            }
            else
            {
                if (this.Value >= 100)
                {
                    this.Value = 0;
                }
                else
                {
                    this.Value++;
                }
            }
        }
    }
}