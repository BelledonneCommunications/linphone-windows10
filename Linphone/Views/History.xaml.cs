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
using Linphone.Resources;
using Linphone.Agents;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

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

            TileManager tileManager = TileManager.Instance;
            tileManager.RemoveMissedCallsTile();
            LinphoneManager.Instance.LinphoneCore.ResetMissedCallsCount();

            List<CallLog> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
        }

        private void deleteAll_Click_1(object sender, EventArgs e)
        {
            LinphoneManager.Instance.ClearCallLogs();

            List<CallLog> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;
        }

        private void deleteSelection_Click_1(object sender, EventArgs e)
        {
            LinphoneManager.Instance.RemoveCallLogs(_selection);

            List<CallLog> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            Calls.ItemsSource = callsHistory;

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
            NavigationService.Navigate(new Uri("/Views/Dialer.xaml?sip=" + address, UriKind.RelativeOrAbsolute));
        }

        private void callLog_Click_1(object sender, RoutedEventArgs e)
        {
            CallLog log = ((sender as StackPanel).Tag as CallLog);

            String address;
            if (log.IsIncoming)
                address = log.From;
            else
                address = log.To;

            SetAddressGoToDialerAndCall(address);
        }
    }
}