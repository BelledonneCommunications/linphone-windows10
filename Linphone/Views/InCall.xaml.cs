using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Linphone.Views
{
    public partial class InCall : PhoneApplicationPage
    {
        private const string speakerOn = "/Assets/AppBar/speaker.png";
        private const string speakerOff = "/Assets/AppBar/speaker.png";
        private const string micOn = "/Assets/AppBar/mic.png";
        private const string micOff = "/Assets/AppBar/mic.png";
        private const string pauseOn = "/Assets/AppBar/play.png";
        private const string pauseOff = "/Assets/AppBar/pause.png";

        public InCall()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("sip"))
            {

            }
        }

        private void hangUp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void speaker_Click_1(object sender, RoutedEventArgs e)
        {
            bool isSpeakerToggled = (bool)speaker.IsChecked;
            speakerImg.Source = new BitmapImage(new Uri(isSpeakerToggled ? speakerOn : speakerOff, UriKind.RelativeOrAbsolute));
        }

        private void microphone_Click_1(object sender, RoutedEventArgs e)
        {
            bool isMicToggled = (bool)microphone.IsChecked;
            microImg.Source = new BitmapImage(new Uri(isMicToggled ? micOn : micOff, UriKind.RelativeOrAbsolute));
        }

        private void pause_Click_1(object sender, RoutedEventArgs e)
        {
            bool isPauseToggled = (bool)pause.IsChecked;
            pauseImg.Source = new BitmapImage(new Uri(isPauseToggled ? pauseOn : pauseOff, UriKind.RelativeOrAbsolute));
        }

        private void dialpad_Click_1(object sender, RoutedEventArgs e)
        {
            bool isDialpadVisible = (bool)dialpad.IsChecked;
            pause.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            speaker.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            microphone.Visibility = isDialpadVisible ? Visibility.Collapsed : Visibility.Visible;
            numpad.Visibility = isDialpadVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Numpad_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}