/*
IncomingChatBubble.xaml.cs
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

using System;
using Linphone.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Input;
using Linphone;

namespace Linphone.Controls
{
    public partial class IncomingChatBubble : UserControl
    {

        public delegate void ImageTappedEventHandler(object sender, String appData);
        public event ImageTappedEventHandler ImageTapped;

        private ChatMessage _message;

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

        public IncomingChatBubble(ChatMessage message)
        {
            InitializeComponent();
            ChatMessage = message;
            Timestamp.Text = HumanFriendlyTimeStamp;

            this.Holding += Bubble_Holding;
            string fileName = (message.FileTransferInformation != null) ? message.FileTransferInformation.Name : null;
            bool isImageMessage = fileName != null && fileName.Length > 0;
            if (isImageMessage)
            {
                Message.Visibility = Visibility.Collapsed;
                //Copy.Visibility = Visibility.Collapsed;
                if (message.Appdata != null && message.Appdata.Length > 0)
                {
                    // Image already downloaded
                    Image.Visibility = Visibility.Visible;
                    Download.Visibility = Visibility.Collapsed;
                    SetImage(message.Appdata);
                }
                else
                {
                    // Image needs to be downloaded
                    Download.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Message.Visibility = Visibility.Visible;
                Image.Visibility = Visibility.Collapsed;
                Download.Visibility = Visibility.Collapsed;
                Message.Blocks.Add(Utils.FormatText(message.TextContent));
            }
        }

        private async void SetImage(string name)
        {
            BitmapImage image = await Utils.ReadImageFromTempStorage(name);
            Image.Source = image;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageDeleted != null)
            {
                MessageDeleted(this, ChatMessage);
            }
        }

        private void Bubble_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutMenu.ShowAt(senderElement);
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            //    Clipboard.SetText(ChatMessage.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // bool result = await Utils.SavePictureInMediaLibrary(ChatMessage.AppData);
            // MessageBox.Show(result ? AppResources.FileSavingSuccess : AppResources.FileSavingFailure, AppResources.FileSaving, MessageBoxButton.OK);
        }

        /// <summary>
        /// Delegate for delete event.
        /// </summary>
        public delegate void MessageDeletedEventHandler(object sender, ChatMessage message);

        /// <summary>
        /// Handler for delete event.
        /// </summary>
        public event MessageDeletedEventHandler MessageDeleted;

        /// <summary>
        /// Delegate for download event.
        /// </summary>
        public delegate void DownloadImageEventHandler(object sender, ChatMessage message);

        /// <summary>
        /// Handler for download event.
        /// </summary>
        public event DownloadImageEventHandler DownloadImage;

        private void DownloadImage_Click(object sender, RoutedEventArgs e)
        {
            if (DownloadImage != null)
            {
                Download.Visibility = Visibility.Collapsed;
                ProgressBar.Visibility = Visibility.Visible;
                DownloadImage(this, ChatMessage);
            }
        }

        public string HumanFriendlyTimeStamp
        {
            get
            {
                return Utils.FormatDate(ChatMessage.Time);
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
        /// Displays the image in the bubble
        /// </summary>
        public void RefreshImage()
        {
            string fileName = (ChatMessage.FileTransferInformation != null) ? ChatMessage.FileTransferInformation.Name : null;
            bool isImageMessage = fileName != null && fileName.Length > 0;
            if (isImageMessage)
            {
                string filePath = ChatMessage.Contents.GetEnumerator().Current.FilePath;
                ProgressBar.Visibility = Visibility.Collapsed;
                if (ChatMessage.Appdata != null && ChatMessage.Appdata.Length > 0)
                {
                    Download.Visibility = Visibility.Collapsed;
                    Image.Visibility = Visibility.Visible;
                    // Save.Visibility = Visibility.Visible;
                    SetImage(ChatMessage.Appdata);
                }
                else
                {
                    Download.Visibility = Visibility.Visible;
                }
            }
        }

        private void Image_Tap(object sender, TappedRoutedEventArgs e)
        {
            ImageTapped(this, ChatMessage.Contents.GetEnumerator().Current.FilePath);
        }
    }
}
