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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using BelledonneCommunications.Linphone.Native;
using Linphone.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Popups;

namespace Linphone.Model
{
    /// <summary>
    /// Param object for contact found event listener.
    /// </summary>
    public class ContactFoundEventArgs : EventArgs
    {
        /// <summary>
        /// Contact found if actually found.
        /// </summary>
        public Contact ContactFound { get; set; }

        /// <summary>
        /// Phone number used to find this contact.
        /// </summary>
        public String PhoneNumber { get; set; }

        /// <summary>
        /// Phone label associated to the phone number.
        /// </summary>
        public String PhoneLabel { get; set; }

        /// <summary>
        /// String used as request
        /// </summary>
        public String Request { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ContactFoundEventArgs(Contact contact, String number, String label = null, String request = null)
        {
            ContactFound = contact;
            PhoneNumber = number;
            PhoneLabel = label;
            Request = request;
        }
    }

    /// <summary>
    /// Utility class used to handle every contact related requests.
    /// </summary>
    public class ContactsManager
    {
        private const int RECENT_CONTACTS_MAX = 6;
        private List<AlphaKeyGroup<ContactItem>> groupsOfContacts;
        private ObservableCollection<ContactItem> _contactsList = new ObservableCollection<ContactItem>();
        private ObservableCollection<ContactItem> contactItems = new ObservableCollection<ContactItem>();
        private ContactStore store;

        private static ContactsManager singleton;
        /// <summary>
        /// Static instance for this class.
        /// </summary>
        public static ContactsManager Instance
        {
            get 
            {
                if (ContactsManager.singleton == null)
                    ContactsManager.singleton = new ContactsManager();

                return ContactsManager.singleton;
            }
        }

        //private List<AlphaKeyGroup<Contact>> _contacts;
        private List<Contact> _allContacts;

        /// <summary>
        /// Represents the selected contact clicked on by the user.
        /// We keep it here to avoid giving it as param when navigating from Contacts.xaml to Contact.xaml
        /// </summary>
        public ContactItem TempContact;

        /// <summary>
        /// Delegate for contact found event.
        /// </summary>
        public delegate void ContactFoundEventHandler(object sender, ContactFoundEventArgs e);

        /// <summary>
        /// Handler for contact found event.
        /// </summary>
        public event ContactFoundEventHandler ContactFound;

        private static ContactStore contactStore = null;
        
        private async void initializeContacts()
        {
            Debug.WriteLine("init");

                initializeContactsStore();
               // readContacts();
           
        }

        private async void initializeContactsStore()
        {
            //if this is throwing Access Denied, you haven't declared the contacts capability in the manifest
            contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);

            if (contactStore == null)
            {
                //Could not open the contacts store because the user denied you in settings, sorry!
                MessageDialog connectionWarning =
                    new MessageDialog("The app needs access to your contacts in order to function correctly. " +
                        "Please grant it access using the options in the settings menu ",
                        "Lost Connection to Store");
                await connectionWarning.ShowAsync();

                //you should probably ask the user before taking him/her to the settings page, but ya know, sample
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-contacts"));
                return;
            }

            //Enable change tracking for your app, this tells the system that you'll want to know about changes that happen
            //you only need to call it once, but it no-ops after the first time
           // contactStore.ChangeTracker.Enable();

            if (contactStore == null)
            {
                MessageDialog storeNotInitializedWarning =
                    new MessageDialog("Please initialize the contact store first.", "Store not initialized");
                await storeNotInitializedWarning.ShowAsync();
                return;
            }

            //before you read all of the contacts [again?], you should reset the change tracker
            //contactStore.ChangeTracker.Reset();

            //ok let's read the contacts

            var contactReader = contactStore.GetContactReader();
            var contactBatch = await contactReader.ReadBatchAsync();

            _allContacts = new List<Contact>();

            while (contactBatch.Contacts.Count != 0)
            {
                foreach (Contact contact in contactBatch.Contacts)
                {
                    //So, what do you want to do with these?
                    _allContacts.Add(contact);
                }

                contactBatch = await contactReader.ReadBatchAsync();
            }
        }

