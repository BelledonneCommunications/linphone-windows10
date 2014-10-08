using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Model;

namespace Linphone.Controls
{
    /// <summary>
    /// Control to display numpad during a call
    /// </summary>
    public partial class Numpad : UserControl
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        public Numpad()
        {
            InitializeComponent();
        }

        private void Numpad_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            String tag = button.Tag as String;
            LinphoneManager.Instance.LinphoneCore.SendDTMF(Convert.ToChar(tag));
        }
    }
}
