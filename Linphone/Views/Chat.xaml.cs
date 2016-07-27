/*
Chat.xaml.cs
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
using System.Collections.Generic;
using System.Linq;
using Linphone.Model;
using System.Diagnostics;
using Linphone.Controls;
using System.Net.Http;
using BelledonneCommunications.Linphone.Native;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.IO;
using Windows.UI.Popups;
using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

namespace Linphone.Views
{
    public interface MessageReceivedListener
    {
        void MessageReceived(ChatMessage message);
        string GetSipAddressAssociatedWithDisplayConversation();
    }

    public interface ComposingReceivedListener
    {
        void ComposeReceived();
        string GetSipAddressAssociatedWithDisplayConversation();
    }

    public partial class Chat : Page, ChatMessageListener, MessageReceivedListener, ComposingReceivedListener
    {
        private const int SENT_IMAGES_QUALITY = 50;

        private HttpClient _httpPostClient { get; set; }

        public Address sipAddress;
        public ChatRoom chatRoom;
        private String upload_filename;
        private ChatMessage messageUploading = null;

        public Chat()
        {
            this.InitializeComponent();
            MessageBox.SendFileClick += send_file;
            MessageBox.SendMessageClick += send_message;

            SystemNavigationManager.GetForCurrentView().BackRequested += Back_requested;
        }

        private void Back_requested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(Views.Chats), null);
            }
            e.Handled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LinphoneManager.Instance.MessageListener = this;
            LinphoneManager.Instance.ComposingListener = this;

            //ContactManager cm = ContactManager.Instance;
            //cm.ContactFound += cm_ContactFound;

            MessageBox.TextChanged += MessageBox_TextChanged;

            NewChat.Visibility = Visibility.Collapsed;
            ContactName.Visibility = Visibility.Visible;
            if (e.Parameter is String)
            {
                sipAddress = LinphoneManager.Instance.Core.InterpretURL(e.Parameter as String);
                CreateChatRoom(sipAddress);
                UpdateComposingMessage();
                chatRoom.MarkAsRead();
                DisplayPastMessages(chatRoom.History);
            }
            if (sipAddress == null)
            {
                ContactName.Visibility = Visibility.Collapsed;
                NewChat.Visibility = Visibility.Visible;
            }
            scrollToBottom();
        }

        #region Events
        private void MessageBox_TextChanged(object sender, string text)
        {
            if (chatRoom != null && text.Length > 0)
                chatRoom.Compose();
        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            if (chatRoom == null)
            {
                LinphoneManager.Instance.NewOutgoingCall(LinphoneManager.Instance.Core.InterpretURL(NewChatSipAddress.Text).AsStringUriOnly());
            }
            else {
                LinphoneManager.Instance.NewOutgoingCall(chatRoom.PeerAddress.AsStringUriOnly());
            }
        }

        private void Cancel_Upload(object sender, RoutedEventArgs e)
        {
            if (_httpPostClient != null)
                _httpPostClient.CancelPendingRequests();
        }

        private void SendMessage(string message)
        {
            if (chatRoom != null)
            {
                ChatMessage chatMessage = chatRoom.CreateMessage(message);
                chatRoom.SendMessage(chatMessage, this);
            }
        }

        private void ChooseContact_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ContactList), null);
        }

        #endregion

        private void DisplayPastMessages(IList<ChatMessage> messages)
        {
            foreach (ChatMessage message in messages)
            {
                if (!message.IsOutgoing)
                {
                    IncomingChatBubble bubble = new IncomingChatBubble(message);
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    bubble.DownloadImage += bubble_DownloadImage;
                    bubble.ImageTapped += Bubble_ImageTapped;
                    MessagesList.Children.Add(bubble);
                }
                else
                {
                    OutgoingChatBubble bubble = new OutgoingChatBubble(message);
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    bubble.UpdateStatus(message.State);
                    MessagesList.Children.Add(bubble);
                }
            }
            scrollToBottom();
        }

        private void Bubble_ImageTapped(object sender, string appData)
        {
            Frame.Navigate(typeof(Views.FullScreenPicture), appData);
        }

        private void bubble_MessageDeleted(object sender, ChatMessage message)
        {
            MessagesList.Children.Remove(sender as UserControl);
            if (chatRoom != null)
            {
                chatRoom.DeleteMessage(message);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            chatRoom = null;
            upload_filename = "";
            LinphoneManager.Instance.MessageListener = null;
            LinphoneManager.Instance.ComposingListener = null;
            MessageBox.TextChanged -= MessageBox_TextChanged;
            base.OnNavigatingFrom(e);
        }

        /*  private void cm_ContactFound(object sender, ContactFoundEventArgs e)
          {
              if (e.ContactFound != null)
              {
                  ContactName.Text = e.ContactFound.DisplayName;
                  ContactManager.Instance.TempContact = e.ContactFound;
                  ContactName.Tap += ContactName_Tap;

                  ContactName.Visibility = Visibility.Visible;
                  NewChat.Visibility = Visibility.Collapsed;
              }
          }*/

        /* private void ContactName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
         {
             NavigationService.Navigate(new Uri("/Views/Contact.xaml", UriKind.RelativeOrAbsolute));
         }*/


        public void MessageStateChanged(ChatMessage message, ChatMessageState state)
        {
            if (LinphoneManager.Instance.CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            LinphoneManager.Instance.CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (ProgressPopup.Visibility == Visibility.Visible)
                {
                    ProgressPopup.Visibility = Visibility.Collapsed;
                    MessageBox.Visibility = Visibility.Visible;
                }

                if (state == ChatMessageState.Delivered)
                {
                    if (messageUploading != null && messageUploading.Equals(message))
                    {
                        messageUploading = null;
                    }
                }

                if (state == ChatMessageState.InProgress && message.IsOutgoing && messageUploading == null)
                {
                    // Create the chat bubble for both text or image messages
                    if (message.AppData != null && messageUploading == null)
                    {
                        messageUploading = message;
                    }
                    OutgoingChatBubble bubble = new OutgoingChatBubble(message);
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    MessagesList.Children.Add(bubble);
                    scrollToBottom();
                }
                else if (state == ChatMessageState.FileTransferDone && !message.IsOutgoing)
                {
                    try
                    {
                        message.AppData = upload_filename;
                        IncomingChatBubble bubble = (IncomingChatBubble)MessagesList.Children.OfType<IncomingChatBubble>().Where(b => message.Equals(((IncomingChatBubble)b).ChatMessage)).Last();
                        if (bubble != null)
                        {
                            ((IncomingChatBubble)bubble).RefreshImage();
                        }
                        EnableDownloadButtons(true);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        foreach (OutgoingChatBubble bubble in MessagesList.Children.OfType<OutgoingChatBubble>())
                        {
                            if (bubble.ChatMessage.Equals(message))
                            {
                                bubble.UpdateStatus(state);
                            }
                        }
                    }
                    catch { }
                }

                if (chatRoom != null)
                {
                    chatRoom.MarkAsRead();
                }
            });
        }

        private void CreateChatRoom(Address sipAddress)
        {
            this.sipAddress = sipAddress;
            //ContactManager.Instance.FindContact(String.Format("{0}@{1}", sipAddress.UserName, sipAddress.Domain));
            ContactName.Text = sipAddress.UserName;
            ContactName.Visibility = Visibility.Visible;
            NewChat.Visibility = Visibility.Collapsed;

            try
            {
                chatRoom = LinphoneManager.Instance.Core.GetChatRoom(sipAddress);
            }
            catch
            {
                Debug.WriteLine("Cannot create chatroom");
                throw;
            }
        }

        private void send_message(object sender)
        {
            if (NewChatSipAddress.Text != null || NewChatSipAddress.Visibility == Visibility.Collapsed)
            {
                if (chatRoom == null)
                {
                    CreateChatRoom(LinphoneManager.Instance.Core.InterpretURL(NewChatSipAddress.Text));
                }

                if (chatRoom != null)
                {
                    if (MessageBox.Text != null && MessageBox.Text.Length > 0)
                    {
                        SendMessage(MessageBox.Text);
                    }
                    else if (MessageBox.ImageName != null && MessageBox.ImageLocalPath != null)
                    {
                        InitiateImageUpload(MessageBox.ImageLocalPath, MessageBox.ImageName);
                    }
                    else
                    {
                        Debug.WriteLine("No text :)");
                    }
                    MessageBox.Reset();
                }
                else
                {
                    //TODO error message
                }
            }
        }

        private async void send_file(object sender)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                imageSelectionTask_Completed(file);
            }
            else
            {
                //TODO error message
            }
        }

        private void back_click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void InitiateImageUpload(string filePath, string fileName)
        {

            if (chatRoom == null) //This code will be executed only in case of new conversation
            {
                CreateChatRoom(LinphoneManager.Instance.Core.InterpretURL(NewChatSipAddress.Text));
            }
            if (chatRoom != null)
            {
                ProgressPopup.Visibility = Visibility.Visible;
                MessageBox.Visibility = Visibility.Collapsed;
                //CancelUpload.Visibility = Visibility.Visible;
                Task.Run(() =>
                {
                    FileInfo fileInfo;
                    try
                    {
                        fileInfo = new FileInfo(filePath);
                        ChatMessage msg = chatRoom.CreateFileTransferMessage("application", "octet-stream", fileName, (int)fileInfo.Length, filePath);
                        msg.AppData = fileName;
                        chatRoom.SendMessage(msg, this);
                    }
                    catch (Exception e)
                    {

                    }
                });

            }
            else
            {
                ProgressPopup.Visibility = Visibility.Collapsed;
                MessageBox.Visibility = Visibility.Visible;
                var messageDialog = new MessageDialog(ResourceLoader.GetForCurrentView().GetString("ChatRoomCreationError"), ResourceLoader.GetForCurrentView().GetString("GenericError"));
                await messageDialog.ShowAsync();
                //CancelUpload.Visibility = Visibility.Collapsed;
            }

        }

        private async void imageSelectionTask_Completed(StorageFile file)
        {

            string fileName = file.Name;
            BitmapImage image = new BitmapImage();
            IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
            try
            {
                image.SetSource(fileStream);
            }
            catch (Exception e)
            {

            }
            /*ChatSettingsManager chatMgr = new ChatSettingsManager();
            chatMgr.Load();
            if ((bool)chatMgr.ScaleDownSentPictures)
            {
                // Resize down the image
                WriteableBitmap bm = new WriteableBitmap(image);
                MemoryStream ms = new MemoryStream();
                int w = image.PixelWidth;
                int h = image.PixelHeight;
                if (w > h && w > 500)
                {
                    h = (500 * h) / w;
                    w = 500;
                }
                else if (h > w && h > 500)
                {
                    w = (500 * w) / h;
                    h = 500;
                }
                bm.SaveJpeg(ms, w, h, 0, 100);
                image.SetSource(ms);
            }*/
            string filename = await Utils.SaveImageInLocalFolder(file);
            MessageBox.SetImage(image);
            MessageBox.ImageName = filename;
            var tempFolder = ApplicationData.Current.LocalFolder;
            StorageFile tempFile = await tempFolder.GetFileAsync(filename);
            MessageBox.ImageLocalPath = tempFile.Path;
            // RefreshSendMessageButtonEnabledState();
        }

        public void MessageReceived(ChatMessage message)
        {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            MessagesList.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IncomingChatBubble bubble = new IncomingChatBubble(message);
                bubble.MessageDeleted += bubble_MessageDeleted;
                bubble.DownloadImage += bubble_DownloadImage;
                MessagesList.Children.Add(bubble);

                if (chatRoom != null)
                {
                    chatRoom.MarkAsRead();
                }

                scrollToBottom();
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        }

        public void ComposeReceived()
        {
            UpdateComposingMessage();
        }

        private void UpdateComposingMessage()
        {
            if (chatRoom == null)
                return;

            bool isRemoteComposing = chatRoom.IsRemoteComposing;
            RemoteComposing.Visibility = isRemoteComposing ? Visibility.Visible : Visibility.Collapsed;

            string remoteName = chatRoom.PeerAddress.DisplayName;
            if (remoteName.Length <= 0)
                remoteName = chatRoom.PeerAddress.UserName;
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            RemoteComposing.Text = remoteName + loader.GetString("RemoteComposing");
        }

        public string GetSipAddressAssociatedWithDisplayConversation()
        {
            return String.Format("{0}@{1}", sipAddress.UserName, sipAddress.Domain);
        }

        private void scrollToBottom()
        {
            MessagesScroll.UpdateLayout();
            MessagesScroll.ScrollToVerticalOffset(MessagesScroll.ScrollableHeight);
        }

        public async void bubble_DownloadImage(object sender, ChatMessage message)
        {
            EnableDownloadButtons(false);
            var tempFolder = ApplicationData.Current.LocalFolder;
            string name = await Utils.GetImageTempFileName();
            StorageFile tempFile = await tempFolder.GetFileAsync(name);
            upload_filename = name;
            message.StartFileDownload(this, tempFile.Path);
        }

        private void EnableDownloadButtons(bool enable)
        {
            foreach (IncomingChatBubble bubble in MessagesList.Children.OfType<IncomingChatBubble>())
            {
                // (bubble as IncomingChatBubble).Download.IsEnabled = enable;  
            }
        }
    }
}