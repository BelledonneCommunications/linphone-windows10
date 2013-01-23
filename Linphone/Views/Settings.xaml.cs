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
using System.Globalization;
using System.Resources;

namespace Linphone.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        private SettingsManager _appSettings = new SettingsManager();

        public Settings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            ResourceManager resourceManager = new ResourceManager("Linphone.Resources.AppResources", typeof(AppResources).Assembly);
            string tcp = resourceManager.GetString("TransportTCP", CultureInfo.CurrentCulture);
            string udp = resourceManager.GetString("TransportUDP", CultureInfo.CurrentCulture);
            List<string> transports = new List<string>
            {
                tcp,
                udp
            };
            Transport.ItemsSource = transports;

            Transport.SelectedItem = _appSettings.Transport;
            Debug.IsChecked = _appSettings.DebugEnabled;
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            _appSettings.DebugEnabled = Debug.IsChecked;
            _appSettings.Transport = Transport.SelectedItem.ToString();

            NavigationService.GoBack();
        }

        private void codecs_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/CodecsSettings.xaml", UriKind.RelativeOrAbsolute));
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

        private async void LockScreenSettings_Click_1(object sender, RoutedEventArgs e)
        {
            var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }
    }
}