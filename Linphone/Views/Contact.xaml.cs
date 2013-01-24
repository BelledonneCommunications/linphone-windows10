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
    public partial class Contact : PhoneApplicationPage
    {
        private Microsoft.Phone.UserData.Contact contact { get; set; }

        public Contact()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
                actions.Children.Add(contactPicture);
            }

            foreach (ContactPhoneNumber phone in contact.PhoneNumbers)
            {
                ContactAction entry = new ContactAction();
                entry.Action = "/Assets/AppBar/feature.phone.png";
                entry.Label = phone.Kind.ToString();
                entry.Phone = phone.PhoneNumber;
                actions.Children.Add(entry);
            }
        }
    }
}