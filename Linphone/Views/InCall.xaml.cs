using Linphone.Core;
using Linphone.Model;
using Linphone.Resources;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
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

        private Timer timer;
        private DateTimeOffset startTime;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public InCall()
        {
            InitializeComponent();

            var call = LinphoneManager.Instance.LinphoneCore.GetCurrentCall();
            if (call != null && call.GetState() == Core.LinphoneCallState.StreamsRunning)
            {
                PauseStateChanged(false);
            }
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Searches for a matching contact using the current call address or number and display information if found.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            // Create LinphoneCore if not created yet, otherwise do nothing
            LinphoneManager.Instance.InitLinphoneCore();

            base.OnNavigatedTo(nee);
            this.ViewModel.MuteListener = this;
            this.ViewModel.PauseListener = this;

            if (NavigationContext.QueryString.ContainsKey("sip"))
            {
                String calledNumber = NavigationContext.QueryString["sip"];
                if (calledNumber.StartsWith("sip:"))
                {
                    calledNumber = calledNumber.Substring(4);
                }
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
            if (timer != null)
            {
                timer.Dispose();
            }

            base.OnNavigatedFrom(nee);
            this.ViewModel.MuteListener = null;
            this.ViewModel.PauseListener = null;
        }

        /// <summary>
        /// Callback called when the search on a phone number for a contact has a match
        /// </summary>
        private void cm_ContactFound(object sender, ContactFoundEventArgs e)
        {
            if (e.ContactFound != null)
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
        }

        private void hangUp_Click(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Dispose();
            }
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

        private void stats_Click_1(object sender, RoutedEventArgs e)
        {
            bool areStatsVisible = (bool)stats.IsChecked;
            emptyPanel.Visibility = areStatsVisible ? Visibility.Collapsed : Visibility.Visible;
            statsPanel.Visibility = areStatsVisible ? Visibility.Visible : Visibility.Collapsed;
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

            if (!isCallPaused)
            {
                timer = new Timer(new TimerCallback(timerTick), null, 0, 1000);
            }
            else if (timer != null)
            {
                timer.Dispose();
            }
        }

        private void timerTick(Object state)
        {
            try
            {
                LinphoneCall call = LinphoneManager.Instance.LinphoneCore.GetCurrentCall();
                if (call == null)
                {
                    timer.Dispose();
                    return;
                }
                startTime = (DateTimeOffset)call.GetCallStartTimeFromContext();
                DateTimeOffset now = DateTimeOffset.Now;
                TimeSpan elapsed = now.Subtract(startTime);
                var ss = elapsed.Seconds;
                var mm = elapsed.Minutes;
                Status.Dispatcher.BeginInvoke(delegate()
                {
                    Status.Text = mm.ToString("00") + ":" + ss.ToString("00");

                    LinphoneCallStats stats = call.GetAudioStats();
                    if (stats != null)
                    {
                        DownBw.Text = String.Format(AppResources.StatDownloadBW + ": {0} kbit/s", stats.GetDownloadBandwidth());
                        UpBw.Text = String.Format(AppResources.StatUploadBW + ": {0} kbit/s", stats.GetUploadBandwidth());
                    }

                    LinphoneCallParams param = call.GetCurrentParamsCopy();
                    PayloadType pt = param.GetUsedAudioCodec();
                    if (pt != null) 
                    {
                        PType.Text = AppResources.StatAudioPayload + ": " + pt.GetMimeType() + "/" + pt.GetClockRate();
                    }
                });
            } catch {
                timer.Dispose();
            }
        }

        private void dialpad_Click_1(object sender, RoutedEventArgs e)
        {
            bool isDialpadVisible = (bool)dialpad.IsChecked;
            pause.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            speaker.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            microphone.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            numpad.Visibility = isDialpadVisible ? Visibility.Visible : Visibility.Collapsed;
            stats.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Numpad_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            String tag = button.Tag as String;
            LinphoneManager.Instance.LinphoneCore.SendDTMF(Convert.ToChar(tag));
        }

        /// <summary>
        /// Do not allow user to leave the incall page while call is active
        /// </summary>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}