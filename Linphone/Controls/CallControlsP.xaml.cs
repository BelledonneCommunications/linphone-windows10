/*
CallControlsP.xaml.cs
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

using Linphone.Model;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Linphone.Controls
{

    public partial class CallControlsP : UserControl
    {
        public delegate void HangUpClickEventHandler(object sender);
        public event HangUpClickEventHandler HangUpClick;

        public delegate void StatsClickEventHandler(object sender, bool areStatsVisible);
        public event StatsClickEventHandler StatsClick;

        public delegate void CameraClickEventHandler(object sender);
        public event CameraClickEventHandler CameraClick;

        public delegate void PauseClickEventHandler(object sender, bool isPaused);
        public event PauseClickEventHandler PauseClick;

        public delegate bool SpeakerClickEventHandler(object sender, bool isSpeakerOn);
        public event SpeakerClickEventHandler SpeakerClick;

        public delegate void MuteClickEventHandler(object sender, bool isMuteOn);
        public event MuteClickEventHandler MuteClick;

        public delegate void VideoClickEventHandler(object sender, bool isVideoOn);
        public event VideoClickEventHandler VideoClick;

        public delegate void DialpadClickEventHandler(object sender, bool isDialpadShown);
        public event DialpadClickEventHandler DialpadClick;

        public delegate void BluetoothClickEventHandler(object sender, bool isBluetoothOn);
        public event DialpadClickEventHandler BluetoothClick;

        public CallControlsP()
        {
            InitializeComponent();
            microphone.IsChecked = LinphoneManager.Instance.Core.IsMicEnabled;
            camera.IsEnabled = LinphoneManager.Instance.IsVideoAvailable && LinphoneManager.Instance.NumberOfCameras > 1;
        }

        #region Button enabled/disabled 
        public void enabledDialpad(bool isDialpadShown)
        {
            dialpad.Visibility = isDialpadShown ? Visibility.Visible : Visibility.Collapsed;
        }

        public void enabledVideo(bool isVideoEnabled)
        {
            video.IsEnabled = isVideoEnabled;
        }

        public void checkedVideo(bool isVideoChecked)
        {
            video.IsChecked = isVideoChecked;
        }

        public void enabledPause(bool isPauseEnabled)
        {
            pause.IsEnabled = isPauseEnabled;
        }

        #endregion

        #region Click Event
        private void hangUp_Click(object sender, RoutedEventArgs e)
        {
            HangUpClick(this);
        }

        private void bluetooth_Click_1(object sender, RoutedEventArgs e)
        {
            bool isBluetoothToggled = (bool)bluetooth.IsChecked;
            BluetoothClick(this, isBluetoothToggled);
        }

        private void speaker_Click_1(object sender, RoutedEventArgs e)
        {
            bool isSpeakerToggled = (bool)speaker.IsChecked;
            SpeakerClick(this, isSpeakerToggled);
        }

        private void microphone_Click_1(object sender, RoutedEventArgs e)
        {
            bool isMicToggled = (bool)microphone.IsChecked;
            MuteClick(this, isMicToggled);
        }

        private void stats_Click_1(object sender, RoutedEventArgs e)
        {
            bool areStatsVisible = (bool)stats.IsChecked;
            stats.IsChecked = areStatsVisible;
            statsPanel.Visibility = areStatsVisible ? Visibility.Visible : Visibility.Collapsed;
            StatsClick(this, areStatsVisible);
        }

        private void video_Click_1(object sender, RoutedEventArgs e)
        {
            bool isVideoToggled = (bool)video.IsChecked;
            if (!isVideoToggled)
            {
                video.IsEnabled = false;
            } else
            {
                video.IsChecked = isVideoToggled;
            }
            VideoClick(this, isVideoToggled);
        }

        private void camera_Click_1(object sender, RoutedEventArgs e)
        {
            CameraClick(this);
        }

        private void dialpad_Click_1(object sender, RoutedEventArgs e)
        {
            bool isDialpadChecked = (bool)dialpad.IsChecked;
            dialpad.IsChecked = isDialpadChecked;
            numpad.Visibility = isDialpadChecked ? Visibility.Visible : Visibility.Collapsed;
            DialpadClick(this, isDialpadChecked);
        }

        private void pause_Click_1(object sender, RoutedEventArgs e)
        {
            bool isPauseToggled = (bool)pause.IsChecked;
            PauseClick(this, isPauseToggled);
        }
        #endregion
    }
}
