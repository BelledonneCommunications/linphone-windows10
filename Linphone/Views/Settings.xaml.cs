/*
Settings.xaml.cs
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.Collections.Generic;
using System.Windows;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Base setting page, provides access to each detailled settings + debug setting.
    /// </summary>
    public partial class Settings : Page
    {

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Settings()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void account_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.AccountSettings), null);
        }

        private void audio_Click_1(object sender, RoutedEventArgs e)
        {
           Frame.Navigate(typeof(Views.AudioSettings), null);
        }

        private void video_Click_1(object sender, RoutedEventArgs e)
        {
           Frame.Navigate(typeof(Views.VideoSettings), null);
        }

        private void advanced_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.AdvancedSettings), null);
        }

        private async void LockScreenSettings_Click_1(object sender, RoutedEventArgs e)
        {
           // var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }

        }
    }
}