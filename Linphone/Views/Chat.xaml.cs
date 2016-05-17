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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
using Windows.UI.Popups;

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

        public Chat()
        {
            this.InitializeComponent();
            MessageBox.SendFileClick += send_file;
            MessageBox.SendMessageClick += send_message;
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

                if (e.NavigationMode != NavigationMode.Back)
                {
                    chatRoom.MarkAsRead();
                    DisplayPastMessages(chatRoom.History);
                }
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

        private void bubble_MessageDeleted(object sender, ChatMessage message)
        {
            //TODO
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            chatRoom = null;
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

                     if (state == ChatMessageState.InProgress)
                     {
                         // Create the chat bubble for both text or image messages
                         OutgoingChatBubble bubble = new OutgoingChatBubble(message);
                         bubble.MessageDeleted += bubble_MessageDeleted;
                         MessagesList.Children.Add(bubble);
                         scrollToBottom();
                     }
                     else if (state == ChatMessageState.FileTransferDone && !message.IsOutgoing)
                     {
                         try
                         {
                             message.AppData = message.FileTransferFilepath;
                            IncomingChatBubble bubble = (IncomingChatBubble)MessagesList.Children.Where(b => message.Equals(((IncomingChatBubble)b).ChatMessage)).Last();
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
                         // Update the outgoing status of the message
                         try
                         {
                             OutgoingChatBubble bubble = (OutgoingChatBubble)MessagesList.Children.Where(b => message.Equals(((OutgoingChatBubble)b).ChatMessage)).Last();
                             if (bubble != null)
                             {
                                 ((OutgoingChatBubble)bubble).UpdateStatus(state);
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
                        Debug.WriteLine(MessageBox.Text);
                        SendMessage(MessageBox.Text);
                    }
                    else if (MessageBox.ImageName != null && MessageBox.ImageLocalPath != null)
                    {
                        //InitiateImageUpload(MessageBox.ImageLocalPath, MessageBox.ImageName);
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
        }

        private void back_click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }


        /* 
        private void InitiateImageUpload(string filePath, string fileName)
      { Dispatcher.BeginInvoke(() =>
          {
              if (chatRoom == null) //This code will be executed only in case of new conversation
              {
                  CreateChatRoom(LinphoneManager.Instance.LinphoneCore.InterpretURL(NewChatSipAddress.Text));
              }
              if (chatRoom != null)
              {
                  ProgressPopup.Visibility = Visibility.Visible;
                  MessageBox.Visibility = Visibility.Collapsed;
                  AddCancelUploadButtonInAppBar();

                  FileInfo fileInfo = new FileInfo(filePath);
                  LinphoneChatMessage msg = chatRoom.CreateFileTransferMessage("application", "octet-stream", fileName, (int)fileInfo.Length, filePath);
                  msg.AppData = filePath;
                  chatRoom.SendMessage(msg, this);
              }
              else
              {
                  ProgressPopup.Visibility = Visibility.Collapsed;
                  MessageBox.Visibility = Visibility.Visible;
                  AddSendButtonsToAppBar();
                  System.Windows.MessageBox.Show(AppResources.ChatRoomCreationError, AppResources.GenericError, MessageBoxButton.OK);
              }
          });
    }*/

    /*  private void imageSelectionTask_Completed(object sender, PhotoResult e)
      {
          if (e.TaskResult == TaskResult.OK)
          {
              string fileName = e.OriginalFileName.Substring(e.OriginalFileName.LastIndexOf("\\") + 1);
              BitmapImage image = new BitmapImage();
              image.SetSource(e.ChosenPhoto);

              ChatSettingsManager chatMgr = new ChatSettingsManager();
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
              }
              string filePath = Utils.SaveImageInLocalFolder(image, fileName);
              MessageBox.SetImage(image);
              MessageBox.ImageName = fileName;
              MessageBox.ImageLocalPath = filePath;
              RefreshSendMessageButtonEnabledState();
          }
      }*/

        public void MessageReceived(ChatMessage message)
        {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
           MessagesList.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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

        public void bubble_DownloadImage(object sender, ChatMessage message)
        {
            EnableDownloadButtons(false);
            message.StartFileDownload(this, Utils.GetImageRandomFileName());
        }

        private void EnableDownloadButtons(bool enable)
        {
            foreach (ChatBubble bubble in MessagesList.Children)
            {
                if (bubble.GetType() == typeof(IncomingChatBubble))
                {
                   // (bubble as IncomingChatBubble).Download.IsEnabled = enable;
                }
            }
        }
    }
}