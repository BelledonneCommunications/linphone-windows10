/*
StatusBar.xaml.cs
Copyright (C) 2016  Belledonne Communications, Grenoble, France
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

using Linphone.Model;
using System;
using Windows.UI.Xaml.Controls;
using BelledonneCommunications.Linphone.Native;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Resources;

namespace Linphone.Controls {

    public partial class StatusBar : UserControl {

        public StatusBar() {
            InitializeComponent();
        }

        public void RefreshStatus() {
            RegistrationState state;
            if (LinphoneManager.Instance.Core.DefaultProxyConfig == null)
                state = RegistrationState.None;
            else
                state = LinphoneManager.Instance.Core.DefaultProxyConfig.State;

            RefreshStatus(state);
        }

        public void RefreshStatus(RegistrationState state) {

            if (state == RegistrationState.Ok) {
                StatusLed.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/led_connected.png"));
                StatusText.Text = ResourceLoader.GetForCurrentView().GetString("Registered");
            } else if (state == RegistrationState.Progress) {
                StatusLed.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/led_inprogress.png"));
                StatusText.Text = ResourceLoader.GetForCurrentView().GetString("RegistrationInProgress");
            } else if (state == RegistrationState.Failed) {
                StatusLed.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/led_error.png"));
                if (LinphoneManager.Instance.Core.DefaultProxyConfig.Error == Reason.Forbidden) {
                    StatusText.Text = ResourceLoader.GetForCurrentView().GetString("RegistrationFailedForbidden");
                } else {
                    StatusText.Text = ResourceLoader.GetForCurrentView().GetString("RegistrationFailed");
                }
            } else {
                StatusLed.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/led_disconnected.png"));
                StatusText.Text = ResourceLoader.GetForCurrentView().GetString("Disconnected");
            }

        }

    }
}
