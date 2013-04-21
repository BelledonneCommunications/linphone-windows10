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

        /// <summary>
        /// Method called when the page is displayed.
        /// Check if the uri contains a sip address, if yes, it displays the matching chat history.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LinphoneManager.Instance.MessageListener = this;

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

            }
        }

        /// <summary>
        /// Method called when this page isn't displayed anymore.
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            LinphoneManager.Instance.MessageListener = null;
        }

        /// <summary>
        /// Callback called when the search on a phone number for a contact has a match
        /// </summary>
        private void cm_ContactFound(object sender, ContactFoundEventArgs e)
        {
            ContactName.Text = e.ContactFound.DisplayName;
            ContactManager.Instance.TempContact = e.ContactFound;
            ContactName.Tap += ContactName_Tap;
        }

        private void ContactName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Contact.xaml", UriKind.RelativeOrAbsolute));
        }

        private void SendMessage(string message)
        {
            LinphoneChatMessage chatMessage = chatRoom.CreateLinphoneChatMessage(message);
            chatRoom.SendMessage(chatMessage, this);

            OutgoingChatBubble bubble = new OutgoingChatBubble(message, "Now");
            MessagesList.Children.Add(bubble);
            scrollToBottom();
        }

        /// <summary>
        /// Callback called by LinphoneCore when the state of a sent message changes.
        /// </summary>
        public void MessageStateChanged(LinphoneChatMessage message, LinphoneChatMessageState state)
        {
            Logger.Msg("[Chat] Message " + message.GetText() + ", state changed: " + state.ToString());
        }

        private void send_Click_1(object sender, EventArgs e)
        {
            SendMessage(MessageBox.Text);
            MessageBox.Reset();
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
                IncomingChatBubble bubble = new IncomingChatBubble(message.GetText(), "Now");
                MessagesList.Children.Add(bubble);
                scrollToBottom();
            });
        }

        private void scrollToBottom()
        {
            MessagesScroll.UpdateLayout();
            MessagesScroll.ScrollToVerticalOffset(MessagesList.ActualHeight);
        }
    }
}