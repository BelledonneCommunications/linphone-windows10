using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Navigation;

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
            rfc2833.IsChecked = _callSettings.SendDTFMsRFC2833;
            sipInfo.IsChecked = _callSettings.SendDTFMsSIPInfo;
            vibrator.IsChecked = _chatSettings.VibrateOnIncomingMessage;

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

            TunnelPanel.Visibility = LinphoneManager.Instance.LinphoneCore.IsTunnelAvailable() && false ? Visibility.Visible : Visibility.Collapsed; //Hidden properties for now
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Create LinphoneCore if not created yet, otherwise do nothing
            await LinphoneManager.Instance.InitLinphoneCore();
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

            _networkSettings.TunnelMode = tunnelMode.SelectedItem.ToString();
            _networkSettings.TunnelServer = tunnelServer.Text;
            _networkSettings.TunnelPort = tunnelPort.Text;
            _networkSettings.Transport = Transport.SelectedItem.ToString();
            _networkSettings.Save();

            _chatSettings.VibrateOnIncomingMessage = vibrator.IsChecked;
            _chatSettings.Save();

            NavigationService.GoBack();
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