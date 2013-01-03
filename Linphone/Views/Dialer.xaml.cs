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

namespace Linphone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private LocalizedStrings _appStrings = new LocalizedStrings();

        public MainPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            // Listener for 0 button: when long pressed, remove the 0 and put a + instead
            DispatcherTimer holdTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            zero.ManipulationStarted += (a, b) => { holdTimer.Start(); };
            zero.ManipulationCompleted += (c, d) => { holdTimer.Stop(); };
            holdTimer.Tick += (s1, e1) =>
            {
                holdTimer.Stop();
                sipAddress.Text = sipAddress.Text.Substring(0, sipAddress.Text.Length - 1); ;
                sipAddress.Text += "+";
            };
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

        private void digit_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = (sender as Button);
            String character = (button.Tag as String);
            sipAddress.Text += character;
        }

        private void sipAddress_ActionIconTapped_1(object sender, EventArgs e)
        {
            if (sipAddress.Text.Length > 0)
                sipAddress.Text = sipAddress.Text.Substring(0, sipAddress.Text.Length - 1);
        }
    }
}