/*
CallControlsL.xaml.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Model;
using System.Windows.Media.Imaging;

namespace Linphone.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CallControlsL : UserControl
    {
        private const string speakerOn = "/Assets/Incall_Icons/speaker_on.png";
        private const string speakerOff = "/Assets/Incall_Icons/speaker_off.png";
        private const string bluetoothOn = "/Assets/Incall_Icons/bluetooth_on.png";
        private const string bluetoothOff = "/Assets/Incall_Icons/bluetooth_off.png";
        private const string videoOn = "/Assets/Incall_Icons/video_on.png";
        private const string videoOff = "/Assets/Incall_Icons/video_off.png";
        private const string pauseOn = "/Assets/Incall_Icons/pause.png";
        private const string pauseOff = "/Assets/Incall_Icons/play.png";
        private const string micOn = "/Assets/Incall_Icons/micro_on.png";
        private const string micOff = "/Assets/Incall_Icons/micro_off.png";

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

        /// <summary>
        /// Public constructor
        /// </summary>
        public CallControlsL()
        {
            InitializeComponent();
        }

        private void hangUp_Click(object sender, RoutedEventArgs e)
        {
            HangUpClick(this);
        }

        private void bluetooth_Click_1(object sender, RoutedEventArgs e)
        {
            bool isBluetoothToggled = (bool)bluetooth.IsChecked;
            bluetoothImg.Source = new BitmapImage(new Uri(isBluetoothToggled ? bluetoothOn : bluetoothOff, UriKind.RelativeOrAbsolute));
            BluetoothClick(this, isBluetoothToggled);
        }

        private void speaker_Click_1(object sender, RoutedEventArgs e)
        {
            bool isSpeakerToggled = (bool)speaker.IsChecked;
            if (SpeakerClick(this, isSpeakerToggled))
            {
                speakerImg.Source = new BitmapImage(new Uri(isSpeakerToggled ? speakerOn : speakerOff, UriKind.RelativeOrAbsolute));
            }
        }

        private void microphone_Click_1(object sender, RoutedEventArgs e)
        {
            bool isMicToggled = (bool)microphone.IsChecked;
            microImg.Source = new BitmapImage(new Uri(isMicToggled ? micOff : micOn, UriKind.RelativeOrAbsolute));
            MuteClick(this, isMicToggled);
        }

        private void stats_Click_1(object sender, RoutedEventArgs e)
        {
            bool areStatsVisible = (bool)stats.IsChecked;
            StatsClick(this, areStatsVisible);
        }

        private void video_Click_1(object sender, RoutedEventArgs e)
        {
            bool isVideoToggled = (bool)video.IsChecked;
            videoImg.Source = new BitmapImage(new Uri(isVideoToggled ? videoOn : videoOff, UriKind.RelativeOrAbsolute));
            VideoClick(this, isVideoToggled);
        }

        private void camera_Click_1(object sender, RoutedEventArgs e)
        {
            CameraClick(this);
        }

        private void dialpad_Click_1(object sender, RoutedEventArgs e)
        {
            bool isDialpadChecked = (bool)dialpad.IsChecked;
            DialpadClick(this, isDialpadChecked);
        }

        private void pause_Click_1(object sender, RoutedEventArgs e)
        {
            bool isPauseToggled = (bool)pause.IsChecked;
            pauseImg.Source = new BitmapImage(new Uri(isPauseToggled ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));
            PauseClick(this, isPauseToggled);
        }
    }
}
