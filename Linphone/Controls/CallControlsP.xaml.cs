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
            try
            {
                LinphoneManager.Instance.EnableSpeaker(isSpeakerToggled);
                speakerImg.Source = new BitmapImage(new Uri(isSpeakerToggled ? speakerOn : speakerOff, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                Logger.Warn("Exception while trying to toggle speaker to {0}", isSpeakerToggled.ToString());
                speaker.IsChecked = !isSpeakerToggled;
            }
        }

        private void microphone_Click_1(object sender, RoutedEventArgs e)
        {
            bool isMicToggled = (bool)microphone.IsChecked;
            LinphoneManager.Instance.MuteMic(isMicToggled);
        }

        private void stats_Click_1(object sender, RoutedEventArgs e)
        {
            bool areStatsVisible = (bool)stats.IsChecked;
            StatsClick(this, areStatsVisible);
        }

        private void video_Click_1(object sender, RoutedEventArgs e)
        {
            bool isVideoToggled = (bool)video.IsChecked;
            if (!LinphoneManager.Instance.EnableVideo(isVideoToggled))
            {
                if (isVideoToggled) video.IsChecked = false;
            }
            videoImg.Source = new BitmapImage(new Uri(isVideoToggled ? videoOn : videoOff, UriKind.RelativeOrAbsolute));
        }

        private void camera_Click_1(object sender, RoutedEventArgs e)
        {
            CameraClick(this);
        }

        private void dialpad_Click_1(object sender, RoutedEventArgs e)
        {
            numpad.Visibility = numpad.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void pause_Click_1(object sender, RoutedEventArgs e)
        {
            bool isPauseToggled = (bool)pause.IsChecked;
            PauseClick(this, isPauseToggled);
        }
    }
}
