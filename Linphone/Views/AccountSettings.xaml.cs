using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Resources;
using Linphone.Model;

namespace Linphone.Views
{
    public partial class AccountSettings : PhoneApplicationPage
    {
        private SettingsManager _appSettings = new SettingsManager();

        public AccountSettings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            Username.Text = _appSettings.Username;
            Password.Password = _appSettings.Password;
            Domain.Text = _appSettings.Domain;
            Proxy.Text = _appSettings.Proxy;
            OutboundProxy.IsChecked = _appSettings.OutboundProxy;
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            _appSettings.Username = Username.Text;
            _appSettings.Password = Password.Password;
            _appSettings.Domain = Domain.Text;
            _appSettings.Proxy = Domain.Text;
            _appSettings.OutboundProxy = OutboundProxy.IsChecked;

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