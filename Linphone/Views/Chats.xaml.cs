﻿using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Displays the list of the previous conversations and permits to create new ones.
    /// </summary>
    public partial class Chats : BasePage
    {
        private bool _usingSelectionAppBar = false;
        private IEnumerable<Conversation> _selection;
        private List<ChatMessage> _allMessages;
        private ObservableCollection<Conversation> _conversations;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Chats()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Fetches the conversations from the LinphoneManager and displays them.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ContactManager cm = ContactManager.Instance;
            cm.ContactFound += cm_ContactFound;

            GetMessagesAndDisplayConversationsList();

            base.OnNavigatedTo(e);
        }
        
        /// <summary>
        /// Method called when the page is hidden.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ContactManager cm = ContactManager.Instance;
            cm.ContactFound -= cm_ContactFound;
            Conversations.ItemsSource = null;

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Callback called when the search on a phone number or an email for a contact has a match
        /// </summary>
        private void cm_ContactFound(object sender, ContactFoundEventArgs e)
        {
            string address = e.PhoneNumber;
            string displayName = address;

            if (e.ContactFound != null)
            {
                displayName = e.ContactFound.DisplayName;
            }
            _conversations.Add(new Conversation(address, displayName, _allMessages.Where(m => m.RemoteContact.Equals(address) || m.LocalContact.Equals(address)).ToList()));
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            SetupAppBarForEmptySelection();
        }

        private void ClearApplicationBar()
        {
            while (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }
        }

        private void GetMessagesAndDisplayConversationsList()
        {
            // Define the query to gather all of the messages.
            var messagesInDB = from ChatMessage message in DatabaseManager.Instance.Messages select message;
            // Execute the query and place the results into a collection.
            _allMessages = new List<ChatMessage>(messagesInDB);
            // Get distinct conversations by addresses.
            var filtered = _allMessages.OrderByDescending(m => m.Timestamp).GroupBy(m => m.Contact).Select(g => g.First()).ToList();

            _conversations = new ObservableCollection<Conversation>();
            foreach (var conversation in filtered)
            {
                string address = conversation.LocalContact.Length > 0 ? conversation.LocalContact : conversation.RemoteContact;
                ContactManager.Instance.FindContact(address);
            }
            Conversations.ItemsSource = _conversations;
        }

        private void deleteSelection_Click_1(object sender, EventArgs e)
        {
            foreach (var c in _selection)
            {
                DatabaseManager.Instance.Messages.DeleteAllOnSubmit(c.Messages);
            }
            DatabaseManager.Instance.SubmitChanges();

            GetMessagesAndDisplayConversationsList();            

            ClearApplicationBar();
            SetupAppBarForEmptySelection();
        }

        private void newChat_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Chat.xaml", UriKind.RelativeOrAbsolute));
        }

        private void SetupAppBarForEmptySelection()
        {
            ApplicationBarIconButton appBarNewChatSelection = new ApplicationBarIconButton(new Uri("/Assets/AppBar/add.png", UriKind.Relative));
            appBarNewChatSelection.Text = AppResources.NewChatMenu;
            ApplicationBar.Buttons.Add(appBarNewChatSelection);
            appBarNewChatSelection.Click += newChat_Click_1;

            _usingSelectionAppBar = false;
        }

        private void SetupAppBarForSelectedItems()
        {
            ApplicationBarIconButton appBarDeleteSelection = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
            appBarDeleteSelection.Text = AppResources.DeleteSelectionMenu;
            ApplicationBar.Buttons.Add(appBarDeleteSelection);
            appBarDeleteSelection.Click += deleteSelection_Click_1;

            _usingSelectionAppBar = true;
        }

        private void conversations_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LongListMultiSelector list = (LongListMultiSelector)sender;
            if (list.SelectedItems.Count == 0)
            {
                ClearApplicationBar();
                SetupAppBarForEmptySelection();
            }
            else if (list.SelectedItems.Count >= 1 && !_usingSelectionAppBar) // Do it only once, when selection was empty and isn't anymore
            {
                _selection = list.SelectedItems.Cast<Conversation>();
                ClearApplicationBar();
                SetupAppBarForSelectedItems();
            }
        }

        private void conversation_Click_1(object sender, RoutedEventArgs e)
        {
            Conversation chat = ((sender as StackPanel).Tag as Conversation);
            NavigationService.Navigate(new Uri("/Views/Chat.xaml?sip=" + chat.SipAddress, UriKind.RelativeOrAbsolute));
        }
    }
}