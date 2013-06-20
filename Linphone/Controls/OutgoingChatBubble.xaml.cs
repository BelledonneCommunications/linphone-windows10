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

namespace Linphone.Controls
{
    /// <summary>
    /// Control to display sent chat messages.
    /// </summary>
    public partial class OutgoingChatBubble : UserControl
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public OutgoingChatBubble(string message, string timestamp)
        {
            InitializeComponent();
            Message.Text = message;
            Timestamp.Text = timestamp;

            System.Windows.Media.Color accent = (System.Windows.Media.Color)Resources["PhoneAccentColor"];
            System.Windows.Media.Color darkAccent = System.Windows.Media.Color.FromArgb(accent.A, (byte)(accent.R / 2), (byte)(accent.G / 2), (byte)(accent.B / 2));
            Background.Fill = new SolidColorBrush(darkAccent);
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
    }
}
