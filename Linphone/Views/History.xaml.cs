/*
History.xaml.cs
Copyright (C) 2016  Belledonne Communications, Grenoble, France
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

using BelledonneCommunications.Linphone.Native;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{
    public partial class History : Page
    {
        private bool _usingSelectionAppBar = false;
        private IEnumerable<CallLogModel> _selection;

        public History()
        {
            this.InitializeComponent();
            SetupAppBarForEmptySelection();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LinphoneManager.Instance.Core.ResetMissedCallsCount();

            List<CallLogModel> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
            Calls.ItemClick += Calls_ItemClick;
            MissedCalls.ItemsSource = (from log in callsHistory where (log.IsMissed) select log).ToList();
        }

        private void deleteAll_Click_1(object sender, RoutedEventArgs e)
        {
            LinphoneManager.Instance.Core.ClearCallLogs();

            List<CallLogModel> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
            MissedCalls.ItemsSource = (from log in callsHistory where (log.IsMissed) select log).ToList();
        }

        private void deleteSelection_Click_1(object sender, RoutedEventArgs e)
        {
            LinphoneManager.Instance.RemoveCallLogs(_selection);

            List<CallLogModel> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
            MissedCalls.ItemsSource = (from log in callsHistory where (log.IsMissed) select log).ToList();

            SetupAppBarForEmptySelection();
        }

        private void SetupAppBarForEmptySelection()
        {
            Delete_button.Click += deleteAll_Click_1;
            _usingSelectionAppBar = false;
        }

        private void SetupAppBarForSelectedItems()
        {
            Delete_button.Click += deleteSelection_Click_1;
            _usingSelectionAppBar = true;
        }

        private void calls_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ListView list = (ListView)sender;
            if (list.SelectedItems.Count == 0)
            {
                SetupAppBarForEmptySelection();
            }
            else if (list.SelectedItems.Count >= 1 && !_usingSelectionAppBar) // Do it only once, when selection was empty and isn't anymore
            {
                _selection = list.SelectedItems.Cast<CallLogModel>();
                SetupAppBarForSelectedItems();
            }
        }

        private void SetAddressGoToDialerAndCall(String address)
        {
            Frame.Navigate(typeof(Views.Dialer), address);
        }

        private void Calls_ItemClick(object sender, ItemClickEventArgs e)
        {
            CallLogModel log = e.ClickedItem as CallLogModel;
            CallLog nativeLog = (CallLog)log.NativeLog;

            String address;
            if (log.IsIncoming)
                address = nativeLog.FromAddress.AsStringUriOnly();
            else
                address = nativeLog.ToAddress.AsStringUriOnly();

            SetAddressGoToDialerAndCall(address);
        }
    }
}