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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Linphone.Model;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Linphone;

namespace Linphone.Controls {
    public partial class OutgoingChatBubble : UserControl {
        private ChatMessage _message;

        public ChatMessage ChatMessage {
            get {
                return _message;
            }
            set {
                _message = value;
            }
        }

        public OutgoingChatBubble(ChatMessage message) {
            InitializeComponent();

            ChatMessage = message;
            this.Holding += Bubble_Holding;
            string filePath = message.Appdata;
            bool isImageMessage = filePath != null && filePath.Length > 0;
            if (isImageMessage) {
                Message.Visibility = Visibility.Collapsed;
                //Copy.Visibility = Visibility.Collapsed;
                Image.Visibility = Visibility.Visible;
                //Save.Visibility = Visibility.Visible;
                SetImage(filePath);
            } else {
                Message.Visibility = Visibility.Visible;
                Message.Blocks.Add(Utils.FormatText(message.Text));
                Image.Visibility = Visibility.Collapsed;
            }

            Timestamp.Text = HumanFriendlyTimeStamp;
        }

        private async void SetImage(string name) {
            BitmapImage image = await Utils.ReadImageFromTempStorage(name);
            Image.Source = image;
        }

        public string HumanFriendlyTimeStamp {
            get {
                return Utils.FormatDate(ChatMessage.Time);
            }
        }

        public void UpdateStatus(ChatMessageState state) {
            if (state == ChatMessageState.InProgress) {
                Status.Glyph = "\uE72A";
            } else if (state == ChatMessageState.NotDelivered) {
                Status.Glyph = "\uE711";
            } else {
                Status.Visibility = Visibility.Collapsed;
            }
        }

        private void Bubble_Holding(object sender, HoldingRoutedEventArgs e) {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutMenu.ShowAt(senderElement);
        }

        private void Delete_Click(object sender, RoutedEventArgs e) {
            if (MessageDeleted != null) {
                MessageDeleted(this, ChatMessage);
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e) {
            //  Clipboard.SetText(ChatMessage.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            // bool result = Utils.SavePictureInMediaLibrary(ChatMessage.AppData);
            //  MessageBox.Show(result ? AppResources.FileSavingSuccess : AppResources.FileSavingFailure, AppResources.FileSaving, MessageBoxButton.OK);
        }

        public void MessageStateChanged(ChatMessage message, ChatMessageState state) {
        }

        public delegate void MessageDeletedEventHandler(object sender, ChatMessage message);
        public event MessageDeletedEventHandler MessageDeleted;

        /*  private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
          {
              BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/FullScreenPicture.xaml?uri=" + ChatMessage.AppData, UriKind.RelativeOrAbsolute));
          }*/
    }
}
