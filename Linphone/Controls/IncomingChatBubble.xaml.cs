using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Linphone.Controls
{
    /// <summary>
    /// Control to display received chat messages.
    /// </summary>
    public partial class IncomingChatBubble : UserControl
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public IncomingChatBubble(string message, string timestamp)
        {
            InitializeComponent();
            Message.Text = message;
            Timestamp.Text = timestamp;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Message.Text);
        }
    }
}
