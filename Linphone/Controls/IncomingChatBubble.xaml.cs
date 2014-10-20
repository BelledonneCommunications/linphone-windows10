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
using Microsoft.Xna.Framework.Media;
using Linphone.Resources;
using Linphone.Core;

namespace Linphone.Controls
{
    /// <summary>
    /// Control to display received chat messages.
    /// </summary>
    public partial class IncomingChatBubble : ChatBubble
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public IncomingChatBubble(LinphoneChatMessage message) :
            base (message)
        {
            InitializeComponent();
            Timestamp.Text = HumanFriendlyTimeStamp;

            string fileName = message.GetFileTransferName();
            string filePath = message.GetAppData();
            bool isImageMessage = fileName != null && fileName.Length > 0;
            if (isImageMessage)
            {
                Message.Visibility = Visibility.Collapsed;
                Copy.Visibility = Visibility.Collapsed;
                if (filePath != null && filePath.Length > 0)
                {
                    // Image already downloaded
                    Image.Visibility = Visibility.Visible;
                    Save.Visibility = Visibility.Visible;

                    BitmapImage image = Utils.ReadImageFromIsolatedStorage(filePath);
                    Image.Source = image;
                }
                else
                {
                    // Image needs to be downloaded
                    DownloadImage.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Message.Visibility = Visibility.Visible;
                Image.Visibility = Visibility.Collapsed;
                Message.Text = message.GetText();
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
            /*bool result = Utils.SavePictureInMediaLibrary(ChatMessage.ImageURL);
            MessageBox.Show(result ? AppResources.FileSavingSuccess : AppResources.FileSavingFailure, AppResources.FileSaving, MessageBoxButton.OK);*/
        }

        /// <summary>
        /// Delegate for delete event.
        /// </summary>
        public delegate void MessageDeletedEventHandler(object sender, LinphoneChatMessage message);

        /// <summary>
        /// Handler for delete event.
        /// </summary>
        public event MessageDeletedEventHandler MessageDeleted;

        private void DownloadImage_Click(object sender, RoutedEventArgs e)
        {
            /*DownloadImage.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Visible;
            BitmapImage image = Utils.GetThumbnailBitmapFromImage(Utils.DownloadImageAndStoreItInIsolatedStorage(ChatMessage.ImageURL, ChatMessage));
            if (image != null)
            {
                Image.Visibility = Visibility.Visible;
                Image.Source = image;
                Save.Visibility = Visibility.Visible;
            }
            else
            {
                DownloadImage.Visibility = Visibility.Visible;
            }
            ProgressBar.Visibility = Visibility.Collapsed;*/
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/FullScreenPicture.xaml?uri=" + ChatMessage.ImageURL, UriKind.RelativeOrAbsolute));
        }
    }
}
