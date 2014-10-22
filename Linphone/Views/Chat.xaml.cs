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
using Linphone.Core;
using Linphone.Resources;
using System.Diagnostics;
using Linphone.Controls;
using Microsoft.Phone.Tasks;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Net.Http.Headers;
using Microsoft.Xna.Framework.Media;
using Linphone.Agents;
using Windows.Storage;

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
        void MessageReceived(LinphoneChatMessage message);

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
    public partial class Chat : BasePage, LinphoneChatMessageListener, MessageReceivedListener, ComposingReceivedListener
    {
        private const int SENT_IMAGES_QUALITY = 50;

        private HttpClient _httpPostClient { get; set; }

        /// <summary>
        /// SIP address linked to the current displayed chat.
        /// </summary>
        public string sipAddress;

        /// <summary>
        /// ChatRoom used to send and receive messages.
        /// </summary>
        public LinphoneChatRoom chatRoom;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Chat()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Check if the uri contains a sip address, if yes, it displays the matching chat history.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LinphoneManager.Instance.MessageListener = this;
            LinphoneManager.Instance.ComposingListener = this;

            // Create LinphoneCore if not created yet, otherwise do nothing
            LinphoneManager.Instance.InitLinphoneCore();

            ContactManager cm = ContactManager.Instance;
            cm.ContactFound += cm_ContactFound;

            MessageBox.TextChanged += MessageBox_TextChanged;

            NewChat.Visibility = Visibility.Collapsed;
            ContactName.Visibility = Visibility.Visible; 
            if (NavigationContext.QueryString.ContainsKey("sip"))
            {
                sipAddress = NavigationContext.QueryString["sip"];
                if (sipAddress.StartsWith("sip:"))
                {
                    sipAddress = sipAddress.Replace("sip:", "");
                }

                CreateChatRoom(sipAddress);
                UpdateComposingMessage();

                if (e.NavigationMode != NavigationMode.Back)
                {
                    chatRoom.MarkAsRead();
                    DisplayPastMessages(chatRoom.GetHistory());
                }
            }
            else if (e.NavigationMode != NavigationMode.Back || sipAddress == null || sipAddress.Length == 0)
            {
                ContactName.Visibility = Visibility.Collapsed; 
                NewChat.Visibility = Visibility.Visible;
                NewChatSipAddress.Focus();
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

        /// <summary>
        /// Add ChatList in the history in case view was directly started (PN)
        /// </summary>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (!NavigationService.CanGoBack)
            {
                e.Cancel = true;
                NavigationService.Navigate(new Uri("/Views/Chats.xaml", UriKind.RelativeOrAbsolute));
                NavigationService.RemoveBackEntry(); //To prevent a new click on back button to start again chat view (simulate a back click)
            }
        }

        private void DisplayPastMessages(IList<Object> messages)
        {
            foreach (LinphoneChatMessage message in messages)
            {
                if (!message.IsOutgoing())
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
                    bubble.UpdateStatus(message.GetState());
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
            LinphoneManager.Instance.MessageListener = null;
            LinphoneManager.Instance.ComposingListener = null;
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Callback called when the search on a phone number or an email for a contact has a match
        /// </summary>
        private void cm_ContactFound(object sender, ContactFoundEventArgs e)
        {
            if (e.ContactFound != null)
            {
                ContactName.Text = e.ContactFound.DisplayName;
                ContactManager.Instance.TempContact = e.ContactFound;
                ContactName.Tap += ContactName_Tap;

                ContactName.Visibility = Visibility.Visible;
                NewChat.Visibility = Visibility.Collapsed;
            }
        }

        private void ContactName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Contact.xaml", UriKind.RelativeOrAbsolute));
        }

        private void SendMessage(string message)
        {
            if (chatRoom != null)
            {
                LinphoneChatMessage chatMessage = chatRoom.CreateLinphoneChatMessage(message);
                chatRoom.SendMessage(chatMessage, this);
            }
        }

        /// <summary>
        /// Callback called by LinphoneCore when the state of a sent message changes.
        /// </summary>
        public void MessageStateChanged(LinphoneChatMessage message, LinphoneChatMessageState state)
        {
            Logger.Msg("[Chat] Message state changed: " + state.ToString());

            Dispatcher.BeginInvoke(() =>
            {
                if (ProgressPopup.Visibility == Visibility.Visible)
                {
                    ProgressPopup.Visibility = Visibility.Collapsed;
                    MessageBox.Visibility = Visibility.Visible;
                    AddSendButtonsToAppBar();
                }

                if (state == LinphoneChatMessageState.InProgress)
                {
                    // Create the chat bubble for both text or image messages
                    OutgoingChatBubble bubble = new OutgoingChatBubble(message);
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    MessagesList.Children.Insert(MessagesList.Children.Count - 1, bubble);
                    scrollToBottom();
                }
                else if (state == LinphoneChatMessageState.FileTransferDone && !message.IsOutgoing())
                {
                    try
                    {
                        message.SetAppData(message.GetFileTransferFilePath());
                        ChatBubble bubble = (ChatBubble)MessagesList.Children.Where(b => message.Equals(((ChatBubble)b).ChatMessage)).Last();
                        if (bubble != null)
                        {
                            ((IncomingChatBubble)bubble).RefreshImage();
                        }
                    }
                    catch { }
                }
                else
                {
                    // Update the outgoing status of the message
                    try
                    {
                        ChatBubble bubble = (ChatBubble)MessagesList.Children.Where(b => message.Equals(((ChatBubble)b).ChatMessage)).Last();
                        if (bubble != null)
                        {
                            ((OutgoingChatBubble)bubble).UpdateStatus(state);
                        }
                    }
                    catch { }
                }
            });
        }

        private void CreateChatRoom(string sipAddress)
        {
            if (!sipAddress.Contains("@"))
            {
                if (LinphoneManager.Instance.LinphoneCore.GetProxyConfigList().Count > 0)
                {
                    LinphoneProxyConfig config = LinphoneManager.Instance.LinphoneCore.GetProxyConfigList()[0] as LinphoneProxyConfig;
                    sipAddress += "@" + config.GetDomain();
                }
                else
                {
                    System.Windows.MessageBox.Show(AppResources.InvalidSipAddressError, AppResources.GenericError, MessageBoxButton.OK);
                    return;
                }
            }

            this.sipAddress = sipAddress;
            ContactManager.Instance.FindContact(sipAddress);
            string displayedSipAddress = sipAddress;
            if (displayedSipAddress.Contains("@"))
            {
                displayedSipAddress = displayedSipAddress.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            ContactName.Text = displayedSipAddress;
            ContactName.Visibility = Visibility.Visible;
            NewChat.Visibility = Visibility.Collapsed;

            try
            {
                chatRoom = LinphoneManager.Instance.LinphoneCore.GetOrCreateChatRoom(sipAddress);
            }
            catch
            {
                Logger.Err("Can't create chat room for sip address {0}", sipAddress);
                throw;
            }
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            if (_httpPostClient != null)
                _httpPostClient.CancelPendingRequests();
            // It will throw an exception that will be catched in task_Completed function and will reset the interface.
        }

        private void send_Click_1(object sender, EventArgs e)
        {
            if (NewChatSipAddress.Text != null || NewChatSipAddress.Visibility == Visibility.Collapsed)
            {
                if (chatRoom == null) //This code will be executed only in case of new conversation
                {
                    CreateChatRoom(NewChatSipAddress.Text);
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
                    MessageBox.Reset();
                }
                else
                {
                    System.Windows.MessageBox.Show(AppResources.ChatRoomCreationError, AppResources.GenericError, MessageBoxButton.OK);
                }
            }
        }

        private void attach_image_Click_1(object sender, EventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.Completed += imageSelectionTask_Completed;
            task.ShowCamera = true;
            task.Show();
        }

        private void InitiateImageUpload(string filePath, string fileName)
        {
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                if (chatRoom == null) //This code will be executed only in case of new conversation
                {
                    CreateChatRoom(NewChatSipAddress.Text);
                }
                if (chatRoom != null)
                {
                    ProgressPopup.Visibility = Visibility.Visible;
                    MessageBox.Visibility = Visibility.Collapsed;
                    AddCancelUploadButtonInAppBar();

                    FileInfo fileInfo = new FileInfo(filePath);
                    LinphoneChatMessage msg = chatRoom.CreateFileTransferMessage("application", "octet-stream", fileName, (int)fileInfo.Length, filePath);
                    msg.SetAppData(filePath);
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
        }

        private void imageSelectionTask_Completed(object sender, PhotoResult e)
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
        }

        #region AppBar Management
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            AddSendButtonsToAppBar();
        }

        private void CleanAppBar()
        {
            while (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }
        }

        private void RefreshSendMessageButtonEnabledState()
        {
            ApplicationBarIconButton button = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (button.Text.Equals(AppResources.SendMessage))
            {
                button.IsEnabled = ShouldSendMessageButtonBeEnabled();
            }
        }

        private bool ShouldSendMessageButtonBeEnabled()
        {
            return ((MessageBox.Text != null && MessageBox.Text.Length > 0) || (MessageBox.ImageName != null && MessageBox.ImageName.Length > 0 && MessageBox.ImageLocalPath != null && MessageBox.ImageLocalPath.Length > 0))
                && ((sipAddress != null && sipAddress.Length > 0) || (NewChatSipAddress.Text != null && NewChatSipAddress.Text.Length > 0));
        }

        private void AddSendButtonsToAppBar()
        {
            CleanAppBar();

            ApplicationBarIconButton appBarSend = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.message.send.png", UriKind.Relative));
            appBarSend.Text = AppResources.SendMessage;
            ApplicationBar.Buttons.Add(appBarSend);
            appBarSend.Click += send_Click_1;
            appBarSend.IsEnabled = ShouldSendMessageButtonBeEnabled();

            ApplicationBarIconButton appBarSendImage = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.paperclip.png", UriKind.Relative));
            appBarSendImage.Text = AppResources.SendPicture;
            ApplicationBar.Buttons.Add(appBarSendImage);
            appBarSendImage.Click += attach_image_Click_1;
            appBarSendImage.IsEnabled = true;
        }

        private void AddCancelUploadButtonInAppBar()
        {
            CleanAppBar();

            ApplicationBarIconButton appBarCancel = new ApplicationBarIconButton(new Uri("/Assets/AppBar/cancel.png", UriKind.Relative));
            appBarCancel.Text = AppResources.CancelUpload;
            ApplicationBar.Buttons.Add(appBarCancel);
            appBarCancel.Click += cancel_Click_1;
            appBarCancel.IsEnabled = true;
        }
        #endregion

        /// <summary>
        /// Callback called by LinphoneManager when a message is received.
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
            MessagesList.Dispatcher.BeginInvoke(() =>
            {
                IncomingChatBubble bubble = new IncomingChatBubble(message);
                bubble.MessageDeleted += bubble_MessageDeleted;
                bubble.DownloadImage += bubble_DownloadImage;
                MessagesList.Children.Insert(MessagesList.Children.Count - 1, bubble);

                scrollToBottom();
            });
        }
        
        /// <summary>
        /// Callback called by LinphoneManager when a composing is received.
        /// </summary>
        public void ComposeReceived()
        {
            Dispatcher.BeginInvoke(() =>
            {
                UpdateComposingMessage();
                scrollToBottom();
            });
        }

        private void UpdateComposingMessage()
        {
            if (chatRoom == null)
                return;

            bool isRemoteComposing = chatRoom.IsRemoteComposing();
            Debug.WriteLine("[Chat] Is remote composing ? " + isRemoteComposing);
            RemoteComposing.Visibility = isRemoteComposing ? Visibility.Visible : Visibility.Collapsed;

            string remoteName = chatRoom.GetPeerAddress().GetDisplayName();
            if (remoteName.Length <= 0)
                remoteName = chatRoom.GetPeerAddress().GetUserName();
            RemoteComposing.Text = remoteName + AppResources.RemoteComposing;
        }

        /// <summary>
        /// Gets the sip address associated to the MessageReceivedListener
        /// </summary>
        /// <returns></returns>
        public string GetSipAddressAssociatedWithDisplayConversation()
        {
            return sipAddress;
        }

        private void scrollToBottom()
        {
            MessagesScroll.UpdateLayout();
            MessagesScroll.ScrollToVerticalOffset(MessagesScroll.ScrollableHeight);
        }

        private void ChooseContact_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Contacts.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Callback called when a user selects the delete context menu
        /// </summary>
        public void bubble_MessageDeleted(object sender, LinphoneChatMessage message)
        {
            MessagesList.Children.Remove(sender as UserControl);
            if (chatRoom != null) 
            {
                chatRoom.DeleteMessageFromHistory(message);
            }
        }

        /// <summary>
        /// Callback called when a user wants to download an image in a message
        /// </summary>
        public void bubble_DownloadImage(object sender, LinphoneChatMessage message)
        {
            message.StartFileDownload(this, Utils.GetImageRandomFileName());
        }
    }
}