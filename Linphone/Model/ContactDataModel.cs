/*
ContactDataModel.cs
Copyright(C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    class ContactDataModel
    {
        private static ContactDataModel _dataModel = new ContactDataModel();
        private ObservableCollection<ContactGroup> _contacts = new ObservableCollection<ContactGroup>();

        public ObservableCollection<ContactGroup> Contacts
        {
            get { return this._contacts; }
        }

        public static IEnumerable<ContactGroup> GetContacts()
        {
            return _dataModel.Fill();
        }

        private IEnumerable<ContactGroup> Fill()
        {
            if (Contacts.Count != 0) return Contacts;

            // TODO: Fill with real data
            AddEntry(new ContactEntry("Gautier PELLOUX-PRAYER", "gpelloux@sip.linphone.org", "+33 (0) 6 01 02 03 04"));
            AddEntry(new ContactEntry("François GRISEZ", "francois.grisez@sip.linphone.org", "+33 (0) 6 05 06 07 08"));
            AddEntry(new ContactEntry("Margaux CLERC", "margaux@sip.linphone.org", "+33 (0) 6 09 10 11 12"));
            AddEntry(new ContactEntry("Sylvain BERFINI", "viish@sip.linphone.org", "+33 (0) 6 13 14 15 16"));
            AddEntry(new ContactEntry("Guillaume BIENKOWSKI", "guillaume@sip.linphone.org", "+33 (0) 6 17 18 19 20"));
            AddEntry(new ContactEntry("Simon MORLAT", "smorlat@sip.linphone.org", "+33 (0) 6 21 22 23 24"));
            AddEntry(new ContactEntry("Jehan MONNIER", "jehan@sip.linphone.org", "+33 (0) 6 25 26 27 28"));
            AddEntry(new ContactEntry("Marielle RELLIER", "marielle@sip.linphone.org", "+33 (0) 6 29 30 31 32"));
            AddEntry(new ContactEntry("Ghislain MARY", "ghislain@sip.linphone.org", "+33 (0) 6 33 34 35 36"));

            return from contact in Contacts orderby contact.Name select contact;
        }

        private void AddEntry(ContactEntry entry)
        {
            ContactGroup group = null;
            foreach (ContactGroup hg in Contacts)
            {
                if (hg.Name == entry.Name.Substring(0, 1))
                {
                    group = hg;
                }
            }
            if (group == null)
            {
                group = new ContactGroup(entry.Name.Substring(0, 1));
                Contacts.Add(group);
            }
            group.Entries.Add(entry);
        }
    }

    public class ContactGroup
    {
        public ContactGroup(string name)
        {
            Name = name;
            Entries = new ObservableCollection<ContactEntry>();
        }

        public string Name { get; private set; }
        public ObservableCollection<ContactEntry> Entries { get; private set; }
    }

    public class ContactEntry
    {
        public ContactEntry(string name, string sipUri, string phoneNumber)
        {
            Name = name;
            SipUri = sipUri;
            PhoneNumber = phoneNumber;
        }

        public string Name { get; private set; }
        public string SipUri { get; private set; }
        public string PhoneNumber { get; private set; }
    }
}
