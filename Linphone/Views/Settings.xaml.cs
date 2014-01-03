using Linphone.Agents;
using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Base setting page, provides access to each detailled settings + debug setting.
    /// </summary>
    public partial class Settings : BasePage
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            Video.Visibility = Customs.EnableVideo ? Visibility.Visible : Visibility.Collapsed;
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

        private void account_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AccountSettings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void audio_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AudioSettings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void video_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/VideoSettings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void advanced_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AdvancedSettings.xaml", UriKind.RelativeOrAbsolute));
        }

        private async void LockScreenSettings_Click_1(object sender, RoutedEventArgs e)
        {
            var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }
    }
}