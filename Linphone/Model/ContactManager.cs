using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public class ContactFoundEventArgs : EventArgs
    {
        public Contact ContactFound;
        public String PhoneNumber;
        public String PhoneLabel;
        public ContactFoundEventArgs(Contact contact, String number, String label = null)
        {
            ContactFound = contact;
            PhoneNumber = number;
            PhoneLabel = label;
        }
    }

    public class ContactManager
    {
        private static ContactManager singleton;
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
        private String tempNumberForContactLookup;
        public Contact TempContact;

        public delegate void ContactFoundEventHandler(object sender, ContactFoundEventArgs e);
        public event ContactFoundEventHandler ContactFound;

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
        }

        /// <summary>
        /// Gets a list of contacts ordered alphabetically
        /// </summary>
        /// <returns>A list of AlphaKeyGroup, where each one contains the contacts starting by the letter represented by the group</returns>
        public List<AlphaKeyGroup<Contact>> GetContactsGroupedByLetters()
        {
            return _contacts;
        }

        /// <summary>
        /// Searches if there is a contact for whom the phone number of the email address is stored
        /// </summary>
        /// <param name="numberOrAddress"></param>
        public void FindContact(String numberOrAddress)
        {
            if (numberOrAddress.Contains('@'))
            {
                FindContactByEmail(numberOrAddress);
            }
            else
            {
                FindContactByNumber(numberOrAddress);
            }
        }

        /// <summary>
        /// Searches if a there is a contact for whom the phone number is stored 
        /// </summary>
        /// <param name="number">phone number to use to filter the contacts</param>
        private void FindContactByNumber(String number)
        {
            Microsoft.Phone.UserData.Contacts contacts = new Microsoft.Phone.UserData.Contacts();
            tempNumberForContactLookup = number;
            contacts.SearchCompleted += contact_PhoneSearchCompleted;

            contacts.SearchAsync(tempNumberForContactLookup, FilterKind.PhoneNumber, "Search by phone number");
        }

        private void contact_PhoneSearchCompleted(object sender, Microsoft.Phone.UserData.ContactsSearchEventArgs e)
        {
            Contact result = e.Results.FirstOrDefault();
            if (result != null)
            {
                String label = null;
                foreach (ContactPhoneNumber phone in result.PhoneNumbers)
                {
                    // We know this contact has this phone number stored.
                    // That's why we strip the phone number from the 3 first characters (maybe international prefix): to facilitate the label search
                    if (phone.PhoneNumber.EndsWith(tempNumberForContactLookup.Substring(3)))
                    {
                        label = phone.Kind.ToString();
                    }
                }
                ContactFound(this, new ContactFoundEventArgs(result, tempNumberForContactLookup, label));
            }
        }

        /// <summary>
        /// Searches if a there is a contact for whom the phone number is stored 
        /// </summary>
        /// <param name="number">phone number to use to filter the contacts</param>
        private void FindContactByEmail(String email)
        {
            Microsoft.Phone.UserData.Contacts contacts = new Microsoft.Phone.UserData.Contacts();
            tempNumberForContactLookup = email;
            contacts.SearchCompleted += contact_EmailSearchCompleted;

            contacts.SearchAsync(tempNumberForContactLookup, FilterKind.EmailAddress, "Search by email address");
        }

        private void contact_EmailSearchCompleted(object sender, Microsoft.Phone.UserData.ContactsSearchEventArgs e)
        {
            Contact result = e.Results.FirstOrDefault();
            if (result != null)
            {
                String label = null;
                foreach (ContactEmailAddress email in result.EmailAddresses)
                {
                    if (email.EmailAddress.Equals(tempNumberForContactLookup))
                    {
                        label = email.Kind.ToString();
                    }
                }
                ContactFound(this, new ContactFoundEventArgs(result, tempNumberForContactLookup, label));
            }
        }
    }
}
