using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public Contact ContactFound;

        /// <summary>
        /// Phone number used to find this contact.
        /// </summary>
        public String PhoneNumber;

        /// <summary>
        /// Phone label associated to the phone number.
        /// </summary>
        public String PhoneLabel;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ContactFoundEventArgs(Contact contact, String number, String label = null)
        {
            ContactFound = contact;
            PhoneNumber = number;
            PhoneLabel = label;
        }
    }

    /// <summary>
    /// Wrapper above system contact class allowing some changes for better display.
    /// </summary>
    public class LinphoneContact
    {
        private Contact _contact;

        /// <summary>
        /// Returns the system Contact object.
        /// </summary>
        public Contact Contact
        {
            get
            {
                return _contact;
            }
        }

        /// <summary>
        /// Returns the Contact's display name formatted for hub display.
        /// </summary>
        public string HubDisplayName
        {
            get
            {
                return _contact.DisplayName.Replace(" ", "\n");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LinphoneContact(Contact contact)
        {
            _contact = contact;
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
        private String tempNumberForContactLookup;

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
        public List<LinphoneContact> GetRecentContacts()
        {
            List<CallLog> history = LinphoneManager.Instance.GetCallsHistory();
            //Get all contacts whe are in the history logs.
            var result = (from contact in _allContacts where ((from log in history where (log.DisplayedName.Equals(contact.DisplayName)) select log).Count() > 0) select new LinphoneContact(contact)).ToList();
            
            if (result.Count > RECENT_CONTACTS_MAX)
                result = result.GetRange(0, RECENT_CONTACTS_MAX);

            return result;
        }

        /// <summary>
        /// Searches if there is a contact for whom the phone number of the email address is stored.
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
        /// Searches if a there is a contact for whom the phone number is stored.
        /// </summary>
        /// <param name="number">phone number to use to filter the contacts.</param>
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
                    // That's why we strip the phone number from the 3 first characters (maybe international prefix): to facilitate the label search.
                    if (phone.PhoneNumber.EndsWith(tempNumberForContactLookup.Substring(3)))
                    {
                        label = phone.Kind.ToString();
                    }
                }
                ContactFound(this, new ContactFoundEventArgs(result, tempNumberForContactLookup, label));
            }
        }

        /// <summary>
        /// Searches if a there is a contact for whom the email is stored.
        /// </summary>
        /// <param name="email">email to use to filter the contacts.</param>
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
