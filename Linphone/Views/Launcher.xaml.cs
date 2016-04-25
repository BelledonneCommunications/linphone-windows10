/*
Launcher.xaml.cs
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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{
    public partial class Launcher : Page
    {
        public Launcher()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
/*
            if (Customs.ShowWizardUntilAccountConfigured)
            {
                NavigationService.Navigate(new Uri("/Views/FirstLogin.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
            }
            NavigationService.RemoveBackEntry(); // Prevent a back to this screen*/
        }
    }
}