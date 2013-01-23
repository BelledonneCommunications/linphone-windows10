using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
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

        public List<AlphaKeyGroup<Contact>> GetContactsGroupedByLetters()
        {
            return _contacts;
        }
    }
}
