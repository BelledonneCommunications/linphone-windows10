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

using Linphone;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Media.Capture;
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
        private Boolean askingVideo;
        private Call pausedCall;

        private bool statsVisible = false;

        private ApplicationViewOrientation displayOrientation;
        private DisplayInformation displayInformation;
        private SimpleOrientationSensor orientationSensor;
        private SimpleOrientation deviceOrientation;

        private readonly object popupLock = new Object();


        public InCall() {
            this.InitializeComponent();
            this.DataContext = new InCallModel();
            askingVideo = false;
            //------------------------------------------------------------------------
            Loaded += OnPageLoaded;
            if (LinphoneManager.Instance.IsVideoAvailable) {
                VideoGrid.Visibility = Visibility.Collapsed;
                StartVideoStream();// Initialize to avoid create new windows.
            }
            else
                StopVideoStream();

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

            // Handling event when app will be suspended
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspended);
            Application.Current.Resuming += new EventHandler<object>(App_Resumed);
            pausedCall = null;
        }
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------

#region Buttons
private async void buttons_VideoClick(object sender, bool isVideoOn) {
            // Workaround to pop the camera permission window
            await openCameraPopup();

            Core core = LinphoneManager.Instance.Core;
            if (!core.VideoSupported())
            {
                Debug.WriteLine("Unable to update video call property. (Video not supported.)");
                return;
            }
            Call call = LinphoneManager.Instance.Core.CurrentCall;
            switch (call.State)
            {
                case CallState.Connected:
                case CallState.StreamsRunning:
                    break;
                default: return;
            }
            CallParams p = call.CurrentParams;
            if (isVideoOn == (p!=null && p.VideoEnabled))
                return;
            CallParams param = LinphoneManager.Instance.Core.CreateCallParams(call);
            param.VideoEnabled = isVideoOn;
            param.VideoDirection = MediaDirection.SendRecv;
            call.Update(param);
            if(isVideoOn)
                StartVideoStream();
            else
                StopVideoStream();
        }

        private void buttons_MuteClick(object sender, bool isMuteOn) {
            LinphoneManager.Instance.Core.MicEnabled = isMuteOn;
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
                Address address = LinphoneManager.Instance.Core.InterpretUrl(calledNumber);
                calledNumber = String.Format("{0}@{1}", address.Username, address.Domain);
                Contact.Text = calledNumber;

                if (calledNumber != null && calledNumber.Length > 0) {
                    // ContactManager cm = ContactManager.Instance;
                    // cm.ContactFound += cm_ContactFound;
                    // cm.FindContact(calledNumber);
                }
            }
            if (parameters.Count >= 2 && parameters[1].Contains("incomingCall")) {
                if (LinphoneManager.Instance.Core.CurrentCall != null) {
                    LinphoneManager.Instance.Core.CurrentCall.Accept();
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
                oneSecondTimer.Start();
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

            //LinphoneManager.Instance.CallStateChangedEvent -= CallStateChanged;
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
            } else if (state == CallState.Resuming) {
                oneSecondTimer = new DispatcherTimer();
                oneSecondTimer.Interval = TimeSpan.FromSeconds(1);
                oneSecondTimer.Tick += timerTick;
                oneSecondTimer.Start();
            } else if (state == CallState.StreamsRunning) {
                statusIcon.Glyph = "\uE768";
                if (!call.MediaInProgress()) {
                    buttons.enabledPause(true);
                    if (LinphoneManager.Instance.IsVideoAvailable) {
                        buttons.enabledVideo(true);
                    }
                }
                if (call.CurrentParams.VideoEnabled) {
                    displayVideo(true);
                    buttons.checkedVideo(true);
                } else {
                    displayVideo(false);
                }
            } else if (state == CallState.PausedByRemote) {
                if (call.CurrentParams.VideoEnabled) {
                    displayVideo(false);
                }
                buttons.enabledVideo(false);
                statusIcon.Glyph = "\uE769";
            } else if (state == CallState.Paused) {
                if (call.CurrentParams.VideoEnabled) {
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
                    CallParams parameters = LinphoneManager.Instance.Core.CreateCallParams(call);
                    call.AcceptUpdate(parameters);
                } else {
                    bool remoteVideo = call.RemoteParams.VideoEnabled;
                    bool localVideo = call.CurrentParams.VideoEnabled;
                    bool autoAcceptCameraPolicy = LinphoneManager.Instance.Core.VideoActivationPolicy.AutomaticallyAccept;
                    if (remoteVideo && !localVideo && !autoAcceptCameraPolicy) {
                        //lock (popupLock) {
                            if (askingVideo) return;
                            askingVideo = true;
                            AskVideoPopup(call);
                        //}
                    }else if(!remoteVideo)
                        askingVideo = false;
                }
            }
            refreshUI();
        }

        private void refreshUI() {
            if (!LinphoneManager.Instance.IsVideoAvailable) {
                buttons.enabledVideo(false);
            } else {
                if (LinphoneManager.Instance.Core.CurrentCall != null && LinphoneManager.Instance.Core.CurrentCall.CurrentParams.VideoEnabled) {
                    buttons.checkedVideo(true);
                } else {
                    buttons.checkedVideo(false);
                    if(LinphoneManager.Instance.Core.CurrentCall == null)
                        askingVideo = false;
                }
            }
        }

        private async Task openCameraPopup() {
            MediaCapture mediaCapture = new Windows.Media.Capture.MediaCapture();
            await mediaCapture.InitializeAsync();
            mediaCapture.Dispose();
        }

        public async void AskVideoPopup(Call call) {
            if (call != null)
            {
                CallParams parameters = LinphoneManager.Instance.Core.CreateCallParams(call);
                bool isEnabled = parameters.VideoEnabled;
                MessageDialog dialog = new MessageDialog(ResourceLoader.GetForCurrentView().GetString("VideoActivationPopupContent"), ResourceLoader.GetForCurrentView().GetString("VideoActivationPopupCaption"));
                dialog.Commands.Clear();
                dialog.Commands.Add(new UICommand { Label = ResourceLoader.GetForCurrentView().GetString("Accept"), Id = 0 });
                dialog.Commands.Add(new UICommand { Label = ResourceLoader.GetForCurrentView().GetString("Dismiss"), Id = 1 });

                var res = await dialog.ShowAsync();

                if ((int)res.Id == 0)
                {
                    // Workaround to pop the camera permission window
                    //await openCameraPopup();
                    parameters.VideoEnabled = true;
                }
                call.AcceptUpdate(parameters);
            }
        }

        #region Video
        private async void App_Suspended(Object sender, Windows.ApplicationModel.SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            // Pause the call when the application is about to be in background
            if (LinphoneManager.Instance.Core.CurrentCall != null && LinphoneManager.Instance.Core.CurrentCall.State != CallState.Paused) {
                pausedCall = LinphoneManager.Instance.Core.CurrentCall;
                pausedCall.Pause();

                // Wait for the Call to pass from Pausing to Paused
                await Task.Delay(1000);
            }
            deferral.Complete();
        }

        private void App_Resumed(Object sender, Object e) {
            if (pausedCall != null && pausedCall.State == CallState.Paused) {
                pausedCall.Resume();
                pausedCall = null;
            }
        }

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

        //-----------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------

        // Create a task for rendering that will be run on a background thread. 
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // The SwapChainPanel has been created and arranged in the page layout, so EGL can be initialized. 
            //CreateRenderSurface();
        }
        private void StartVideoStream() {
            LinphoneManager.StartVideoStream(VideoSwapChainPanel, PreviewSwapChainPanel);
        }

        private void StopVideoStream()
        {
            LinphoneManager.StopVideoStream();
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
                audioStats = call.GetStats(StreamType.Audio);
            } catch { }

            if (audioStats != null) {
                audioDownloadBandwidth = String.Format("{0:0.00}", audioStats.DownloadBandwidth);
                audioUploadBandwidth = String.Format("{0:0.00}", audioStats.UploadBandwidth);
                ((InCallModel)this.DataContext).ICE = audioStats.IceState.ToString();
            }

            PayloadType audiopt = param.UsedAudioPayloadType;
            if (audiopt != null) {
                audioPayloadType = audiopt.MimeType + "/" + audiopt.ClockRate;
            }

            if (param.VideoEnabled) {
                CallStats videoStats = call.GetStats(StreamType.Video);
                if (videoStats != null) {
                    videoDownloadBandwidth = String.Format("{0:0.00}", videoStats.DownloadBandwidth);
                    videoUploadBandwidth = String.Format("{0:0.00}", videoStats.UploadBandwidth);
                }

                PayloadType videopt = param.UsedVideoPayloadType;
                if (videopt != null) {
                    videoPayloadType = videopt.MimeType;
                }
                VideoDefinition receivedVideoSize = param.ReceivedVideoDefinition;
                String NewReceivedVideoSize = String.Format("{0}x{1}", receivedVideoSize.Width, receivedVideoSize.Height);
                String OldReceivedVideoSize = ((InCallModel)this.DataContext).ReceivedVideoSize;
                if (OldReceivedVideoSize != NewReceivedVideoSize) {
                    ((InCallModel)this.DataContext).ReceivedVideoSize = String.Format("{0}x{1}", receivedVideoSize.Width, receivedVideoSize.Height);
                    ((InCallModel)this.DataContext).IsVideoActive = false;
                    if (NewReceivedVideoSize != "0x0") {
                        ((InCallModel)this.DataContext).IsVideoActive = true;
                    }
                }
                VideoDefinition sentVideoSize = param.SentVideoDefinition;
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