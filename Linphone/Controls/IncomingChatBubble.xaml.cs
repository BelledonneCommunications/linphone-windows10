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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using Linphone.Model;
using BelledonneCommunications.Linphone.Native;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Linphone.Controls
{
    public partial class IncomingChatBubble : UserControl
    {

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

            string fileName = message.FileTransferName;
            string filePath = message.AppData;
            bool isImageMessage = fileName != null && fileName.Length > 0;
            if (isImageMessage)
            {
                Message.Visibility = Visibility.Collapsed;
                //Copy.Visibility = Visibility.Collapsed;
                if (filePath != null && filePath.Length > 0)
                {
                    // Image already downloaded
                    Image.Visibility = Visibility.Visible;
//                  Save.Visibility = Visibility.Visible;

                    BitmapImage image = Utils.ReadImageFromIsolatedStorage(filePath);
                    Image.Source = image;
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
                Message.Blocks.Add(Utils.FormatText(message.Text));
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageDeleted != null)
            {
               // MessageDeleted(this, ChatMessage);
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
        //    Clipboard.SetText(ChatMessage.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //bool result = Utils.SavePictureInMediaLibrary(ChatMessage.AppData);
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
            //string filePath = ChatMessage.AppData;
           /* ProgressBar.Visibility = Visibility.Collapsed;
            if (filePath != null && filePath.Length > 0)
            {
                Download.Visibility = Visibility.Collapsed;
                Image.Visibility = Visibility.Visible;
               // Save.Visibility = Visibility.Visible;

                BitmapImage image = Utils.ReadImageFromIsolatedStorage(filePath);
                Image.Source = image;
            }
            else
            {
                Download.Visibility = Visibility.Visible;
            }*/
        }

       /* private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/FullScreenPicture.xaml?uri=" + ChatMessage.AppData, UriKind.RelativeOrAbsolute));
        }*/
    }
}
