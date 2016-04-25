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
using System.Net;
using System.Windows;
using Linphone.Model;
using System.Diagnostics;
using Linphone.Controls;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Net.Http.Headers;
using Windows.Storage;
using BelledonneCommunications.Linphone.Native;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;

namespace Linphone.Views
{
    /// <summary>
    /// Listener to let this view to be notified by LinphoneManager when a new message arrives.
    /// </summary>
    public interface MessageReceivedListener
    {
        /// <summary>
        /// Callback called when a message is received.
        /// </summary>
        void MessageReceived(ChatMessage message);

        /// <summary>
        /// Returns the sip address of the current displayed conversation if possible
        /// </summary>
        string GetSipAddressAssociatedWithDisplayConversation();
    }

    /// <summary>
    /// Listener to let this view to be notified by LinphoneManager when a composing is received.
    /// </summary>
    public interface ComposingReceivedListener
    {
        /// <summary>
        /// Callback called when a composing is received.
        /// </summary>
        void ComposeReceived();

        /// <summary>
        /// Returns the sip address of the current displayed conversation if possible
        /// </summary>
        string GetSipAddressAssociatedWithDisplayConversation();
    }

    /// <summary>
    /// Displays chat messages between two users.
    /// </summary>
    public partial class Chat : Page, ChatMessageListener, MessageReceivedListener, ComposingReceivedListener
    {
        private const int SENT_IMAGES_QUALITY = 50;

        private HttpClient _httpPostClient { get; set; }

        /// <summary>
        /// SIP address linked to the current displayed chat.
        /// </summary>
        public Address sipAddress;

        /// <summary>
        /// ChatRoom used to send and receive messages.
        /// </summary>
        public ChatRoom chatRoom;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Chat()
        {
            this.InitializeComponent();
           // BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Check if the uri contains a sip address, if yes, it displays the matching chat history.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LinphoneManager.Instance.MessageListener = this;
           // LinphoneManager.Instance.ComposingListener = this;

         //   ContactManager cm = ContactManager.Instance;
          //  cm.ContactFound += cm_ContactFound;

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
               // NewChatSipAddress.Focus();
            }
            RefreshSendMessageButtonEnabledState();

            scrollToBottom();
        }

        private void MessageBox_TextChanged(object sender, string text)
        {
            if (chatRoom != null && text.Length > 0)
                chatRoom.Compose();
            RefreshSendMessageButtonEnabledState();
        }

