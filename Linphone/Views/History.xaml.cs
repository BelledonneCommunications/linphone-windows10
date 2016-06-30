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
        private IEnumerable<CallLogModel> _selection;

        public History()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LinphoneManager.Instance.Core.ResetMissedCallsCount();

            List<CallLogModel> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            if(callsHistory.Count > 0)
            {
                Calls.Visibility = Visibility.Visible;
                EmptyText.Visibility = Visibility.Collapsed;
                Calls.ItemsSource = callsHistory;
                Calls.ItemClick += Calls_ItemClick;
                MissedCalls.ItemsSource = (from log in callsHistory where (log.IsMissed) select log).ToList();
            } else
            {
                Calls.Visibility = Visibility.Collapsed;
                EmptyText.Visibility = Visibility.Visible;
            }

            SetCommandsVisibility(Calls);
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
        }

        private void calls_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ListView list = (ListView)sender;
            if (_selection.Count() == 0)
            {
                Calls.Visibility = Visibility.Collapsed;
                EmptyText.Visibility = Visibility.Visible;
            }
            else if (_selection.Count() >= 1)
            {
                Calls.Visibility = Visibility.Visible;
                EmptyText.Visibility = Visibility.Collapsed;
                _selection = list.SelectedItems.Cast<CallLogModel>();
            }
        }

        private void updateCallList()
        {
            List<CallLogModel> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
            MissedCalls.ItemsSource = (from log in callsHistory where (log.IsMissed) select log).ToList();
        }

        private void SetAddressGoToDialerAndCall(String address)
        {
            Frame.Navigate(typeof(Views.Dialer), address);
        }

        private void Calls_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Calls.SelectionMode == ListViewSelectionMode.None || Calls.SelectedItems.Count == 0)
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

        private void SelectItems_Click(object sender, RoutedEventArgs e)
        {
            Calls.SelectionMode = ListViewSelectionMode.Multiple;
            SetCommandsVisibility(Calls);
        }

        private void SetCommandsVisibility(ListView listView)
        {
            if (listView.SelectionMode == ListViewSelectionMode.Multiple || listView.SelectedItems.Count > 1)
            {
                SelectItems.Visibility = Visibility.Collapsed;
                CancelBtn.Visibility = Visibility.Visible;
                DeleteItem.Visibility = Visibility.Visible;
                SelectAll.Visibility = Visibility.Visible;
                DeselectAll.Visibility = Visibility.Collapsed;
            }
            else
            {
                SelectItems.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Collapsed;
                DeleteItem.Visibility = Visibility.Collapsed;
                SelectAll.Visibility = Visibility.Collapsed;
                DeselectAll.Visibility = Visibility.Collapsed;
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            List<CallLogModel> _selectItem = new List<CallLogModel>();
            foreach (CallLogModel item in Calls.SelectedItems)
            {
                _selectItem.Add(item);
            }
            foreach (CallLogModel item in _selectItem)
            {
                LinphoneManager.Instance.Core.RemoveCallLog((CallLog)item.NativeLog);
            }
            Calls.SelectionMode = ListViewSelectionMode.None;
            updateCallList();
            
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Calls.SelectionMode = ListViewSelectionMode.None;
            SetCommandsVisibility(Calls);
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            Calls.SelectAll();
            DeselectAll.Visibility = Visibility.Visible;
            SelectAll.Visibility = Visibility.Collapsed;
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            Calls.SelectedItems.Clear();
            DeselectAll.Visibility = Visibility.Collapsed;
            SelectAll.Visibility = Visibility.Visible;
        }
    }
}