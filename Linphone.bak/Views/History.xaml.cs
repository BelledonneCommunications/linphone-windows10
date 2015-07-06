/*
History.xaml.cs
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

using Linphone.Agents;
using Linphone.Core;
using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Page displaying the call logs.
    /// </summary>
    public partial class History : BasePage
    {
        private bool _usingSelectionAppBar = false;
        private IEnumerable<CallLog> _selection;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public History()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Fetches the logs from the LinphoneManager and displays them.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LinphoneManager.Instance.LinphoneCore.ResetMissedCallsCount();

            List<CallLog> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
            MissedCalls.ItemsSource = (from log in callsHistory where (log.IsMissed) select log).ToList();
        }

        private void deleteAll_Click_1(object sender, EventArgs e)
        {
            LinphoneManager.Instance.ClearCallLogs();

            List<CallLog> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
            MissedCalls.ItemsSource = (from log in callsHistory where (log.IsMissed) select log).ToList();
        }

        private void deleteSelection_Click_1(object sender, EventArgs e)
        {
            LinphoneManager.Instance.RemoveCallLogs(_selection);

            List<CallLog> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
            MissedCalls.ItemsSource = (from log in callsHistory where (log.IsMissed) select log).ToList();

            ClearApplicationBar();
            SetupAppBarForEmptySelection();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            SetupAppBarForEmptySelection();
        }

        private void ClearApplicationBar()
        {
            while (ApplicationBar.Buttons.Count > 0)
            {
                ApplicationBar.Buttons.RemoveAt(0);
            }
        }

        private void SetupAppBarForEmptySelection()
        {
            ApplicationBarIconButton appBarDeleteAll = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
            appBarDeleteAll.Text = AppResources.DeleteAllMenu;
            ApplicationBar.Buttons.Add(appBarDeleteAll);
            appBarDeleteAll.Click += deleteAll_Click_1;

            _usingSelectionAppBar = false;
        }

        private void SetupAppBarForSelectedItems()
        {
            ApplicationBarIconButton appBarDeleteSelection = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
            appBarDeleteSelection.Text = AppResources.DeleteSelectionMenu;
            ApplicationBar.Buttons.Add(appBarDeleteSelection);
            appBarDeleteSelection.Click += deleteSelection_Click_1;

            _usingSelectionAppBar = true;
        }

        private void calls_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LongListMultiSelector list = (LongListMultiSelector)sender;
            if (list.SelectedItems.Count == 0)
            {
                ClearApplicationBar();
                SetupAppBarForEmptySelection();
            }
            else if (list.SelectedItems.Count >= 1 && !_usingSelectionAppBar) // Do it only once, when selection was empty and isn't anymore
            {
                _selection = list.SelectedItems.Cast<CallLog>();
                ClearApplicationBar();
                SetupAppBarForSelectedItems();
            }
        }

        private void SetAddressGoToDialerAndCall(String address)
        {
            NavigationService.Navigate(new Uri("/Views/Dialer.xaml?sip=" + Utils.ReplacePlusInUri(address), UriKind.RelativeOrAbsolute));
            NavigationService.RemoveBackEntry(); // Prevent a back to this screen
            NavigationService.RemoveBackEntry(); // Prevent a back to the dialer from the dialer
        }

        private void callLog_Click_1(object sender, RoutedEventArgs e)
        {
            CallLog log = ((sender as StackPanel).Tag as CallLog);
            LinphoneCallLog nativeLog = (LinphoneCallLog)log.NativeLog;

            String address;
            if (log.IsIncoming)
                address = nativeLog.From.AsStringUriOnly();
            else
                address = nativeLog.To.AsStringUriOnly();

            SetAddressGoToDialerAndCall(address);
        }
    }
}