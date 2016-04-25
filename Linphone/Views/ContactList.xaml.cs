using Linphone.Helpers;
using Linphone.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml.Controls;

namespace Linphone.Views
{

    public sealed partial class ContactList : Page
    {

        private List<AlphaKeyGroup<ContactItem>> groupsOfContacts;
        private ObservableCollection<ContactItem> _contactsList = new ObservableCollection<ContactItem>();
        private ObservableCollection<ContactItem> contactItems = new ObservableCollection<ContactItem>();
        private ContactStore store;

        public ContactList()
        {
            this.InitializeComponent();
            groupedContactsCvs.Source = ContactsManager.Instance.GroupsOfContacts;
            ContactListView.ItemsSource = ContactsManager.Instance.ContactsList;
        }

        

        private void ContactSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ContactsManager.Instance.TempContact = (e.ClickedItem as ContactItem);
            Frame.Navigate(typeof(Views.ContactDetail), null);
        }

        private void ContactGroupView_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {

        }
    }
}
