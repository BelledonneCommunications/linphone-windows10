/*
Contacts.xaml.cs
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

using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Page using native style list to display phone contacts.
    /// </summary>
    public partial class Contacts : Page
    {

        private List<Windows.ApplicationModel.Contacts.Contact> _allcontacts;
        /// <summary>
        /// Public constructor.
        /// </summary>
        public Contacts()
        {
            this.InitializeComponent();
            

            if(ContactsManager.Instance.getContacts() != null)
            {
                _allcontacts = ContactsManager.Instance.getContacts();
                Debug.WriteLine(_allcontacts.Count);
                contactsList.ItemsSource = _allcontacts;
            }
            
            //recentContacts.ItemsSource = ContactsManager.Instance.GetRecentContacts();
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
            ListView list = (sender as ListView);
            //Microsoft.Phone.UserData.Contact selectedContact = (list.SelectedItem as Microsoft.Phone.UserData.Contact);
            //ContactManager.Instance.TempContact = selectedContact;
            //NavigationService.Navigate(new Uri("/Views/Contact.xaml", UriKind.RelativeOrAbsolute));
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
                   // HubTileService.FreezeGroup("RecentContactsGroup");
                    break;

                case 1:
                    //HubTileService.UnfreezeGroup("RecentContactsGroup");
                    break;
            }
        }
    }
}