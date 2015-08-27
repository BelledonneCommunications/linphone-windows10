/*
AssistantPage.xaml.cs
Copyright(C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using Linphone.Controls;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Linphone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssistantPage : Page
    {
        public AssistantPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
            _model = new AssistantModel();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WelcomePage.AssistantWelcomePageButtonClick = WelcomePageButtonClick;
            VisualStateManager.GoToState(this, WelcomeState.Name, true);
        }

        private void MainHeader_MenuClick(object sender, RoutedEventArgs e)
        {
            MenuSplitView.IsPaneOpen = !MenuSplitView.IsPaneOpen;
        }

        private void AssistantBackButton_Click(object sender, RoutedEventArgs e)
        {
            AssistantState previousState = _model.PreviousState;
            if (previousState == AssistantState.Welcome)
            {
                VisualStateManager.GoToState(this, WelcomeState.Name, true);
            }
            // TODO
        }

        private void AssistantDialerButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).GoBack();
        }

        private void WelcomePageButtonClick(object sender, RoutedEventArgs e)
        {
            AssistantButton button = sender as AssistantButton;
            if (button.Name == "CreateLinphoneAccountButton")
            {
                GoToState(AssistantState.CreateLinphoneAccount);
            }
            else if (button.Name == "UseExistingLinphoneAccountButton")
            {
                GoToState(AssistantState.UseLinphoneAccount);
            }
            else if (button.Name == "UseExistingSipAccountButton")
            {
                GoToState(AssistantState.UseSipAccount);
            }
            else if (button.Name == "DownloadConfigurationButton")
            {
                GoToState(AssistantState.DownloadConfiguration);
            }
        }

        private void GoToState(AssistantState state)
        {
            _model.State = state;
            if (state == AssistantState.CreateLinphoneAccount)
            {
                VisualStateManager.GoToState(this, CreateLinphoneAccountState.Name, true);
            }
            // TODO
        }

        private AssistantModel _model;
    }
}
