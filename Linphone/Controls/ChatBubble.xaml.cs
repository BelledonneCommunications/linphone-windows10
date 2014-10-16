using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Core;

namespace Linphone.Controls
{
    public partial class ChatBubble : UserControl
    {
        private LinphoneChatMessage _message;

        /// <summary>
        /// Chat message associated with this bubble
        /// </summary>
        public LinphoneChatMessage ChatMessage
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        public ChatBubble(LinphoneChatMessage message)
        {
            InitializeComponent();
            ChatMessage = message;
        }
    }
}
