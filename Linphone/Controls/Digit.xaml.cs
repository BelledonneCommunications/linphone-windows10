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
    /// Custom control to represent a Digit in the numpad.
    /// </summary>
    public partial class Digit : UserControl
    {
        /// <summary>
        /// Dependency property for the idle image.
        /// </summary>
        public static readonly DependencyProperty DefaultImageProperty = DependencyProperty.Register("DefaultImage", typeof(String), typeof(Digit), new PropertyMetadata(""));
        /// <summary>
        /// Dependency property for the pressed image.
        /// </summary>
        public static readonly DependencyProperty OverImageProperty = DependencyProperty.Register("OverImage", typeof(String), typeof(Digit), new PropertyMetadata(""));
        /// <summary>
        /// Dependency property for the numpad host.
        /// </summary>
        public static readonly DependencyProperty NumpadProperty = DependencyProperty.Register("Numpad", typeof(Numpad), typeof(Digit), new PropertyMetadata(null));

        /// <summary>
        /// Idle image for the digit.
        /// </summary>
        public String DefaultImage
        {
            get { return (String)GetValue(DefaultImageProperty); }
            set { SetValue(DefaultImageProperty, value); }
        }

        /// <summary>
        /// Image to display when the digit is pressed.
        /// </summary>
        public String OverImage
        {
            get { return (String)GetValue(OverImageProperty); }
            set { SetValue(OverImageProperty, value); }
        }

        /// <summary>
        /// Numpad in which the digit is displayed.
        /// </summary>
        public Numpad Numpad
        {
            get { return (Numpad)GetValue(NumpadProperty); }
            set { SetValue(NumpadProperty, value); }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Digit()
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

        private void digit_Click_1(object sender, RoutedEventArgs e)
        {
            Numpad.digit_Click_1(this, e);
        }
    }
}
