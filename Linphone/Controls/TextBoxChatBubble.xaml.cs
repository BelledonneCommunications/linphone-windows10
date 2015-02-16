/*
TextBoxChatBubble.xaml.cs
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
using System.Windows.Media.Imaging;

namespace Linphone.Controls
{
    /// <summary>
    /// Custom TextBox to look like a chat bubble.
    /// </summary>
    public partial class TextBoxChatBubble : ChatBubble
    {
        /// <summary>
        /// Public accessor to this control text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Sets the filename associated with the image to send
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// Sets the local path where the image is stored
        /// </summary>
        public string ImageLocalPath { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public TextBoxChatBubble() 
            : base(null)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Empty the content of the textbox.
        /// </summary>
        public void Reset()
        {
            Message.Text = "";
            Text = null;
            ImageName = null;
            ImageLocalPath = null;
            Message.Visibility = Visibility.Visible;
            Image.Visibility = Visibility.Collapsed;
        }

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = Message.Text;
            if (TextChanged != null)
            {
                TextChanged(this, Message.Text);
            }
        }

        /// <summary>
        /// Sets and displays the BitmapImage that will be sent we the user clicks on the send button
        /// </summary>
        /// <param name="image">the BitmapImage to display</param>
        public void SetImage(BitmapImage image)
        {
            Image.Source = image;
            Image.Visibility = Visibility.Visible;
            Message.Visibility = Visibility.Collapsed;
            Message.Text = "";
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string previousText = Text;
            Reset();
            Text = previousText;
            Message.Text = Text;
        }

        /// <summary>
        /// Delegate for text changed event.
        /// </summary>
        public delegate void TextChangedEventHandler(object sender, string text);

        /// <summary>
        /// Handler for text changed event.
        /// </summary>
        public event TextChangedEventHandler TextChanged;
    }
}
