using Linphone.Model;
using Microsoft.Phone.Controls;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Page using native style list to display phone contacts.
    /// </summary>
    public partial class Contacts : BasePage
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public Contacts()
        {
            InitializeComponent();

            contactsList.ItemsSource = ContactManager.Instance.GetContactsGroupedByLetters();
            recentContacts.ItemsSource = ContactManager.Instance.GetRecentContacts();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        /// <param name="nee"></param>
        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);
        }

        private void contactsList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector list = (sender as LongListSelector);
            Microsoft.Phone.UserData.Contact selectedContact;

            if (list.SelectedItem.GetType() == typeof(Microsoft.Phone.UserData.Contact))
                selectedContact = (list.SelectedItem as Microsoft.Phone.UserData.Contact);
            else
                selectedContact = (list.SelectedItem as LinphoneContact).Contact;

            ContactManager.Instance.TempContact = selectedContact;
            NavigationService.Navigate(new Uri("/Views/Contact.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;

            if (pivot == null)
            {
                return;
            }

            switch (pivot.SelectedIndex)
            {
                case 0:
                    HubTileService.FreezeGroup("RecentContactsGroup");
                    break;

                case 1:
                    HubTileService.UnfreezeGroup("RecentContactsGroup");
                    break;
            }
        }
    }
}