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
using System.Windows.Controls.Primitives;
using Linphone.Resources;
using Linphone.Core;

namespace Linphone.Controls
{
    /// <summary>
    /// Custom message box to dispaly incoming messages
    /// </summary>
    public partial class MessageReceivedNotification : UserControl
    {
        private Popup _popup;
        private LinphoneChatMessage _message;

        /// <summary>
        /// Getter for Popup.IsOpen
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _popup.IsOpen;
            }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public MessageReceivedNotification(Popup popup, LinphoneChatMessage message)
        {
            InitializeComponent();

            _popup = popup;
            _message = message;

            string text = message.GetText();
            if (text != null && text.Length > 0)
                Message.Text = text;
            else
                Message.Text = AppResources.ImageMessageReceived;

            string displayName = message.GetPeerAddress().GetDisplayName();
            Sender.Text = displayName == null || displayName.Length <= 0 ? message.GetPeerAddress().GetUserName() : displayName;

            ContactManager.Instance.ContactFound += (sender, e) =>
            {
                if (e.ContactFound != null)
                {
                    Sender.Text = e.ContactFound.DisplayName;
                }
            };
            ContactManager.Instance.FindContact(message.GetPeerAddress().AsStringUriOnly());

            BaseModel.CurrentPage.ApplicationBar.IsVisible = false;
        }

        /// <summary>
        /// Closes the popup.
        /// </summary>
        public void Hide()
        {
            BaseModel.CurrentPage.ApplicationBar.IsVisible = true;
            _popup.IsOpen = false;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Delegate for show button clicked event.
        /// </summary>
        public delegate void ShowClickedEventHandler(object sender, LinphoneChatMessage message);

        /// <summary>
        /// Handler for show button clicked event.
        /// </summary>
        public event ShowClickedEventHandler ShowClicked;

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            if (ShowClicked != null)
                ShowClicked(this, _message);
        }
    }
}
