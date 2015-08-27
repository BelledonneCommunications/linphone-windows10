/*
MainPage.xaml.cs
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Linphone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, DialerState.Name, true);
        }

        private void MainHeader_MenuClick(object sender, RoutedEventArgs e)
        {
            MenuSplitView.IsPaneOpen = !MenuSplitView.IsPaneOpen;
        }

        private void MainFooter_PageChanged(object sender, RoutedEventArgs e)
        {
            Controls.LinphonePageRadioButton rb = sender as Controls.LinphonePageRadioButton;
            if (rb.Name.StartsWith("History"))
            {
                VisualStateManager.GoToState(this, HistoryState.Name, true);
            }
            else if (rb.Name.StartsWith("Contact"))
            {
                VisualStateManager.GoToState(this, ContactState.Name, true);
            }
            else if (rb.Name.StartsWith("Chat"))
            {
                VisualStateManager.GoToState(this, ChatState.Name, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DialerState.Name, true);
            }
        }

        private void HistoryPage_HistoryEntryClick(object sender, ItemClickEventArgs e)
        {
            HistoryEntry historyEntry = (e.ClickedItem as HistoryEntry);
            VisualStateManager.GoToState(this, HistoryDetailsState.Name, true);
        }

        private void HistoryDetailsPage_HistoryDetailsBackButtonClick(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, HistoryState.Name, true);
        }

        private void ContactPage_ContactEntryClick(object sender, ItemClickEventArgs e)
        {
            ContactEntry contactEntry = (e.ClickedItem as ContactEntry);
            ContactDetailsPage.Contact = contactEntry;
            VisualStateManager.GoToState(this, ContactDetailsState.Name, true);
        }

        private void ContactDetailsPage_ContactDetailsBackButtonClick(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, ContactState.Name, true);
        }
    }
}
