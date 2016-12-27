/*
Contact.xaml.cs
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

using Linphone.Controls;
using Linphone.Model;
using System;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views {
    /// <summary>
    /// Page displaying contact information + action buttons associated to phone numbers and email addresses.
    /// </summary>
    public partial class ContactDetail : Page {
        private ContactItem contact {
            get; set;
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ContactDetail() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method called when the page is displayed, fetches and display contact information and create actions.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            while (actions.Children.Count > 0) {
                actions.Children.RemoveAt(0);
            }

            contact = ContactsManager.Instance.TempContact;
            contactName.Text = contact.ContactName;

            if (contact.ContactImage != null) {

                contactPicture.ImageSource = contact.ContactImage;
            }

            foreach (ContactPhone phone in contact.ContactPhones) {
                ContactAction entry = new ContactAction();
                entry.Label = phone.Kind.ToString();
                entry.NumberOrAddress = phone.Number;
                entry.Click += action_Click_1;
                entry.Click2 += action_Click_2;
                actions.Children.Add(entry);
            }

            foreach (ContactEmail email in contact.ContactEmails) {
                ContactAction entry = new ContactAction();
                entry.Label = email.Kind.ToString();
                entry.NumberOrAddress = email.Address;
                entry.Click += action_Click_1;
                entry.Click2 += action_Click_2;
                actions.Children.Add(entry);
            }
        }

        private void SetAddressGoToDialerAndCall(String address) {
            Frame.Navigate(typeof(Views.Dialer), address);
        }

        private void action_Click_1(object sender, RoutedEventArgs e) {
            String numberOrAddress = (sender as Button).Tag.ToString();
            SetAddressGoToDialerAndCall(numberOrAddress);
        }

        private void action_Click_2(object sender, RoutedEventArgs e) {
            String numberOrAddress = (sender as Button).Tag.ToString();
            Frame.Navigate(typeof(Views.Chat), numberOrAddress);
        }
    }
}