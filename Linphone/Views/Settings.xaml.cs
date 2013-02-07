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
using System.IO;
using System.Text;

namespace Linphone.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        private SettingsManager _appSettings = new SettingsManager();

        public Settings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            Debug.IsChecked = _appSettings.DebugEnabled;
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            _appSettings.DebugEnabled = Debug.IsChecked;

            NavigationService.GoBack();
        }

        private void account_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AccountSettings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void codecs_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/CodecsSettings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void advanced_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AdvancedSettings.xaml", UriKind.RelativeOrAbsolute));
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

        private void Simulate_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create an HTTPWebRequest that posts the raw notification to the Microsoft Push Notification Service. 
                // HTTP POST is the only method allowed to send the notification. 
                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(((App)App.Current).PushChannelUri);
                sendNotificationRequest.Method = "POST";

                // Create the raw message. 
                string rawMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<root>" +
                    "<Value1>" + "Sylvain Berfini" + "<Value1>" +
                    "<Value2>" + "+33952636505" + "<Value2>" +
                "</root>";
                byte[] notificationMessage = Encoding.UTF8.GetBytes(rawMessage);

                // Set the required web request headers 
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers["X-NotificationClass"] = "4";

                // Write the request body 
                sendNotificationRequest.BeginGetRequestStream((IAsyncResult arRequest) =>
                {
                    try
                    {
                        using (Stream requestStream = sendNotificationRequest.EndGetRequestStream(arRequest))
                        {
                            requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                        }

                        // Get the response. 
                        sendNotificationRequest.BeginGetResponse((IAsyncResult arResponse) =>
                        {
                            try
                            {
                                HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.EndGetResponse(arResponse);
                                string notificationStatus = response.Headers["X-NotificationStatus"];
                                string subscriptionStatus = response.Headers["X-SubscriptionStatus"];
                                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];
                            }
                            catch (Exception ex)
                            {
                            }
                        }, null);
                    }
                    catch (Exception ex)
                    {
                    }
                }, null);
            }
            catch (Exception ex)
            {
            } 
        }
    }
}