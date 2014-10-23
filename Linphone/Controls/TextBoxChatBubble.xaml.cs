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
