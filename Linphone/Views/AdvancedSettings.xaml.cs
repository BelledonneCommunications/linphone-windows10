/*
AdvancedSettings.xaml.cs
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

using Linphone.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Linphone;
using Windows.UI.Core;

namespace Linphone.Views {

    public partial class AdvancedSettings : Page {
        private CallSettingsManager _callSettings = new CallSettingsManager();
        private NetworkSettingsManager _networkSettings = new NetworkSettingsManager();
        private ChatSettingsManager _chatSettings = new ChatSettingsManager();
        private ApplicationSettingsManager _settings = new ApplicationSettingsManager();
        private bool saveSettingsOnLeave = true;

        public AdvancedSettings() {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += back_Click;

            _callSettings.Load();
            _networkSettings.Load();
            _chatSettings.Load();
            _settings.Load();

            rfc2833.IsOn = (bool)_callSettings.SendDTFMsRFC2833;
            sipInfo.IsOn = (bool)_callSettings.SendDTFMsSIPInfo;
            //vibrator.IsOn = (bool) _chatSettings.VibrateOnIncomingMessage;
            //resizeDown.IsOn = (bool) _chatSettings.ScaleDownSentPictures;

            List<string> mediaEncryptions = new List<string> {
                ResourceLoader.GetForCurrentView().GetString("MediaEncryptionNone")
            };

            if (LinphoneManager.Instance.Core.MediaEncryptionSupported(MediaEncryption.SRTP))
                mediaEncryptions.Add(ResourceLoader.GetForCurrentView().GetString("MediaEncryptionSRTP"));
            if (LinphoneManager.Instance.Core.MediaEncryptionSupported(MediaEncryption.DTLS))
                mediaEncryptions.Add(ResourceLoader.GetForCurrentView().GetString("MediaEncryptionDTLS"));
            if (LinphoneManager.Instance.Core.MediaEncryptionSupported(MediaEncryption.ZRTP))
                mediaEncryptions.Add(ResourceLoader.GetForCurrentView().GetString("MediaEncryptionZRTP"));


            mediaEncryption.ItemsSource = mediaEncryptions;
            mediaEncryption.SelectedItem = _networkSettings.MEncryption;

            ICE.IsOn = LinphoneManager.Instance.Core.NatPolicy.IceEnabled;

            Stun.Text = _networkSettings.StunServer;

            List<string> tunnelModes = new List<string>
            {
                ResourceLoader.GetForCurrentView().GetString("TunnelModeDisabled"),
                ResourceLoader.GetForCurrentView().GetString("TunnelMode3GOnly"),
                ResourceLoader.GetForCurrentView().GetString("TunnelModeAlways"),
                ResourceLoader.GetForCurrentView().GetString("TunnelModeAuto")
            };
            tunnelMode.ItemsSource = tunnelModes;
            tunnelMode.SelectedItem = _networkSettings.TunnelMode;
            tunnelPort.Text = _networkSettings.TunnelPort;
            tunnelServer.Text = _networkSettings.TunnelServer;
            IPV6.IsOn = LinphoneManager.Instance.Core.Ipv6Enabled;

            TunnelPanel.Visibility = LinphoneManager.Instance.Core.Tunnel != null ? Visibility.Visible : Visibility.Collapsed; //Hidden properties for now

            Debug.IsOn = _settings.DebugEnabled;
            SendLogs.IsEnabled = _settings.DebugEnabled;
            ResetLogs.IsEnabled = _settings.DebugEnabled;
        }

        private void Save() {
            _callSettings.SendDTFMsRFC2833 = rfc2833.IsOn;
            _callSettings.SendDTFMsSIPInfo = sipInfo.IsOn;
            _callSettings.Save();

            if (mediaEncryption.SelectedItem != null)
                _networkSettings.MEncryption = mediaEncryption.SelectedItem.ToString();
            _networkSettings.FWPolicy = ICE.IsOn;
            _networkSettings.StunServer = Stun.Text;
            _networkSettings.IPV6 = IPV6.IsOn;

            if (TunnelPanel.Visibility == Visibility.Visible) {
                _networkSettings.TunnelMode = tunnelMode.SelectedItem.ToString();
                _networkSettings.TunnelServer = tunnelServer.Text;
                _networkSettings.TunnelPort = tunnelPort.Text;
            }
            _networkSettings.Save();

            //_chatSettings.VibrateOnIncomingMessage = vibrator.IsOn;
            //_chatSettings.ScaleDownSentPictures = resizeDown.IsOn;
            _chatSettings.Save();

            _settings.DebugEnabled = (bool)Debug.IsOn;
            _settings.Save();

        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            saveSettingsOnLeave = true;
        }

        /// <summary>
        /// Method called when the page is hidden.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee) {
            base.OnNavigatedFrom(nee);
            LinphoneManager.Instance.getCoreListener().OnLogCollectionUploadProgressIndication -= LogUploadProgressIndication;
            LinphoneManager.Instance.getCoreListener().OnLogCollectionUploadStateChanged -= LogUploadStateChanged;
            BugReportUploadPopup.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Method called when the user is navigation away from this page
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            if (saveSettingsOnLeave) {
                Save();
            }
            base.OnNavigatingFrom(e);
        }

        private void LogUploadProgressIndication(Core lc, long offset, long total) {
            BugReportUploadProgressBar.Maximum = total;
            if (offset < total) {
                BugReportUploadProgressBar.Value = offset;
            } else {
                BugReportUploadPopup.Visibility = Visibility.Collapsed;
            }
        }

        private void LogUploadStateChanged(Core lc, CoreLogCollectionUploadState state, string info) {
            if (state.Equals(CoreLogCollectionUploadState.NotDelivered)) {
                BugReportUploadPopup.Visibility = Visibility.Collapsed;
            }
        }

        private void SendLogs_Click(object sender, RoutedEventArgs e) {
            saveSettingsOnLeave = false;
            BugReportUploadPopup.Visibility = Visibility.Visible;
            LinphoneManager.Instance.getCoreListener().OnLogCollectionUploadProgressIndication += LogUploadProgressIndication;
            LinphoneManager.Instance.getCoreListener().OnLogCollectionUploadStateChanged += LogUploadStateChanged;
            LinphoneManager.Instance.Core.UploadLogCollection();
        }

        private void Debug_Checked(object sender, RoutedEventArgs e) {
            _settings.DebugEnabled = true;
            _settings.LogLevelSetting = LogCollectionState.Enabled;
            SendLogs.IsEnabled = true;
            ResetLogs.IsEnabled = true;
        }

        private void Debug_Unchecked(object sender, RoutedEventArgs e) {
            _settings.DebugEnabled = false;
            _settings.LogLevelSetting = LogCollectionState.Disabled;
            SendLogs.IsEnabled = false;
            ResetLogs.IsEnabled = false;
        }

        private void ResetLogs_Click(object sender, RoutedEventArgs e) {
            LinphoneManager.Instance.resetLogCollection();
        }

        private void back_Click(object sender, BackRequestedEventArgs e) {
            if (Frame.CanGoBack) {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        private void Debug_Toggled(object sender, RoutedEventArgs e) {
            if (Debug.IsOn) {
                Debug_Checked(sender, e);
            } else {
                Debug_Unchecked(sender, e);
            }
        }
    }
}