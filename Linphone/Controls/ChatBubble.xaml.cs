/*
ChatBubble.xaml.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Linphone;
using Linphone.Model;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Linphone.Controls
{
    /// <summary>
    /// Parent class for OutgoingChatBubble and IncomingChatBubble
    /// </summary>
    public partial class ChatBubble : UserControl
    {
        private ChatMessage _message;

        /// <summary>
        /// Chat message associated with this bubble
        /// </summary>
        public ChatMessage ChatMessage
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
        /// Returns a formatted text with clickable links if there are any
        /// </summary>
        protected Paragraph TextWithLinks
        {
            get
            {
                return Utils.FormatText(ChatMessage.TextContent);
            }
        }

        /// <summary>
        /// Human readable timestamp
        /// </summary>
        public string HumanFriendlyTimeStamp
        {
            get
            {
                return Utils.FormatDate(ChatMessage.Time);
            }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public ChatBubble(ChatMessage message)
        {
            this.InitializeComponent();
            ChatMessage = message;
        }
    }
}
