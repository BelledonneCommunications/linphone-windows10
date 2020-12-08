/*
InCallModel.cs
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
using System.Diagnostics;
using Windows.UI.Xaml;

namespace Linphone.Views {
    /// <summary>
    /// Model for the InCall page that handles the display of the various elements of the page.
    /// </summary>
    public class InCallModel : BaseModel {

        /// <summary>
        /// Public constructor.
        /// </summary>
        public InCallModel()
            : base() {
            if (CurrentPage != null) {
                //PageOrientation = CurrentPage.Orientation;
            }

            bool isVideoAvailable = LinphoneManager.Instance.IsVideoAvailable;
            videoButtonEnabled = isVideoAvailable;
            cameraButtonEnabled = isVideoAvailable && LinphoneManager.Instance.NumberOfCameras >= 2;
        }

        /// <summary>
        /// Called when user rotates the device
        /// </summary>
       /* public void OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            PageOrientation = e.Orientation;
            Logger.Msg("[InCall] OrientationChanged");
            if (IsVideoActive)
            {
                HideVideo();
                ShowVideo();
                Call call = GetCurrentCall();
                if (call != null)
                {
                    call.SendVFURequest();
                }
            }
        }*/

        public Call GetCurrentCall() {
            Call call = LinphoneManager.Instance.Core.CurrentCall;
            if (call == null) {
                if (LinphoneManager.Instance.Core.CallsNb > 0) {
                    call = (Call)LinphoneManager.Instance.Core.Calls.GetEnumerator().Current;
                }
            }
            return call;
        }

        #region Actions

        private void AdjustDisplayAccordingToOrientation() {
            /* if ((PageOrientation & Microsoft.Phone.Controls.PageOrientation.Portrait) == Microsoft.Phone.Controls.PageOrientation.Portrait)
             {
                 PortraitButtonsVisibility = Visibility.Visible;
                 LandscapeButtonsVisibility = Visibility.Collapsed;
             }
             else if ((PageOrientation & Microsoft.Phone.Controls.PageOrientation.Landscape) == Microsoft.Phone.Controls.PageOrientation.Landscape)
             {
                 PortraitButtonsVisibility = Visibility.Collapsed;
                 LandscapeButtonsVisibility = Visibility.Visible;
             }*/
        }

        /// <summary>
        /// Changes the camera used to capture video
        /// </summary>
        public void ToggleCameras() {
            Debug.WriteLine("[InCall] ToggleCameras");
            HideVideo();
            LinphoneManager.Instance.ToggleCameras();
            ShowVideo();
        }

        private void ShowRemoteVideo() {
            int rotation = 0;
            /*  switch (PageOrientation)
              {
                  case Microsoft.Phone.Controls.PageOrientation.PortraitUp:
                      rotation = 0;
                      break;
                  case Microsoft.Phone.Controls.PageOrientation.PortraitDown:
                      rotation = 180;
                      break;
                  case Microsoft.Phone.Controls.PageOrientation.LandscapeLeft:
                      rotation = 270;
                      break;
                  case Microsoft.Phone.Controls.PageOrientation.LandscapeRight:
                      rotation = 90;
                      break;
              }*/
            RemoteVideoRotation = rotation;
            //Object id = LinphoneManager.Instance.Core.NativeVideoWindowIdString;
            //RemoteVideoUri = Mediastreamer2.WP8Video.VideoRenderer.StreamUri(id);

            if (RemoteVideoVisibility == Visibility.Collapsed) {
                //RemoteVideoProgressBarVisibility = Visibility.Visible;
            }
        }

        public void RemoteVideoOpened() {
            RemoteVideoVisibility = Visibility.Visible;
            //RemoteVideoProgressBarVisibility = Visibility.Collapsed;
        }

        private void HideRemoteVideo() {
            RemoteVideoUri = null;
            RemoteVideoVisibility = Visibility.Collapsed;
        }

        private void ShowLocalVideo() {
            String device = LinphoneManager.Instance.Core.VideoDevice;
            //LocalVideoUri = Mediastreamer2.WP8Video.VideoRenderer.CameraUri(device);
        }

        public void LocalVideoOpened() {
            Int32 rotation = LinphoneManager.Instance.Core.CameraSensorRotation;
            /*switch (pageOrientation)
            {
                case Microsoft.Phone.Controls.PageOrientation.LandscapeLeft:
                    rotation += 270;
                    break;
                case Microsoft.Phone.Controls.PageOrientation.LandscapeRight:
                    rotation += 90;
                    break;
            }*/
            LocalVideoRotation = rotation % 360;
            String device = LinphoneManager.Instance.Core.VideoDevice;
            /*Boolean mirrored = Mediastreamer2.WP8Video.VideoRenderer.IsCameraMirrored(device);
            if (mirrored)
            {
                LocalVideoScaleX = -1.0;
            }
            else
            {
                LocalVideoScaleX = 1.0;
            }*/
            LocalVideoVisibility = Visibility.Visible;
        }

        private void HideLocalVideo() {
            LocalVideoUri = null;
            LocalVideoVisibility = Visibility.Collapsed;
        }

        public void ShowVideo() {
            Debug.WriteLine("[InCall] ShowVideo");
            ShowRemoteVideo();
            if (LinphoneManager.Instance.Core.SelfViewEnabled) {
                ShowLocalVideo();
            }
        }

        public void HideVideo() {
            Debug.WriteLine("[InCall] HideVideo");
            HideRemoteVideo();
            HideLocalVideo();
        }

        internal void ShowButtonsAndPanel() {
            AdjustDisplayAccordingToOrientation();
        }

        internal void HideButtonsAndPanel() {
            PortraitButtonsVisibility = Visibility.Collapsed;
            LandscapeButtonsVisibility = Visibility.Collapsed;
        }

        #endregion

        /* public Microsoft.Phone.Controls.PageOrientation PageOrientation
         {
             get
             {
                 return this.pageOrientation;
             }
             set
             {
                 if (this.pageOrientation != value)
                 {
                     this.pageOrientation = value;
                     AdjustDisplayAccordingToOrientation();
                     this.OnPropertyChanged();
                 }
             }
         }*/

        #region Button properties
        public bool VideoButtonVisibility {
            get {
                return this.videoButtonEnabled;
            }
            set {
                if (this.videoButtonEnabled != value) {
                    this.videoButtonEnabled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool CameraButtonEnabled {
            get {
                return this.cameraButtonEnabled;
            }
            set {
                if (this.cameraButtonEnabled != value) {
                    this.cameraButtonEnabled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the portrait buttons.
        /// </summary>
        public Visibility PortraitButtonsVisibility {
            get {
                return this.portraitButtonsVisibility;
            }
            set {
                if (this.portraitButtonsVisibility != value) {
                    this.portraitButtonsVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the landscape buttons.
        /// </summary>
        public Visibility LandscapeButtonsVisibility {
            get {
                return this.landscapeButtonsVisibility;
            }
            set {
                if (this.landscapeButtonsVisibility != value) {
                    this.landscapeButtonsVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Video properties
        /// <summary>
        /// Is the video active (Boolean).
        /// </summary>
        public Boolean IsVideoActive {
            get {
                return this.isVideoActive;
            }
            set {
                if (this.isVideoActive != value) {
                    this.isVideoActive = value;
                    if (this.isVideoActive) {
                        ShowVideo();
                    } else {
                        HideVideo();
                    }
                }
            }
        }

        /// <summary>
        /// Tells whether the video is shown (Boolean).
        /// </summary>
        public Boolean VideoShown {
            get {
                return (this.remoteVideoUri != null);
            }
        }

        /// <summary>
        /// Uri of the remote video stream.
        /// </summary>
        public Uri RemoteVideoUri {
            get {
                return this.remoteVideoUri;
            }
            set {
                if (this.remoteVideoUri != value) {
                    this.remoteVideoUri = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the remote video.
        /// </summary>
        public Visibility RemoteVideoVisibility {
            get {
                return this.remoteVideoVisibility;
            }
            set {
                if (this.remoteVideoVisibility != value) {
                    this.remoteVideoVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Uri of the local video stream.
        /// </summary>
        public Uri LocalVideoUri {
            get {
                return this.localVideoUri;
            }
            set {
                if (this.localVideoUri != value) {
                    this.localVideoUri = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the local video.
        /// </summary>
        public Visibility LocalVideoVisibility {
            get {
                return this.localVideoVisibility;
            }
            set {
                if (this.localVideoVisibility != value) {
                    this.localVideoVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Rotation of the local video.
        /// </summary>
        public Double LocalVideoRotation {
            get {
                return this.localVideoRotation;
            }
            set {
                if (this.localVideoRotation != value) {
                    this.localVideoRotation = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Scale of the X axis of the local video.
        /// </summary>
        public Double LocalVideoScaleX {
            get {
                return this.localVideoScaleX;
            }
            set {
                if (this.localVideoScaleX != value) {
                    this.localVideoScaleX = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int RemoteVideoRotation {
            get {
                return this.remoteVideoRotation;
            }
            set {
                if (this.remoteVideoRotation != value) {
                    this.remoteVideoRotation = value;
                    LinphoneManager.Instance.Core.DeviceRotation = this.remoteVideoRotation;
                    Call call = GetCurrentCall();
                    if (call != null)
                        call.Update(null);
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Stats properties
        public string MediaEncryption {
            get {
                return mediaEncryption;
            }
            set {
                mediaEncryption = value;
                this.OnPropertyChanged();
            }
        }
        public string DownBandwidth {
            get {
                return downBandwidth;
            }
            set {
                downBandwidth = value;
                this.OnPropertyChanged();
            }
        }
        public string PayloadType {
            get {
                return payloadType;
            }
            set {
                payloadType = value;
                this.OnPropertyChanged();
            }
        }
        public string UpBandwidth {
            get {
                return upBandwidth;
            }
            set {
                upBandwidth = value;
                this.OnPropertyChanged();
            }
        }
        public string ICE {
            get {
                return ice;
            }
            set {
                ice = value;
                this.OnPropertyChanged();
            }
        }
        public string ReceivedVideoSize {
            get {
                return receivedVideoSize;
            }
            set {
                receivedVideoSize = value;
                this.OnPropertyChanged();
            }
        }
        public string SentVideoSize {
            get {
                return sentVideoSize;
            }
            set {
                sentVideoSize = value;
                this.OnPropertyChanged();
            }
        }
        public Visibility VideoStatsVisibility {
            get {
                return videoStatsVisibility;
            }
            set {
                videoStatsVisibility = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Private variables
        // private Microsoft.Phone.Controls.PageOrientation pageOrientation = Microsoft.Phone.Controls.PageOrientation.Portrait;
        private bool videoButtonEnabled = false;
        private bool cameraButtonEnabled = false;
        private Visibility portraitButtonsVisibility = Visibility.Visible;
        private Visibility landscapeButtonsVisibility = Visibility.Collapsed;
        private Boolean isVideoActive = false;
        private Uri remoteVideoUri = null;
        private Visibility remoteVideoVisibility = Visibility.Collapsed;
        private Visibility remoteVideoProgressBarVisibility = Visibility.Collapsed;
        private int remoteVideoRotation = 0;
        private Uri localVideoUri = null;
        private Visibility localVideoVisibility = Visibility.Collapsed;
        private Double localVideoRotation = 0;
        private Double localVideoScaleX = 1.0;
        private string mediaEncryption = "";
        private string downBandwidth = "";
        private string upBandwidth = "";
        private string payloadType = "";
        private string receivedVideoSize = "";
        private string sentVideoSize = "";
        private string ice = "";
        private Visibility videoStatsVisibility = Visibility.Collapsed;
        #endregion
    }
}
