using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Page used to display SIP account settings
    /// </summary>
    public partial class AccountSettings : BasePage
    {
        private SIPAccountSettingsManager _settings = new SIPAccountSettingsManager();

        /// <summary>
        /// Public constructor.
        /// </summary>
        public AccountSettings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            _settings.Load();
            Username.Text = _settings.Username;
            Password.Password = _settings.Password;
            Domain.Text = _settings.Domain;
            Proxy.Text = _settings.Proxy;
            OutboundProxy.IsChecked = _settings.OutboundProxy;
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            _settings.Username = Username.Text;
            _settings.Password = Password.Password;
            _settings.Domain = Domain.Text;
            _settings.Proxy = Proxy.Text;
            _settings.OutboundProxy = OutboundProxy.IsChecked;
            _settings.Save();

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