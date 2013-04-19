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
                chatRoom = LinphoneManager.Instance.LinphoneCore.CreateChatRoom(sipAddress);
            }
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
    }
}