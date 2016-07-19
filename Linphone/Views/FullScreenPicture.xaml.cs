/*
FullScreenPicture.xaml.cs
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
using Linphone.Model;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;

namespace Linphone.Views
{
    /// <summary>
    /// Displays on full screen received or sent pictures
    /// </summary>
    public partial class FullScreenPicture : Page
    {
        private String _fileName;

        public FullScreenPicture()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is String)
            {
                _fileName = (e.Parameter as String);
                BitmapImage image = await Utils.ReadImageFromTempStorage(_fileName);
                if (image != null)
                {
                    Image.Source = image;
                }
                else
                {
                    //TODO ?
                }
            }
        }

        private async void Save_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bool result = await Utils.SavePictureInMediaLibrary(_fileName);
            //MessageBox.Show(result ? AppResources.FileSavingSuccess : AppResources.FileSavingFailure, AppResources.FileSaving, MessageBoxButton.OK); bool result = Utils.SavePictureInMediaLibrary(_fileName);
        }
    }
}