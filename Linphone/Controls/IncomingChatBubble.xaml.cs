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
    /// Control to display received chat messages.
    /// </summary>
    public partial class IncomingChatBubble : UserControl
    {
        private ChatMessage _message;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public IncomingChatBubble(ChatMessage message, string timestamp)
        {
            InitializeComponent();
            _message = message;
            Message.Text = message.Message;
            Timestamp.Text = timestamp;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageDeleted != null)
            {
                MessageDeleted(this, _message);
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Message.Text);
        }

        /// <summary>
        /// Delegate for delete event.
        /// </summary>
        public delegate void MessageDeletedEventHandler(object sender, ChatMessage message);

        /// <summary>
        /// Handler for delete event.
        /// </summary>
        public event MessageDeletedEventHandler MessageDeleted;
    }
}
