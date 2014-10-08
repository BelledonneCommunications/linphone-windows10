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
    public partial class CallControlsP : UserControl
    {
        private const string speakerOn = "/Assets/AppBar/speaker.png";
        private const string speakerOff = "/Assets/AppBar/speaker.png";
        private const string videoOn = "/Assets/AppBar/feature.video.png";
        private const string videoOff = "/Assets/AppBar/feature.video.png";

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

        /// <summary>
        /// Public constructor
        /// </summary>
        public CallControlsP()
        {
            InitializeComponent();
        }

        private void hangUp_Click(object sender, RoutedEventArgs e)
        {
            HangUpClick(this);
        }

        private void speaker_Click_1(object sender, RoutedEventArgs e)
        {
            bool isSpeakerToggled = (bool)speaker.IsChecked;
            if (SpeakerClick(this, isSpeakerToggled)) {
                speakerImg.Source = new BitmapImage(new Uri((bool)speaker.IsChecked ? speakerOn : speakerOff, UriKind.RelativeOrAbsolute));
            }
        }

        private void microphone_Click_1(object sender, RoutedEventArgs e)
        {
            bool isMicToggled = (bool)microphone.IsChecked;
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
            videoImg.Source = new BitmapImage(new Uri((bool)video.IsChecked ? videoOn : videoOff, UriKind.RelativeOrAbsolute));
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
            PauseClick(this, isPauseToggled);
        }
    }
}
