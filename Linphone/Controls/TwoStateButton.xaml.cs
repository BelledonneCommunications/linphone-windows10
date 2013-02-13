using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace Linphone.Controls
{
    /// <summary>
    /// Custom control to represent a two state button where each state is an image.
    /// </summary>
    public partial class TwoStateButton : UserControl
    {
        /// <summary>
        /// Dependency property for the idle image.
        /// </summary>
        public static readonly DependencyProperty DefaultImageProperty = DependencyProperty.Register("DefaultImage", typeof(String), typeof(TwoStateButton), new PropertyMetadata(""));
        /// <summary>
        /// Dependency property for the pressed image.
        /// </summary>
        public static readonly DependencyProperty OverImageProperty = DependencyProperty.Register("OverImage", typeof(String), typeof(TwoStateButton), new PropertyMetadata(""));

        /// <summary>
        /// Image displayed while the button is in idle state.
        /// </summary>
        public String DefaultImage
        {
            get { return (String)GetValue(DefaultImageProperty); }
            set { SetValue(DefaultImageProperty, value); }
        }

        /// <summary>
        /// Image displayed while the button is pressed.
        /// </summary>
        public String OverImage
        {
            get { return (String)GetValue(OverImageProperty); }
            set { SetValue(OverImageProperty, value); }
        }

        /// <summary>
        /// Event triggered when the button is clicked.
        /// </summary>
        public RoutedEventHandler Click
        {
            set { button.Click += value; }
            get { return null; }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public TwoStateButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Image_ManipulationStarted_1(object sender, ManipulationStartedEventArgs e)
        {
            Image image = (sender as Image);
            image.Source = new BitmapImage(new Uri(OverImage, UriKind.RelativeOrAbsolute));
        }

        private void Image_ManipulationCompleted_1(object sender, ManipulationCompletedEventArgs e)
        {
            Image image = (sender as Image);
            image.Source = new BitmapImage(new Uri(DefaultImage, UriKind.RelativeOrAbsolute));
        }
    }
}
