/*
ContactDetailsPage.xaml.cs
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

using Linphone.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Linphone.Controls
{
    public sealed partial class ContactDetailsPage : UserControl, INotifyPropertyChanged
    {
        public ContactDetailsPage()
        {
            this.InitializeComponent();
        }

        public event RoutedEventHandler ContactDetailsBackButtonClick;

        private void ContactDetailsHeader_ContactDetailsBackButtonClick(object sender, RoutedEventArgs e)
        {
            if (ContactDetailsBackButtonClick != null)
            {
                ContactDetailsBackButtonClick(sender, e);
            }
        }

        public ContactEntry Contact
        {
            get { return (ContactEntry)GetValue(ContactProperty); }
            set { SetValue(ContactProperty, value); UpdateProperties(); }
        }

        // Using a DependencyProperty as the backing store for Contact.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContactProperty =
            DependencyProperty.Register("Contact", typeof(ContactEntry), typeof(ContactDetailsPage), new PropertyMetadata(0));

        public event PropertyChangedEventHandler PropertyChanged;

        private String _contactName;
        public String ContactName
        {
            get { return _contactName; }
            set
            {
                _contactName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ContactName"));
            }
        }

        private String _contactSipAddress;
        public String ContactSipAddress
        {
            get { return _contactSipAddress; }
            set
            {
                _contactSipAddress = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ContactSipAddress"));
            }
        }

        private String _contactPhoneNumber;
        public String ContactPhoneNumber
        {
            get { return _contactPhoneNumber; }
            set
            {
                _contactPhoneNumber = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ContactPhoneNumber"));
            }
        }

        private void UpdateProperties()
        {
            ContactName = Contact.Name;
            ContactSipAddress = Contact.SipUri;
            ContactPhoneNumber = Contact.PhoneNumber;
        }
    }
}
