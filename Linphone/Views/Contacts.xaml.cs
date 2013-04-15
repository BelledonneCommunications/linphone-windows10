using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.UserData;
using Linphone.Model;
using System.Diagnostics;

namespace Linphone.Views
{
    /// <summary>
    /// Page using native style list to display phone contacts.
    /// </summary>
    public partial class Contacts : BasePage
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public Contacts()
        {
            InitializeComponent();

            contactsList.ItemsSource = ContactManager.Instance.GetContactsGroupedByLetters();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        /// <param name="nee"></param>
        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);
        }

        private void contactsList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Microsoft.Phone.UserData.Contact selectedContact = ((sender as LongListSelector).SelectedItem as Microsoft.Phone.UserData.Contact);
            ContactManager.Instance.TempContact = selectedContact;
            NavigationService.Navigate(new Uri("/Views/Contact.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}