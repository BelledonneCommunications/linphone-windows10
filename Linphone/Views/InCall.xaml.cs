/*
InCall.xaml.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Windows.Phone.Media.Devices;

namespace Linphone.Views
{
    /// <summary>
    /// InCall page, displayed for both incoming and outgoing calls.
    /// </summary>
    public partial class InCall : BasePage, MuteChangedListener, PauseChangedListener, CallUpdatedByRemoteListener
    {
        private DispatcherTimer oneSecondTimer;
        private Timer fadeTimer;
        private DateTimeOffset startTime;

        private static double BUTTON_DISABLED_OPACITY = 0.4;
        private static double BUTTON_ENABLED_OPACITY = 1.0;

        private bool statsVisible = false;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public InCall()
            : base(new InCallModel())
        {
            InitializeComponent();

            var call = LinphoneManager.Instance.LinphoneCore.CurrentCall;
            if (call != null && call.State == Core.LinphoneCallState.StreamsRunning)
            {
                PauseStateChanged(call, false, false);
                if (LinphoneManager.Instance.SpeakerEnabled)
                {
                    buttons.speaker.IsChecked = true;
                    buttons_landscape.speaker.IsChecked = true;
                }
                if (LinphoneManager.Instance.LinphoneCore.MicMuted)
                {
                    buttons.microphone.IsChecked = true;
                    buttons_landscape.microphone.IsChecked = true;
                }
            }

            buttons.HangUpClick += buttons_HangUpClick;
            buttons_landscape.HangUpClick += buttons_HangUpClick;
            buttons.StatsClick += buttons_StatsClick;
            buttons_landscape.StatsClick += buttons_StatsClick;
            buttons.CameraClick += buttons_CameraClick;
            buttons_landscape.CameraClick += buttons_CameraClick;
            buttons.PauseClick += buttons_PauseClick;
            buttons_landscape.PauseClick += buttons_PauseClick;
            buttons.SpeakerClick += buttons_SpeakerClick;
            buttons_landscape.SpeakerClick += buttons_SpeakerClick;
            buttons.MuteClick += buttons_MuteClick;
            buttons_landscape.MuteClick += buttons_MuteClick;
            buttons.VideoClick += buttons_VideoClick;
            buttons_landscape.VideoClick += buttons_VideoClick;
            buttons.DialpadClick += buttons_DialpadClick;
            buttons_landscape.DialpadClick += buttons_DialpadClick;
            buttons.BluetoothClick += buttons_BluetoothClick;
            buttons_landscape.BluetoothClick += buttons_BluetoothClick;
        }

        private void buttons_DialpadClick(object sender, bool isDialpadShown)
        {
            buttons.dialpad.IsChecked = isDialpadShown;
            buttons_landscape.dialpad.IsChecked = isDialpadShown;
            buttons.numpad.Visibility = isDialpadShown ? Visibility.Visible : Visibility.Collapsed;
            buttons_landscape.numpad.Visibility = isDialpadShown ? Visibility.Visible : Visibility.Collapsed;
        }

        private void buttons_VideoClick(object sender, bool isVideoOn)
        {
            buttons.video.IsChecked = isVideoOn;
            buttons_landscape.video.IsChecked = isVideoOn;
            if (!LinphoneManager.Instance.EnableVideo(isVideoOn))
            {
                buttons.video.IsChecked = !isVideoOn;
                buttons_landscape.video.IsChecked = !isVideoOn;
            }
        }

        private void buttons_MuteClick(object sender, bool isMuteOn)
        {
            LinphoneManager.Instance.LinphoneCore.NotifyMute(isMuteOn);
            if (LinphoneManager.Instance.CallListener != null)
                LinphoneManager.Instance.CallListener.MuteStateChanged(isMuteOn);
        }

        private void buttons_BluetoothClick(object sender, bool isBluetoothOn)
        {
            buttons.speaker.IsChecked = false;
            buttons_landscape.speaker.IsChecked = false;
            buttons.bluetooth.IsChecked = isBluetoothOn;
            buttons_landscape.bluetooth.IsChecked = isBluetoothOn;
            try
            {
                LinphoneManager.Instance.BluetoothEnabled = isBluetoothOn;
            }
            catch
            {
                Logger.Warn("Exception while trying to toggle bluetooth to {0}", isBluetoothOn.ToString());
                buttons.bluetooth.IsChecked = isBluetoothOn;
                buttons_landscape.bluetooth.IsChecked = isBluetoothOn;
            }
        }

        private bool buttons_SpeakerClick(object sender, bool isSpeakerOn)
        {
            buttons.bluetooth.IsChecked = false;
            buttons_landscape.bluetooth.IsChecked = false;
            buttons.speaker.IsChecked = isSpeakerOn;
            buttons_landscape.speaker.IsChecked = isSpeakerOn;
            try
            {
                LinphoneManager.Instance.SpeakerEnabled = isSpeakerOn;
                return true;
            }
            catch
            {
                Logger.Warn("Exception while trying to toggle speaker to {0}", isSpeakerOn.ToString());
                buttons.speaker.IsChecked = isSpeakerOn;
                buttons_landscape.speaker.IsChecked = isSpeakerOn;
            }
            return false;
        }

        private void buttons_PauseClick(object sender, bool isPaused)
        {
            if (isPaused)
                LinphoneManager.Instance.PauseCurrentCall();
            else
                LinphoneManager.Instance.ResumeCurrentCall();
        }

        private void buttons_CameraClick(object sender)
        {
            ((InCallModel)ViewModel).ToggleCameras();
        }

        private void buttons_StatsClick(object sender, bool areStatsVisible)
        {
            buttons.stats.IsChecked = areStatsVisible;
            buttons_landscape.stats.IsChecked = areStatsVisible;
            buttons.statsPanel.Visibility = areStatsVisible ? Visibility.Visible : Visibility.Collapsed;
            buttons_landscape.statsPanel.Visibility = areStatsVisible ? Visibility.Visible : Visibility.Collapsed;
            statsVisible = areStatsVisible;
        }

        private void buttons_HangUpClick(object sender)
        {
            oneSecondTimer.Stop();
            LinphoneManager.Instance.EndCurrentCall();
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
            this.ViewModel.CallUpdatedByRemoteListener = this;
            LinphoneManager.Instance.CallStateChanged += CallStateChanged;
            AudioRoutingManager.GetDefault().AudioEndpointChanged += AudioEndpointChanged;

            if (NavigationContext.QueryString.ContainsKey("sip"))
            {
                String calledNumber = NavigationContext.QueryString["sip"];
                LinphoneAddress address = LinphoneManager.Instance.LinphoneCore.InterpretURL(calledNumber);
                calledNumber = String.Format("{0}@{1}", address.UserName, address.Domain);
                // While we dunno if the number matches a contact one, we consider it won't and we display the phone number as username
                Contact.Text = calledNumber;

                if (calledNumber != null && calledNumber.Length > 0)
                {
                    ContactManager cm = ContactManager.Instance;
                    cm.ContactFound += cm_ContactFound;
                    cm.FindContact(calledNumber);
                }
            }

            ApplicationSettingsManager settings = new ApplicationSettingsManager();
            settings.Load();

            // Callback CallStateChanged set too late when call is incoming, so trigger it manually
            if (LinphoneManager.Instance.LinphoneCore.CallsNb > 0)
            {
                LinphoneCall call = (LinphoneCall)LinphoneManager.Instance.LinphoneCore.Calls[0];
                if (call.State == LinphoneCallState.StreamsRunning)
                {
                    CallStateChanged(call, LinphoneCallState.StreamsRunning);
                    if (settings.VideoActiveWhenGoingToBackground)
                    {
                        LinphoneManager.Instance.LinphoneCore.VideoPolicy.AutomaticallyAccept = settings.VideoAutoAcceptWhenGoingToBackground;
                        LinphoneCallParams callParams = call.GetCurrentParamsCopy();
                        callParams.VideoEnabled = true;
                        LinphoneManager.Instance.LinphoneCore.UpdateCall(call, callParams);
                    }
                }
                else if (call.State == LinphoneCallState.UpdatedByRemote)
                {
                    // The call was updated by the remote party while we were in background
                    LinphoneManager.Instance.CallState(call, call.State, "call updated while in background");
                }
            }

            settings.VideoActiveWhenGoingToBackground = false;
            settings.Save();

            oneSecondTimer = new DispatcherTimer();
            oneSecondTimer.Interval = TimeSpan.FromSeconds(1);
            oneSecondTimer.Tick += timerTick;
            oneSecondTimer.Start();
        }

        private const string bluetoothOn = "/Assets/Incall_Icons/bluetooth_on.png";
        private const string bluetoothOff = "/Assets/Incall_Icons/bluetooth_off.png";

        private void AudioEndpointChanged(AudioRoutingManager sender, object args)
        {
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                bool isBluetoothAudioRouteAvailable = LinphoneManager.Instance.IsBluetoothAvailable;
                buttons.bluetooth.IsEnabled = isBluetoothAudioRouteAvailable;
                buttons_landscape.bluetooth.IsEnabled = isBluetoothAudioRouteAvailable;
                buttons.bluetoothImg.Opacity = isBluetoothAudioRouteAvailable ? BUTTON_ENABLED_OPACITY : BUTTON_DISABLED_OPACITY;
                buttons_landscape.bluetoothImg.Opacity = isBluetoothAudioRouteAvailable ? BUTTON_ENABLED_OPACITY : BUTTON_DISABLED_OPACITY;

                bool isUsingBluetoothAudioRoute = LinphoneManager.Instance.BluetoothEnabled;
                buttons.bluetooth.IsChecked = isUsingBluetoothAudioRoute;
                buttons_landscape.bluetooth.IsChecked = isUsingBluetoothAudioRoute;
                buttons.bluetoothImg.Source = new BitmapImage(new Uri(isUsingBluetoothAudioRoute ? bluetoothOn : bluetoothOff, UriKind.RelativeOrAbsolute));
                buttons_landscape.bluetoothImg.Source = new BitmapImage(new Uri(isUsingBluetoothAudioRoute ? bluetoothOn : bluetoothOff, UriKind.RelativeOrAbsolute));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="call"></param>
        /// <param name="state"></param>
        public void CallStateChanged(LinphoneCall call, LinphoneCallState state)
        {
            Logger.Dbg("[InCall] Call state changed: " + state.ToString());
            if (state == LinphoneCallState.StreamsRunning || state == LinphoneCallState.Connected)
            {
                buttons.pause.IsEnabled = true;
                buttons.microphone.IsEnabled = true;
                buttons_landscape.pause.IsEnabled = true;
                buttons_landscape.microphone.IsEnabled = true;
                buttons.pauseImg.Opacity = BUTTON_ENABLED_OPACITY;
                buttons.microImg.Opacity = BUTTON_ENABLED_OPACITY;
                buttons_landscape.pauseImg.Opacity = BUTTON_ENABLED_OPACITY;
                buttons_landscape.microImg.Opacity = BUTTON_ENABLED_OPACITY;

                bool isVideoAvailable = LinphoneManager.Instance.IsVideoAvailable;
                buttons.video.IsEnabled = isVideoAvailable;
                buttons_landscape.video.IsEnabled = isVideoAvailable;
                buttons.videoImg.Opacity = isVideoAvailable ? BUTTON_ENABLED_OPACITY : BUTTON_DISABLED_OPACITY;
                buttons_landscape.videoImg.Opacity = isVideoAvailable ? BUTTON_ENABLED_OPACITY : BUTTON_DISABLED_OPACITY;
                buttons.camera.IsEnabled = isVideoAvailable;
                buttons_landscape.camera.IsEnabled = isVideoAvailable;
                buttons.cameraImg.Opacity = isVideoAvailable ? BUTTON_ENABLED_OPACITY : BUTTON_DISABLED_OPACITY;
                buttons_landscape.cameraImg.Opacity = isVideoAvailable ? BUTTON_ENABLED_OPACITY : BUTTON_DISABLED_OPACITY;

                bool isVideoEnabled = call.GetCurrentParamsCopy().VideoEnabled;
                buttons.video.IsChecked = isVideoEnabled;
                buttons_landscape.video.IsChecked = isVideoEnabled;
            }
            else if (state == LinphoneCallState.PausedByRemote)
            {
                buttons.pause.IsEnabled = false;
                buttons.microphone.IsEnabled = false;
                buttons.video.IsEnabled = false;
                buttons.camera.IsEnabled = false;
                buttons.pauseImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons.microImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons.videoImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons.cameraImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons_landscape.pause.IsEnabled = false;
                buttons_landscape.microphone.IsEnabled = false;
                buttons_landscape.video.IsEnabled = false;
                buttons_landscape.camera.IsEnabled = false;
                buttons_landscape.pauseImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons_landscape.microImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons_landscape.videoImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons_landscape.cameraImg.Opacity = BUTTON_DISABLED_OPACITY;
            }
            else if (state == LinphoneCallState.Paused)
            {
                buttons.microphone.IsEnabled = false;
                buttons.video.IsEnabled = false;
                buttons.camera.IsEnabled = false;
                buttons.microImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons.videoImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons.cameraImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons_landscape.microphone.IsEnabled = false;
                buttons_landscape.video.IsEnabled = false;
                buttons_landscape.camera.IsEnabled = false;
                buttons_landscape.microImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons_landscape.videoImg.Opacity = BUTTON_DISABLED_OPACITY;
                buttons_landscape.cameraImg.Opacity = BUTTON_DISABLED_OPACITY;
            }
            else if (state == LinphoneCallState.Error || state == LinphoneCallState.CallEnd)
            {
                oneSecondTimer.Stop();
            }

            AudioEndpointChanged(null, null);
        }

        /// <summary>
        /// Method called when the page is leaved.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            if (((InCallModel)ViewModel).IsVideoActive)
            {
                LinphoneCall call = null;
                try
                {
                    call = (LinphoneCall)LinphoneManager.Instance.LinphoneCore.Calls[0];
                }
                catch (System.ArgumentOutOfRangeException) { }
                if (call != null)
                {
                    ApplicationSettingsManager settings = new ApplicationSettingsManager();
                    settings.Load();
                    settings.VideoActiveWhenGoingToBackground = true;
                    settings.VideoAutoAcceptWhenGoingToBackground = LinphoneManager.Instance.LinphoneCore.VideoPolicy.AutomaticallyAccept;
                    settings.Save();
                    LinphoneManager.Instance.LinphoneCore.VideoPolicy.AutomaticallyAccept = false;
                    LinphoneCallParams callParams = call.GetCurrentParamsCopy();
                    callParams.VideoEnabled = false;
                    LinphoneManager.Instance.LinphoneCore.UpdateCall(call, callParams);
                }
            }

            oneSecondTimer.Stop();
            if (fadeTimer != null)
            {
                fadeTimer.Dispose();
                fadeTimer = null;
            }

            base.OnNavigatedFrom(nee);
            this.ViewModel.MuteListener = null;
            this.ViewModel.PauseListener = null;
            LinphoneManager.Instance.CallStateChanged -= CallStateChanged;
            AudioRoutingManager.GetDefault().AudioEndpointChanged -= AudioEndpointChanged;
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

        private const string micOn = "/Assets/Incall_Icons/micro_on.png";
        private const string micOff = "/Assets/Incall_Icons/micro_off.png";

        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        public void MuteStateChanged(Boolean isMicMuted)
        {
            buttons.microphone.IsChecked = isMicMuted;
            buttons_landscape.microphone.IsChecked = isMicMuted;
            buttons.microImg.Source = new BitmapImage(new Uri(isMicMuted ? micOff : micOn, UriKind.RelativeOrAbsolute));
            buttons_landscape.microImg.Source = new BitmapImage(new Uri(isMicMuted ? micOff : micOn, UriKind.RelativeOrAbsolute));
        }

        private const string pauseOn = "/Assets/Incall_Icons/pause.png";
        private const string pauseOff = "/Assets/Incall_Icons/play.png";
        private const string videoOn = "/Assets/Incall_Icons/video_on.png";
        private const string videoOff = "/Assets/Incall_Icons/video_off.png";

        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        public void PauseStateChanged(LinphoneCall call, bool isCallPaused, bool isCallPausedByRemote)
        {
            buttons.pause.IsChecked = isCallPaused || isCallPausedByRemote;
            buttons_landscape.pause.IsChecked = isCallPaused || isCallPausedByRemote;
            buttons.pauseImg.Source = new BitmapImage(new Uri(isCallPaused || isCallPausedByRemote ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));
            buttons_landscape.pauseImg.Source = new BitmapImage(new Uri(isCallPaused || isCallPausedByRemote ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));

            if (!isCallPaused && !isCallPausedByRemote)
            {
                if (call.GetCurrentParamsCopy().VideoEnabled && !((InCallModel)ViewModel).IsVideoActive)
                {
                    // Show video if it was not shown yet
                    ((InCallModel)ViewModel).IsVideoActive = true;
                    buttons.video.IsChecked = true;
                    buttons_landscape.video.IsChecked = true;
                    buttons.videoImg.Source = new BitmapImage(new Uri(videoOn, UriKind.RelativeOrAbsolute));
                    buttons_landscape.videoImg.Source = new BitmapImage(new Uri(videoOn, UriKind.RelativeOrAbsolute));
                    ButtonsFadeInVideoAnimation.Begin();
                    StartFadeTimer();
                }
                else if (!call.GetCurrentParamsCopy().VideoEnabled && ((InCallModel)ViewModel).IsVideoActive)
                {
                    // Stop video if it is no longer active
                    ((InCallModel)ViewModel).IsVideoActive = false;
                    buttons.video.IsChecked = false;
                    buttons_landscape.video.IsChecked = false;
                    buttons.videoImg.Source = new BitmapImage(new Uri(videoOff, UriKind.RelativeOrAbsolute));
                    buttons_landscape.videoImg.Source = new BitmapImage(new Uri(videoOff, UriKind.RelativeOrAbsolute));
                    ButtonsFadeInAudioAnimation.Begin();
                    StopFadeTimer();
                }
                if (((InCallModel)ViewModel).IsVideoActive)
                {
                    ButtonsFadeOutAnimation.Begin();
                    ((InCallModel)ViewModel).ShowVideo();
                }
            }
            else
            {
                ((InCallModel)ViewModel).ShowButtonsAndPanel();
                ButtonsFadeInAudioAnimation.Begin();
                StopFadeTimer();
                ((InCallModel)ViewModel).HideVideo();
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
                            parameters.VideoEnabled = true;
                        }
                        LinphoneManager.Instance.LinphoneCore.AcceptCallUpdate(call, parameters);
                    },
                    null);
            }
        }

        private void timerTick(Object sender, EventArgs args)
        {
            LinphoneCall call = ((InCallModel)ViewModel).GetCurrentCall();
            if (call == null) return;

            startTime = (DateTimeOffset)call.CallStartTimeFromContext;
            DateTimeOffset now = DateTimeOffset.Now;
            TimeSpan elapsed = now.Subtract(startTime);
            var hh = elapsed.Hours;
            var ss = elapsed.Seconds;
            var mm = elapsed.Minutes;
            Status.Text = hh.ToString("00") + ":" + mm.ToString("00") + ":" + ss.ToString("00");

            string audioPayloadType = "";
            string audioDownloadBandwidth = "";
            string audioUploadBandwidth = "";
            string videoPayloadType = "";
            string videoDownloadBandwidth = "";
            string videoUploadBandwidth = "";

            LinphoneCallParams param = call.GetCurrentParamsCopy();
            ((InCallModel)ViewModel).MediaEncryption = param.MediaEncryption.ToString();

            LinphoneCallStats audioStats = null;
            try
            {
                audioStats = call.AudioStats;
            }
            catch { }

            if (audioStats != null)
            {
                audioDownloadBandwidth = String.Format("{0:0.00}", audioStats.DownloadBandwidth);
                audioUploadBandwidth = String.Format("{0:0.00}", audioStats.UploadBandwidth);
                ((InCallModel)ViewModel).ICE = audioStats.IceState.ToString(); 
            }

            PayloadType audiopt = param.UsedAudioCodec;
            if (audiopt != null) 
            {
                audioPayloadType = audiopt.MimeType + "/" + audiopt.ClockRate;
            }

            if (param.VideoEnabled)
            {
                LinphoneCallStats videoStats = call.VideoStats;
                if (videoStats != null)
                {
                    videoDownloadBandwidth = String.Format("{0:0.00}", videoStats.DownloadBandwidth);
                    videoUploadBandwidth = String.Format("{0:0.00}", videoStats.UploadBandwidth);
                }

                PayloadType videopt = param.UsedVideoCodec;
                if (videopt != null)
                {
                    videoPayloadType = videopt.MimeType;
                }
                Windows.Foundation.Size receivedVideoSize = param.ReceivedVideoSize;
                String NewReceivedVideoSize = String.Format("{0}x{1}", receivedVideoSize.Width, receivedVideoSize.Height);
                if (((InCallModel)ViewModel).ReceivedVideoSize != NewReceivedVideoSize)
                {
                    ((InCallModel)ViewModel).ReceivedVideoSize = String.Format("{0}x{1}", receivedVideoSize.Width, receivedVideoSize.Height);
                    ((InCallModel)ViewModel).HideVideo();
                    ((InCallModel)ViewModel).ShowVideo();
                }
                Windows.Foundation.Size sentVideoSize = param.SentVideoSize;
                ((InCallModel)ViewModel).SentVideoSize = String.Format("{0}x{1}", sentVideoSize.Width, sentVideoSize.Height);
                ((InCallModel)ViewModel).VideoStatsVisibility = Visibility.Visible;
            }
            else
            {
                ((InCallModel)ViewModel).VideoStatsVisibility = Visibility.Collapsed;
            }

            string downloadBandwidth = audioDownloadBandwidth;
            if ((downloadBandwidth != "") && (videoDownloadBandwidth != "")) downloadBandwidth += " - ";
            if (videoDownloadBandwidth != "") downloadBandwidth += videoDownloadBandwidth;
            ((InCallModel)ViewModel).DownBandwidth = String.Format("{0} kb/s", downloadBandwidth);
            string uploadBandwidth = audioUploadBandwidth;
            if ((uploadBandwidth != "") && (videoUploadBandwidth != "")) uploadBandwidth += " - ";
            if (videoUploadBandwidth != "") uploadBandwidth += videoUploadBandwidth;
            ((InCallModel)ViewModel).UpBandwidth = String.Format("{0} kb/s", uploadBandwidth);
            string payloadType = audioPayloadType;
            if ((payloadType != "") && (videoPayloadType != "")) payloadType += " - ";
            if (videoPayloadType != "") payloadType += videoPayloadType;
            ((InCallModel)ViewModel).PayloadType = payloadType;
        }

        private void remoteVideo_MediaOpened_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Logger.Msg("[InCall] RemoteVideo Opened: " + ((MediaElement)sender).Source.AbsoluteUri);
            ((InCallModel)ViewModel).RemoteVideoOpened();
        }

        private void remoteVideo_MediaFailed_1(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            Logger.Err("[InCall] RemoteVideo Failed: " + e.ErrorException.Message);
        }

        private void localVideo_MediaOpened_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Logger.Msg("[InCall] LocalVideo Opened: " + ((MediaElement)sender).Source.AbsoluteUri);
            ((InCallModel)ViewModel).LocalVideoOpened();
        }

        private void localVideo_MediaFailed_1(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            Logger.Err("[InCall] LocalVideo Failed: " + e.ErrorException.Message);
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
            ((InCallModel)ViewModel).HideButtonsAndPanel();
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
            if (!statsVisible)
            {
                fadeTimer = new Timer(new TimerCallback(HideButtons), null, 4000, Timeout.Infinite);
            }
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
            ((InCallModel)ViewModel).ShowButtonsAndPanel();
            Status.Visibility = Visibility.Visible;
            Contact.Visibility = Visibility.Visible;
            Number.Visibility = Visibility.Visible;
            if (((InCallModel)ViewModel).VideoShown)
            {
                ButtonsFadeInVideoAnimation.Begin();
                StartFadeTimer();
            }
            else
            {
                ButtonsFadeInAudioAnimation.Begin();
            }
        }

        new private void OrientationChanged(object sender, Microsoft.Phone.Controls.OrientationChangedEventArgs e)
        {
            InCallModel model = (InCallModel)ViewModel;
            remoteVideo.Width = LayoutRoot.ActualWidth;
            remoteVideo.Height = LayoutRoot.ActualHeight;
            HUD.Width = LayoutRoot.ActualWidth;
            HUD.Height = LayoutRoot.ActualHeight;
            model.OrientationChanged(sender, e);
        }
    }
}