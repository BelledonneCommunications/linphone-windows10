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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LinphoneManager.Instance.MessageListener = this;
            _SentMessages = new List<OutgoingChatBubble>();

            // Create LinphoneCore if not created yet, otherwise do nothing
            LinphoneManager.Instance.InitLinphoneCore();

            // Check for the navigation direction to avoid going to incall view when coming back from incall view
            if (NavigationContext.QueryString.ContainsKey("sip") && e.NavigationMode != NavigationMode.Back)
            {
                sipAddress = NavigationContext.QueryString["sip"];

                ContactName.Text = sipAddress;
                ContactManager cm = ContactManager.Instance;
                cm.ContactFound += cm_ContactFound;
                cm.FindContact(sipAddress);

                chatRoom = LinphoneManager.Instance.LinphoneCore.CreateChatRoom(sipAddress);

                // Define the query to gather all of the messages linked to the current contact.
                var messagesInDB = from message in DatabaseManager.Instance.Messages where (message.LocalContact.Contains(sipAddress) || message.RemoteContact.Contains(sipAddress)) select message;
                // Execute the query and place the results into a collection.
                List<ChatMessage> messages = messagesInDB.ToList();
                DisplayPastMessages(messages);
            }
        }

        private void DisplayPastMessages(List<ChatMessage> messages)
        {
            foreach (var message in messages)
            {
                DateTime date = new DateTime(message.Timestamp * TimeSpan.TicksPerSecond);
                if (message.IsIncoming)
                {
                    IncomingChatBubble bubble = new IncomingChatBubble(message.Message, FormatDate(date));
                    MessagesList.Children.Add(bubble);
                }
                else
                {
                    OutgoingChatBubble bubble = new OutgoingChatBubble(message.Message, FormatDate(date));
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
            OutgoingChatBubble bubble = new OutgoingChatBubble(message, FormatDate(now));
            MessagesList.Children.Add(bubble);
            _SentMessages.Add(bubble);
            scrollToBottom();

            ChatMessage msg = new ChatMessage { Message = message, MarkedAsRead = true, IsIncoming = false, RemoteContact = sipAddress, LocalContact = "", Timestamp = (now.Ticks / TimeSpan.TicksPerSecond), Status = (int)LinphoneChatMessageState.InProgress };
            DatabaseManager.Instance.Messages.InsertOnSubmit(msg);
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
                    DatabaseManager.Instance.SubmitChanges();// Submit changes to be able to search for the message inside the db.

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
            if (MessageBox.Text != null && MessageBox.Text.Length > 0)
            {
                SendMessage(MessageBox.Text);
                MessageBox.Reset();
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarSend = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.message.send.png", UriKind.Relative));
            appBarSend.Text = AppResources.SendMessage;
            ApplicationBar.Buttons.Add(appBarSend);
            appBarSend.Click += send_Click_1;
        }

        /// <summary>
        /// Callback called by LinphoneManager when a message is received.
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
            MessagesList.Dispatcher.BeginInvoke(() =>
            {
                //FIXME: message.GetTime() returns a bad value.
                DateTime date = new DateTime();
                date = date.AddYears(1969); //Timestamp is calculated from 01/01/1970, and DateTime is initialized to 01/01/0001.
                date = date.AddSeconds(message.GetTime());
                date = date.Add(TimeZoneInfo.Local.GetUtcOffset(date));
                IncomingChatBubble bubble = new IncomingChatBubble(message.GetText(), FormatDate(date));
                MessagesList.Children.Add(bubble);

                ChatMessage msg = new ChatMessage { Message = message.GetText(), MarkedAsRead = true, IsIncoming = true, LocalContact = sipAddress, RemoteContact = "", Timestamp = (date.Ticks / TimeSpan.TicksPerSecond) };
                DatabaseManager.Instance.Messages.InsertOnSubmit(msg);

                scrollToBottom();
            });
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
    }
}