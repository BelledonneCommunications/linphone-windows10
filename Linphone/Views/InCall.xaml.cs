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
    public partial class InCall : BasePage, MuteChangedListener, PauseChangedListener
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
        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);
            this.ViewModel.MuteListener = this;
            this.ViewModel.PauseListener = this;
            
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
        /// Method called when the page is leaved.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);
            this.ViewModel.MuteListener = null;
            this.ViewModel.PauseListener = null;
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
            LinphoneManager.Instance.EnableSpeaker(isSpeakerToggled);
        }

        private void microphone_Click_1(object sender, RoutedEventArgs e)
        {
            bool isMicToggled = (bool)microphone.IsChecked;
            LinphoneManager.Instance.MuteMic(isMicToggled);

            if (isMicToggled)
                LinphoneManager.Instance.CallController.NotifyMuted();
            else
                LinphoneManager.Instance.CallController.NotifyUnmuted();
        }

        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        public void MuteStateChanged(Boolean isMicMuted)
        {
            microphone.IsChecked = isMicMuted;
            microImg.Source = new BitmapImage(new Uri(isMicMuted ? micOn : micOff, UriKind.RelativeOrAbsolute));
        }

        private void pause_Click_1(object sender, RoutedEventArgs e)
        {
            bool isPauseToggled = (bool)pause.IsChecked;
            if (isPauseToggled)
                LinphoneManager.Instance.PauseCurrentCall();
            else
                LinphoneManager.Instance.ResumeCurrentCall();
        }

        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        public void PauseStateChanged(bool isCallPaused)
        {
            pause.IsChecked = isCallPaused;
            pauseImg.Source = new BitmapImage(new Uri(isCallPaused ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));
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