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
    /// Displays chat messages between two users.
    /// </summary>
    public partial class Chat : BasePage, LinphoneChatMessageListener, MessageReceivedListener
    {
        private const int LOCAL_IMAGES_QUALITY = 100;
        private const int SENT_IMAGES_QUALITY = 50;
        private const int THUMBNAIL_WIDTH = 420;

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

        private List<OutgoingChatBubble> _SentMessages;

        /// <summary>
        /// Method called when the page is displayed.
        /// Check if the uri contains a sip address, if yes, it displays the matching chat history.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LinphoneManager.Instance.MessageListener = this;
            _SentMessages = new List<OutgoingChatBubble>();

            // Create LinphoneCore if not created yet, otherwise do nothing
            await LinphoneManager.Instance.InitLinphoneCore();

            ContactManager cm = ContactManager.Instance;
            cm.ContactFound += cm_ContactFound;

            NewChat.Visibility = Visibility.Collapsed;
            ContactName.Visibility = Visibility.Visible; 
            if (NavigationContext.QueryString.ContainsKey("sip"))
            {
                sipAddress = NavigationContext.QueryString["sip"];
                if (sipAddress.StartsWith("sip:"))
                {
                    sipAddress = sipAddress.Replace("sip:", "");
                }

                ContactName.Text = sipAddress;
                cm.FindContact(sipAddress);

                chatRoom = LinphoneManager.Instance.LinphoneCore.CreateChatRoom(sipAddress);

                // Define the query to gather all of the messages linked to the current contact.
                var messagesInDB = from message in DatabaseManager.Instance.Messages where (message.LocalContact.Contains(sipAddress) || message.RemoteContact.Contains(sipAddress)) select message;
                // Execute the query and place the results into a collection.
                List<ChatMessage> messages = messagesInDB.ToList();
                DisplayPastMessages(messages);
            }
            else if (e.NavigationMode != NavigationMode.Back)
            {
                ContactName.Visibility = Visibility.Collapsed; 
                NewChat.Visibility = Visibility.Visible;
                NewChatSipAddress.Focus();
            }
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

        private void DisplayPastMessages(List<ChatMessage> messages)
        {
            foreach (var message in messages)
            {
                DateTime date = new DateTime(message.Timestamp * TimeSpan.TicksPerSecond);
                if (message.IsIncoming)
                {
                    IncomingChatBubble bubble = new IncomingChatBubble(message, FormatDate(date));
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    MessagesList.Children.Add(bubble);
                }
                else
                {
                    OutgoingChatBubble bubble;
                    if (message.ImageURL != null && message.ImageURL.Length > 0)
                    {
                        bubble = new OutgoingChatBubble(message, null, FormatDate(date));
                    }
                    else
                    {
                        bubble = new OutgoingChatBubble(message, FormatDate(date));
                    }
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    bubble.UpdateStatus((LinphoneChatMessageState)message.Status);
                    MessagesList.Children.Add(bubble);
                }
            }
            scrollToBottom();
        }

        /// <summary>
        /// Saves an image sent or received in the media library of the device.
        /// </summary>
        /// <param name="fileName">File's name in the isolated storage</param>
        public static void SavePictureInMediaLibrary(string fileName)
        {
            MediaLibrary library = new MediaLibrary();
            byte[] data;
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream file = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        data = new byte[file.Length];
                        file.Read(data, 0, data.Length);
                        file.Close();
                    }
                }

                library.SavePicture(fileName, data);
            }
            catch { }
        }

        /// <summary>
        /// Get a thumbnail of a picture (used for chat images)
        /// </summary>
        /// <param name="image">The image to size down</param>
        /// <returns>The thumbnail picture</returns>
        public static BitmapImage GetThumbnailBitmapFromImage(BitmapImage image)
        {
            if (image == null)
                return null;

            if (image.PixelWidth <= THUMBNAIL_WIDTH)
                return image;

            MemoryStream ms = new MemoryStream();
            WriteableBitmap bitmap = new WriteableBitmap(image);

            int w, h;
            w = THUMBNAIL_WIDTH;
            h = (image.PixelHeight * w) / image.PixelWidth;

            bitmap.SaveJpeg(ms, w, h, 0, LOCAL_IMAGES_QUALITY);

            BitmapImage thumbnail = new BitmapImage();
            thumbnail.SetSource(ms);

            return thumbnail;
        }

        /// <summary>
        /// Returns a BitmapImage of a file stored in isolated storage
        /// </summary>
        /// <param name="fileName">Name of the file to open</param>
        /// <returns>a BitmapImage or null</returns>
        public static BitmapImage ReadImageFromIsolatedStorage(string fileName)
        {
            byte[] data;
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream file = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        data = new byte[file.Length];
                        file.Read(data, 0, data.Length);
                        file.Close();
                    }
                }

                MemoryStream ms = new MemoryStream(data);
                BitmapImage image = new BitmapImage();
                image.SetSource(ms);
                ms.Close();
                return image;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Method called when this page isn't displayed anymore.
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            chatRoom = null;
            DatabaseManager.Instance.SubmitChanges();
            base.OnNavigatingFrom(e);
            LinphoneManager.Instance.MessageListener = null;
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
                long time = chatMessage.GetTime();
                chatRoom.SendMessage(chatMessage, this);
            }

            DateTime now = DateTime.Now;
            ChatMessage msg = new ChatMessage { Message = message, ImageURL = "", MarkedAsRead = true, IsIncoming = false, RemoteContact = sipAddress, LocalContact = "", Timestamp = (now.Ticks / TimeSpan.TicksPerSecond), Status = (int)LinphoneChatMessageState.InProgress };
            DatabaseManager.Instance.Messages.InsertOnSubmit(msg);
            DatabaseManager.Instance.SubmitChanges();

            OutgoingChatBubble bubble = new OutgoingChatBubble(msg, FormatDate(now));
            bubble.MessageDeleted += bubble_MessageDeleted;
            MessagesList.Children.Add(bubble);
            _SentMessages.Add(bubble);
            scrollToBottom();
        }

        /// <summary>
        /// Download an image received using the given url and store it locally
        /// </summary>
        /// <param name="url">Url to download the image</param>
        /// <param name="message">ChatMessage linked to the image</param>
        /// <returns>The Bitmap Image for display</returns>
        public static BitmapImage DownloadImageAndStoreItInIsolatedStorage(string url, ChatMessage message)
        {
            //Download image
            BitmapImage image = new BitmapImage(new Uri(url));
            image.ImageOpened += (sender, e) => 
            {
                //Store it in isolated storage
                string fileName = url.Substring(url.LastIndexOf("/") + 1);
                SaveImageInLocalFolder(image, fileName);

                //Update image url in database to point on local image
                message.ImageURL = fileName;
                DatabaseManager.Instance.SubmitChanges();
            };

            return image;
        }

        private static void SaveImageInLocalFolder(BitmapImage image, string fileName)
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(fileName))
                    {
                        store.DeleteFile(fileName);
                    }

                    using (IsolatedStorageFileStream file = store.CreateFile(fileName))
                    {
                        WriteableBitmap bitmap = new WriteableBitmap(image);
                        Extensions.SaveJpeg(bitmap, file, bitmap.PixelWidth, bitmap.PixelHeight, 0, LOCAL_IMAGES_QUALITY);
                        file.Flush();
                        file.Close();
                        bitmap = null;
                    }
                }
            }
            catch { }
        }

        private async Task<string> UploadImageMessage(BitmapImage image, string filePath)
        {
            string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);

            //Copy image in local folder
            SaveImageInLocalFolder(image, fileName);

            //Upload the image
            string boundary = "----------" + DateTime.Now.Ticks.ToString();

            string response;
            using (var client = new HttpClient())
            {
                _httpPostClient = client;
                using (var content = new MultipartFormDataContent(boundary))
                {
                    MemoryStream ms = new MemoryStream();
                    WriteableBitmap bitmap = new WriteableBitmap(image);
                    Extensions.SaveJpeg(bitmap, ms, image.PixelWidth, image.PixelHeight, 0, SENT_IMAGES_QUALITY);
                    ms.Flush();
                    ms.Position = 0;
                    StreamContent streamContent = new StreamContent(ms);
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"userfile\"",
                        FileName = "\"" + fileName + "\""
                    };
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Add(streamContent);

                    using (var message = await client.PostAsync(DefaultValues.PictureUploadScriptURL, content))
                    {
                        message.EnsureSuccessStatusCode();
                        response = await message.Content.ReadAsStringAsync();
                    }
                }
            }
            return response;
        }

        private void SendImageMessage(string localFileName, string url, BitmapImage image)
        {
            if (url == null || url.Length == 0)
            {
                return;
            }

            if (chatRoom != null)
            {
                LinphoneChatMessage chatMessage = chatRoom.CreateLinphoneChatMessage("");
                chatMessage.SetExternalBodyUrl(url);
                long time = chatMessage.GetTime();
                chatRoom.SendMessage(chatMessage, this);
            }

            DateTime now = DateTime.Now;
            ChatMessage msg = new ChatMessage { ImageURL = localFileName, Message = "", MarkedAsRead = true, IsIncoming = false, RemoteContact = sipAddress, LocalContact = "", Timestamp = (now.Ticks / TimeSpan.TicksPerSecond), Status = (int)LinphoneChatMessageState.InProgress };
            DatabaseManager.Instance.Messages.InsertOnSubmit(msg);
            DatabaseManager.Instance.SubmitChanges();

            OutgoingChatBubble bubble = new OutgoingChatBubble(msg, image, FormatDate(now));
            bubble.MessageDeleted += bubble_MessageDeleted;
            MessagesList.Children.Add(bubble);
            _SentMessages.Add(bubble);
            scrollToBottom();
        }

        /// <summary>
        /// Callback called by LinphoneCore when the state of a sent message changes.
        /// </summary>
        public void MessageStateChanged(LinphoneChatMessage message, LinphoneChatMessageState state)
        {
            string messageText = message.GetText();
            string externalBodyUrl = message.GetExternalBodyUrl();
            Logger.Msg("[Chat] Message " + messageText + ", state changed: " + state.ToString());
            if (state == LinphoneChatMessageState.InProgress)
            {
                return; //We don't need to save the inprogress event in db.
            }

            Dispatcher.BeginInvoke(() =>
            {
                OutgoingChatBubble bubble = _SentMessages.Where(b => b.ChatMessage.Message.Equals(messageText)).Last();
                if (bubble != null)
                {
                    bubble.UpdateStatus(state);
                    _SentMessages.Remove(bubble);

                    ChatMessage msgToUpdate = bubble.ChatMessage;
                    if (msgToUpdate != null)
                    {
                        msgToUpdate.Status = (int)state;
                        DatabaseManager.Instance.SubmitChanges();// Submit changes to save the changes while the object is locally referenced.
                    }
                }
            });
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
                    sipAddress = NewChatSipAddress.Text;

                    if (!sipAddress.Contains("@"))
                    {
                        if (LinphoneManager.Instance.LinphoneCore.GetProxyConfigList().Count > 0)
                        {
                            LinphoneProxyConfig config = LinphoneManager.Instance.LinphoneCore.GetProxyConfigList()[0] as LinphoneProxyConfig;
                            sipAddress += "@" + config.GetDomain();
                        }
                    }

                    ContactManager.Instance.FindContact(sipAddress);
                    ContactName.Text = sipAddress;
                    ContactName.Visibility = Visibility.Visible;
                    NewChat.Visibility = Visibility.Collapsed;

                    chatRoom = LinphoneManager.Instance.LinphoneCore.CreateChatRoom(sipAddress);

                    if (DatabaseManager.Instance.Messages.Count(m => m.LocalContact.Equals(sipAddress) || m.RemoteContact.Equals(sipAddress)) > 0)
                    {
                        // Define the query to gather all of the messages linked to the current contact.
                        var messagesInDB = from message in DatabaseManager.Instance.Messages where (message.LocalContact.Contains(sipAddress) || message.RemoteContact.Contains(sipAddress)) select message;
                        // Execute the query and place the results into a collection.
                        List<ChatMessage> messages = messagesInDB.ToList();
                        DisplayPastMessages(messages);
                    }
                }

                if (MessageBox.Text != null && MessageBox.Text.Length > 0)
                {
                    SendMessage(MessageBox.Text);
                }
                MessageBox.Reset();
            }
        }

        private void attach_image_Click_1(object sender, EventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.Completed += task_Completed;
            task.ShowCamera = true;
            task.Show();
        }

        private async void task_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(e.ChosenPhoto);

                try
                {
                    ProgressPopup.Visibility = Visibility.Visible;
                    MessageBox.Visibility = Visibility.Collapsed;
                    AddCancelUploadButtonInAppBar();

                    string url = await UploadImageMessage(image, e.OriginalFileName);

                    string fileName = e.OriginalFileName.Substring(e.OriginalFileName.LastIndexOf("\\") + 1);
                    SendImageMessage(fileName, url, image);
                }
                catch { }
                finally
                {
                    ProgressPopup.Visibility = Visibility.Collapsed;
                    MessageBox.Visibility = Visibility.Visible;
                    AddSendButtonsToAppBar();
                }
            }
        }

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

        private void AddSendButtonsToAppBar()
        {
            CleanAppBar();

            ApplicationBarIconButton appBarSend = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.message.send.png", UriKind.Relative));
            appBarSend.Text = AppResources.SendMessage;
            ApplicationBar.Buttons.Add(appBarSend);
            appBarSend.Click += send_Click_1;

            ApplicationBarIconButton appBarSendImage = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.camera.png", UriKind.Relative));
            appBarSendImage.Text = AppResources.SendPicture;
            ApplicationBar.Buttons.Add(appBarSendImage);
            appBarSendImage.Click += attach_image_Click_1;
        }

        private void AddCancelUploadButtonInAppBar()
        {
            CleanAppBar();

            ApplicationBarIconButton appBarCancel = new ApplicationBarIconButton(new Uri("/Assets/AppBar/cancel.png", UriKind.Relative));
            appBarCancel.Text = AppResources.CancelUpload;
            ApplicationBar.Buttons.Add(appBarCancel);
            appBarCancel.Click += cancel_Click_1;
        }

        /// <summary>
        /// Callback called by LinphoneManager when a message is received.
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
            MessagesList.Dispatcher.BeginInvoke(() =>
            {
                DateTime date = new DateTime();

                date = date.AddYears(1969); //Timestamp is calculated from 01/01/1970, and DateTime is initialized to 01/01/0001.
                date = date.AddSeconds(message.GetTime());
                date = date.Add(TimeZoneInfo.Local.GetUtcOffset(date));

                //TODO: Temp hack to remove
                string url = message.GetExternalBodyUrl();
                url = url.Replace("\"", "");
                ChatMessage msg = new ChatMessage { Message = message.GetText(), ImageURL = url, MarkedAsRead = true, IsIncoming = true, LocalContact = sipAddress, RemoteContact = "", Timestamp = (date.Ticks / TimeSpan.TicksPerSecond) };
                DatabaseManager.Instance.Messages.InsertOnSubmit(msg);
                DatabaseManager.Instance.SubmitChanges();

                IncomingChatBubble bubble = new IncomingChatBubble(msg, FormatDate(date));
                bubble.MessageDeleted += bubble_MessageDeleted;
                MessagesList.Children.Add(bubble);

                scrollToBottom();
            });
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
            MessagesScroll.ScrollToVerticalOffset(MessagesList.ActualHeight);
        }

        private string FormatDate(DateTime date)
        {
            DateTime now = DateTime.Now;
            if (now.Year == date.Year && now.Month == date.Month && now.Day == date.Day)
                return String.Format("{0:HH:mm}", date);
            else if (now.Year == date.Year)
                return String.Format("{0:ddd d MMM, HH:mm}", date);
            else
                return String.Format("{0:ddd d MMM yyyy, HH:mm}", date);
        }

        private void ChooseContact_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Contacts.xaml", UriKind.RelativeOrAbsolute));
        }

        void bubble_MessageDeleted(object sender, ChatMessage message)
        {
            MessagesList.Children.Remove(sender as UserControl);
            DatabaseManager.Instance.Messages.DeleteOnSubmit(message);
        }
    }
}