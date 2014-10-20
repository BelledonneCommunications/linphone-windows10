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
    /// <summary>
    /// Parent class for OutgoingChatBubble and IncomingChatBubble
    /// </summary>
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

        /// <summary>
        /// Human readable timestamp
        /// </summary>
        public string HumanFriendlyTimeStamp
        {
            get
            {
                DateTime date = new DateTime(ChatMessage.GetTime() * TimeSpan.TicksPerSecond).AddYears(1969);
                return FormatDate(date);
            }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public ChatBubble(LinphoneChatMessage message)
        {
            InitializeComponent();
            ChatMessage = message;
        }

        private string FormatDate(DateTime date)
        {
            DateTime now = DateTime.Now;
            if (now.Year == date.Year && now.Month == date.Month && now.Day == date.Day)
                return String.Format("{0:HH:mm}", date);
            else if (now.Year == date.Year)
                return String.Format("{0:ddd d MMM, HH:mm}", date);
            else
                return String.Format("{0:ddd d MMM yyyy, HH:mm}", date);
        }
    }
}
