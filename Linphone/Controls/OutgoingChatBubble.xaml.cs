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

using Linphone.Model;
using BelledonneCommunications.Linphone.Native;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Linphone.Controls
{
    public partial class OutgoingChatBubble : UserControl, ChatMessageListener
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
                Message.Blocks.Add(Utils.FormatText(message.Text));
                //Image.Visibility = Visibility.Collapsed;
            }

            Timestamp.Text = HumanFriendlyTimeStamp;
        }

        public string HumanFriendlyTimeStamp
        {
            get
            {
                return Utils.FormatDate(ChatMessage.Time);
            }
        }

        public void UpdateStatus(ChatMessageState state)
        {
            if (state == ChatMessageState.InProgress)
            {
                Status.Glyph = "\uE72A";
            }
            else if (state == ChatMessageState.NotDelivered)
            {
                Status.Glyph = "\uE711";
            } else
            {
                Status.Visibility = Visibility.Collapsed;
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

        public void MessageStateChanged(ChatMessage message, ChatMessageState state)
        {
        }

        public delegate void MessageDeletedEventHandler(object sender, ChatMessage message);
        public event MessageDeletedEventHandler MessageDeleted;

      /*  private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/FullScreenPicture.xaml?uri=" + ChatMessage.AppData, UriKind.RelativeOrAbsolute));
        }*/
    }
}
