using Linphone.Model;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// InCall page, displayed for both incoming and outgoing calls.
    /// </summary>
    public partial class InCall : BasePage
    {
        private const string speakerOn = "/Assets/AppBar/speaker.png";
        private const string speakerOff = "/Assets/AppBar/speaker.png";
        private const string micOn = "/Assets/AppBar/mic.png";
        private const string micOff = "/Assets/AppBar/mic.png";
        private const string pauseOn = "/Assets/AppBar/play.png";
        private const string pauseOff = "/Assets/AppBar/pause.png";

        /// <summary>
        /// Public constructor.
        /// </summary>
        public InCall()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Searches for a matching contact using the current call address or number and display information if found.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            LinphoneManager.Instance.EnableDebug(SettingsManager.isDebugEnabled);
            // Create LinphoneCore if not created yet, otherwise do nothing
            LinphoneManager.Instance.InitLinphoneCore();

            if (NavigationContext.QueryString.ContainsKey("sip"))
            {
                String calledNumber = NavigationContext.QueryString["sip"];
                // While we dunno if the number matches a contact one, we consider it won't and we display the phone number as username
                Contact.Text = calledNumber;

                if (calledNumber != null && calledNumber.Length > 0)
                {
                    ContactManager cm = ContactManager.Instance;
                    cm.ContactFound += cm_ContactFound;
                    cm.FindContact(calledNumber);
                }
            }
        }

        /// <summary>
        /// Callback called when the search on a phone number for a contact has a match
        /// </summary>
        private void cm_ContactFound(object sender, ContactFoundEventArgs e)
        {
            Contact.Text = e.ContactFound.DisplayName;
            if (e.PhoneLabel != null)
            {
                Number.Text = e.PhoneLabel + " : " + e.PhoneNumber;
            }
            else
            {
                Number.Text = e.PhoneNumber;
            }
        }

        private void hangUp_Click(object sender, RoutedEventArgs e)
        {
            LinphoneManager.Instance.EndCurrentCall();
        }

        private void speaker_Click_1(object sender, RoutedEventArgs e)
        {
            bool isSpeakerToggled = (bool)speaker.IsChecked;
            speakerImg.Source = new BitmapImage(new Uri(isSpeakerToggled ? speakerOn : speakerOff, UriKind.RelativeOrAbsolute));
            LinphoneManager.Instance.LinphoneCore.EnableSpeaker(isSpeakerToggled);
        }

        private void microphone_Click_1(object sender, RoutedEventArgs e)
        {
            bool isMicToggled = (bool)microphone.IsChecked;
            microImg.Source = new BitmapImage(new Uri(isMicToggled ? micOn : micOff, UriKind.RelativeOrAbsolute));
            LinphoneManager.Instance.LinphoneCore.MuteMic(isMicToggled);

            if (isMicToggled)
                LinphoneManager.Instance.CallController.NotifyMuted();
            else
                LinphoneManager.Instance.CallController.NotifyUnmuted();
        }

        private void pause_Click_1(object sender, RoutedEventArgs e)
        {
            bool isPauseToggled = (bool)pause.IsChecked;
            pauseImg.Source = new BitmapImage(new Uri(isPauseToggled ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));
            if (isPauseToggled)
                LinphoneManager.Instance.PauseCurrentCall();
            else
                LinphoneManager.Instance.ResumeCurrentCall();
        }

        private void dialpad_Click_1(object sender, RoutedEventArgs e)
        {
            bool isDialpadVisible = (bool)dialpad.IsChecked;
            pause.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            speaker.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            microphone.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            numpad.Visibility = isDialpadVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Numpad_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            String tag = button.Tag as String;
            LinphoneManager.Instance.LinphoneCore.SendDTMF(Convert.ToChar(tag));
        }
    }
}