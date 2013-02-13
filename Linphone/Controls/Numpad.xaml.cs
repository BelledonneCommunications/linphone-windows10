using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace Linphone.Controls
{
    /// <summary>
    /// Custom control to represent a numpad (composed of Digits and an AddressBox).
    /// </summary>
    public partial class Numpad : UserControl
    {
        /// <summary>
        /// Dependency property for the AddressBox.
        /// </summary>
        public static readonly DependencyProperty AddressProperty = DependencyProperty.Register("Address", typeof(AddressBox), typeof(Numpad), new PropertyMetadata(new AddressBox()));

        /// <summary>
        /// AddressBox (custom control) in which to append the clicked digits.
        /// </summary>
        public AddressBox Address
        {
            get { return (AddressBox)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Numpad()
        {
            InitializeComponent();
            this.DataContext = this;
            initNumpadForDigits();
        }

        private void initNumpadForDigits()
        {
            one.Numpad = this;
            two.Numpad = this;
            three.Numpad = this;
            four.Numpad = this;
            five.Numpad = this;
            six.Numpad = this;
            seven.Numpad = this;
            eight.Numpad = this;
            nine.Numpad = this;
            zero.Numpad = this;
            star.Numpad = this;
            sharp.Numpad = this;
        }

        /// <summary>
        /// Event triggered when a Digit is pressed and released.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void digit_Click_1(object sender, RoutedEventArgs e)
        {
            Digit button = (sender as Digit);
            String character = (button.Tag as String);
            Address.Text += character;
        }

        private void zero_Hold_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Address.Text.Length > 0)
                Address.Text = Address.Text.Substring(0, Address.Text.Length - 1); ;
            Address.Text += "+";
        }
    }
}
