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
            Microsoft.Phone.UserData.Contact selectedContact = (list.SelectedItem as Microsoft.Phone.UserData.Contact);
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