/*
OutgoingChatBubble.xaml.cs
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
using Linphone.Model;
using Linphone.Views;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Linphone.Controls;
using BelledonneCommunications.Linphone.Native;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        /* private SolidColorBrush _darkAccentBrush
         {
             get
             {
                Color accent = (Color)Resources["PhoneAccentColor"];
                 Color darkAccent = Color.FromArgb(accent.A, (byte)(accent.R / 2), (byte)(accent.G / 2), (byte)(accent.B / 2));
                 return new SolidColorBrush(darkAccent);
             }
         }*/

        /// <summary>
        /// Public constructor.
        /// </summary>
        public OutgoingChatBubble(ChatMessage message)
        {
            InitializeComponent();

            ChatMessage = message;
            string filePath = message.AppData;
            bool isImageMessage = filePath != null && filePath.Length > 0;
            if (isImageMessage)
            {
               // Message.Visibility = Visibility.Collapsed;
               // Copy.Visibility = Visibility.Collapsed;
               // Image.Visibility = Visibility.Visible;
               // Save.Visibility = Visibility.Visible;

                BitmapImage image = Utils.ReadImageFromIsolatedStorage(filePath);
                //Image.Source = image;
            }
            else
            {
                Message.Visibility = Visibility.Visible;
                Message.Text = message.Text;
                //Image.Visibility = Visibility.Collapsed;
                //Message.Blocks.Add(TextWithLinks);
            }

            Timestamp.Text = HumanFriendlyTimeStamp;
            //Background.Fill = _darkAccentBrush;
            //Path.Fill = _darkAccentBrush;
        }

        public string HumanFriendlyTimeStamp
        {
            get
            {
                DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                long unixTimeStampInTicks = (long)(ChatMessage.Time * TimeSpan.TicksPerSecond);
                DateTime date = new DateTime(unixStart.Ticks + unixTimeStampInTicks).ToLocalTime();
                return FormatDate(date);
            }
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


        /// <summary>
        /// Changes the icon indicating the status of the message (InProgress, Delivered or NotDelivered).
        /// </summary>
        public void UpdateStatus(ChatMessageState state)
        {
            //string delivered = "/Assets/AppBar/check.png";
            //string notdelivered = "/Assets/AppBar/stop.png";

            if (state == ChatMessageState.Delivered)
            {
              //  Status.Source = new BitmapImage(new Uri(delivered, UriKind.RelativeOrAbsolute));
            }
            else if (state == ChatMessageState.NotDelivered)
            {
               // Status.Source = new BitmapImage(new Uri(notdelivered, UriKind.RelativeOrAbsolute));
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageDeleted != null)
            {
                //MessageDeleted(this, ChatMessage);
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
          //  Clipboard.SetText(ChatMessage.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
           // bool result = Utils.SavePictureInMediaLibrary(ChatMessage.AppData);
          //  MessageBox.Show(result ? AppResources.FileSavingSuccess : AppResources.FileSavingFailure, AppResources.FileSaving, MessageBoxButton.OK);
        }

        /// <summary>
        /// Delegate for delete event.
        /// </summary>
        public delegate void MessageDeletedEventHandler(object sender, ChatMessage message);

        /// <summary>
        /// Handler for delete event.
        /// </summary>
        public event MessageDeletedEventHandler MessageDeleted;

      /*  private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/FullScreenPicture.xaml?uri=" + ChatMessage.AppData, UriKind.RelativeOrAbsolute));
        }*/
    }
}
