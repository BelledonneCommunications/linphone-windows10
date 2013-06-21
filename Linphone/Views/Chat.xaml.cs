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

            if (!NavigationService.CanGoBack) 
            {
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
                    OutgoingChatBubble bubble = new OutgoingChatBubble(message, FormatDate(date));
                    bubble.MessageDeleted += bubble_MessageDeleted;
                    bubble.UpdateStatus((LinphoneChatMessageState)message.Status);
                    MessagesList.Children.Add(bubble);
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
            ChatMessage msg = new ChatMessage { Message = message, MarkedAsRead = true, IsIncoming = false, RemoteContact = sipAddress, LocalContact = "", Timestamp = (now.Ticks / TimeSpan.TicksPerSecond), Status = (int)LinphoneChatMessageState.InProgress };
            DatabaseManager.Instance.Messages.InsertOnSubmit(msg);
            DatabaseManager.Instance.SubmitChanges();

            OutgoingChatBubble bubble = new OutgoingChatBubble(msg, FormatDate(now));
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
            Logger.Msg("[Chat] Message " + messageText + ", state changed: " + state.ToString());
            if (state == LinphoneChatMessageState.InProgress)
            {
                return; //We don't need to save the inprogress event in db.
            }

            Dispatcher.BeginInvoke(() =>
            {
                OutgoingChatBubble bubble = _SentMessages.Where(b => b.Message.Text.Equals(messageText)).Last();
                if (bubble != null)
                {
                    bubble.UpdateStatus(state);
                    _SentMessages.Remove(bubble);

                    ChatMessage msgToUpdate = DatabaseManager.Instance.Messages.Where(m => m.IsIncoming == false && m.Message.Equals(messageText) && m.Status == (int)LinphoneChatMessageState.InProgress).ToList().LastOrDefault();
                    if (msgToUpdate != null)
                    {
                        msgToUpdate.Status = (int)state;
                        DatabaseManager.Instance.SubmitChanges();// Submit changes to save the changes while the object is locally referenced.
                    }
                }
            });
        }

        private void send_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Text != null && MessageBox.Text.Length > 0 && (NewChatSipAddress.Text != null || NewChatSipAddress.Visibility == Visibility.Collapsed))
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

                SendMessage(MessageBox.Text);
                MessageBox.Reset();
            }
        }

        private void send_image_Click_1(object sender, EventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.Completed += task_Completed;
            task.ShowCamera = true;
            task.Show();
        }

        void task_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage image = new BitmapImage();
                image.SetSource(e.ChosenPhoto);

                //TODO: Actually send the message

                DateTime now = DateTime.Now;
                OutgoingChatBubble bubble = new OutgoingChatBubble(image, FormatDate(now));
                bubble.MessageDeleted += bubble_MessageDeleted;
                MessagesList.Children.Add(bubble);
                scrollToBottom();
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarSend = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.message.send.png", UriKind.Relative));
            appBarSend.Text = AppResources.SendMessage;
            ApplicationBar.Buttons.Add(appBarSend);
            appBarSend.Click += send_Click_1;

            ApplicationBarIconButton appBarSendImage = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.camera.png", UriKind.Relative));
            appBarSendImage.Text = AppResources.SendPicture;
            ApplicationBar.Buttons.Add(appBarSendImage);
            appBarSendImage.Click += send_image_Click_1;
        }

        /// <summary>
        /// Callback called by LinphoneManager when a message is received.
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
            MessagesList.Dispatcher.BeginInvoke(() =>
            {
                DateTime date = new DateTime();
                ChatMessage msg = new ChatMessage { Message = message.GetText(), MarkedAsRead = true, IsIncoming = true, LocalContact = sipAddress, RemoteContact = "", Timestamp = (date.Ticks / TimeSpan.TicksPerSecond) };
                DatabaseManager.Instance.Messages.InsertOnSubmit(msg);
                DatabaseManager.Instance.SubmitChanges();

                date = date.AddYears(1969); //Timestamp is calculated from 01/01/1970, and DateTime is initialized to 01/01/0001.
                date = date.AddSeconds(message.GetTime());
                date = date.Add(TimeZoneInfo.Local.GetUtcOffset(date));
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