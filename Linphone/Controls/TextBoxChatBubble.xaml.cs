/*
TextBoxChatBubble.xaml.cs
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Linphone.Controls
{
   
    public partial class TextBoxChatBubble : UserControl
    {
        public string Text { get; set; }

        public string ImageName { get; set; }

        public string ImageLocalPath { get; set; }

        public delegate void SendFileClickEventHandler(object sender);
        public event SendFileClickEventHandler SendFileClick;

        public delegate void SendMessageClickEventHandler(object sender);
        public event SendMessageClickEventHandler SendMessageClick;

        public TextBoxChatBubble()
        {
            InitializeComponent(); SendMessage.IsEnabled = true;
        }

        public void Reset()
        {
            Message.Text = "";
            Text = null;
            ImageName = null;
            ImageLocalPath = null;
            SendMessage.IsEnabled = false;
            Message.Visibility = Visibility.Visible;
            Image.Visibility = Visibility.Collapsed;
        }

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = Message.Text;
            SendMessage.IsEnabled = (Text.Length > 0 ? true : false || Image.Visibility == Visibility.Visible);
            if (TextChanged != null)
            {           
                TextChanged(this, Message.Text);
            }
        }

        public void SetImage(BitmapImage image)
        {
            Image.Source = image;
            Image.Visibility = Visibility.Visible;
            Message.Visibility = Visibility.Collapsed;
            Message.Text = "";
            SendMessage.IsEnabled = true;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string previousText = Text;
            Reset();
            Text = previousText;
            Message.Text = Text;
        }

        public delegate void TextChangedEventHandler(object sender, string text);

        public event TextChangedEventHandler TextChanged;

        private void SendFile_Click(object sender, RoutedEventArgs e)
        {
            SendFileClick(this);
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessageClick(this);
        }
    }
}
