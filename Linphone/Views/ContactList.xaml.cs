/*
ContactList.xaml.cs
Copyright (C) 2016  Belledonne Communications, Grenoble, France
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

using Linphone.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Linphone.Views
{

    public sealed partial class ContactList : Page
    {

        private List<AlphaKeyGroup<ContactItem>> groupsOfContacts;
        private ObservableCollection<ContactItem> _contactsList = new ObservableCollection<ContactItem>();
        private ObservableCollection<ContactItem> contactItems = new ObservableCollection<ContactItem>();

        public ContactList()
        {
            this.InitializeComponent();
            groupedContactsCvs.Source = ContactsManager.Instance.GroupsOfContacts;

            if (ContactsManager.Instance.ContactsList.Count == 0)
            {
                ContactListView.Visibility = Visibility.Collapsed;
                EmptyText.Visibility = Visibility.Visible;
            }
            else 
            {
                ContactListView.Visibility = Visibility.Visible;
                EmptyText.Visibility = Visibility.Collapsed;
            }
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
