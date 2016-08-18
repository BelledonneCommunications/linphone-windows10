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
using BelledonneCommunications.Linphone.Native;

namespace Linphone.Views
{

    public partial class AdvancedSettings : Page
    {
        private CallSettingsManager _callSettings = new CallSettingsManager();
        private NetworkSettingsManager _networkSettings = new NetworkSettingsManager();
        private ChatSettingsManager _chatSettings = new ChatSettingsManager();
        private ApplicationSettingsManager _settings = new ApplicationSettingsManager();
        private bool saveSettingsOnLeave = true;

        public AdvancedSettings()
        {
            this.InitializeComponent();

            _callSettings.Load();
            _networkSettings.Load();
            _chatSettings.Load();
            _settings.Load();

            rfc2833.IsOn = (bool) _callSettings.SendDTFMsRFC2833;
            sipInfo.IsOn = (bool) _callSettings.SendDTFMsSIPInfo;
            //vibrator.IsOn = (bool) _chatSettings.VibrateOnIncomingMessage;
            //resizeDown.IsOn = (bool) _chatSettings.ScaleDownSentPictures;

            List<string> mediaEncryptions = new List<string>
            {
                ResourceLoader.GetForCurrentView().GetString("MediaEncryptionNone"),
                ResourceLoader.GetForCurrentView().GetString("MediaEncryptionSRTP")
            };
            mediaEncryption.ItemsSource = mediaEncryptions;
            mediaEncryption.SelectedItem = _networkSettings.MEncryption;

            List<string> firewallPolicies = new List<string>
            {
                ResourceLoader.GetForCurrentView().GetString("FirewallPolicyNone"),
                ResourceLoader.GetForCurrentView().GetString("FirewallPolicyStun"),
                ResourceLoader.GetForCurrentView().GetString("FirewallPolicyIce")
            };
            firewallPolicy.ItemsSource = firewallPolicies;
            firewallPolicy.SelectedItem = _networkSettings.FWPolicy;

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

            TunnelPanel.Visibility = LinphoneManager.Instance.Core.Tunnel != null ? Visibility.Visible : Visibility.Collapsed; //Hidden properties for now

            Debug.IsOn = _settings.DebugEnabled;
            SendLogs.IsEnabled = _settings.DebugEnabled;
            ResetLogs.IsEnabled = _settings.DebugEnabled;
        }

        private void Save()
        {
            _callSettings.SendDTFMsRFC2833 = rfc2833.IsOn;
            _callSettings.SendDTFMsSIPInfo = sipInfo.IsOn;
            _callSettings.Save();

            _networkSettings.MEncryption = mediaEncryption.SelectedItem.ToString();
            _networkSettings.FWPolicy = firewallPolicy.SelectedItem.ToString();
            _networkSettings.StunServer = Stun.Text;

            if(TunnelPanel.Visibility == Visibility.Visible)
            {
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            saveSettingsOnLeave = true;
        }

        /// <summary>
        /// Method called when the page is hidden.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);
            LinphoneManager.Instance.LogUploadProgressIndicationEH -= LogUploadProgressIndication;
            BugReportUploadPopup.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Method called when the user is navigation away from this page
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (saveSettingsOnLeave)
            {
                Save();
            }
            base.OnNavigatingFrom(e);
        }

        private void LogUploadProgressIndication(int offset, int total)
        {         
            BugReportUploadProgressBar.Maximum = total;
            if (offset <= total)
            {
                BugReportUploadProgressBar.Value = offset;
            }
        }
        
        private void SendLogs_Click(object sender, RoutedEventArgs e)
        {
            saveSettingsOnLeave = false;
            BugReportUploadPopup.Visibility = Visibility.Visible;
            LinphoneManager.Instance.LogUploadProgressIndicationEH += LogUploadProgressIndication;
            LinphoneManager.Instance.Core.UploadLogCollection();
        }

        private void Debug_Checked(object sender, RoutedEventArgs e)
        {
            _settings.LogLevel = OutputTraceLevel.Message;
            SendLogs.IsEnabled = true;
            ResetLogs.IsEnabled = true;
        }

        private void Debug_Unchecked(object sender, RoutedEventArgs e)
        {
            _settings.LogLevel = OutputTraceLevel.None;
            SendLogs.IsEnabled = false;
            ResetLogs.IsEnabled = false;
        }

        private void ResetLogs_Click(object sender, RoutedEventArgs e)
        {
            LinphoneManager.Instance.resetLogCollection();
        }
    }
}