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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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
using System.Windows.Documents;

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
        /// Returns a formatted text with clickable links if there are any
        /// </summary>
        protected Paragraph TextWithLinks
        {
            get
            {
                return FormatText();
            }
        }

        /// <summary>
        /// Human readable timestamp
        /// </summary>
        public string HumanFriendlyTimeStamp
        {
            get
            {
                DateTime date = new DateTime(ChatMessage.Time * TimeSpan.TicksPerSecond, DateTimeKind.Utc).AddYears(1969).ToLocalTime();
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

        private Paragraph FormatText()
        {
            Paragraph paragraph = new Paragraph();
            if (ChatMessage != null)
            {
                string text = ChatMessage.Text;
                if (text.Contains("http://") || text.Contains("https://"))
                {
                    string[] split = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in split)
                    {
                        if (word.StartsWith("http://") || word.StartsWith("https://"))
                        {
                            Hyperlink link = new Hyperlink();
                            link.NavigateUri = new Uri(word);
                            link.Inlines.Add(word);
                            link.TargetName = "_blank";
                            paragraph.Inlines.Add(link);
                        }
                        else
                        {
                            paragraph.Inlines.Add(word);
                        }
                        paragraph.Inlines.Add(" ");
                    }
                }
                else
                {
                    paragraph.Inlines.Add(text);
                }
            }
            return paragraph;
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
