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

namespace Linphone.Views
{
    /// <summary>
    /// Displays chat messages between two users.
    /// </summary>
    public partial class Chat : BasePage, LinphoneChatMessageListener
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
            SendMessage(Message.Text);
            Message.Text = "";
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarSend = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.message.send.png", UriKind.Relative));
            appBarSend.Text = AppResources.SendMessage;
            ApplicationBar.Buttons.Add(appBarSend);
            appBarSend.Click += send_Click_1;
        }
    }
}