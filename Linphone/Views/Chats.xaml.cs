/*
Chats.xaml.cs
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Linphone;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views {

    public sealed partial class Chats : Page {
        private bool _usingSelectionAppBar = false;
        private IEnumerable<Conversation> _selection;
        private ObservableCollection<Conversation> _conversations, _sortedConversations;

        public ObservableCollection<Conversation> ChatList {
            get {
                GetMessagesAndDisplayConversationsList();
                return _conversations;
            }
        }

        public Chats()
        //  :base(new ChatsModel())
        {
            _conversations = new ObservableCollection<Conversation>();
            _sortedConversations = new ObservableCollection<Conversation>();

            this.InitializeComponent();

            SetCommandsVisibility(Conversations);
            SystemNavigationManager.GetForCurrentView().BackRequested += Back_requested;
            Conversations.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            /*ContactManager cm = ContactManager.Instance;
            cm.ContactFound += cm_ContactFound;*/

            LinphoneManager.Instance.MessageReceived += MessageReceived;
            GetMessagesAndDisplayConversationsList();

            Conversations.ItemsSource = _conversations;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            /* ContactManager cm = ContactManager.Instance;
             cm.ContactFound -= cm_ContactFound;*/

            base.OnNavigatedFrom(e);
        }

        private void MessageReceived(ChatRoom room, ChatMessage message) {
            GetMessagesAndDisplayConversationsList();
        }

        /// <summary>
        /// Callback called when the search on a phone number or an email for a contact has a match
        /// </summary>
        /* private void cm_ContactFound(object sender, ContactFoundEventArgs e)
         {
             string address = e.PhoneNumber;
             string displayName = address;

             if (e.ContactFound != null)
             {
                 displayName = e.ContactFound.DisplayName;
                 Conversation conv = null;
                 foreach (var conversation in _conversations)
                 {
                     if (conversation.SipAddress.Equals(e.Request))
                     {
                         conv = conversation;
                         break;
                     }
                 }
                 if (conv != null)
                 {
                     _conversations.Remove(conv);
                     conv.DisplayedName = displayName;
                     _conversations.Add(conv);
                 }

                 _sortedConversations = new ObservableCollection<Conversation>();
                 foreach (var i in _conversations.OrderByDescending(g => g.Messages.Last().Time).ToList())
                 {
                     _sortedConversations.Add(i);
                 }
                 ((ChatsModel)ViewModel).Conversations = _sortedConversations;
             }
         }*/

        private void GetMessagesAndDisplayConversationsList() {
            _conversations.Clear();
            _sortedConversations.Clear();
            foreach (ChatRoom conversation in LinphoneManager.Instance.Core.ChatRooms) {
                if (conversation.HistorySize > 0) {
                    Address peerAddress = conversation.PeerAddress;
                    string address = String.Format("{0}@{1}", peerAddress.Username, peerAddress.Domain);
                    string name = peerAddress.DisplayName;
                    if (name == null || name.Length <= 0) {
                        name = peerAddress.Username;
                    }
                    _conversations.Add(new Conversation(address, name, conversation.GetHistory(conversation.HistorySize)));
                    //ContactManager.Instance.FindContact(address);
                }
            }

            if (_conversations.Count() == 0) {
                EmptyText.Visibility = Visibility.Visible;
                Conversations.Visibility = Visibility.Collapsed;
                SelectItems.IsEnabled = false;
            } else {
                EmptyText.Visibility = Visibility.Collapsed;
                Conversations.Visibility = Visibility.Visible;
                SelectItems.IsEnabled = true;
            }

            _sortedConversations = new ObservableCollection<Conversation>();
            foreach (var i in _conversations.OrderByDescending(g => g.Messages.Last().Time).ToList()) {
                _sortedConversations.Add(i);
            }
            //((ChatsModel)ViewModel).Conversations = _sortedConversations;*/

            SetCommandsVisibility(Conversations);
        }

        private void Delete_Click(object sender, RoutedEventArgs e) {
            /* string sipAddress = (string)item.CommandParameter;
             if (sipAddress != null && sipAddress.Length > 0)
             {
                 LinphoneManager.Instance.Core.GetChatRoomFromUri(sipAddress).DeleteHistory();
                 GetMessagesAndDisplayConversationsList();
             }*/
        }

        private void newChat_Click_1(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(Views.Chat), null);
        }

        private void Back_requested(object sender, BackRequestedEventArgs e) {
            if (Frame.CanGoBack) {
                Frame.Navigate(typeof(Views.Dialer), null);
                Frame.BackStack.Clear();
            }
            e.Handled = true;
        }

        private void SelectItmesBtn_Click(object sender, RoutedEventArgs e) {
            Conversations.SelectionMode = ListViewSelectionMode.Multiple;
            SetCommandsVisibility(Conversations);
        }

        private void SetCommandsVisibility(ListView listView) {
            if (listView.SelectionMode == ListViewSelectionMode.Multiple || listView.SelectedItems.Count > 1) {
                SelectItems.Visibility = Visibility.Collapsed;
                CancelBtn.Visibility = Visibility.Visible;
                NewConversation.Visibility = Visibility.Collapsed;
                DeleteItem.Visibility = Visibility.Visible;
                SelectAll.Visibility = Visibility.Visible;
                DeselectAll.Visibility = Visibility.Collapsed;
            } else {
                SelectItems.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Collapsed;
                NewConversation.Visibility = Visibility.Visible;
                DeleteItem.Visibility = Visibility.Collapsed;
                SelectAll.Visibility = Visibility.Collapsed;
                DeselectAll.Visibility = Visibility.Collapsed;
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e) {
            List<Conversation> _selectItem = new List<Conversation>();
            foreach (Conversation item in Conversations.SelectedItems) {
                _selectItem.Add(item);
            }
            foreach (Conversation item in _selectItem) {
                LinphoneManager.Instance.Core.GetChatRoomFromUri(item.SipAddress).DeleteHistory();
            }
            Conversations.SelectionMode = ListViewSelectionMode.None;
            GetMessagesAndDisplayConversationsList();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            Conversations.SelectionMode = ListViewSelectionMode.None;
            SetCommandsVisibility(Conversations);
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e) {
            Conversations.SelectAll();
            DeselectAll.Visibility = Visibility.Visible;
            SelectAll.Visibility = Visibility.Collapsed;
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e) {
            Conversations.SelectedItems.Clear();
            DeselectAll.Visibility = Visibility.Collapsed;
            SelectAll.Visibility = Visibility.Visible;
        }

        private void Conversations_ItemClick(object sender, ItemClickEventArgs e) {
            if (Conversations.SelectionMode != ListViewSelectionMode.Multiple) {
                Frame.Navigate(typeof(Views.Chat), (e.ClickedItem as Conversation).SipAddress);
            }
        }
    }
}