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
using Linphone.Views;
using System.Windows.Media.Imaging;

namespace Linphone.Controls
{
    /// <summary>
    /// Control to display received chat messages.
    /// </summary>
    public partial class IncomingChatBubble : UserControl
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
        /// Public constructor.
        /// </summary>
        public IncomingChatBubble(ChatMessage message, string timestamp)
        {
            InitializeComponent();
            ChatMessage = message;
            Message.Text = message.Message;
            Timestamp.Text = timestamp;

            if (ChatMessage.ImageURL != null && ChatMessage.ImageURL.Length > 0)
            {
                Message.Visibility = Visibility.Collapsed;
                Copy.Visibility = Visibility.Collapsed;

                if (ChatMessage.ImageURL.StartsWith("http"))
                {
                    DownloadImage.Visibility = Visibility.Visible;
                }
                else
                {
                    ShowImage.Visibility = Visibility.Visible;
                }
            }
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

        private void DownloadImage_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Chat.DownloadImageAndStoreItInIsolatedStorage(ChatMessage.ImageURL, ChatMessage);
            Image.Source = image;

            DownloadImage.Visibility = Visibility.Collapsed;
            Image.Visibility = Visibility.Visible;
        }

        private void ShowImage_Click(object sender, RoutedEventArgs e)
        {
            Image.Source = Chat.GetThumbnailBitmapFromImage(Chat.ReadImageFromIsolatedStorage(ChatMessage.ImageURL));
            ShowImage.Visibility = Visibility.Collapsed;
            Image.Visibility = Visibility.Visible;
        }
    }
}
