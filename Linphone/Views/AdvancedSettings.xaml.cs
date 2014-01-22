using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Navigation;
using Linphone.Agents;
using Linphone.Core;

namespace Linphone.Views
{
    /// <summary>
    ///  Page displaying advanced settings (such as Tunnel, DTMFs, ...)
    /// </summary>
    public partial class AdvancedSettings : BasePage
    {
        private CallSettingsManager _callSettings = new CallSettingsManager();
        private NetworkSettingsManager _networkSettings = new NetworkSettingsManager();
        private ChatSettingsManager _chatSettings = new ChatSettingsManager();
        private ApplicationSettingsManager _settings = new ApplicationSettingsManager();

        /// <summary>
        /// Public constructor.
        /// </summary>
        public AdvancedSettings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            _callSettings.Load();
            _networkSettings.Load();
            _chatSettings.Load();
            _settings.Load();

            rfc2833.IsChecked = _callSettings.SendDTFMsRFC2833;
            sipInfo.IsChecked = _callSettings.SendDTFMsSIPInfo;
            vibrator.IsChecked = _chatSettings.VibrateOnIncomingMessage;
            resizeDown.IsChecked = _chatSettings.ScaleDownSentPictures;

            List<string> mediaEncryptions = new List<string>
            {
                AppResources.MediaEncryptionNone,
                AppResources.MediaEncryptionSRTP,
            };
            MediaEncryption.ItemsSource = mediaEncryptions;
            MediaEncryption.SelectedItem = _networkSettings.MEncryption;

            List<string> firewallPolicies = new List<string>
            {
                AppResources.FirewallPolicyNone,
                AppResources.FirewallPolicyStun,
                AppResources.FirewallPolicyIce
            };
            FirewallPolicy.ItemsSource = firewallPolicies;
            FirewallPolicy.SelectedItem = _networkSettings.FWPolicy;

            Stun.Text = _networkSettings.StunServer;

            List<string> tunnelModes = new List<string>
            {
                AppResources.TunnelModeDisabled,
                AppResources.TunnelMode3GOnly,
                AppResources.TunnelModeAlways,
                AppResources.TunnelModeAuto
            };
            tunnelMode.ItemsSource = tunnelModes;
            tunnelMode.SelectedItem = _networkSettings.TunnelMode;
            tunnelPort.Text = _networkSettings.TunnelPort;
            tunnelServer.Text = _networkSettings.TunnelServer;

            List<string> transports = new List<string>
            {
                AppResources.TransportUDP,
                AppResources.TransportTCP,
                AppResources.TransportTLS
            };
            Transport.ItemsSource = transports;
            Transport.SelectedItem = _networkSettings.Transport;

            TunnelPanel.Visibility = LinphoneManager.Instance.LinphoneCore.IsTunnelAvailable() && Customs.IsTunnelEnabled ? Visibility.Visible : Visibility.Collapsed; //Hidden properties for now

            List<string> debugModes = new List<string>
            {
                OutputTraceDest.None.ToString(),
                OutputTraceDest.File.ToString(),
                OutputTraceDest.Debugger.ToString()
            };
            if (Customs.AllowTCPRemote)
                debugModes.Add(OutputTraceDest.TCPRemote.ToString());
            Debug.ItemsSource = debugModes;
            TCPRemote.Text = _settings.LogOption.Equals(ApplicationSettingsManager.LinphoneLogFileName) ? "" : _settings.LogOption;
            Debug.SelectedItem = _settings.LogDestination.ToString();
        }

        private void Debug_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Contains(OutputTraceDest.TCPRemote.ToString()))
                FadeIn.Begin();
            else
                FadeOut.Begin();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Create LinphoneCore if not created yet, otherwise do nothing
            await LinphoneManager.Instance.InitLinphoneCore();

            DeleteLogs.IsEnabled = BugCollector.HasLinphoneLogFile();
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            _callSettings.SendDTFMsRFC2833 = rfc2833.IsChecked;
            _callSettings.SendDTFMsSIPInfo = sipInfo.IsChecked;
            _callSettings.Save();

            _networkSettings.MEncryption = MediaEncryption.SelectedItem.ToString();
            _networkSettings.FWPolicy = FirewallPolicy.SelectedItem.ToString();
            _networkSettings.StunServer = Stun.Text;
            _networkSettings.TunnelMode = tunnelMode.SelectedItem.ToString();
            _networkSettings.TunnelServer = tunnelServer.Text;
            _networkSettings.TunnelPort = tunnelPort.Text;
            _networkSettings.Transport = Transport.SelectedItem.ToString();
            _networkSettings.Save();

            _chatSettings.VibrateOnIncomingMessage = vibrator.IsChecked;
            _chatSettings.ScaleDownSentPictures = resizeDown.IsChecked;
            _chatSettings.Save();

            OutputTraceDest debugMode = OutputTraceDest.None;
            if (Debug.SelectedItem.Equals(OutputTraceDest.File.ToString()))
                debugMode = OutputTraceDest.File;
            else if (Debug.SelectedItem.Equals(OutputTraceDest.Debugger.ToString()))
                debugMode = OutputTraceDest.Debugger;
            else if (Debug.SelectedItem.Equals(OutputTraceDest.TCPRemote.ToString()))
                debugMode = OutputTraceDest.TCPRemote;

            _settings.LogDestination = debugMode;
            if (debugMode == OutputTraceDest.TCPRemote)
                _settings.LogOption = TCPRemote.Text;
            _settings.DebugEnabled = debugMode != OutputTraceDest.None;
            _settings.Save();

            NavigationService.GoBack();
        }

        private void deleteLogs_Click_1(object sender, EventArgs e)
        {
            BugCollector.DeleteLinphoneLogFile();
            DeleteLogs.IsEnabled = false;
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarSave = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
            appBarSave.Text = AppResources.SaveSettings;
            ApplicationBar.Buttons.Add(appBarSave);
            appBarSave.Click += save_Click_1;

            ApplicationBarIconButton appBarCancel = new ApplicationBarIconButton(new Uri("/Assets/AppBar/cancel.png", UriKind.Relative));
            appBarCancel.Text = AppResources.CancelChanges;
            ApplicationBar.Buttons.Add(appBarCancel);
            appBarCancel.Click += cancel_Click_1;
        }
    }
}