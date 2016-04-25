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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using BelledonneCommunications.Linphone.Native;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{

    public sealed partial class Chats : Page
    {
        private bool _usingSelectionAppBar = false;
        private IEnumerable<Conversation> _selection;
        private ObservableCollection<Conversation> _conversations, _sortedConversations;

        public ObservableCollection<Conversation> ChatList
        {
            get
            {
                GetMessagesAndDisplayConversationsList();
                return _conversations;
            }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Chats()
          //  :base(new ChatsModel())
        {
            _conversations = new ObservableCollection<Conversation>();
            _sortedConversations = new ObservableCollection<Conversation>();
            this.InitializeComponent();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Fetches the conversations from the LinphoneManager and displays them.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            /*ContactManager cm = ContactManager.Instance;
            cm.ContactFound += cm_ContactFound;

            
            */

            LinphoneManager.Instance.MessagReceived += MessagReceived;

            GetMessagesAndDisplayConversationsList();
            Conversations.ItemsSource = _conversations;
            base.OnNavigatedTo(e);
        }
        
        /// <summary>
        /// Method called when the page is hidden.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            /* ContactManager cm = ContactManager.Instance;
             cm.ContactFound -= cm_ContactFound;*/
            
            base.OnNavigatedFrom(e);
        }

        private void MessagReceived(object sender, EventArgs e)
        {
            //GetMessagesAndDisplayConversationsList();
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

        private void GetMessagesAndDisplayConversationsList()
        {
            _conversations.Clear();
            _sortedConversations.Clear();
            foreach (ChatRoom conversation in LinphoneManager.Instance.Core.ChatRooms)
            {
                if (conversation.HistorySize > 0)
                {
                    Address peerAddress = conversation.PeerAddress;
                    string address = String.Format("{0}@{1}", peerAddress.UserName, peerAddress.Domain);
                    string name = peerAddress.DisplayName;
                    if (name == null || name.Length <= 0)
                    {
                        name = peerAddress.UserName;
                    }
                    _conversations.Add(new Conversation(address, name, conversation.History));
                    //ContactManager.Instance.FindContact(address);
                }
            }

            _sortedConversations = new ObservableCollection<Conversation>();
            foreach (var i in _conversations.OrderByDescending(g => g.Messages.Last().Time).ToList())
            {
                _sortedConversations.Add(i);
            }
          //  ((ChatsModel)ViewModel).Conversations = _sortedConversations;*/
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
           /* string sipAddress = (string)item.CommandParameter;
            if (sipAddress != null && sipAddress.Length > 0)
            {
                LinphoneManager.Instance.Core.GetChatRoomFromUri(sipAddress).DeleteHistory();
                GetMessagesAndDisplayConversationsList();
            }*/
        }

        private void deleteSelection_Click_1(object sender, EventArgs e)
        {
           /* foreach (var c in _selection)
            {
                LinphoneManager.Instance.LinphoneCore.GetChatRoomFromUri(c.SipAddress).DeleteHistory();
            }

            GetMessagesAndDisplayConversationsList();

            ClearApplicationBar();
            SetupAppBarForEmptySelection();*/
        }

        private void newChat_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Chat), null);
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }

        }

        /* private void SetupAppBarForEmptySelection()
         {
             Del
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
         }*/

        private void conversation_Click_1(object sender, RoutedEventArgs e)
         {
             Conversation chat = ((sender as StackPanel).Tag as Conversation);
            if (chat != null)
            {
                Frame.Navigate(typeof(Views.Chat), chat.SipAddress);
            }
            
         }

        private void Conversations_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
        
        }

        private void Conversations_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(Views.Chat), (e.ClickedItem as Conversation).DisplayedName);
        }

        /// <summary>
        /// Add Dialer in the history if not already there
        /// </summary>
        /*protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (!NavigationService.CanGoBack)
            {
                e.Cancel = true;
                NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                NavigationService.RemoveBackEntry(); //To prevent a new click on back button to start again chat view (simulate a back click)
            }
        }*/
    }
}