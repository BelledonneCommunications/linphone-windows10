using Linphone.Agents;
using Linphone.Core;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Linphone.Views
{
    /// <summary>
    /// Model for the InCall page that handles the display of the various elements of the page.
    /// </summary>
    public class InCallModel : BaseModel
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public InCallModel()
            : base()
        {
            if (CurrentPage != null)
            {
                PageOrientation = CurrentPage.Orientation;
            }

            if (LinphoneManager.Instance.isLinphoneRunning)
            {
                bool isVideoAvailable = LinphoneManager.Instance.IsVideoAvailable;
                videoButtonVisibility = isVideoAvailable ? Visibility.Visible : Visibility.Collapsed;
                cameraButtonVisibility = isVideoAvailable && LinphoneManager.Instance.NumberOfCameras >= 2 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called when user rotates the device
        /// </summary>
        public void OrientationChanged(object sender, Microsoft.Phone.Controls.OrientationChangedEventArgs e)
        {
            PageOrientation = e.Orientation;
            if (IsVideoActive)
            {
                HideVideo();
                ShowVideo();
                LinphoneCall call = GetCurrentCall();
                if (call != null)
                {
                    call.SendVFURequest();
                }
            }
        }

        public LinphoneCall GetCurrentCall()
        {
            LinphoneCall call = LinphoneManager.Instance.LinphoneCore.GetCurrentCall();
            if (call == null)
            {
                if (LinphoneManager.Instance.LinphoneCore.GetCallsNb() > 0)
                {
                    call = (LinphoneCall)LinphoneManager.Instance.LinphoneCore.GetCalls()[0];
                }
            }
            return call;
        }

        #region Actions

        private void AdjustDisplayAccordingToOrientation()
        {
            if ((PageOrientation & Microsoft.Phone.Controls.PageOrientation.Portrait) == Microsoft.Phone.Controls.PageOrientation.Portrait)
            {
                PortraitButtonsVisibility = Visibility.Visible;
                LandscapeButtonsVisibility = Visibility.Collapsed;
            }
            else if ((PageOrientation & Microsoft.Phone.Controls.PageOrientation.Landscape) == Microsoft.Phone.Controls.PageOrientation.Landscape)
            {
                PortraitButtonsVisibility = Visibility.Collapsed;
                LandscapeButtonsVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Changes the camera used to capture video
        /// </summary>
        public void ToggleCameras()
        {
            LinphoneManager.Instance.ToggleCameras();
            if (LinphoneManager.Instance.LinphoneCore.IsSelfViewEnabled())
            {
                ShowLocalVideo();
            }
        }

        private void ShowRemoteVideo()
        {
            int rotation = 0;
            switch (PageOrientation)
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
            }
            RemoteVideoRotation = rotation;
            Int32 id = LinphoneManager.Instance.LinphoneCore.GetNativeVideoWindowId();
            RemoteVideoUri = Mediastreamer2.WP8Video.VideoRenderer.StreamUri(id);
            RemoteVideoProgressBarVisibility = Visibility.Visible;
        }

        public void RemoteVideoOpened()
        {
            RemoteVideoVisibility = Visibility.Visible;
            RemoteVideoProgressBarVisibility = Visibility.Collapsed;
        }

        private void HideRemoteVideo()
        {
            RemoteVideoUri = null;
            RemoteVideoVisibility = Visibility.Collapsed;
        }

        private void ShowLocalVideo()
        {
            String device = LinphoneManager.Instance.LinphoneCore.GetVideoDevice();
            LocalVideoUri = Mediastreamer2.WP8Video.VideoRenderer.CameraUri(device);
        }

        public void LocalVideoOpened()
        {
            Int32 rotation = LinphoneManager.Instance.LinphoneCore.GetCameraSensorRotation();
            switch (pageOrientation)
            {
                case Microsoft.Phone.Controls.PageOrientation.LandscapeLeft:
                    rotation += 270;
                    break;
                case Microsoft.Phone.Controls.PageOrientation.LandscapeRight:
                    rotation += 90;
                    break;
            }
            LocalVideoRotation = rotation % 360;
            String device = LinphoneManager.Instance.LinphoneCore.GetVideoDevice();
            Boolean mirrored = Mediastreamer2.WP8Video.VideoRenderer.IsCameraMirrored(device);
            if (mirrored)
            {
                LocalVideoScaleX = -1.0;
            }
            else
            {
                LocalVideoScaleX = 1.0;
            }
            LocalVideoVisibility = Visibility.Visible;
        }

        private void HideLocalVideo()
        {
            LocalVideoUri = null;
            LocalVideoVisibility = Visibility.Collapsed;
        }

        public void ShowVideo()
        {
            Logger.Msg("[InCall] ShowVideo");
            ShowRemoteVideo();
            if (LinphoneManager.Instance.LinphoneCore.IsSelfViewEnabled())
            {
                ShowLocalVideo();
            }
        }

        public void HideVideo()
        {
            Logger.Msg("[InCall] HideVideo");
            HideRemoteVideo();
            HideLocalVideo();
        }

        internal void ShowButtonsAndPanel()
        {
            AdjustDisplayAccordingToOrientation();
        }

        internal void HideButtonsAndPanel()
        {
            PortraitButtonsVisibility = Visibility.Collapsed;
            LandscapeButtonsVisibility = Visibility.Collapsed;
        }

        #endregion

        public Microsoft.Phone.Controls.PageOrientation PageOrientation
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
        }

        #region Button properties
        /// <summary>
        /// Visibility of the video button.
        /// </summary>
        public Visibility VideoButtonVisibility
        {
            get
            {
                return this.videoButtonVisibility;
            }
            set
            {
                if (this.videoButtonVisibility != value)
                {
                    this.videoButtonVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the camera button.
        /// </summary>
        public Visibility CameraButtonVisibility
        {
            get
            {
                return this.cameraButtonVisibility;
            }
            set
            {
                if (this.cameraButtonVisibility != value)
                {
                    this.cameraButtonVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the portrait buttons.
        /// </summary>
        public Visibility PortraitButtonsVisibility
        {
            get
            {
                return this.portraitButtonsVisibility;
            }
            set
            {
                if (this.portraitButtonsVisibility != value)
                {
                    this.portraitButtonsVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the landscape buttons.
        /// </summary>
        public Visibility LandscapeButtonsVisibility
        {
            get
            {
                return this.landscapeButtonsVisibility;
            }
            set
            {
                if (this.landscapeButtonsVisibility != value)
                {
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
        public Boolean IsVideoActive
        {
            get
            {
                return this.isVideoActive;
            }
            set
            {
                if (this.isVideoActive != value)
                {
                    this.isVideoActive = value;
                    if (this.isVideoActive)
                    {
                        ShowVideo();
                    }
                    else
                    {
                        HideVideo();
                    }
                }
            }
        }

        /// <summary>
        /// Tells whether the video is shown (Boolean).
        /// </summary>
        public Boolean VideoShown
        {
            get
            {
                return (this.remoteVideoUri != null);
            }
        }

        /// <summary>
        /// Uri of the remote video stream.
        /// </summary>
        public Uri RemoteVideoUri
        {
            get
            {
                return this.remoteVideoUri;
            }
            set
            {
                if (this.remoteVideoUri != value)
                {
                    this.remoteVideoUri = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the remote video.
        /// </summary>
        public Visibility RemoteVideoVisibility
        {
            get
            {
                return this.remoteVideoVisibility;
            }
            set
            {
                if (this.remoteVideoVisibility != value)
                {
                    this.remoteVideoVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Visibility RemoteVideoProgressBarVisibility
        {
            get
            {
                return this.remoteVideoProgressBarVisibility;
            }
            set
            {
                if (this.RemoteVideoProgressBarVisibility != value)
                {
                    this.remoteVideoProgressBarVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Uri of the local video stream.
        /// </summary>
        public Uri LocalVideoUri
        {
            get
            {
                return this.localVideoUri;
            }
            set
            {
                if (this.localVideoUri != value)
                {
                    this.localVideoUri = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the local video.
        /// </summary>
        public Visibility LocalVideoVisibility
        {
            get
            {
                return this.localVideoVisibility;
            }
            set
            {
                if (this.localVideoVisibility != value)
                {
                    this.localVideoVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Rotation of the local video.
        /// </summary>
        public Double LocalVideoRotation
        {
            get
            {
                return this.localVideoRotation;
            }
            set
            {
                if (this.localVideoRotation != value)
                {
                    this.localVideoRotation = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Scale of the X axis of the local video.
        /// </summary>
        public Double LocalVideoScaleX
        {
            get
            {
                return this.localVideoScaleX;
            }
            set
            {
                if (this.localVideoScaleX != value)
                {
                    this.localVideoScaleX = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int RemoteVideoRotation
        {
            get
            {
                return this.remoteVideoRotation;
            }
            set
            {
                if (this.remoteVideoRotation != value)
                {
                    this.remoteVideoRotation = value;
                    LinphoneManager.Instance.LinphoneCore.SetDeviceRotation(this.remoteVideoRotation);
                    LinphoneCall call = GetCurrentCall();
                    if (call != null) LinphoneManager.Instance.LinphoneCore.UpdateCall(call, null);
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Stats properties
        public string MediaEncryption
        {
            get
            {
                return mediaEncryption;
            }
            set
            {
                mediaEncryption = value;
                this.OnPropertyChanged();
            }
        }
        public string DownBandwidth
        {
            get
            {
                return downBandwidth;
            }
            set
            {
                downBandwidth = value;
                this.OnPropertyChanged();
            }
        }
        public string PayloadType
        {
            get
            {
                return payloadType;
            }
            set
            {
                payloadType = value;
                this.OnPropertyChanged();
            }
        }
        public string UpBandwidth
        {
            get
            {
                return upBandwidth;
            }
            set
            {
                upBandwidth = value;
                this.OnPropertyChanged();
            }
        }
        public string ICE
        {
            get
            {
                return ice;
            }
            set
            {
                ice = value;
                this.OnPropertyChanged();
            }
        }
        public string ReceivedVideoSize
        {
            get
            {
                return receivedVideoSize;
            }
            set
            {
                receivedVideoSize = value;
                this.OnPropertyChanged();
            }
        }
        public string SentVideoSize
        {
            get
            {
                return sentVideoSize;
            }
            set
            {
                sentVideoSize = value;
                this.OnPropertyChanged();
            }
        }
        public Visibility VideoStatsVisibility
        {
            get
            {
                return videoStatsVisibility;
            }
            set
            {
                videoStatsVisibility = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Private variables
        private Microsoft.Phone.Controls.PageOrientation pageOrientation = Microsoft.Phone.Controls.PageOrientation.Portrait;
        private Visibility videoButtonVisibility = Visibility.Collapsed;
        private Visibility cameraButtonVisibility = Visibility.Collapsed;
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
