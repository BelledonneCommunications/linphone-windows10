using Linphone.Agents;
using Linphone.Core;
using Linphone.Model;
using Linphone.Resources;
using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// InCall page, displayed for both incoming and outgoing calls.
    /// </summary>
    public partial class InCall : BasePage, MuteChangedListener, PauseChangedListener, CallUpdatedByRemoteListener
    {
        private const string speakerOn = "/Assets/AppBar/speaker.png";
        private const string speakerOff = "/Assets/AppBar/speaker.png";
        private const string micOn = "/Assets/AppBar/mic.png";
        private const string micOff = "/Assets/AppBar/mic.png";
        private const string pauseOn = "/Assets/AppBar/play.png";
        private const string pauseOff = "/Assets/AppBar/pause.png";
        private const string videoOn = "/Assets/AppBar/feature.video.png";
        private const string videoOff = "/Assets/AppBar/feature.video.png";

        private Timer oneSecondTimer;
        private Timer fadeTimer;
        private DateTimeOffset startTime;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public InCall()
            : base(new InCallModel())
        {
            InitializeComponent();

            var call = LinphoneManager.Instance.LinphoneCore.GetCurrentCall();
            if (call != null && call.GetState() == Core.LinphoneCallState.StreamsRunning)
            {
                PauseStateChanged(call, false);
            }
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Searches for a matching contact using the current call address or number and display information if found.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs nee)
        {
            // Create LinphoneCore if not created yet, otherwise do nothing
            Task t = LinphoneManager.Instance.InitLinphoneCore();

            base.OnNavigatedTo(nee);
            this.ViewModel.MuteListener = this;
            this.ViewModel.PauseListener = this;
            this.ViewModel.CallUpdatedByRemoteListener = this;

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

            await t;
        }

        /// <summary>
        /// Method called when the page is leaved.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            if (oneSecondTimer != null)
            {
                oneSecondTimer.Dispose();
            }
            if (fadeTimer != null)
            {
                fadeTimer.Dispose();
                fadeTimer = null;
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
            if (oneSecondTimer != null)
            {
                oneSecondTimer.Dispose();
            }
            LinphoneManager.Instance.EndCurrentCall();
        }

        private void speaker_Click_1(object sender, RoutedEventArgs e)
        {
            bool isSpeakerToggled = (bool)speaker.IsChecked;
            try
            {
                LinphoneManager.Instance.EnableSpeaker(isSpeakerToggled);
                speakerImg.Source = new BitmapImage(new Uri(isSpeakerToggled ? speakerOn : speakerOff, UriKind.RelativeOrAbsolute));
            }
            catch 
            {
                Logger.Warn("Exception while trying to toggle speaker to {0}", isSpeakerToggled.ToString());
                speaker.IsChecked = !isSpeakerToggled;
            }
        }

        private void microphone_Click_1(object sender, RoutedEventArgs e)
        {
            bool isMicToggled = (bool)microphone.IsChecked;
            LinphoneManager.Instance.MuteMic(isMicToggled);
        }

        private void stats_Click_1(object sender, RoutedEventArgs e)
        {
            bool areStatsVisible = (bool)stats.IsChecked;
            emptyPanel.Visibility = areStatsVisible ? Visibility.Collapsed : Visibility.Visible;
            statsPanel.Visibility = areStatsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void video_Click_1(object sender, RoutedEventArgs e)
        {
            bool isVideoToggled = (bool)video.IsChecked;
            if (!LinphoneManager.Instance.EnableVideo(isVideoToggled))
            {
                if (isVideoToggled) video.IsChecked = false;
            }
            videoImg.Source = new BitmapImage(new Uri(isVideoToggled ? videoOn : videoOff, UriKind.RelativeOrAbsolute));
        }

        private void camera_Click_1(object sender, RoutedEventArgs e)
        {
            ((InCallModel)ViewModel).ToggleCameras();
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
        public void PauseStateChanged(LinphoneCall call, bool isCallPaused)
        {
            pause.IsChecked = isCallPaused;
            pauseImg.Source = new BitmapImage(new Uri(isCallPaused ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));

            if (!isCallPaused)
            {
                oneSecondTimer = new Timer(new TimerCallback(timerTick), null, 0, 1000);
                if (call.GetCurrentParamsCopy().IsVideoEnabled() && !((InCallModel)ViewModel).IsVideoActive)
                {
                    // Show video if it was not shown yet
                    ((InCallModel)ViewModel).IsVideoActive = true;
                    video.IsChecked = true;
                    ButtonsFadeInVideoAnimation.Begin();
                    StartFadeTimer();
                }
                else if (!call.GetCurrentParamsCopy().IsVideoEnabled() && ((InCallModel)ViewModel).IsVideoActive)
                {
                    // Stop video if it is no longer active
                    ((InCallModel)ViewModel).IsVideoActive = false;
                    video.IsChecked = false;
                    ButtonsFadeInAudioAnimation.Begin();
                    StopFadeTimer();
                }
            }
            else if (oneSecondTimer != null)
            {
                oneSecondTimer.Dispose();
            }
        }

        /// <summary>
        /// Called when the call is updated by the remote party.
        /// </summary>
        /// <param name="call">The call that has been updated</param>
        /// <param name="isVideoAdded">A boolean telling whether the remote party added video</param>
        public void CallUpdatedByRemote(LinphoneCall call, bool isVideoAdded)
        {
            if (isVideoAdded)
            {
                Guide.BeginShowMessageBox(AppResources.VideoActivationPopupCaption,
                    AppResources.VideoActivationPopupContent,
                    new List<String> { "Accept", "Dismiss" },
                    0,
                    MessageBoxIcon.Alert,
                    asyncResult =>
                    {
                        int? res = Guide.EndShowMessageBox(asyncResult);
                        LinphoneCallParams parameters = call.GetCurrentParamsCopy();
                        if (res == 0)
                        {
                            parameters.EnableVideo(true);
                        }
                        LinphoneManager.Instance.LinphoneCore.AcceptCallUpdate(call, parameters);
                    },
                    null);
            }
        }

        private void timerTick(Object state)
        {
            try
            {
                LinphoneCall call = LinphoneManager.Instance.LinphoneCore.GetCurrentCall();
                if (call == null)
                {
                    oneSecondTimer.Dispose();
                    return;
                }
                startTime = (DateTimeOffset)call.GetCallStartTimeFromContext();
                DateTimeOffset now = DateTimeOffset.Now;
                TimeSpan elapsed = now.Subtract(startTime);
                var ss = elapsed.Seconds;
                var mm = elapsed.Minutes;
                Status.Dispatcher.BeginInvoke(delegate()
                {
                    LinphoneCallParams param = call.GetCurrentParamsCopy();
                    Status.Text = mm.ToString("00") + ":" + ss.ToString("00");

                    MediaEncryption.Text = String.Format(AppResources.StatMediaEncryption + ": {0}", param.GetMediaEncryption().ToString());

                    LinphoneCallStats audioStats = null;
                    try
                    {
                        audioStats = call.GetAudioStats();
                    }
                    catch { }

                    if (audioStats != null)
                    {
                        AudioDownBw.Text = String.Format(AppResources.StatDownloadBW + ": {0:0.00} kb/s", audioStats.GetDownloadBandwidth());
                        AudioUpBw.Text = String.Format(AppResources.StatUploadBW + ": {0:0.00} kb/s", audioStats.GetUploadBandwidth());
                        ICE.Text = String.Format(AppResources.StatICE + ": {0}", audioStats.GetIceState().ToString()); 
                    }

                    PayloadType audiopt = param.GetUsedAudioCodec();
                    if (audiopt != null) 
                    {
                        AudioPType.Text = AppResources.StatPayload + ": " + audiopt.GetMimeType() + "/" + audiopt.GetClockRate();
                    }

                    if (call.GetCurrentParamsCopy().IsVideoEnabled())
                    {
                        LinphoneCallStats videoStats = call.GetVideoStats();
                        if (videoStats != null)
                        {
                            VideoDownBw.Text = String.Format(AppResources.StatDownloadBW + ": {0:0.00} kb/s", videoStats.GetDownloadBandwidth());
                            VideoUpBw.Text = String.Format(AppResources.StatUploadBW + ": {0:0.00} kb/s", videoStats.GetUploadBandwidth());
                        }

                        PayloadType videopt = param.GetUsedVideoCodec();
                        if (videopt != null)
                        {
                            VideoPType.Text = AppResources.StatPayload + ": " + videopt.GetMimeType() + "/" + videopt.GetClockRate();
                        }

                        VideoStats.Visibility = Visibility.Visible;
                        VideoDownBw.Visibility = Visibility.Visible;
                        VideoUpBw.Visibility = Visibility.Visible;
                        VideoPType.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        VideoStats.Visibility = Visibility.Collapsed;
                        VideoDownBw.Visibility = Visibility.Collapsed;
                        VideoUpBw.Visibility = Visibility.Collapsed;
                        VideoPType.Visibility = Visibility.Collapsed;
                    }
                });
            } catch {
                oneSecondTimer.Dispose();
            }
        }

        private void dialpad_Click_1(object sender, RoutedEventArgs e)
        {
            ((InCallModel)ViewModel).ToggleDialpad();
        }

        private void Numpad_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            String tag = button.Tag as String;
            LinphoneManager.Instance.LinphoneCore.SendDTMF(Convert.ToChar(tag));
        }

        private void remoteVideo_MediaOpened_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Logger.Msg("RemoteVideo Opened: " + ((MediaElement)sender).Source.AbsoluteUri);
        }

        private void remoteVideo_MediaFailed_1(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            Logger.Err("RemoteVideo Failed: " + e.ErrorException.Message);
        }

        private void localVideo_MediaOpened_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Logger.Msg("LocalVideo Opened: " + ((MediaElement)sender).Source.AbsoluteUri);
        }

        private void localVideo_MediaFailed_1(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            Logger.Err("LocalVideo Failed: " + e.ErrorException.Message);
        }

        /// <summary>
        /// Do not allow user to leave the incall page while call is active
        /// </summary>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void ButtonsFadeOutAnimation_Completed(object sender, EventArgs e)
        {
            buttons.Visibility = Visibility.Collapsed;
            statsPanel.Visibility = Visibility.Collapsed;
            Status.Visibility = Visibility.Collapsed;
            Contact.Visibility = Visibility.Collapsed;
            Number.Visibility = Visibility.Collapsed;
        }

        private void HideButtons(Object state)
        {
            Status.Dispatcher.BeginInvoke(delegate()
            {
                ButtonsFadeOutAnimation.Begin();
            });
        }

        private void StartFadeTimer()
        {
            if (fadeTimer != null)
            {
                fadeTimer.Dispose();
            }
            fadeTimer = new Timer(new TimerCallback(HideButtons), null, 4000, Timeout.Infinite);
        }

        private void StopFadeTimer()
        {
            if (fadeTimer != null)
            {
                fadeTimer.Dispose();
                fadeTimer = null;
            }
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            buttons.Visibility = Visibility.Visible;
            statsPanel.Visibility = ((bool)stats.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            Status.Visibility = Visibility.Visible;
            Contact.Visibility = Visibility.Visible;
            Number.Visibility = Visibility.Visible;
            if (((InCallModel)ViewModel).IsVideoActive)
            {
                ButtonsFadeInVideoAnimation.Begin();
                StartFadeTimer();
            }
            else
            {
                ButtonsFadeInAudioAnimation.Begin();
            }
        }
    }
}