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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using Linphone.Controls;
using Linphone.Model;
using Microsoft.Phone;
using Microsoft.Phone.UserData;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Page displaying contact information + action buttons associated to phone numbers and email addresses.
    /// </summary>
    public partial class Contact : BasePage
    {
        private Microsoft.Phone.UserData.Contact contact { get; set; }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Contact()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method called when the page is displayed, fetches and display contact information and create actions.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            while (actions.Children.Count > 0)
            {
                actions.Children.RemoveAt(0);
            }

            contact = ContactManager.Instance.TempContact;
            contactName.Text = contact.DisplayName;

            Stream imgStream = contact.GetPicture();
            if (imgStream != null)
            {
                Image contactPicture = new Image();
                contactPicture.Width = 150;
                contactPicture.Height = 150;
                contactPicture.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                contactPicture.Source = PictureDecoder.DecodeJpeg(imgStream);
                contactPicture.Margin = new Thickness(0,0,0,15);
                actions.Children.Add(contactPicture);
            }

            foreach (ContactPhoneNumber phone in contact.PhoneNumbers)
            {
                ContactAction entry = new ContactAction();
                entry.Action = "/Assets/AppBar/feature.phone.png";
                entry.Action2 = "/Assets/AppBar/chat.png";
                entry.Label = phone.Kind.ToString();
                entry.NumberOrAddress = phone.PhoneNumber;
                entry.Click += action_Click_1;
                entry.Click2 += action_Click_2;
                actions.Children.Add(entry);
            }

            foreach (ContactEmailAddress email in contact.EmailAddresses)
            {
                ContactAction entry = new ContactAction();
                entry.Action = "/Assets/AppBar/feature.phone.png";
                entry.Action2 = "/Assets/AppBar/chat.png";
                entry.Label = email.Kind.ToString();
                entry.NumberOrAddress = email.EmailAddress;
                entry.Click += action_Click_1;
                entry.Click2 += action_Click_2;
                actions.Children.Add(entry);
            }
        }

        private void SetAddressGoToDialerAndCall(String address)
        {
            NavigationService.Navigate(new Uri("/Views/Dialer.xaml?sip=" + Utils.ReplacePlusInUri(address), UriKind.RelativeOrAbsolute));
        }

        private void action_Click_1(object sender, EventArgs e)
        {
            String numberOrAddress = (sender as Button).Tag.ToString();
            SetAddressGoToDialerAndCall(numberOrAddress);
        }

        private void action_Click_2(object sender, EventArgs e)
        {
            String numberOrAddress = (sender as Button).Tag.ToString();
            NavigationService.Navigate(new Uri("/Views/Chat.xaml?sip=" + Utils.ReplacePlusInUri(numberOrAddress), UriKind.RelativeOrAbsolute));
        }
    }
}