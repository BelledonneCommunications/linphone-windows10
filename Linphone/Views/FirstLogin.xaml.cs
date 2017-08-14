/*
FirstLogin.xaml.cs
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Linphone.Model;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;

namespace Linphone.Views {
    /// <summary>
    /// Handles the first login and the account creation
    /// </summary>
    public partial class FirstLogin : Page {
        private SIPAccountSettingsManager _settings = new SIPAccountSettingsManager();

        /// <summary>
        /// Public constructor
        /// </summary>
        public FirstLogin() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            if (LinphoneManager.Instance.Core.ProxyConfigList.Count() > 0) {
                // NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                // NavigationService.RemoveBackEntry(); // Prevent a back to this screen from the dialer
            }
        }

        private void LogIn_Click(object sender, RoutedEventArgs e) {
            /* ErrorMessage.Text = "";
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

                 //NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
                 //NavigationService.RemoveBackEntry(); // Prevent a back to this screen from the dialer
             }
             else
             {
                 ErrorMessage.Text = AppResources.ErrorLogin;
             }*/
        }

        private void Register_Click(object sender, RoutedEventArgs e) {
            //TODO
        }

        private void LinphoneLogIn_Click(object sender, RoutedEventArgs e) {
            /* Domain.Text = "sip.linphone.org";
             LinphoneLogIn.IsEnabled = false;*/
        }

        private void Skip_Click(object sender, RoutedEventArgs e) {
            // NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
            // NavigationService.RemoveBackEntry(); // Prevent a back to this screen from the dialer
        }
    }
}