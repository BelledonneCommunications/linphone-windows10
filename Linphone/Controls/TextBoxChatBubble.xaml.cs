using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace Linphone.Controls
{
    /// <summary>
    /// Custom TextBox to look like a chat bubble.
    /// </summary>
    public partial class TextBoxChatBubble : UserControl
    {
        /// <summary>
        /// Public accessor to this control text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Public accessor to this control image.
        /// </summary>
        public BitmapImage Picture { get; set; }

        /// <summary>
        /// Public accessor to this control image name.
        /// </summary>
        public string PicturePath { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public TextBoxChatBubble()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Empty the content of the textbox.
        /// </summary>
        public void Reset()
        {
            Message.Text = "";
            PicturePath = "";
            Picture = null;
            Image.Visibility = Visibility.Collapsed;
            Message.Visibility = Visibility.Visible;
        }

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = Message.Text;
        }

        /// <summary>
        /// Displays the image the user is about to send
        /// </summary>
        /// <param name="bitmap">The bitmap to send</param>
        /// <param name="filename">The filename of the picture to send</param>
        public void SetImage(BitmapImage bitmap, string filename)
        {
            Picture = bitmap;
            PicturePath = filename;
            Image.Visibility = Visibility.Visible;
            Message.Visibility = Visibility.Collapsed;
            Image.Source = bitmap;
        }
    }
}
