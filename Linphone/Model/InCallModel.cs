using Linphone.Core;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private void ShowRemoteVideo()
        {
            Int32 id = LinphoneManager.Instance.LinphoneCore.GetNativeVideoWindowId();
            RemoteVideoUri = Mediastreamer2.WP8Video.VideoRenderer.StreamUri(id);
            RemoteVideoVisibility = Visibility.Visible;
            ButtonsOpacity = InCallModel.ButtonsOpacityInVideoMode;
        }

        private void HideRemoteVideo()
        {
            RemoteVideoUri = null;
            RemoteVideoVisibility = Visibility.Collapsed;
            ButtonsOpacity = InCallModel.ButtonsOpacityInAudioMode;
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
        /// Opacity of the action buttons (Double).
        /// </summary>
        public Double ButtonsOpacity
        {
            get
            {
                return this.buttonsOpacity;
            }
            set
            {
                if (this.buttonsOpacity != value)
                {
                    this.buttonsOpacity = value;
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
                    }
                    else
                    {
                        HideRemoteVideo();
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

        #endregion

        #region Private variables

        private Boolean isDialpadToggled = false;
        private Double buttonsOpacity = InCallModel.ButtonsOpacityInAudioMode;
        private Visibility numpadVisibility = Visibility.Collapsed;
        private Visibility pauseButtonVisibility = Visibility.Visible;
        private Visibility microphoneButtonVisibility = Visibility.Visible;
        private Visibility speakerButtonVisibility = Visibility.Visible;
        private Visibility statsButtonVisibility = Visibility.Visible;
        private Boolean isVideoActive = false;
        private Uri remoteVideoUri = null;
        private Visibility remoteVideoVisibility = Visibility.Collapsed;

        private static Double ButtonsOpacityInVideoMode = 0.7;
        private static Double ButtonsOpacityInAudioMode = 1.0;

        #endregion
    }
}
