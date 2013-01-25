using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Resources;
using Linphone.Model;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace Linphone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private LocalizedStrings _appStrings = new LocalizedStrings();

        public MainPage()
        {
            InitializeComponent();
            numpad.Address = sipAddress;
            BuildLocalizedApplicationBar();

            ContactManager contactManager = ContactManager.Instance; //Force creation and init of ContactManager

            call.Click += call_Click_1;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LinphoneManager.Instance.EnableDebug(SettingsManager.isDebugEnabled);

            // Check for the navigation direction to avoid going to incall view when coming back from incall view
            if (NavigationContext.QueryString.ContainsKey("sip") && e.NavigationMode != NavigationMode.Back)
            {
                String sipAddressToCall = NavigationContext.QueryString["sip"];
                sipAddress.Text = sipAddressToCall;
                NewOutgoingCall(sipAddressToCall);
            }
        }

        private void NewOutgoingCall(String address)
        {
            if (address != null && address.Length > 0)
            {
                NavigationService.Navigate(new Uri("/Views/InCall.xaml?sip=" + address, UriKind.RelativeOrAbsolute));
                LinphoneManager.Instance.NewOutgoingCall(numpad.Address.Text);
            }
        }

        private void call_Click_1(object sender, EventArgs e)
        {
            NewOutgoingCall(numpad.Address.Text);
        }

        private void history_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/History.xaml", UriKind.RelativeOrAbsolute));
        }

        private void contacts_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Contacts.xaml", UriKind.RelativeOrAbsolute));
        }

        private void settings_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void about_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/About.xaml", UriKind.RelativeOrAbsolute));
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarHistory = new ApplicationBarIconButton(new Uri("/Assets/AppBar/time.png", UriKind.Relative));
            appBarHistory.Text = AppResources.HistoryMenu;
            ApplicationBar.Buttons.Add(appBarHistory);
            appBarHistory.Click += history_Click_1;

            ApplicationBarIconButton appBarContacts = new ApplicationBarIconButton(new Uri("/Assets/AppBar/people.contacts.png", UriKind.Relative));
            appBarContacts.Text = AppResources.ContactsMenu;
            ApplicationBar.Buttons.Add(appBarContacts);
            appBarContacts.Click += contacts_Click_1;

            ApplicationBarIconButton appBarSettings = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.settings.png", UriKind.Relative));
            appBarSettings.Text = AppResources.SettingsMenu;
            ApplicationBar.Buttons.Add(appBarSettings);
            appBarSettings.Click += settings_Click_1;

            ApplicationBarMenuItem appBarAbout = new ApplicationBarMenuItem(AppResources.AboutMenu);
            appBarAbout.Click += about_Click_1;
            ApplicationBar.MenuItems.Add(appBarAbout);
        }
    }
}