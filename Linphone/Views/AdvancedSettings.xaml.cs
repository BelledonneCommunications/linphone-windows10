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
    public partial class AdvancedSettings : BasePage
    {
        private SettingsManager _appSettings = new SettingsManager();

        public AdvancedSettings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            rfc2833.IsChecked = _appSettings.SendDTFMsRFC2833;

            List<string> tunnelModes = new List<string>
            {
                AppResources.TunnelModeDisabled,
                AppResources.TunnelMode3GOnly,
                AppResources.TunnelModeAlways,
                AppResources.TunnelModeAuto
            };
            tunnelMode.ItemsSource = tunnelModes;
            tunnelMode.SelectedItem = _appSettings.TunnelMode;
            tunnelPort.Text = _appSettings.TunnelPort;
            tunnelServer.Text = _appSettings.TunnelServer;

            List<string> transports = new List<string>
            {
                AppResources.TransportUDP,
                AppResources.TransportTCP
            };
            Transport.ItemsSource = transports;
            Transport.SelectedItem = _appSettings.Transport;
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            _appSettings.SendDTFMsRFC2833 = rfc2833.IsChecked;
            _appSettings.TunnelMode = tunnelMode.SelectedItem.ToString();
            _appSettings.TunnelServer = tunnelServer.Text;
            _appSettings.TunnelPort = tunnelPort.Text;
            _appSettings.Transport = Transport.SelectedItem.ToString();

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