        public async void readContacts()
        {
            Debug.WriteLine("Read contacts");
            if (contactStore == null)
            {
                MessageDialog storeNotInitializedWarning =
                    new MessageDialog("Please initialize the contact store first.", "Store not initialized");
                await storeNotInitializedWarning.ShowAsync();
                return;
            }

            //before you read all of the contacts [again?], you should reset the change tracker
            contactStore.ChangeTracker.Reset();

            //ok let's read the contacts
            var contactReader = contactStore.GetContactReader();

            var contactBatch = await contactReader.ReadBatchAsync();

            while (contactBatch.Contacts.Count != 0)
            {
                foreach (Contact contact in contactBatch.Contacts)
                {
                    //So, what do you want to do with these?
                    //Debug.WriteLine(contact.Id + ": " + contact.DisplayName);
                    _allContacts.Add(contact);
                }

                contactBatch = await contactReader.ReadBatchAsync();
            }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ContactsManager()
        {

            Debug.WriteLine("init contacts");
            //initializeContacts();
            UpdateContactListAsync();
            //readContacts();
            //contacts.SearchCompleted += contacts_SearchCompleted;

            //contacts.SearchAsync(String.Empty, FilterKind.None, "Phone Contacts");
        }

        public async void UpdateContactListAsync()
        {
            await LoadContactsFromStoreAsync();
        }

        private async Task LoadContactsFromStoreAsync()
        {
            //Try loading the contact atore
            try
            {
                store = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Potential contact store bug: " + ex, "error");
            }

            //If we can access the store without crashing (There seems to be a bug with the store).
            //Check to make sure we actually have access.
            if (store == null)
            {
                //Launch the settings app to fix the security settings
                Debug.WriteLine("Could not open contact store, is app access disabled in privacy settings?", "error");
                return;
            }
            Debug.WriteLine("Contact store opened for reading successfully.", "informational");
            //Load the contacts into the ListView on the page
            ContactReader reader = store.GetContactReader();
            await DisplayContactsFromReaderAsync(reader, true);
            return;
        }

        private async Task SearchForTextAsync(string ContactFilter)
        {
            if (store == null)
            {
                //Shouldn't happen, and I don't want to deal with opening the store in multiple locations
                await LoadContactsFromStoreAsync();
                return;
            }
            //A null query string is being treated as a query for "*"
            if (!string.IsNullOrWhiteSpace(ContactFilter))
            {
                ContactQueryOptions option = new ContactQueryOptions(ContactFilter, ContactQuerySearchFields.All);
                ContactReader reader = store.GetContactReader(option);
                await DisplayContactsFromReaderAsync(reader, false);
            }
            else
            {
                ContactReader reader = store.GetContactReader();
                await DisplayContactsFromReaderAsync(reader, true);
            }
            return;
        }

        private async Task DisplayContactsFromReaderAsync(ContactReader reader, bool isGroup)
        {
            contactItems.Clear();
            ContactBatch contactBatch = await reader.ReadBatchAsync();
            if (contactBatch.Contacts.Count == 0)
            {
                Debug.WriteLine("Contact store empty");
                return;
            }

            while (contactBatch.Contacts.Count != 0)
            {
                //should batch add to avoid triggering callbacks            
                foreach (Contact c in contactBatch.Contacts)
                {
                    //Debug.WriteLine(c.DisplayName);
                    ContactItem contactToAdd = new ContactItem(c.Id, c.DisplayName);
                    contactToAdd.SetImageAsync(c.Thumbnail);
                    contactItems.Add(contactToAdd);
                }
                contactBatch = await reader.ReadBatchAsync();
            }

            if (isGroup)
            {
                Debug.WriteLine("Is group");
                groupsOfContacts = alphaGroupSorting(contactItems);
            }
            else
            {
                Debug.WriteLine("Find  contacts");
                _contactsList = contactItems;
            }
            return;
        }

        private List<AlphaKeyGroup<ContactItem>> alphaGroupSorting(IEnumerable<ContactItem> items)
        {
            var returnGroup = AlphaKeyGroup<ContactItem>.CreateGroups(
                items,                                      // ungrouped list of items
                (ContactItem s) => { return s.ContactName; },  // the property to sort 
                true);                                      // order the items alphabetically 

            return returnGroup;
        }


        public ObservableCollection<ContactItem> ContactsList
        {
            get
            {
                return _contactsList;
            }
        }

        /// <summary>
        /// Gets and sets the grouped contacts list.
        /// </summary>
        public List<AlphaKeyGroup<ContactItem>> GroupsOfContacts
        {
            get
            {
                return groupsOfContacts;
            }
            private set
            {
                if (groupsOfContacts != value)
                {
                    groupsOfContacts = value;
                }
            }
        }

        public List<Contact> getContacts()
        {
            return _allContacts;
        }

        /* private void contacts_SearchCompleted(object sender, Microsoft.Phone.UserData.ContactsSearchEventArgs e)
          {
              /*_contacts = AlphaKeyGroup<Contact>.CreateGroups(e.Results, System.Threading.Thread.CurrentThread.CurrentUICulture,
                  (Contact c) => { return c.DisplayName; }, true);
              _allContacts = e.Results.ToList();
          }

          /// <summary>
          /// Gets a list of contacts ordered alphabetically.
          /// </summary>
          /// <returns>A list of AlphaKeyGroup, where each one contains the contacts starting by the letter represented by the group.</returns>
          public List<AlphaKeyGroup<Contact>> GetContactsGroupedByLetters()
          {
              return _contacts;
          }

          /// <summary>
          /// Gets a list of contacts recently used to display in hubtiles.
          /// </summary>
          /// <returns>A list of LinphoneContact.</returns>
          public List<Contact> GetRecentContacts()
          {
              List<CallLog> history = LinphoneManager.Instance.GetCallsHistory();
              //Get all contacts whe are in the history logs.
              var recent = new List<Contact>();
              foreach (var log in history)
              {
                  if (recent.Count >= RECENT_CONTACTS_MAX)
                      break;


                  try
                  {
                      Contact contact = (from c in _allContacts where (log.DisplayedName.Equals(c.DisplayName)) select c).FirstOrDefault();
                      if (contact != null && !recent.Contains(contact))
                          recent.Add(contact);
                  }
                  catch { } // Prevent the app from crashing if current contact's displayname in the iteration is null
              }

              return recent;
          }*/

        private static bool IsPhoneNumber(string str)
        {
            foreach (char c in str)
            {
                if ((c < '0' || c > '9') && c != '+')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Searches if there is a contact for whom the phone number of the email address is stored.
        /// </summary>
        /// <param name="numberOrAddress"></param>
        public void FindContact(String numberOrAddress)
        {
            Address address = LinphoneManager.Instance.Core.InterpretURL(numberOrAddress);
            string addressWithoutScheme = String.Format("{0}@{1}", address.UserName, address.Domain);
            string username = address.UserName;
            if (IsPhoneNumber(username))
            {
                FindContactByNumber(username, addressWithoutScheme);
            }
          //  FindContactByEmail(addressWithoutScheme, addressWithoutScheme);
        }

        /// <summary>
        /// Searches if a there is a contact for whom the phone number is stored.
        /// </summary>
        /// <param name="number">phone number to use to filter the contacts.</param>
        private void FindContactByNumber(String number, String original)
        {
            //Microsoft.Phone.UserData.Contacts contacts = new Microsoft.Phone.UserData.Contacts();
            //contacts.SearchCompleted += contact_PhoneSearchCompleted;

            //contacts.SearchAsync(number, FilterKind.PhoneNumber, original);
        }

        /*private void contact_PhoneSearchCompleted(object sender, Microsoft.Phone.UserData.ContactsSearchEventArgs e)
        {
            string number = e.Filter;
            Contact result = e.Results.FirstOrDefault();
            if (result != null)
            {
                String label = null;
                foreach (ContactPhoneNumber phone in result.PhoneNumbers)
                {
                    // We know this contact has this phone number stored.
                    // That's why we strip the phone number from the 3 first characters (maybe international prefix): to facilitate the label search.
                    if (phone.PhoneNumber.EndsWith(number.Substring(3)))
                    {
                        label = phone.Kind.ToString();
                    }
                }
                ContactFound(this, new ContactFoundEventArgs(result, number, label, (string)e.State));
            }
        }

        /// <summary>
        /// Searches if a there is a contact for whom the email is stored.
        /// </summary>
        /// <param name="email">email to use to filter the contacts.</param>
        private void FindContactByEmail(String email, String original)
        {
            Microsoft.Phone.UserData.Contacts contacts = new Microsoft.Phone.UserData.Contacts();
            contacts.SearchCompleted += contact_EmailSearchCompleted;

            contacts.SearchAsync(email, FilterKind.EmailAddress, original);
        }

        private void contact_EmailSearchCompleted(object sender, Microsoft.Phone.UserData.ContactsSearchEventArgs e)
        {
            string address = e.Filter;
            Contact result = e.Results.FirstOrDefault();
            if (result != null)
            {
                String label = null;
                foreach (ContactEmailAddress email in result.EmailAddresses)
                {
                    if (email.EmailAddress.Equals(address))
                    {
                        label = email.Kind.ToString();
                    }
                }
                ContactFound(this, new ContactFoundEventArgs(result, address, label, (string)e.State));
            }
        }*/
    }
}
