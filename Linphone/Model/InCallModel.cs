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
        /// <summary>
        /// Public constructor.
        /// </summary>
        public InCallModel()
            : base()
        {
            VideoButtonVisibility = Visibility.Visible;
            CameraButtonVisibility = Visibility.Visible;
        }

        #region Actions

        /// <summary>
        /// Show or hide the display of the dialpad.
        /// </summary>
        public void ToggleDialpad()
        {
            IsDialpadToggled = !IsDialpadToggled;
            if (IsDialpadToggled)
            {
                NumpadVisibility = Visibility.Visible;
                PauseButtonVisibility = Visibility.Collapsed;
                MicrophoneButtonVisibility = Visibility.Collapsed;
                SpeakerButtonVisibility = Visibility.Collapsed;
                StatsButtonVisibility = Visibility.Collapsed;
            }
            else
            {
                NumpadVisibility = Visibility.Collapsed;
                PauseButtonVisibility = Visibility.Visible;
                MicrophoneButtonVisibility = Visibility.Visible;
                SpeakerButtonVisibility = Visibility.Visible;
                StatsButtonVisibility = Visibility.Visible;
            }
        }

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

        #endregion

        #region Button properties

        /// <summary>
        /// Is the dialpad displayed (Boolean).
        /// </summary>
        public Boolean IsDialpadToggled
        {
            get
            {
                return this.isDialpadToggled;
            }
            set
            {
                if (this.isDialpadToggled != value)
                {
                    this.isDialpadToggled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the numeric pad.
        /// </summary>
        public Visibility NumpadVisibility
        {
            get
            {
                return this.numpadVisibility;
            }
            set
            {
                if (this.numpadVisibility != value)
                {
                    this.numpadVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the pause button.
        /// </summary>
        public Visibility PauseButtonVisibility
        {
            get
            {
                return this.pauseButtonVisibility;
            }
            set
            {
                if (this.pauseButtonVisibility != value)
                {
                    this.pauseButtonVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the microphone button.
        /// </summary>
        public Visibility MicrophoneButtonVisibility
        {
            get
            {
                return this.microphoneButtonVisibility;
            }
            set
            {
                if (this.microphoneButtonVisibility != value)
                {
                    this.microphoneButtonVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the speaker button.
        /// </summary>
        public Visibility SpeakerButtonVisibility
        {
            get
            {
                return this.speakerButtonVisibility;
            }
            set
            {
                if (this.speakerButtonVisibility != value)
                {
                    this.speakerButtonVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Visibility of the statistics button.
        /// </summary>
        public Visibility StatsButtonVisibility
        {
            get
            {
                return this.statsButtonVisibility;
            }
            set
            {
                if (this.statsButtonVisibility != value)
                {
                    this.statsButtonVisibility = value;
                    this.OnPropertyChanged();
                }
            }
        }

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
                if (!LinphoneManager.Instance.LinphoneCore.IsVideoSupported()
                    || !LinphoneManager.Instance.LinphoneCore.IsVideoEnabled())
                {
                    value = Visibility.Collapsed;
                }
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
                if (!LinphoneManager.Instance.LinphoneCore.IsVideoSupported()
                    || !LinphoneManager.Instance.LinphoneCore.IsVideoEnabled()
                    || (LinphoneManager.Instance.NumberOfCameras < 2))
                {
                    value = Visibility.Collapsed;
                }
                if (this.cameraButtonVisibility != value)
                {
                    this.cameraButtonVisibility = value;
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

        #region Private variables

        private Boolean isDialpadToggled = false;
        private Visibility numpadVisibility = Visibility.Collapsed;
        private Visibility pauseButtonVisibility = Visibility.Visible;
        private Visibility microphoneButtonVisibility = Visibility.Visible;
        private Visibility speakerButtonVisibility = Visibility.Visible;
        private Visibility statsButtonVisibility = Visibility.Visible;
        private Visibility videoButtonVisibility = Visibility.Collapsed;
        private Visibility cameraButtonVisibility = Visibility.Collapsed;
        private Boolean isVideoActive = false;
        private Uri remoteVideoUri = null;
        private Visibility remoteVideoVisibility = Visibility.Collapsed;
        private Timer localVideoTimer;
        private Uri localVideoUri = null;
        private Visibility localVideoVisibility = Visibility.Collapsed;
        private Double localVideoRotation = 0;
        private Double localVideoScaleX = 1.0;

        #endregion
    }
}