        private void NewChatSipAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshSendMessageButtonEnabledState();
        }

        private void DisplayPastMessages(IList<ChatMessage> messages)
        {
            foreach (ChatMessage message in messages)
            {
                if (!message.IsOutgoing)
                {
                    IncomingChatBubble bubble = new IncomingChatBubble(message);
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    bubble.DownloadImage += bubble_DownloadImage;
                    MessagesList.Children.Insert(MessagesList.Children.Count - 1, bubble);
                }
                else
                {
                    OutgoingChatBubble bubble = new OutgoingChatBubble(message);
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    bubble.UpdateStatus(message.State);
                    MessagesList.Children.Insert(MessagesList.Children.Count - 1, bubble);
                }
            }
            scrollToBottom();
        }

        /// <summary>
        /// Method called when this page isn't displayed anymore.
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            chatRoom = null;
           // LinphoneManager.Instance.MessageListener = null;
           // LinphoneManager.Instance.ComposingListener = null;
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Callback called when the search on a phone number or an email for a contact has a match
        /// </summary>
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

        private void SendMessage(string message)
        {
            if (chatRoom != null)
            {
                ChatMessage chatMessage = chatRoom.CreateMessage(message);
                chatRoom.SendMessage(chatMessage, this);
            }
        }

        /// <summary>
        /// Callback called by LinphoneCore when the state of a sent message changes.
        /// </summary>
        public void MessageStateChanged(ChatMessage message, ChatMessageState state)
        {
            Debug.WriteLine("[Chat] Message state changed: " + state.ToString());
            if (LinphoneManager.Instance.CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            LinphoneManager.Instance.CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (ProgressPopup.Visibility == Visibility.Visible)
                     {
                         ProgressPopup.Visibility = Visibility.Collapsed;
                         MessageBox.Visibility = Visibility.Visible;
                         AddSendButtonsToAppBar();
                     }

                     if (state == ChatMessageState.InProgress)
                     {
                         // Create the chat bubble for both text or image messages
                         OutgoingChatBubble bubble = new OutgoingChatBubble(message);
                         bubble.MessageDeleted += bubble_MessageDeleted;
                         MessagesList.Children.Insert(MessagesList.Children.Count - 1, bubble);
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
            Debug.WriteLine(sipAddress.AsStringUriOnly());
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
                // Logger.Err("Can't create chat room for sip address {0}", sipAddress);
                Debug.WriteLine("Cannot create chatroom");
                throw;
            }
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            if (_httpPostClient != null)
                _httpPostClient.CancelPendingRequests();
            // It will throw an exception that will be catched in task_Completed function and will reset the interface.
        }

        private void send_Click_1(object sender, RoutedEventArgs e)
        {
               if (NewChatSipAddress.Text != null || NewChatSipAddress.Visibility == Visibility.Collapsed)
             {
                Debug.WriteLine(NewChatSipAddress.Text);
                if (chatRoom == null) //This code will be executed only in case of new conversation
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
                         InitiateImageUpload(MessageBox.ImageLocalPath, MessageBox.ImageName);
                     } else
                    {
                        Debug.WriteLine("No text :)");
                    }
                     MessageBox.Reset();
                 }
                 else
                 {
                    Debug.WriteLine("Else");
                    // System.Windows.MessageBox.Show(AppResources.ChatRoomCreationError, AppResources.GenericError, MessageBoxButton.OK);
                 }
             }
        }

        private void back_click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void attach_image_Click_1(object sender, EventArgs e)
        {
           /* PhotoChooserTask task = new PhotoChooserTask();
            task.Completed += imageSelectionTask_Completed;
            task.ShowCamera = true;
            task.Show();*/
        }

        private void InitiateImageUpload(string filePath, string fileName)
        {
          /*  Dispatcher.BeginInvoke(() =>
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
            });*/
        }

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

    #region AppBar Management
    private void BuildLocalizedApplicationBar()
        {
           // ApplicationBar = new ApplicationBar();
            //AddSendButtonsToAppBar();
        }

        private void CleanAppBar()
        {
          /*  while (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }*/
        }

        private void RefreshSendMessageButtonEnabledState()
        {
          /*  ApplicationBarIconButton button = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (button.Text.Equals(AppResources.SendMessage))
            {
                button.IsEnabled = ShouldSendMessageButtonBeEnabled();
            }*/
        }

        private bool ShouldSendMessageButtonBeEnabled()
        {
            return true;
           /* return ((MessageBox.Text != null && MessageBox.Text.Length > 0) || (MessageBox.ImageName != null && MessageBox.ImageName.Length > 0 && MessageBox.ImageLocalPath != null && MessageBox.ImageLocalPath.Length > 0))
                && ((sipAddress != null) || (NewChatSipAddress.Text != null && NewChatSipAddress.Text.Length > 0));
                */
        }

        private void AddSendButtonsToAppBar()
        {
            CleanAppBar();

         /*   ApplicationBarIconButton appBarSend = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.message.send.png", UriKind.Relative));
            appBarSend.Text = AppResources.SendMessage;
            ApplicationBar.Buttons.Add(appBarSend);
            appBarSend.Click += send_Click_1;
            appBarSend.IsEnabled = ShouldSendMessageButtonBeEnabled();

            ApplicationBarIconButton appBarSendImage = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.paperclip.png", UriKind.Relative));
            appBarSendImage.Text = AppResources.SendPicture;
            ApplicationBar.Buttons.Add(appBarSendImage);
            appBarSendImage.Click += attach_image_Click_1;
            appBarSendImage.IsEnabled = true;*/
        }

        private void AddCancelUploadButtonInAppBar()
        {
            CleanAppBar();

           /* ApplicationBarIconButton appBarCancel = new ApplicationBarIconButton(new Uri("/Assets/AppBar/cancel.png", UriKind.Relative));
            appBarCancel.Text = AppResources.CancelUpload;
            ApplicationBar.Buttons.Add(appBarCancel);
            appBarCancel.Click += cancel_Click_1;
            appBarCancel.IsEnabled = true;*/
        }
        #endregion

        /// <summary>
        /// Callback called by LinphoneManager when a message is received.
        /// </summary>
        public void MessageReceived(ChatMessage message)
        {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            MessagesList.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
           {
                IncomingChatBubble bubble = new IncomingChatBubble(message);
                bubble.MessageDeleted += bubble_MessageDeleted;
                bubble.DownloadImage += bubble_DownloadImage;
                MessagesList.Children.Insert(MessagesList.Children.Count - 1, bubble);

                if (chatRoom != null)
                {
                    chatRoom.MarkAsRead();
                }

                scrollToBottom();
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        }
        
        /// <summary>
        /// Callback called by LinphoneManager when a composing is received.
        /// </summary>
        public void ComposeReceived()
        {
          /*  Dispatcher.BeginInvoke(() =>
            {
                UpdateComposingMessage();
                scrollToBottom();
            });*/
        }

        private void UpdateComposingMessage()
        {
          /*  if (chatRoom == null)
                return;

            bool isRemoteComposing = chatRoom.IsRemoteComposing;
            Debug.WriteLine("[Chat] Is remote composing ? " + isRemoteComposing);
            RemoteComposing.Visibility = isRemoteComposing ? Visibility.Visible : Visibility.Collapsed;

            string remoteName = chatRoom.PeerAddress.DisplayName;
            if (remoteName.Length <= 0)
                remoteName = chatRoom.PeerAddress.UserName;
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            RemoteComposing.Text = remoteName + loader.GetString("RemoteComposing");*/
        }

        /// <summary>
        /// Gets the sip address associated to the MessageReceivedListener
        /// </summary>
        /// <returns></returns>
        public string GetSipAddressAssociatedWithDisplayConversation()
        {
            return String.Format("{0}@{1}", sipAddress.UserName, sipAddress.Domain);
        }

        private void scrollToBottom()
        {
           MessagesScroll.UpdateLayout();
           MessagesScroll.ScrollToVerticalOffset(MessagesScroll.ScrollableHeight);
        }

        private void ChooseContact_Click(object sender, RoutedEventArgs e)
        {
          //  NavigationService.Navigate(new Uri("/Views/Contacts.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Callback called when a user selects the delete context menu
        /// </summary>
        public void bubble_MessageDeleted(object sender, ChatMessage message)
        {
           /* MessagesList.Children.Remove(sender as UserControl);
            if (chatRoom != null) 
            {
                chatRoom.DeleteMessage(message);
            }*/
        }

        /// <summary>
        /// Callback called when a user wants to download an image in a message
        /// </summary>
        public void bubble_DownloadImage(object sender, ChatMessage message)
        {
            EnableDownloadButtons(false);
            //this.Focus(); // Focus the page in order to remove focus from the text box and hide the soft keyboard
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