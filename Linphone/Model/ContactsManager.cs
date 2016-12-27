/*
ContactManager.cs
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Popups;

namespace Linphone.Model {
    public class ContactFoundEventArgs : EventArgs {
        public ContactItem ContactFound {
            get; set;
        }

        public ContactFoundEventArgs(ContactItem contact) {
            ContactFound = contact;
        }
    }

    public class ContactsManager {
        private List<AlphaKeyGroup<ContactItem>> groupsOfContacts;
        private ObservableCollection<ContactItem> _contactsList = new ObservableCollection<ContactItem>();
        private ObservableCollection<ContactItem> contactItems = new ObservableCollection<ContactItem>();
        private ContactStore store;

        private static ContactsManager singleton;
        public static ContactsManager Instance {
            get {
                if (ContactsManager.singleton == null)
                    ContactsManager.singleton = new ContactsManager();

                return ContactsManager.singleton;
            }
        }

        public ContactItem TempContact;
        public delegate void ContactFoundEventHandler(object sender, ContactFoundEventArgs e);

        public event ContactFoundEventHandler ContactFound;
        private static ContactStore contactStore = null;

        public ContactsManager() {
            UpdateContactListAsync();
        }

        public async void UpdateContactListAsync() {
            await LoadContactsFromStoreAsync();
        }

        private async Task LoadContactsFromStoreAsync() {
            try {
                store = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
            } catch (Exception ex) {
                Debug.WriteLine("Potential contact store bug: " + ex, "error");
            }

            if (store == null) {

                MessageDialog connectionWarning =
                    new MessageDialog("The app needs access to your contacts in order to function correctly. " +
                        "Please grant it access using the options in the settings menu ",
                        "Lost Connection to Store");
                await connectionWarning.ShowAsync();
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-contacts"));
                return;
            }

            store.ChangeTracker.Enable();
            ContactReader reader = store.GetContactReader();
            await DisplayContactsFromReaderAsync(reader, true);
            return;
        }

        private async Task SearchForTextAsync(string ContactFilter) {
            if (store == null) {
                await LoadContactsFromStoreAsync();
                return;
            }
            if (!string.IsNullOrWhiteSpace(ContactFilter)) {
                ContactQueryOptions option = new ContactQueryOptions(ContactFilter, ContactQuerySearchFields.All);
                ContactReader reader = store.GetContactReader(option);
                await DisplayContactsFromReaderAsync(reader, false);
            } else {
                ContactReader reader = store.GetContactReader();
                await DisplayContactsFromReaderAsync(reader, true);
            }
            return;
        }

        private async Task DisplayContactsFromReaderAsync(ContactReader reader, bool isGroup) {
            contactItems.Clear();
            ContactBatch contactBatch = await reader.ReadBatchAsync();
            if (contactBatch.Contacts.Count == 0) {
                return;
            }

            while (contactBatch.Contacts.Count != 0) {
                foreach (Contact c in contactBatch.Contacts) {
                    if (c.Phones.Count > 0 || c.Emails.Count > 0) {
                        ContactItem contactToAdd = new ContactItem(c.Id, c.DisplayName);
                        contactToAdd.ContactEmails = c.Emails;
                        contactToAdd.ContactPhones = c.Phones;
                        contactToAdd.SetImageAsync(c.Thumbnail);
                        contactItems.Add(contactToAdd);
                    }

                }
                contactBatch = await reader.ReadBatchAsync();
            }

            if (isGroup) {
                groupsOfContacts = alphaGroupSorting(contactItems);
            } else {
                _contactsList = contactItems;
            }
            return;
        }

        private List<AlphaKeyGroup<ContactItem>> alphaGroupSorting(IEnumerable<ContactItem> items) {
            var returnGroup = AlphaKeyGroup<ContactItem>.CreateGroups(items, (ContactItem s) => {
                return s.ContactName;
            }, true);
            return returnGroup;
        }


        public ObservableCollection<ContactItem> ContactsList {
            get {
                return contactItems;
            }
        }

        public List<AlphaKeyGroup<ContactItem>> GroupsOfContacts {
            get {
                return groupsOfContacts;
            }
            private set {
                if (groupsOfContacts != value) {
                    groupsOfContacts = value;
                }
            }
        }

        private static bool IsPhoneNumber(string str) {
            foreach (char c in str) {
                if ((c < '0' || c > '9') && c != '+')
                    return false;
            }
            return true;
        }
    }
}
