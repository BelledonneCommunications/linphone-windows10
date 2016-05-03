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

using BelledonneCommunications.Linphone.Native;
using Linphone.Model;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Phone.Media.Devices;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{
    public partial class InCall : Page {
        private DispatcherTimer oneSecondTimer;
        private Timer fadeTimer;
        private DateTimeOffset startTime;

        private bool statsVisible = false;

        private ApplicationViewOrientation displayOrientation;
        private DisplayInformation displayInformation;
        private SimpleOrientationSensor orientationSensor;
        private SimpleOrientation deviceOrientation;

        public InCall()
        {
            this.InitializeComponent();
            this.DataContext = new InCallModel();

            if (LinphoneManager.Instance.Core.IsVideoSupported)
            {
                StartVideoStream();
                VideoGrid.Visibility = Visibility.Collapsed;
            }

            Call call = LinphoneManager.Instance.Core.CurrentCall;
            LinphoneManager.Instance.Changed += Instance_Changed;

            Debug.WriteLine("Device orientation :" + LinphoneManager.Instance.Core.DeviceRotation);

            displayOrientation = ApplicationView.GetForCurrentView().Orientation;
            displayInformation = DisplayInformation.GetForCurrentView();
            deviceOrientation = SimpleOrientation.NotRotated;
            orientationSensor = SimpleOrientationSensor.GetDefault();
            if (orientationSensor != null)
            {
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
        }

        private async void OrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            // Keep previous orientation when the user puts its device faceup or facedown
            Debug.WriteLine(args.Orientation.ToString());
            if ((args.Orientation != SimpleOrientation.Faceup) && (args.Orientation != SimpleOrientation.Facedown))
            {
                deviceOrientation = args.Orientation;
                Debug.WriteLine(deviceOrientation.ToString());
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => SetVideoOrientation());
            }
        }

        private void AdaptVideoSize()
        {
            if (ActualWidth > 640)
            {
                VideoGrid.Width = 640;
            }
            else
            {
                VideoGrid.Width = ActualWidth;
            }
            VideoGrid.Height = VideoGrid.Width * 3 / 4;
            PreviewSwapChainPanel.Width = VideoGrid.Width / 4;
            PreviewSwapChainPanel.Height = VideoGrid.Height / 4;
        }


        private void Instance_Changed(object sender, EventArgs e)
        {
            Call _call = (e as LinphoneManager.CallEventArgs).call;
            //Debug.WriteLine("Call STATE: " + _call.State);
            if(_call.State == CallState.Connected || oneSecondTimer == null)
            {
                oneSecondTimer = new DispatcherTimer();
                oneSecondTimer.Interval = TimeSpan.FromSeconds(1);
                oneSecondTimer.Tick += timerTick;
                oneSecondTimer.Start();
            }
            if (_call.State == CallState.StreamsRunning)
            {

                if (!_call.MediaInProgress)
                {
                    buttons.enabledPause(true);
                    buttons.enabledVideo(true);
                }
                if (_call.CurrentParams.IsVideoEnabled) {
                    displayVideo(true);
                } else
                {
                    displayVideo(false);
                }
            }
            if(_call.State == CallState.UpdatedByRemote)
            {
                if (_call.RemoteParams.IsVideoEnabled)
                {
                    
                }
            }
        }

        private async void SetVideoOrientation()
        {
            SimpleOrientation orientation = deviceOrientation;
            if (displayInformation.NativeOrientation == DisplayOrientations.Portrait)
            {
                switch (orientation)
                {
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
            switch (orientation)
            {
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
            if (currentDegrees != degrees)
            {
                Debug.WriteLine(degrees);
                LinphoneManager.Instance.Core.DeviceRotation = degrees;
            }
        }

        private void Video_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if(buttons.Visibility == Visibility.Visible)
            {
                buttons.Visibility = Visibility.Collapsed;
            } else
            {
                buttons.Visibility = Visibility.Visible;
            }
        }

        private void displayVideo(bool isVisible)
        {
            if(isVisible)
            {
                buttons.Visibility = Visibility.Collapsed;
                VideoGrid.Visibility = Visibility.Visible;
                ContactHeader.Visibility = Visibility.Collapsed;
            } else
            {
                buttons.Visibility = Visibility.Visible;
                VideoGrid.Visibility = Visibility.Collapsed;
                ContactHeader.Visibility = Visibility.Visible;
            }
        }


        private MSWinRTVideo.SwapChainPanelSource _videoSource;
        private MSWinRTVideo.SwapChainPanelSource _previewSource;

        private void StartVideoStream()
        {
            try
            {
                _videoSource = new MSWinRTVideo.SwapChainPanelSource();
                _videoSource.Start(VideoSwapChainPanel);
                _previewSource = new MSWinRTVideo.SwapChainPanelSource();
                _previewSource.Start(PreviewSwapChainPanel);

                LinphoneManager.Instance.Core.NativeVideoWindowId = VideoSwapChainPanel.Name;         
                LinphoneManager.Instance.Core.NativePreviewWindowId = PreviewSwapChainPanel.Name;
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("StartVideoStream: Exception {0}", e.Message));
            }
        }

        private async void StopVideoStream()
        {
            try
            {
                if (_videoSource != null)
                {
                    _videoSource.Stop();
                    _videoSource = null;
                }
                if (_previewSource != null)
                {
                    _previewSource.Stop();
                    _previewSource = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("StartVideoStream: Exception {0}", e.Message));
            }

        }

        //private MSWinRTVideo.SwapChainPanelSource _videoSource;
        private void buttons_VideoClick(object sender, bool isVideoOn)
        {
            Call call = LinphoneManager.Instance.Core.CurrentCall;
            CallParams param = call.CurrentParams.Copy();
            param.IsVideoEnabled = isVideoOn;
            LinphoneManager.Instance.Core.UpdateCall(call, param);
        }

        private void buttons_MuteClick(object sender, bool isMuteOn)
        {
            LinphoneManager.Instance.Core.IsMicEnabled=isMuteOn;
        }

        private void buttons_BluetoothClick(object sender, bool isBluetoothOn)
        {
            /* buttons.speaker.IsChecked = false;
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
             }*/
        }

        private bool buttons_SpeakerClick(object sender, bool isSpeakerOn)
        {


            /* buttons.bluetooth.IsChecked = false;
             buttons_landscape.bluetooth.IsChecked = false;
             buttons.speaker.IsChecked = isSpeakerOn;
             buttons_landscape.speaker.IsChecked = isSpeakerOn;*/
             try
             {
                 LinphoneManager.Instance.SpeakerEnabled = isSpeakerOn;
                 return true;
             }
             catch
             {
                 Debug.WriteLine("Exception while trying to toggle speaker to {0}" + isSpeakerOn.ToString());
                // buttons.speaker.IsChecked = isSpeakerOn;
                // buttons_landscape.speaker.IsChecked = isSpeakerOn;
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
            LinphoneManager.Instance.ToggleCameras();
        }

        private void buttons_StatsClick(object sender, bool areStatsVisible)
        {
           /* buttons.stats.IsChecked = areStatsVisible;
            buttons_landscape.stats.IsChecked = areStatsVisible;
            buttons.statsPanel.Visibility = areStatsVisible ? Visibility.Visible : Visibility.Collapsed;
            buttons_landscape.statsPanel.Visibility = areStatsVisible ? Visibility.Visible : Visibility.Collapsed;*/
            statsVisible = areStatsVisible;
        }

        private void buttons_HangUpClick(object sender)
        {
            oneSecondTimer.Stop();
            StopVideoStream();
            LinphoneManager.Instance.EndCurrentCall();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Searches for a matching contact using the current call address or number and display information if found.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);

            AudioRoutingManager.GetDefault().AudioEndpointChanged += AudioEndpointChanged;

            if ((nee.Parameter as String).Contains("sip"))
            {
                String calledNumber = nee.Parameter as String;
                Address address = LinphoneManager.Instance.Core.InterpretURL(calledNumber);
                calledNumber = String.Format("{0}@{1}", address.UserName, address.Domain);
                // While we dunno if the number matches a contact one, we consider it won't and we display the phone number as username
                Contact.Text = calledNumber;

                if (calledNumber != null && calledNumber.Length > 0)
                {
                  //  ContactManager cm = ContactManager.Instance;
                   // cm.ContactFound += cm_ContactFound;
                   // cm.FindContact(calledNumber);
                }
            }

            ApplicationSettingsManager settings = new ApplicationSettingsManager();
            settings.Load();

            // Callback CallStateChanged set too late when call is incoming, so trigger it manually
            if (LinphoneManager.Instance.Core.CallsNb > 0)
            {
                Call call = (Call)LinphoneManager.Instance.Core.Calls[0];
                if (call.State == CallState.StreamsRunning)
                {
                    /*CallStateChanged(call, CallState.StreamsRunning);
                    if (settings.VideoActiveWhenGoingToBackground)
                    {
                        LinphoneManager.Instance.Core.VideoPolicy.AutomaticallyAccept = settings.VideoAutoAcceptWhenGoingToBackground;
                        CallParams callParams = call.CurrentParams;
                        callParams.IsVideoEnabled = true;
                        LinphoneManager.Instance.Core.UpdateCall(call, callParams);
                    }*/
                }
                else if (call.State == CallState.UpdatedByRemote)
                {
                    // The call was updated by the remote party while we were in background
                    //LinphoneManager.Instance.CallState(call, call.State, "call updated while in background");
                }
            }

            //settings.VideoActiveWhenGoingToBackground = false;
            //settings.Save();
        }

        //private const string bluetoothOn = "/Assets/Incall_Icons/bluetooth_on.png";
        //private const string bluetoothOff = "/Assets/Incall_Icons/bluetooth_off.png";

        private async void AudioEndpointChanged(AudioRoutingManager sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                /* bool isBluetoothAudioRouteAvailable = LinphoneManager.Instance.IsBluetoothAvailable;
                 buttons.bluetooth.IsEnabled = isBluetoothAudioRouteAvailable;
                 buttons_landscape.bluetooth.IsEnabled = isBluetoothAudioRouteAvailable;
                 buttons.bluetoothImg.Opacity = isBluetoothAudioRouteAvailable ? BUTTON_ENABLED_OPACITY : BUTTON_DISABLED_OPACITY;
                 buttons_landscape.bluetoothImg.Opacity = isBluetoothAudioRouteAvailable ? BUTTON_ENABLED_OPACITY : BUTTON_DISABLED_OPACITY;

                 bool isUsingBluetoothAudioRoute = LinphoneManager.Instance.BluetoothEnabled;
                 buttons.bluetooth.IsChecked = isUsingBluetoothAudioRoute;
                 buttons_landscape.bluetooth.IsChecked = isUsingBluetoothAudioRoute;
                 buttons.bluetoothImg.Source = new BitmapImage(new Uri(isUsingBluetoothAudioRoute ? bluetoothOn : bluetoothOff, UriKind.RelativeOrAbsolute));
                 buttons_landscape.bluetoothImg.Source = new BitmapImage(new Uri(isUsingBluetoothAudioRoute ? bluetoothOn : bluetoothOff, UriKind.RelativeOrAbsolute));*/
             });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="call"></param>
        /// <param name="state"></param>
        public void CallStateChanged(Call call, CallState state)
        {
            Debug.WriteLine("[InCall] Call state changed: " + state.ToString());
            if (state == CallState.StreamsRunning || state == CallState.Connected)
            {
                /*buttons.pause.IsEnabled = true;
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
                buttons_landscape.video.IsChecked = isVideoEnabled;*/
            }
            else if (state == CallState.PausedByRemote)
            {
                /*buttons.pause.IsEnabled = false;
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
                buttons_landscape.cameraImg.Opacity = BUTTON_DISABLED_OPACITY;*/
            }
            else if (state == CallState.Paused)
            {
                /*buttons.microphone.IsEnabled = false;
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
                buttons_landscape.cameraImg.Opacity = BUTTON_DISABLED_OPACITY;*/
            }
            else if (state == CallState.Error || state == CallState.End)
            {
                //oneSecondTimer.Stop();
            }

            //AudioEndpointChanged(null, null);
        }

        /// <summary>
        /// Method called when the page is leaved.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            if (((InCallModel)this.DataContext).IsVideoActive)
            {
                Call call = null;
                try
                {
                    call = (Call)LinphoneManager.Instance.Core.Calls[0];
                }
                catch (System.ArgumentOutOfRangeException) { }
                if (call != null)
                {
                    ApplicationSettingsManager settings = new ApplicationSettingsManager();
                    settings.Load();
                    settings.VideoActiveWhenGoingToBackground = true;
                    settings.VideoAutoAcceptWhenGoingToBackground = LinphoneManager.Instance.Core.VideoPolicy.AutomaticallyAccept;
                    settings.Save();
                    LinphoneManager.Instance.Core.VideoPolicy.AutomaticallyAccept = false;
                    CallParams callParams = call.CurrentParams;
                    callParams.IsVideoEnabled = false;
                    LinphoneManager.Instance.Core.UpdateCall(call, callParams);
                }
            }

            StopVideoStream();

            Frame.BackStack.Clear();
            oneSecondTimer.Stop();
            /*if (fadeTimer != null)
            {
                fadeTimer.Dispose();
                fadeTimer = null;
            }*/

            base.OnNavigatedFrom(nee);
            ((InCallModel)this.DataContext).MuteListener = null;
            ((InCallModel)this.DataContext).PauseListener = null;
            //LinphoneManager.Instance.CallStateChanged -= CallStateChanged;
            AudioRoutingManager.GetDefault().AudioEndpointChanged -= AudioEndpointChanged;
        }

        /// <summary>
        /// Callback called when the search on a phone number for a contact has a match
        /// </summary>
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

        private const string micOn = "/Assets/Incall_Icons/micro_on.png";
        private const string micOff = "/Assets/Incall_Icons/micro_off.png";

        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        public void MuteStateChanged(Boolean isMicMuted)
        {
          /*  buttons.microphone.IsChecked = isMicMuted;
            buttons_landscape.microphone.IsChecked = isMicMuted;
            buttons.microImg.Source = new BitmapImage(new Uri(isMicMuted ? micOff : micOn, UriKind.RelativeOrAbsolute));
            buttons_landscape.microImg.Source = new BitmapImage(new Uri(isMicMuted ? micOff : micOn, UriKind.RelativeOrAbsolute));*/
        }

        private const string pauseOn = "/Assets/Incall_Icons/pause.png";
        private const string pauseOff = "/Assets/Incall_Icons/play.png";
        private const string videoOn = "/Assets/Incall_Icons/video_on.png";
        private const string videoOff = "/Assets/Incall_Icons/video_off.png";

        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        public void PauseStateChanged(Call call, bool isCallPaused, bool isCallPausedByRemote)
        {
            //Debug.WriteLine("Pause state changed");
          /*  buttons.pause.IsChecked = isCallPaused || isCallPausedByRemote;
            buttons_landscape.pause.IsChecked = isCallPaused || isCallPausedByRemote;
            buttons.pauseImg.Source = new BitmapImage(new Uri(isCallPaused || isCallPausedByRemote ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));
            buttons_landscape.pauseImg.Source = new BitmapImage(new Uri(isCallPaused || isCallPausedByRemote ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));*/

            if (!isCallPaused && !isCallPausedByRemote)
            {
                if (call.CurrentParams.IsVideoEnabled && !((InCallModel)this.DataContext).IsVideoActive)
                {
                    // Show video if it was not shown yet
                    ((InCallModel)this.DataContext).IsVideoActive = true;
                    /*buttons.video.IsChecked = true;
                    buttons_landscape.video.IsChecked = true;
                    buttons.videoImg.Source = new BitmapImage(new Uri(videoOn, UriKind.RelativeOrAbsolute));
                    buttons_landscape.videoImg.Source = new BitmapImage(new Uri(videoOn, UriKind.RelativeOrAbsolute));*/
                }
                else if (!call.CurrentParams.IsVideoEnabled && ((InCallModel)this.DataContext).IsVideoActive)
                {
                    // Stop video if it is no longer active
                    ((InCallModel)this.DataContext).IsVideoActive = false;
                    /*buttons.video.IsChecked = false;
                    buttons_landscape.video.IsChecked = false;
                    buttons.videoImg.Source = new BitmapImage(new Uri(videoOff, UriKind.RelativeOrAbsolute));
                    buttons_landscape.videoImg.Source = new BitmapImage(new Uri(videoOff, UriKind.RelativeOrAbsolute));*/
                    //ButtonsFadeInAudioAnimation.Begin();
                    //StopFadeTimer();
                }
                if (((InCallModel)this.DataContext).IsVideoActive)
                {
                    //ButtonsFadeOutAnimation.Begin();
                }
            }
            else
            {
                ((InCallModel)this.DataContext).IsVideoActive = false;
                ((InCallModel)this.DataContext).ShowButtonsAndPanel();
                //ButtonsFadeInAudioAnimation.Begin();
                //StopFadeTimer();
            }
        }

        /// <summary>
        /// Called when the call is updated by the remote party.
        /// </summary>
        /// <param name="call">The call that has been updated</param>
        /// <param name="isVideoAdded">A boolean telling whether the remote party added video</param>
        public void CallUpdatedByRemote(Call call, bool isVideoAdded)
        {
            if (isVideoAdded)
            {
                /*Guide.BeginShowMessageBox(AppResources.VideoActivationPopupCaption,
                    AppResources.VideoActivationPopupContent,
                    new List<String> { "Accept", "Dismiss" },
                    0,
                    MessageBoxIcon.Alert,
                    asyncResult =>
                    {
                        int? res = Guide.EndShowMessageBox(asyncResult);
                        CallParams parameters = call.GetCurrentParamsCopy();
                        if (res == 0)
                        {
                            parameters.VideoEnabled = true;
                        }
                        LinphoneManager.Instance.Core.AcceptCallUpdate(call, parameters);
                    },
                    null);*/
            }
        }

        private void timerTick(Object sender, Object e)
        {
            Call call = ((InCallModel)this.DataContext).GetCurrentCall();
            if (call == null) return;

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
            try
            {
                audioStats = call.AudioStats;
            }
            catch { }

            if (audioStats != null)
            {
                audioDownloadBandwidth = String.Format("{0:0.00}", audioStats.DownloadBandwidth);
                audioUploadBandwidth = String.Format("{0:0.00}", audioStats.UploadBandwidth);
                ((InCallModel)this.DataContext).ICE = audioStats.IceState.ToString(); 
            }

            PayloadType audiopt = param.UsedAudioCodec;
            if (audiopt != null) 
            {
                audioPayloadType = audiopt.MimeType + "/" + audiopt.ClockRate;
            }

            if (param.IsVideoEnabled)
            {
                CallStats videoStats = call.VideoStats;
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
                VideoSize receivedVideoSize = param.ReceivedVideoSize;
                String NewReceivedVideoSize = String.Format("{0}x{1}", receivedVideoSize.Width, receivedVideoSize.Height);
                String OldReceivedVideoSize = ((InCallModel)this.DataContext).ReceivedVideoSize;
                if (OldReceivedVideoSize != NewReceivedVideoSize)
                {
                    ((InCallModel)this.DataContext).ReceivedVideoSize = String.Format("{0}x{1}", receivedVideoSize.Width, receivedVideoSize.Height);
                    ((InCallModel)this.DataContext).IsVideoActive = false;
                    if (NewReceivedVideoSize != "0x0")
                    {
                        ((InCallModel)this.DataContext).IsVideoActive = true;
                    }
                }
                VideoSize sentVideoSize = param.SentVideoSize;
                ((InCallModel)this.DataContext).SentVideoSize = String.Format("{0}x{1}", sentVideoSize.Width, sentVideoSize.Height);
                ((InCallModel)this.DataContext).VideoStatsVisibility = Visibility.Visible;
            }
            else
            {
                ((InCallModel)this.DataContext).VideoStatsVisibility = Visibility.Collapsed;
            }

            string downloadBandwidth = audioDownloadBandwidth;
            if ((downloadBandwidth != "") && (videoDownloadBandwidth != "")) downloadBandwidth += " - ";
            if (videoDownloadBandwidth != "") downloadBandwidth += videoDownloadBandwidth;
            ((InCallModel)this.DataContext).DownBandwidth = String.Format("{0} kb/s", downloadBandwidth);
            string uploadBandwidth = audioUploadBandwidth;
            if ((uploadBandwidth != "") && (videoUploadBandwidth != "")) uploadBandwidth += " - ";
            if (videoUploadBandwidth != "") uploadBandwidth += videoUploadBandwidth;
            ((InCallModel)this.DataContext).UpBandwidth = String.Format("{0} kb/s", uploadBandwidth);
            string payloadType = audioPayloadType;
            if ((payloadType != "") && (videoPayloadType != "")) payloadType += " - ";
            if (videoPayloadType != "") payloadType += videoPayloadType;
            ((InCallModel)this.DataContext).PayloadType = payloadType;
        }

        private void remoteVideo_MediaOpened_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[InCall] RemoteVideo Opened: " + ((MediaElement)sender).Source.AbsoluteUri);
            ((InCallModel)this.DataContext).RemoteVideoOpened();
        }

        private void remoteVideo_MediaFailed_1(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("[InCall] RemoteVideo Failed: " + e.ErrorMessage);
        }

        /// <summary>
        /// Do not allow user to leave the incall page while call is active
        /// </summary>
        /*protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;
        }*/

       private void ButtonsFadeOutAnimation_Completed(object sender, object e)
        {
            /*((InCallModel)this.DataContext).HideButtonsAndPanel();
            Status.Visibility = Visibility.Collapsed;
            Contact.Visibility = Visibility.Collapsed;
            Number.Visibility = Visibility.Collapsed;*/
        }

        private void HideButtons(Object state)
        {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            Status.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //ButtonsFadeOutAnimation.Begin();
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
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

        private void LayoutRoot_Tap(object sender, RoutedEventArgs e)
        {
            ((InCallModel)this.DataContext).ShowButtonsAndPanel();
            Status.Visibility = Visibility.Visible;
            Contact.Visibility = Visibility.Visible;
            Number.Visibility = Visibility.Visible;
            if (((InCallModel)this.DataContext).VideoShown)
            {
                //ButtonsFadeInVideoAnimation.Begin();
                //StartFadeTimer();
            }
            else
            {
                //ButtonsFadeInAudioAnimation.Begin();
            }
        }

        private void DoubleAnimation_Completed(object sender, object e)
        {

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
    }
}