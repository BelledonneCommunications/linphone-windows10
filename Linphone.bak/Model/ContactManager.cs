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

using Linphone.Core;
using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
    public class ContactManager
    {
        private const int RECENT_CONTACTS_MAX = 6;

        private static ContactManager singleton;
        /// <summary>
        /// Static instance for this class.
        /// </summary>
        public static ContactManager Instance
        {
            get 
            {
                if (ContactManager.singleton == null)
                    ContactManager.singleton = new ContactManager();

                return ContactManager.singleton;
            }
        }

        private List<AlphaKeyGroup<Contact>> _contacts;
        private List<Contact> _allContacts;

        /// <summary>
        /// Represents the selected contact clicked on by the user.
        /// We keep it here to avoid giving it as param when navigating from Contacts.xaml to Contact.xaml
        /// </summary>
        public Contact TempContact;

        /// <summary>
        /// Delegate for contact found event.
        /// </summary>
        public delegate void ContactFoundEventHandler(object sender, ContactFoundEventArgs e);

        /// <summary>
        /// Handler for contact found event.
        /// </summary>
        public event ContactFoundEventHandler ContactFound;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ContactManager()
        {
            Microsoft.Phone.UserData.Contacts contacts = new Microsoft.Phone.UserData.Contacts();
            contacts.SearchCompleted += contacts_SearchCompleted;

            contacts.SearchAsync(String.Empty, FilterKind.None, "Phone Contacts");
        }

        private void contacts_SearchCompleted(object sender, Microsoft.Phone.UserData.ContactsSearchEventArgs e)
        {
            _contacts = AlphaKeyGroup<Contact>.CreateGroups(e.Results, System.Threading.Thread.CurrentThread.CurrentUICulture,
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
        }

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
            LinphoneAddress address = LinphoneManager.Instance.LinphoneCore.InterpretURL(numberOrAddress);
            string addressWithoutScheme = String.Format("{0}@{1}", address.UserName, address.Domain);
            string username = address.UserName;
            if (IsPhoneNumber(username))
            {
                FindContactByNumber(username, addressWithoutScheme);
            }
            FindContactByEmail(addressWithoutScheme, addressWithoutScheme);
        }

        /// <summary>
        /// Searches if a there is a contact for whom the phone number is stored.
        /// </summary>
        /// <param name="number">phone number to use to filter the contacts.</param>
        private void FindContactByNumber(String number, String original)
        {
            Microsoft.Phone.UserData.Contacts contacts = new Microsoft.Phone.UserData.Contacts();
            contacts.SearchCompleted += contact_PhoneSearchCompleted;

            contacts.SearchAsync(number, FilterKind.PhoneNumber, original);
        }

        private void contact_PhoneSearchCompleted(object sender, Microsoft.Phone.UserData.ContactsSearchEventArgs e)
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
        }
    }
}
