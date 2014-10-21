using Linphone.Core;
using Linphone.Model;
using System;
using System.Collections.Generic;
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
        private string currentOrientation = "PortraitUp";
        /// <summary>
        /// Public constructor.
        /// </summary>
        public InCallModel()
            : base()
        {
            if (CurrentPage != null)
            {
                currentOrientation = CurrentPage.Orientation.ToString();
            }

            if (LinphoneManager.Instance.isLinphoneRunning)
            {
                bool isVideoAvailable = LinphoneManager.Instance.IsVideoAvailable;
                videoButtonVisibility = Visibility.Visible;// isVideoEnabled ? Visibility.Visible : Visibility.Collapsed;
                cameraButtonVisibility = isVideoAvailable && LinphoneManager.Instance.NumberOfCameras >= 2 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called when user rotates the device
        /// </summary>
        public void OrientationChanged(object sender, Microsoft.Phone.Controls.OrientationChangedEventArgs e)
        {
            string orientation = e.Orientation.ToString();
            if (!orientation.Equals(currentOrientation))
            {
                currentOrientation = orientation;
                AdjustDisplayAccordingToOrientation();
            }
        }

        #region Actions

        private void AdjustDisplayAccordingToOrientation()
        {
            if (currentOrientation.StartsWith("Portrait"))
            {
                PortraitButtonsVisibility = Visibility.Visible;
                LandscapeButtonsVisibility = Visibility.Collapsed;
            }
            else if (currentOrientation.StartsWith("Landscape"))
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
            ShowLocalVideo();
        }

        private void ShowRemoteVideo()
        {
            Int32 id = LinphoneManager.Instance.LinphoneCore.GetNativeVideoWindowId();
            RemoteVideoUri = Mediastreamer2.WP8Video.VideoRenderer.StreamUri(id);
            RemoteVideoVisibility = Visibility.Visible;
        }

        private void HideRemoteVideo()
        {
            RemoteVideoUri = null;
            RemoteVideoVisibility = Visibility.Collapsed;
        }

        private void ShowLocalVideo()
        {
            int rotation = LinphoneManager.Instance.LinphoneCore.GetCameraSensorRotation();
            if (rotation < 0)
            {
                localVideoTimer = new Timer(new TimerCallback(LocalVideoTimerCallback), null, 200, 15);
            }
            else
            {
                LocalVideoTimerCallback(null);
            }
        }

        private void LocalVideoTimerCallback(Object state)
        {
            if (LinphoneManager.Instance.LinphoneCore == null)
            {
                localVideoTimer.Dispose();
                localVideoTimer = null;
                return;
            }

            int rotation = LinphoneManager.Instance.LinphoneCore.GetCameraSensorRotation();
            if (rotation >= 0)
            {
                if (localVideoTimer != null)
                {
                    localVideoTimer.Dispose();
                    localVideoTimer = null;
                }
                ((InCall)Page).Status.Dispatcher.BeginInvoke(delegate()
                {
                    String device = LinphoneManager.Instance.LinphoneCore.GetVideoDevice();
                    Boolean mirrored = Mediastreamer2.WP8Video.VideoRenderer.IsCameraMirrored(device);
                    LocalVideoUri = Mediastreamer2.WP8Video.VideoRenderer.CameraUri(device);
                    LocalVideoVisibility = Visibility.Visible;
                    LocalVideoRotation = rotation;
                    if (mirrored)
                    {
                        LocalVideoScaleX = -1.0;
                    }
                    else
                    {
                        LocalVideoScaleX = 1.0;
                    }
                });
            }
        }

        private void HideLocalVideo()
        {
            LocalVideoUri = null;
            LocalVideoVisibility = Visibility.Collapsed;
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
                        ShowRemoteVideo();
                        if (LinphoneManager.Instance.LinphoneCore.IsSelfViewEnabled())
                        {
                            ShowLocalVideo();
                        }
                    }
                    else
                    {
                        HideRemoteVideo();
                        HideLocalVideo();
                    }
                }
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
        public string AudioDownBw
        {
            get
            {
                return audioDownBw;
            }
            set
            {
                audioDownBw = value;
                this.OnPropertyChanged();
            }
        }
        public string AudioPType
        {
            get
            {
                return audioPType;
            }
            set
            {
                audioPType = value;
                this.OnPropertyChanged();
            }
        }
        public string AudioUpBw
        {
            get
            {
                return audioUpBw;
            }
            set
            {
                audioUpBw = value;
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
        public string VideoDownBw
        {
            get
            {
                return videoDownBw;
            }
            set
            {
                videoDownBw = value;
                this.OnPropertyChanged();
            }
        }
        public string VideoUpBw
        {
            get
            {
                return videoUpBw;
            }
            set
            {
                videoUpBw = value;
                this.OnPropertyChanged();
            }
        }
        public string VideoPType
        {
            get
            {
                return videoPType;
            }
            set
            {
                videoPType = value;
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
        private Visibility videoButtonVisibility = Visibility.Collapsed;
        private Visibility cameraButtonVisibility = Visibility.Collapsed;
        private Visibility portraitButtonsVisibility = Visibility.Visible;
        private Visibility landscapeButtonsVisibility = Visibility.Collapsed;
        private Boolean isVideoActive = false;
        private Uri remoteVideoUri = null;
        private Visibility remoteVideoVisibility = Visibility.Collapsed;
        private Timer localVideoTimer;
        private Uri localVideoUri = null;
        private Visibility localVideoVisibility = Visibility.Collapsed;
        private Double localVideoRotation = 0;
        private Double localVideoScaleX = 1.0;
        private string mediaEncryption = "";
        private string audioDownBw = "";
        private string audioPType = "";
        private string audioUpBw = "";
        private string videoDownBw = "";
        private string videoPType = "";
        private string videoUpBw = "";
        private string ice = "";
        private Visibility videoStatsVisibility = Visibility.Collapsed;
        #endregion
    }
}
