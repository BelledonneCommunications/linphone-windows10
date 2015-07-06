/*
StatusBar.xaml.cs
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

using Linphone.Core;
using Linphone.Model;
using Linphone.Resources;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Linphone.Controls
{
    /// <summary>
    /// User control to be displayed on each page, giving the state of the account registration.
    /// </summary>
    public partial class StatusBar : UserControl
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public StatusBar()
        {
            InitializeComponent();
            RefreshStatus(LinphoneManager.Instance.LastKnownState);
        }

        /// <summary>
        /// Refresh the status icon reading the default proxy config state.
        /// </summary>
        public void RefreshStatus()
        {
            RegistrationState state;
            if (LinphoneManager.Instance.LinphoneCore.DefaultProxyConfig == null)
                state = RegistrationState.RegistrationNone;
            else
                state = LinphoneManager.Instance.LinphoneCore.DefaultProxyConfig.State;

            RefreshStatus(state);
        }

        /// <summary>
        /// Refresh the status icon given the registration state in param.
        /// </summary>
        public void RefreshStatus(RegistrationState state)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (state == RegistrationState.RegistrationOk) {
                    StatusLed.Source = new BitmapImage(new Uri("/Assets/led_connected.png", UriKind.Relative));
                    StatusText.Text = AppResources.Registered;
                }
                else if (state == RegistrationState.RegistrationInProgress) {
                    StatusLed.Source = new BitmapImage(new Uri("/Assets/led_inprogress.png", UriKind.Relative));
                    StatusText.Text = AppResources.RegistrationInProgress;
                }
                else if (state == RegistrationState.RegistrationFailed) {
                    StatusLed.Source = new BitmapImage(new Uri("/Assets/led_error.png", UriKind.Relative));
                    StatusText.Text = AppResources.RegistrationFailed;
                }
                else {
                    StatusLed.Source = new BitmapImage(new Uri("/Assets/led_disconnected.png", UriKind.Relative));
                    StatusText.Text = AppResources.Disconnected;
                }
            });
        }
    }
}
