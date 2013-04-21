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
    /// Control to display sent chat messages.
    /// </summary>
    public partial class OutgoingChatBubble : UserControl
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        public OutgoingChatBubble(string message, string timestamp)
        {
            InitializeComponent();
            Message.Text = message;
            Timestamp.Text = timestamp;
        }
    }
}
