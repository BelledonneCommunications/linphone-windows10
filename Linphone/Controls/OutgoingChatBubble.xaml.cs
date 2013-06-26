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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Linphone.Model;
using Linphone.Views;

namespace Linphone.Controls
{
    /// <summary>
    /// Control to display sent chat messages.
    /// </summary>
    public partial class OutgoingChatBubble : UserControl
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

        private SolidColorBrush _darkAccentBrush
        {
            get
            {
                System.Windows.Media.Color accent = (System.Windows.Media.Color)Resources["PhoneAccentColor"];
                System.Windows.Media.Color darkAccent = System.Windows.Media.Color.FromArgb(accent.A, (byte)(accent.R / 2), (byte)(accent.G / 2), (byte)(accent.B / 2));
                return new SolidColorBrush(darkAccent);
            }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public OutgoingChatBubble(ChatMessage message, string timestamp)
        {
            InitializeComponent();

            ChatMessage = message;
            Message.Visibility = Visibility.Visible;
            Image.Visibility = Visibility.Collapsed;
            ShowImage.Visibility = Visibility.Collapsed;
            Message.Text = message.Message;
            Timestamp.Text = timestamp;

            Background.Fill = _darkAccentBrush;
            Path.Fill = _darkAccentBrush;
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public OutgoingChatBubble(ChatMessage message, BitmapImage image, string timestamp)
        {
            InitializeComponent();
            ChatMessage = message;
            Message.Visibility = Visibility.Collapsed;
            Copy.Visibility = Visibility.Collapsed;

            if (image == null)
            {
                ShowImage.Visibility = Visibility.Visible;
                Image.Visibility = Visibility.Collapsed;
            }
            else
            {
                ShowImage.Visibility = Visibility.Collapsed;
                Image.Visibility = Visibility.Visible;
            }
            Image.Source = image;
            Timestamp.Text = timestamp;

            Background.Fill = _darkAccentBrush;
            Path.Fill = _darkAccentBrush;
        }

        /// <summary>
        /// Changes the icon indicating the status of the message (InProgress, Delivered or NotDelivered).
        /// </summary>
        public void UpdateStatus(LinphoneChatMessageState state)
        {
            string delivered = "/Assets/AppBar/check.png";
            string notdelivered = "/Assets/AppBar/stop.png";

            if (state == LinphoneChatMessageState.Delivered)
            {
                Status.Source = new BitmapImage(new Uri(delivered, UriKind.RelativeOrAbsolute));
            }
            else if (state == LinphoneChatMessageState.NotDelivered)
            {
                Status.Source = new BitmapImage(new Uri(notdelivered, UriKind.RelativeOrAbsolute));
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageDeleted != null)
            {
                MessageDeleted(this, ChatMessage);
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Message.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Chat.SavePictureInMediaLibrary(ChatMessage.ImageURL);
        }

        /// <summary>
        /// Delegate for delete event.
        /// </summary>
        public delegate void MessageDeletedEventHandler(object sender, ChatMessage message);

        /// <summary>
        /// Handler for delete event.
        /// </summary>
        public event MessageDeletedEventHandler MessageDeleted;

        private void ShowImage_Click(object sender, RoutedEventArgs e)
        {
            Image.Source = Chat.GetThumbnailBitmapFromImage(Chat.ReadImageFromIsolatedStorage(ChatMessage.ImageURL));
            ShowImage.Visibility = Visibility.Collapsed;
            Image.Visibility = Visibility.Visible;
            Save.Visibility = Visibility.Visible;
        }
    }
}
