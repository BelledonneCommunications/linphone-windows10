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
            await LinphoneManager.Instance.InitLinphoneCore();

            if (LinphoneManager.Instance.LinphoneCore.GetProxyConfigList().Count > 0)
            {
                NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                NavigationService.RemoveBackEntry(); // Prevent a back to this screen from the dialer
            }
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
            if (Username.Text.Length > 0 && Domain.Text.Length > 0)
            {
                LogIn.IsEnabled = false;
                Register.IsEnabled = false;
                LinphoneLogIn.IsEnabled = false;
                Loading.Visibility = Visibility.Visible;
                Username.IsEnabled = false;
                Password.IsEnabled = false;
                Domain.IsEnabled = false;

                _settings.Username = Username.Text;
                _settings.Password = Password.Text;
                _settings.Domain = Domain.Text;
                _settings.Save();

                NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                NavigationService.RemoveBackEntry(); // Prevent a back to this screen from the dialer
            }
            else
            {
                ErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void LinphoneLogIn_Click(object sender, RoutedEventArgs e)
        {
            Domain.Text = "sip.linphone.org";
            LinphoneLogIn.Visibility = Visibility.Collapsed;
            ErrorMessage.Visibility = Visibility.Collapsed;
        }
    }
}