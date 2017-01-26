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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using BelledonneCommunications.Linphone.Native;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Phone.Media.Devices;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views {
    public partial class InCall : Page {
        private DispatcherTimer oneSecondTimer;
        private Timer fadeTimer;
        private DateTimeOffset startTime;

        private bool statsVisible = false;

        private ApplicationViewOrientation displayOrientation;
        private DisplayInformation displayInformation;
        private SimpleOrientationSensor orientationSensor;
        private SimpleOrientation deviceOrientation;

        public InCall() {
            this.InitializeComponent();
            this.DataContext = new InCallModel();

            if (LinphoneManager.Instance.IsVideoAvailable) {
                StartVideoStream();
                VideoGrid.Visibility = Visibility.Collapsed;
            }

                if (LinphoneManager.Instance.Core.CurrentCall.State == CallState.StreamsRunning)
                Status.Text = "00:00:00";

            displayOrientation = ApplicationView.GetForCurrentView().Orientation;
            displayInformation = DisplayInformation.GetForCurrentView();
            deviceOrientation = SimpleOrientation.NotRotated;
            orientationSensor = SimpleOrientationSensor.GetDefault();
            if (orientationSensor != null) {
                deviceOrientation = orientationSensor.GetCurrentOrientation();
                SetVideoOrientation();
                orientationSensor.OrientationChanged += OrientationSensor_OrientationChanged;
            }

            buttons.HangUpClick += buttons_HangUpClick;
            buttons.StatsClick += buttons_StatsClick;
            buttons.CameraClick += buttons_CameraClick;
            buttons.PauseClick += buttons_PauseClick;
            buttons.SpeakerClick += buttons_SpeakerClick;
            buttons.MuteClick += buttons_MuteClick;
            buttons.VideoClick += buttons_VideoClick;
            buttons.BluetoothClick += buttons_BluetoothClick;
            buttons.DialpadClick += buttons_DialpadClick;
        }

        #region Buttons
        private void buttons_VideoClick(object sender, bool isVideoOn) {
            Call call = LinphoneManager.Instance.Core.CurrentCall;
            CallParams param = call.CurrentParams.Copy();
            param.IsVideoEnabled = isVideoOn;
            LinphoneManager.Instance.Core.UpdateCall(call, param);
        }

        private void buttons_MuteClick(object sender, bool isMuteOn) {
            LinphoneManager.Instance.Core.IsMicEnabled = isMuteOn;
        }

        private void buttons_BluetoothClick(object sender, bool isBluetoothOn) {
            try {
                LinphoneManager.Instance.BluetoothEnabled = isBluetoothOn;
            } catch {
                Debug.WriteLine("Exception while trying to toggle bluetooth to " + isBluetoothOn.ToString());
            }
        }

        private void buttons_DialpadClick(object sender, bool isBluetoothOn) {
        }

        private bool buttons_SpeakerClick(object sender, bool isSpeakerOn) {
            try {
                LinphoneManager.Instance.SpeakerEnabled = isSpeakerOn;
                return true;
            } catch {
                Debug.WriteLine("Exception while trying to toggle speaker to " + isSpeakerOn.ToString());
            }
            return false;
        }

        private void buttons_PauseClick(object sender, bool isPaused) {
            if (isPaused)
                LinphoneManager.Instance.PauseCurrentCall();
            else
                LinphoneManager.Instance.ResumeCurrentCall();
        }

        private void buttons_CameraClick(object sender) {
            LinphoneManager.Instance.ToggleCameras();
            if (LinphoneManager.Instance.Core.VideoDevice.Contains("Front")) {
                PreviewRender.ScaleX = -1;
            } else {
                PreviewRender.ScaleX = 1;
            }

        }

        private void buttons_StatsClick(object sender, bool areStatsVisible) {
            statsVisible = areStatsVisible;
        }

        private void buttons_HangUpClick(object sender) {
            LinphoneManager.Instance.EndCurrentCall();
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs nee) {
            List<string> parameters;
            base.OnNavigatedTo(nee);
            parameters = nee.Parameter as List<string>;

            LinphoneManager.Instance.CallStateChangedEvent += CallStateChanged;

            if (parameters == null)
                return;

            if (parameters.Count >= 1 && parameters[0].Contains("sip")) {
                String calledNumber = parameters[0];
                Address address = LinphoneManager.Instance.Core.InterpretURL(calledNumber);
                calledNumber = String.Format("{0}@{1}", address.UserName, address.Domain);
                Contact.Text = calledNumber;

                if (calledNumber != null && calledNumber.Length > 0) {
                    // ContactManager cm = ContactManager.Instance;
                    // cm.ContactFound += cm_ContactFound;
                    // cm.FindContact(calledNumber);
                }
            }
            if (parameters.Count >= 2 && parameters[1].Contains("incomingCall")) {
                if (LinphoneManager.Instance.Core.CurrentCall != null) {
                    LinphoneManager.Instance.Core.AcceptCall(LinphoneManager.Instance.Core.CurrentCall);
                } else {
                    if (Frame.CanGoBack) {
                        Frame.GoBack();
                    }
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs nee) {
            StopVideoStream();
            if (oneSecondTimer != null) {
                oneSecondTimer.Stop();
            }

            if (LinphoneManager.Instance.isMobileVersion()) {
                ToggleFullScreenMode(false);
            }
            /*if (fadeTimer != null)
            {
                fadeTimer.Dispose();
                fadeTimer = null;
            }*/
            Frame.BackStack.Clear();
            base.OnNavigatedFrom(nee);

            LinphoneManager.Instance.CallStateChangedEvent -= CallStateChanged;
        }

        public void CallStateChanged(Call call, CallState state) {
            if (call == null)
                return;

            if (state == CallState.Connected && oneSecondTimer == null) {
                oneSecondTimer = new DispatcherTimer();
                oneSecondTimer.Interval = TimeSpan.FromSeconds(1);
                oneSecondTimer.Tick += timerTick;
                oneSecondTimer.Start();

                statusIcon.Visibility = Visibility.Visible;
                buttons.enabledVideo(false);
            }
            if (state == CallState.StreamsRunning) {
                statusIcon.Glyph = "\uE768";
                if (!call.MediaInProgress) {
                    buttons.enabledPause(true);
                    if (LinphoneManager.Instance.IsVideoAvailable) {
                        buttons.enabledVideo(true);
                    }
                }
                if (call.CurrentParams.IsVideoEnabled) {
                    displayVideo(true);
                    buttons.checkedVideo(true);
                } else {
                    displayVideo(false);
                }
            } else if (state == CallState.PausedByRemote) {
                if (call.CurrentParams.IsVideoEnabled) {
                    displayVideo(false);
                }
                buttons.enabledVideo(false);
                statusIcon.Glyph = "\uE769";
            } else if (state == CallState.Paused) {
                if (call.CurrentParams.IsVideoEnabled) {
                    displayVideo(false);
                }
                buttons.enabledVideo(false);
                statusIcon.Glyph = "\uE769";
            } else if (state == CallState.Error || state == CallState.End) {
                if (oneSecondTimer != null) {
                    oneSecondTimer.Stop();
                }
            } else if (state == CallState.UpdatedByRemote) {
                if (!LinphoneManager.Instance.IsVideoAvailable) {
                    CallParams parameters = call.CurrentParams.Copy();
                    LinphoneManager.Instance.Core.AcceptCallUpdate(call, parameters);
                } else {
                    bool remoteVideo = call.RemoteParams.IsVideoEnabled;
                    bool localVideo = call.CurrentParams.IsVideoEnabled;
                    bool autoAcceptCameraPolicy = LinphoneManager.Instance.Core.VideoPolicy.AutomaticallyAccept;
                    if (remoteVideo && !localVideo && !autoAcceptCameraPolicy) {
                        AskVideoPopup(call);
                    }
                }
            }
            refreshUI();
        }

        private void refreshUI() {
            if (!LinphoneManager.Instance.IsVideoAvailable) {
                buttons.enabledVideo(false);
            } else {
                if (LinphoneManager.Instance.Core.CurrentCall != null && LinphoneManager.Instance.Core.CurrentCall.CurrentParams.IsVideoEnabled) {
                    buttons.checkedVideo(true);
                } else {
                    buttons.checkedVideo(false);
                }
            }
        }

        public async void AskVideoPopup(Call call) {
            MessageDialog dialog = new MessageDialog(ResourceLoader.GetForCurrentView().GetString("VideoActivationPopupContent"), ResourceLoader.GetForCurrentView().GetString("VideoActivationPopupCaption"));
            dialog.Commands.Clear();
            dialog.Commands.Add(new UICommand { Label = ResourceLoader.GetForCurrentView().GetString("Accept"), Id = 0 });
            dialog.Commands.Add(new UICommand { Label = ResourceLoader.GetForCurrentView().GetString("Dismiss"), Id = 1 });

            var res = await dialog.ShowAsync();
            CallParams parameters = LinphoneManager.Instance.Core.CreateCallParams(call);
            if ((int)res.Id == 0) {
                parameters.IsVideoEnabled = true;
            }
            LinphoneManager.Instance.Core.AcceptCallUpdate(call, parameters);
        }

        #region Video
        private void AdaptVideoSize() {
            if (ActualWidth > 640) {
                VideoGrid.Width = 640;
            } else {
                VideoGrid.Width = ActualWidth;
            }
            VideoGrid.Height = VideoGrid.Width * 3 / 4;
            //PreviewSwapChainPanel.Width = VideoGrid.Width / 4;
            //PreviewSwapChainPanel.Height = VideoGrid.Height / 4;
        }

        private void Video_Tapped(object sender, TappedRoutedEventArgs e) {
            if (buttons.Visibility == Visibility.Visible) {
                buttons.Visibility = Visibility.Collapsed;
            } else {
                buttons.Visibility = Visibility.Visible;
            }
        }

        private void displayVideo(bool isVisible) {
            if (LinphoneManager.Instance.isMobileVersion()) {
                ToggleFullScreenMode(isVisible);
            }
            if (isVisible) {
                if (LinphoneManager.Instance.Core.VideoDevice.Contains("Front")) {
                    PreviewRender.ScaleX = -1;
                } else {
                    PreviewRender.ScaleX = 1;
                }

                buttons.Visibility = Visibility.Collapsed;
                VideoGrid.Visibility = Visibility.Visible;
                ContactHeader.Visibility = Visibility.Collapsed;

            } else {
                buttons.Visibility = Visibility.Visible;
                VideoGrid.Visibility = Visibility.Collapsed;
                ContactHeader.Visibility = Visibility.Visible;
            }
        }

        private MSWinRTVideo.SwapChainPanelSource _videoSource;
        private MSWinRTVideo.SwapChainPanelSource _previewSource;

        private void StartVideoStream() {
            try {
                _videoSource = new MSWinRTVideo.SwapChainPanelSource();
                _videoSource.Start(VideoSwapChainPanel);
                _previewSource = new MSWinRTVideo.SwapChainPanelSource();
                _previewSource.Start(PreviewSwapChainPanel);

                LinphoneManager.Instance.Core.NativeVideoWindowId = VideoSwapChainPanel.Name;
                LinphoneManager.Instance.Core.NativePreviewWindowId = PreviewSwapChainPanel.Name;
            } catch (Exception e) {
                Debug.WriteLine(String.Format("StartVideoStream: Exception {0}", e.Message));
            }
        }

        private void StopVideoStream() {
            try {
                if (_videoSource != null) {
                    _videoSource.Stop();
                    _videoSource = null;
                }
                if (_previewSource != null) {
                    _previewSource.Stop();
                    _previewSource = null;
                }
            } catch (Exception e) {
                Debug.WriteLine(String.Format("StartVideoStream: Exception {0}", e.Message));
            }

        }
        #endregion

        #region Orientation
        private async void OrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args) {
            // Keep previous orientation when the user puts its device faceup or facedown
            if ((args.Orientation != SimpleOrientation.Faceup) && (args.Orientation != SimpleOrientation.Facedown)) {
                deviceOrientation = args.Orientation;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => SetVideoOrientation());
            }
        }

        private void SetVideoOrientation() {
            SimpleOrientation orientation = deviceOrientation;
            if (displayInformation.NativeOrientation == DisplayOrientations.Portrait) {
                switch (orientation) {
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        orientation = SimpleOrientation.NotRotated;
                        break;
                    case SimpleOrientation.Rotated180DegreesCounterclockwise:
                        orientation = SimpleOrientation.Rotated90DegreesCounterclockwise;
                        break;
                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        orientation = SimpleOrientation.Rotated180DegreesCounterclockwise;
                        break;
                    case SimpleOrientation.NotRotated:
                    default:
                        orientation = SimpleOrientation.Rotated270DegreesCounterclockwise;
                        break;
                }
            }
            int degrees = 0;
            switch (orientation) {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    degrees = 90;
                    break;
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    degrees = 180;
                    break;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    degrees = 270;
                    break;
                case SimpleOrientation.NotRotated:
                default:
                    degrees = 0;
                    break;
            }

            int currentDegrees = LinphoneManager.Instance.Core.DeviceRotation;
            if (currentDegrees != degrees) {
                LinphoneManager.Instance.Core.DeviceRotation = degrees;
            }
        }
        #endregion

        /*private void cm_ContactFound(object sender, ContactFoundEventArgs e)
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
        }*/

        private void timerTick(Object sender, Object e) {
            Call call = ((InCallModel)this.DataContext).GetCurrentCall();
            if (call == null)
                return;

            //startTime = (DateTimeOffset)call.CallStartTimeFromContext;


            TimeSpan callDuration = new TimeSpan(call.Duration * TimeSpan.TicksPerSecond);
            var hh = callDuration.Hours;
            var ss = callDuration.Seconds;
            var mm = callDuration.Minutes;
            Status.Text = hh.ToString("00") + ":" + mm.ToString("00") + ":" + ss.ToString("00");

            string audioPayloadType = "";
            string audioDownloadBandwidth = "";
            string audioUploadBandwidth = "";
            string videoPayloadType = "";
            string videoDownloadBandwidth = "";
            string videoUploadBandwidth = "";

            CallParams param = call.CurrentParams;
            ((InCallModel)this.DataContext).MediaEncryption = param.MediaEncryption.ToString();

            CallStats audioStats = null;
            try {
                audioStats = call.AudioStats;
            } catch { }

            if (audioStats != null) {
                audioDownloadBandwidth = String.Format("{0:0.00}", audioStats.DownloadBandwidth);
                audioUploadBandwidth = String.Format("{0:0.00}", audioStats.UploadBandwidth);
                ((InCallModel)this.DataContext).ICE = audioStats.IceState.ToString();
            }

            PayloadType audiopt = param.UsedAudioCodec;
            if (audiopt != null) {
                audioPayloadType = audiopt.MimeType + "/" + audiopt.ClockRate;
            }

            if (param.IsVideoEnabled) {
                CallStats videoStats = call.VideoStats;
                if (videoStats != null) {
                    videoDownloadBandwidth = String.Format("{0:0.00}", videoStats.DownloadBandwidth);
                    videoUploadBandwidth = String.Format("{0:0.00}", videoStats.UploadBandwidth);
                }

                PayloadType videopt = param.UsedVideoCodec;
                if (videopt != null) {
                    videoPayloadType = videopt.MimeType;
                }
                VideoSize receivedVideoSize = param.ReceivedVideoSize;
                String NewReceivedVideoSize = String.Format("{0}x{1}", receivedVideoSize.Width, receivedVideoSize.Height);
                String OldReceivedVideoSize = ((InCallModel)this.DataContext).ReceivedVideoSize;
                if (OldReceivedVideoSize != NewReceivedVideoSize) {
                    ((InCallModel)this.DataContext).ReceivedVideoSize = String.Format("{0}x{1}", receivedVideoSize.Width, receivedVideoSize.Height);
                    ((InCallModel)this.DataContext).IsVideoActive = false;
                    if (NewReceivedVideoSize != "0x0") {
                        ((InCallModel)this.DataContext).IsVideoActive = true;
                    }
                }
                VideoSize sentVideoSize = param.SentVideoSize;
                ((InCallModel)this.DataContext).SentVideoSize = String.Format("{0}x{1}", sentVideoSize.Width, sentVideoSize.Height);
                ((InCallModel)this.DataContext).VideoStatsVisibility = Visibility.Visible;
            } else {
                ((InCallModel)this.DataContext).VideoStatsVisibility = Visibility.Collapsed;
            }

            string downloadBandwidth = audioDownloadBandwidth;
            if ((downloadBandwidth != "") && (videoDownloadBandwidth != ""))
                downloadBandwidth += " - ";
            if (videoDownloadBandwidth != "")
                downloadBandwidth += videoDownloadBandwidth;
            ((InCallModel)this.DataContext).DownBandwidth = String.Format("{0} kb/s", downloadBandwidth);
            string uploadBandwidth = audioUploadBandwidth;
            if ((uploadBandwidth != "") && (videoUploadBandwidth != ""))
                uploadBandwidth += " - ";
            if (videoUploadBandwidth != "")
                uploadBandwidth += videoUploadBandwidth;
            ((InCallModel)this.DataContext).UpBandwidth = String.Format("{0} kb/s", uploadBandwidth);
            string payloadType = audioPayloadType;
            if ((payloadType != "") && (videoPayloadType != ""))
                payloadType += " - ";
            if (videoPayloadType != "")
                payloadType += videoPayloadType;
            ((InCallModel)this.DataContext).PayloadType = payloadType;
        }

        private void remoteVideo_MediaOpened_1(object sender, RoutedEventArgs e) {
            Debug.WriteLine("[InCall] RemoteVideo Opened: " + ((MediaElement)sender).Source.AbsoluteUri);
            ((InCallModel)this.DataContext).RemoteVideoOpened();
        }

        private void remoteVideo_MediaFailed_1(object sender, ExceptionRoutedEventArgs e) {
            Debug.WriteLine("[InCall] RemoteVideo Failed: " + e.ErrorMessage);
        }

        /// <summary>
        /// Do not allow user to leave the incall page while call is active
        /// </summary>
        /*protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;
        }*/

        private void ButtonsFadeOutAnimation_Completed(object sender, object e) {
            /*((InCallModel)this.DataContext).HideButtonsAndPanel();
            Status.Visibility = Visibility.Collapsed;
            Contact.Visibility = Visibility.Collapsed;
            Number.Visibility = Visibility.Collapsed;*/
        }

        private void HideButtons(Object state) {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            Status.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                //ButtonsFadeOutAnimation.Begin();
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        }

        private void StartFadeTimer() {
            if (fadeTimer != null) {
                fadeTimer.Dispose();
            }
            if (!statsVisible) {
                fadeTimer = new Timer(new TimerCallback(HideButtons), null, 4000, Timeout.Infinite);
            }
        }

        private void StopFadeTimer() {
            if (fadeTimer != null) {
                fadeTimer.Dispose();
                fadeTimer = null;
            }
        }

        private void LayoutRoot_Tap(object sender, RoutedEventArgs e) {
            ((InCallModel)this.DataContext).ShowButtonsAndPanel();
            Status.Visibility = Visibility.Visible;
            Contact.Visibility = Visibility.Visible;
            //Number.Visibility = Visibility.Visible;
            if (((InCallModel)this.DataContext).VideoShown) {
                //ButtonsFadeInVideoAnimation.Begin();
                //StartFadeTimer();
            } else {
                //ButtonsFadeInAudioAnimation.Begin();
            }
        }

        private void DoubleAnimation_Completed(object sender, object e) {

        }


        /* new private void OrientationChanged(object sender, OrientationChangedEventArgs e)
         {
             InCallModel model = (InCallModel)ViewModel;
             remoteVideo.Width = LayoutRoot.ActualWidth;
             remoteVideo.Height = LayoutRoot.ActualHeight;
             HUD.Width = LayoutRoot.ActualWidth;
             HUD.Height = LayoutRoot.ActualHeight;
             model.OrientationChanged(sender, e);
         }*/

        private void ToggleFullScreenMode(bool fullScreen) {
            var view = ApplicationView.GetForCurrentView();
            if (!fullScreen) {
                view.ExitFullScreenMode();
            } else {
                if (view.TryEnterFullScreenMode()) {
                    Debug.WriteLine("Entering full screen mode");
                } else {
                    Debug.WriteLine("Failed to enter full screen mode");
                }
            }
        }
    }
}