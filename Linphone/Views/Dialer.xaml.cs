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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LinphoneManager.Instance.EnableDebug(SettingsManager.isDebugEnabled);

            if (NavigationContext.QueryString.ContainsKey("sip"))
            {
                String sipAddressToCall = NavigationContext.QueryString["sip"];
                sipAddress.Text = sipAddressToCall;
                LinphoneManager.Instance.NewOutgoingCall(sipAddressToCall);
            }
        }

        private void history_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/History.xaml", UriKind.RelativeOrAbsolute));
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

            ApplicationBarIconButton appBarHistory = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.phone.png", UriKind.Relative));
            appBarHistory.Text = AppResources.HistoryMenu;
            ApplicationBar.Buttons.Add(appBarHistory);
            appBarHistory.Click += history_Click_1;

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