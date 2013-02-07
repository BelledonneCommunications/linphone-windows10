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

namespace Linphone.Views
{
    public partial class History : BasePage
    {
        private bool _usingSelectionAppBar = false;
        private IEnumerable<CallLog> _selection;

        public History()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TileManager tileManager = TileManager.Instance;
            tileManager.RemoveMissedCallsTile();

            List<CallLogs> callsHistory = LinphoneManager.Instance.GetCallsHistory();
            history.ItemsSource = callsHistory;
        }

        private void deleteAll_Click_1(object sender, EventArgs e)
        {
            history.ItemsSource = null;
            LinphoneManager.Instance.ClearCallLogs();
        }

        private void deleteSelection_Click_1(object sender, EventArgs e)
        {
            history.ItemsSource = LinphoneManager.Instance.RemoveCallLogs(_selection);
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
            CallLog log = ((sender as Button).Tag as CallLog);

            String address;
            if (log.IsIncoming)
                address = log.From;
            else
                address = log.To;

            SetAddressGoToDialerAndCall(address);
        }
    }
}