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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Utils.SavePictureInMediaLibrary(ChatMessage.ImageURL);
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
            DownloadImage.Visibility = Visibility.Collapsed;
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
            ProgressBar.Visibility = Visibility.Collapsed;

        }

        private void ShowImage_Click(object sender, RoutedEventArgs e)
        {
            Image.Source = Utils.GetThumbnailBitmapFromImage(Utils.ReadImageFromIsolatedStorage(ChatMessage.ImageURL));
            ShowImage.Visibility = Visibility.Collapsed;
            Image.Visibility = Visibility.Visible;
            Save.Visibility = Visibility.Visible;
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/FullScreenPicture.xaml?uri=" + ChatMessage.ImageURL, UriKind.RelativeOrAbsolute));
        }
    }
}
