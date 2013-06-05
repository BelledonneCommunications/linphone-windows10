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
    public class InCallModel : BaseModel
    {
        public InCallModel()
            : base()
        {
        }

        #region Actions

        public void ToggleDialpad()
        {
            Logger.Err("ToggleDialpad " + IsDialpadToggled);
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

        public void ShowRemoteVideo()
        {
            Int32 id = LinphoneManager.Instance.LinphoneCore.GetNativeVideoWindowId();
            RemoteVideoUri = Mediastreamer2.WP8Video.VideoRenderer.StreamUri(id);
            RemoteVideoVisibility = Visibility.Visible;
        }

        #endregion

        #region Button properties

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
                    this.OnPropertyChanged("IsDialpadToggled");
                }
            }
        }

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
                    this.OnPropertyChanged("NumpadVisibility");
                }
            }
        }

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
                    this.OnPropertyChanged("PauseButtonVisibility");
                }
            }
        }

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
                    this.OnPropertyChanged("MicrophoneButtonVisibility");
                }
            }
        }

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
                    this.OnPropertyChanged("SpeakerButtonVisibility");
                }
            }
        }

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
                    this.OnPropertyChanged("StatsButtonVisibility");
                }
            }
        }

        #endregion

        #region Video properties

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
                    this.OnPropertyChanged("RemoteVideoUri");
                }
            }
        }

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
                    this.OnPropertyChanged("RemoteVideoVisibility");
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
        private Uri remoteVideoUri = null;
        private Visibility remoteVideoVisibility = Visibility.Collapsed;

        #endregion
    }
}
