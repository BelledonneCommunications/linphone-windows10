/*
Dialer.xaml.cs
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

using Linphone.Controls;
using Linphone.Core;
using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Xml.Serialization;
using Windows.Storage;

namespace Linphone
{
    /// <summary>
    /// Home page for the application, displays a numpad and links to Settings/History/Contacts pages.
    /// </summary>
    public partial class Dialer : BasePage, AddressBoxFocused
    {
        private LocalizedStrings _appStrings = new LocalizedStrings();

        /// <summary>
        /// Public constructor.
        /// </summary>
        public Dialer()
        {
            InitializeComponent();
            addressBox.FocusListener = this;

            ContactManager contactManager = ContactManager.Instance; //Force creation and init of ContactManager
        }

        private void LogUploadProgressIndication(int offset, int total)
        {
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                BugReportUploadProgressBar.Maximum = total;
                if (offset <= total)
                {
                    BugReportUploadProgressBar.Value = offset;
                }
            });
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Check if the uri contains a sip address, if yes, it starts a call to this address.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                if (BugCollector.HasExceptionToReport())
                {
                    // Allow to report exceptions before the creation of the core in case the problem is in there
                    CustomMessageBox reportIssueDialog = new CustomMessageBox()
                    {
                        Caption = AppResources.ReportCrashDialogCaption,
                        Message = AppResources.ReportCrashDialogMessage,
                        LeftButtonContent = AppResources.ReportCrash,
                        RightButtonContent = AppResources.Close
                    };

                    reportIssueDialog.Dismissed += (s, ev) =>
                    {
                        switch (ev.Result)
                        {
                            case CustomMessageBoxResult.LeftButton:
                                BugReportUploadProgressBar.Minimum = 0;
                                BugReportUploadProgressBar.Maximum = 100;
                                BugReportUploadPopup.Visibility = Visibility.Visible;
                                LinphoneManager.Instance.LogUploadProgressIndicationEH += LogUploadProgressIndication;
                                LinphoneManager.Instance.LinphoneCore.UploadLogCollection();
                                break;
                            case CustomMessageBoxResult.RightButton:
                                BugCollector.DeleteFile();
                                break;
                        }
                    };

                    reportIssueDialog.Show();
                }
                else
                {
                    BugReportUploadPopup.Visibility = Visibility.Collapsed;
                }
            }

            StatusBar = status;
            BasePage.StatusBar.RefreshStatus(LinphoneManager.Instance.LastKnownState);

            BuildLocalizedApplicationBar();

            // Check for the navigation direction to avoid going to incall view when coming back from incall view
            if (NavigationContext.QueryString.ContainsKey("sip") && e.NavigationMode != NavigationMode.Back)
            {
                String sipAddressToCall = NavigationContext.QueryString["sip"];
                addressBox.Text = sipAddressToCall;
            }

            // Navigate to the current call (if existing) when using the app launcher to restart the app while call is in background
            if (LinphoneManager.Instance.LinphoneCore.CallsNb > 0)
            {
                LinphoneCall call = LinphoneManager.Instance.LinphoneCore.CurrentCall;
                String uri = call.RemoteAddress.AsStringUriOnly();
                NavigationService.Navigate(new Uri("/Views/InCall.xaml?sip=" + Utils.ReplacePlusInUri(uri), UriKind.RelativeOrAbsolute));
            }
        }

        /// <summary>
        /// Method called when the page is hidden.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);
            LinphoneManager.Instance.LogUploadProgressIndicationEH -= LogUploadProgressIndication;
            BugReportUploadPopup.Visibility = Visibility.Collapsed;
        }

        private void call_Click_1(object sender, EventArgs e)
        {
            if (addressBox.Text.Length > 0)
            {
                LinphoneManager.Instance.NewOutgoingCall(addressBox.Text);
            }
            else
            {
                string lastDialedNumber = LinphoneManager.Instance.GetLastCalledNumber();
                addressBox.Text = lastDialedNumber == null ? "" : lastDialedNumber;
            }
        }

        private void Numpad_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            String tag = button.Tag as String;
            LinphoneManager.Instance.LinphoneCore.PlayDTMF(Convert.ToChar(tag), 1000);

            addressBox.Text += tag;
        }

        private void zero_Hold_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (addressBox.Text.Length > 0)
                addressBox.Text = addressBox.Text.Substring(0, addressBox.Text.Length - 1); ;
            addressBox.Text += "+";
        }

        private void chat_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Chats.xaml", UriKind.RelativeOrAbsolute));
        }

        private void history_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/History.xaml", UriKind.RelativeOrAbsolute));
        }

        private void contacts_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Contacts.xaml", UriKind.RelativeOrAbsolute));
        }

        private void settings_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void about_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/About.xaml", UriKind.RelativeOrAbsolute));
        }

        private void disconnect_Click_1(object sender, EventArgs e)
        {
            EnableRegister(false);
            ApplicationBar.MenuItems.Clear();
            BuildLocalizedApplicationBar();
        }

        private void connect_Click_1(object sender, EventArgs e)
        {
            EnableRegister(true);
            ApplicationBar.MenuItems.Clear();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarHistory = new ApplicationBarIconButton(new Uri("/Assets/AppBar/time.png", UriKind.Relative));
            appBarHistory.Text = AppResources.HistoryMenu;
            ApplicationBar.Buttons.Add(appBarHistory);
            appBarHistory.Click += history_Click_1;

            ApplicationBarIconButton appBarContacts = new ApplicationBarIconButton(new Uri("/Assets/AppBar/people.contacts.png", UriKind.Relative));
            appBarContacts.Text = AppResources.ContactsMenu;
            ApplicationBar.Buttons.Add(appBarContacts);
            appBarContacts.Click += contacts_Click_1;

            ApplicationBarIconButton appBarChat = new ApplicationBarIconButton(new Uri("/Assets/AppBar/chat.png", UriKind.Relative));
            appBarChat.Text = AppResources.ChatMenu;
            ApplicationBar.Buttons.Add(appBarChat);
            appBarChat.Click += chat_Click_1;

            ApplicationBarIconButton appBarSettings = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.settings.png", UriKind.Relative));
            appBarSettings.Text = AppResources.SettingsMenu;
            ApplicationBar.Buttons.Add(appBarSettings);
            appBarSettings.Click += settings_Click_1;

            ApplicationBarMenuItem appBarAbout = new ApplicationBarMenuItem(AppResources.AboutMenu);
            appBarAbout.Click += about_Click_1;
            ApplicationBar.MenuItems.Add(appBarAbout);

            if (IsAccountConfigured())
            {
                if (IsRegisterEnabled())
                {
                    ApplicationBarMenuItem appBarDisconnect = new ApplicationBarMenuItem(AppResources.DisconnectMenu);
                    appBarDisconnect.Click += disconnect_Click_1;
                    ApplicationBar.MenuItems.Add(appBarDisconnect);
                }
                else
                {
                    ApplicationBarMenuItem appBarConnect = new ApplicationBarMenuItem(AppResources.ConnectMenu);
                    appBarConnect.Click += connect_Click_1;
                    ApplicationBar.MenuItems.Add(appBarConnect);
                }
            }
        }

        private void Title_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (LinphoneManager.Instance.isLinphoneRunning)
                LinphoneManager.Instance.LinphoneCore.RefreshRegisters();
        }

        /// <summary>
        /// Called when the addressbox get focused.
        /// </summary>
        public void Focused()
        {
            numpad.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Called when the addressbox lost its focus.
        /// </summary>
        public void UnFocused()
        {
            numpad.Visibility = Visibility.Visible;
        }

        private bool IsRegisterEnabled()
        {
            LinphoneCore lc = LinphoneManager.Instance.LinphoneCore;
            LinphoneProxyConfig cfg = lc.DefaultProxyConfig;
            if (cfg != null)
            {
                return cfg.RegisterEnabled;
            }
            return true;
        }

        private bool IsAccountConfigured()
        {
            LinphoneCore lc = LinphoneManager.Instance.LinphoneCore;
            LinphoneProxyConfig cfg = lc.DefaultProxyConfig;
            return cfg != null;
        }

        private void EnableRegister(bool enable)
        {
            LinphoneCore lc = LinphoneManager.Instance.LinphoneCore;
            LinphoneProxyConfig cfg = lc.DefaultProxyConfig;
            if (cfg != null)
            {
                cfg.Edit();
                cfg.RegisterEnabled = enable;
                cfg.Done();
            }
        }
    }
}