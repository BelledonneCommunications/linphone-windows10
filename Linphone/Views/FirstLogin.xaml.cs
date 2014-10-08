using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Model;
using Linphone.Resources;

namespace Linphone.Views
{
    /// <summary>
    /// Handles the first login and the account creation
    /// </summary>
    public partial class FirstLogin : BasePage
    {
        private SIPAccountSettingsManager _settings = new SIPAccountSettingsManager();

        /// <summary>
        /// Public constructor
        /// </summary>
        public FirstLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Create LinphoneCore if not created yet, otherwise do nothing
            LinphoneManager.Instance.InitLinphoneCore();

            if (LinphoneManager.Instance.LinphoneCore.GetProxyConfigList().Count > 0)
            {
                NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                NavigationService.RemoveBackEntry(); // Prevent a back to this screen from the dialer
            }
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            if (Username.Text.Length > 0 && Domain.Text.Length > 0)
            {
                LogIn.IsEnabled = false;
                Register.IsEnabled = false;
                LinphoneLogIn.IsEnabled = false;
                Loading.Visibility = Visibility.Visible;
                Username.IsEnabled = false;
                Password.IsEnabled = false;
                Domain.IsEnabled = false;

                NetworkSettingsManager networkSettings = new NetworkSettingsManager();
                _settings.Username = Username.Text;
                _settings.Password = Password.Password;
                _settings.Domain = Domain.Text;
                if (Domain.Text.Equals("sip.linphone.org"))
                {
                    _settings.Proxy = "<sip:sip.linphone.org:5223;transport=tls>";
                    _settings.Transport = AppResources.TransportTLS;
                    _settings.OutboundProxy = true;

                    networkSettings.StunServer = "stun.linphone.org";
                    networkSettings.FWPolicy = AppResources.FirewallPolicyIce;
                }
                networkSettings.Save();
                _settings.Save();

                NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                NavigationService.RemoveBackEntry(); // Prevent a back to this screen from the dialer
            }
            else
            {
                ErrorMessage.Text = AppResources.ErrorLogin;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void LinphoneLogIn_Click(object sender, RoutedEventArgs e)
        {
            Domain.Text = "sip.linphone.org";
            LinphoneLogIn.IsEnabled = false;
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
            NavigationService.RemoveBackEntry(); // Prevent a back to this screen from the dialer
        }
    }
}