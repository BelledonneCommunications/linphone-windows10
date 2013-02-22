using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using System.IO;
using Microsoft.Phone.UserData;
using Linphone.Model;
using Linphone.Controls;

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
            StatusBar = status;

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
                entry.Label = phone.Kind.ToString();
                entry.NumberOrAddress = phone.PhoneNumber;
                entry.Click += action_Click_1;
                actions.Children.Add(entry);
            }

            foreach (ContactEmailAddress email in contact.EmailAddresses)
            {
                ContactAction entry = new ContactAction();
                entry.Action = "/Assets/AppBar/feature.phone.png";
                entry.Label = email.Kind.ToString();
                entry.NumberOrAddress = email.EmailAddress;
                entry.Click += action_Click_1;
                actions.Children.Add(entry);
            }
        }

        private void SetAddressGoToDialerAndCall(String address)
        {
            NavigationService.Navigate(new Uri("/Views/Dialer.xaml?sip=" + address, UriKind.RelativeOrAbsolute));
        }

        private void action_Click_1(object sender, EventArgs e)
        {
            String numberOrAddress = (sender as Button).Tag.ToString();
            SetAddressGoToDialerAndCall(numberOrAddress);
        }
    }
}