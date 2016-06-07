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

using Windows.UI.Xaml.Controls;
using Linphone.Model;
using Windows.UI.Xaml.Navigation;
using System;
using BelledonneCommunications.Linphone.Native;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using System.ComponentModel;

namespace Linphone.Views
{

    public sealed partial class Dialer : Page, INotifyPropertyChanged
    {

        public Dialer()
        {
            this.InitializeComponent();
            DataContext = this;
            ContactsManager contactsManager = ContactsManager.Instance;
        }

        private int unreadMessageCount;
        public int UnreadMessageCount
        {
            get
            {
                return unreadMessageCount;
            }

            set
            {
                unreadMessageCount = value;
                if(unreadMessageCount > 0)
                {
                    unreadMassageText.Visibility = Visibility.Visible; 
                } else
                {
                    unreadMassageText.Visibility = Visibility.Collapsed;
                }

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("UnreadMessageCount"));
                }
            }
        }

        private int missedCallCount;
        public int MissedCallCount
        {
            get
            {
                return missedCallCount;
            }

            set
            {
                missedCallCount = value;
                if (missedCallCount > 0)
                {
                    MissedCallText.Visibility = Visibility.Visible;
                }
                else
                {
                    MissedCallText.Visibility = Visibility.Collapsed;
                }

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("MissedCallCount"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void LogUploadProgressIndication(int offset, int total)
        {
           /* base.UIDispatcher.BeginInvoke(() =>
            {
                BugReportUploadProgressBar.Maximum = total;
                if (offset <= total)
                {
                    BugReportUploadProgressBar.Value = offset;
                }
            });*/
        }

        private void RegistrationChanged(object sender, EventArgs e)
        {
            status.RefreshStatus();
        }

        private void MessageReceived(object sender, EventArgs e)
        {

            UnreadMessageCount = LinphoneManager.Instance.GetUnreadMessageCount();
        }

        private void CallStateChanged(object sender, EventArgs e)
        {
            MissedCallCount = LinphoneManager.Instance.Core.MissedCallsCount;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LinphoneManager.Instance.CoreDispatcher = Dispatcher;
            LinphoneManager.Instance.RegistrationChanged += RegistrationChanged;
            LinphoneManager.Instance.MessageReceived += MessageReceived;
            LinphoneManager.Instance.CallStateChanged += CallStateChanged;
            status.RefreshStatus();
            /*    if (e.NavigationMode == NavigationMode.New)
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
                }*/


            if (e.Parameter is String && (e.Parameter as String)?.Length > 0 && e.NavigationMode != NavigationMode.Back)
            {
                try {
                    Address address = LinphoneManager.Instance.Core.InterpretURL(e.Parameter as String);
                    String sipAddressToCall = address.AsStringUriOnly();
                    addressBox.Text = sipAddressToCall;
                } catch(Exception exception)
                { }
            }
        
            if (LinphoneManager.Instance.Core.CallsNb > 0)
            {
                Call call = LinphoneManager.Instance.Core.CurrentCall;
                String uri = call.RemoteAddress.AsStringUriOnly();
                Frame.Navigate(typeof(Views.InCall), uri);
            }

            if(LinphoneManager.Instance.GetUnreadMessageCount() > 0)
            {
                UnreadMessageCount = LinphoneManager.Instance.GetUnreadMessageCount();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);
           // LinphoneManager.Instance.LogUploadProgressIndicationEH -= LogUploadProgressIndication;
           // BugReportUploadPopup.Visibility = Visibility.Collapsed;
        }

        private void call_Click(object sender, RoutedEventArgs e)
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

        private void numpad_Click(object sender,RoutedEventArgs e)
        {
            Button button = sender as Button;
            String tag = button.Tag as String;
            LinphoneManager.Instance.Core.PlayDtmf(Convert.ToChar(tag), 1000);

            addressBox.Text += tag;
        }

        private void VoicemailClick(object sender, RoutedEventArgs e)
        {

        }

        private void zero_Hold(object sender, RoutedEventArgs e)
        {
            if (addressBox.Text.Length > 0)
                addressBox.Text = addressBox.Text.Substring(0, addressBox.Text.Length - 1);
            addressBox.Text += "+";
        }

        private void chat_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Chats), null);
        }

        private void history_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.History), null);
        }

        private void contacts_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ContactList), null);
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Settings), null);
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.About), null);
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            EnableRegister(false);
        }

        private void connect_Click(object sender, EventArgs e)
        {
            EnableRegister(true);
        }

        private void EnableRegister(bool enable)
        {
            Core lc = LinphoneManager.Instance.Core;
            ProxyConfig cfg = lc.DefaultProxyConfig;
            if (cfg != null)
            {
                cfg.Edit();
                cfg.IsRegisterEnabled = enable;
                cfg.Done();
            }
        }

        private void status_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LinphoneManager.Instance.Core.RefreshRegisters();
        }
    }
}