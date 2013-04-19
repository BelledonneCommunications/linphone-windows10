using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Resources;
using Linphone.Model;

namespace Linphone.Views
{
    /// <summary>
    /// Displays the list of the previous conversations and permits to create new ones.
    /// </summary>
    public partial class Chats : BasePage
    {
        private bool _usingSelectionAppBar = false;
        private IEnumerable<Conversation> _selection;

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
            base.OnNavigatedTo(e);

            //TODO
            List<Conversation> conversations = new List<Conversation>();
            conversations.Add(new Conversation("sip:pauline@sip.example.org", "Pauline LEPOUTRE", null));
            Conversations.ItemsSource = conversations;
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

        private void deleteSelection_Click_1(object sender, EventArgs e)
        {
            ClearApplicationBar();
            SetupAppBarForEmptySelection();
        }

        private void SetupAppBarForEmptySelection()
        {
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