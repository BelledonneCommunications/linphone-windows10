using Linphone.Controls;
using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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

        /// <summary>
        /// Method called when the page is displayed.
        /// Check if the uri contains a sip address, if yes, it starts a call to this address.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
                            BugCollector.ReportExceptions();
                            break;
                        case CustomMessageBoxResult.RightButton:
                            BugCollector.DeleteFile();
                            break;
                    }
                };

                reportIssueDialog.Show();
            }

            BugCollector.DeleteLinphoneLogFileIfFileTooBig();

            StatusBar = status;
            BasePage.StatusBar.RefreshStatus(LinphoneManager.Instance.LastKnownState);

            // Create LinphoneCore if not created yet, otherwise do nothing
            await LinphoneManager.Instance.InitLinphoneCore();

            BuildLocalizedApplicationBar();

            // Check for the navigation direction to avoid going to incall view when coming back from incall view
            if (NavigationContext.QueryString.ContainsKey("sip") && e.NavigationMode != NavigationMode.Back)
            {
                String sipAddressToCall = NavigationContext.QueryString["sip"];
                addressBox.Text = sipAddressToCall;
            }
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

        private void console_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Console.xaml", UriKind.RelativeOrAbsolute));
        }

        private async void BuildLocalizedApplicationBar()
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

            ApplicationSettingsManager appSettings = new ApplicationSettingsManager();
            appSettings.Load();
            StorageFile logFile = null;
            try
            {
                logFile = await ApplicationData.Current.LocalFolder.GetFileAsync(appSettings.LogOption);
            }
            catch { }
            if (appSettings.LogDestination == Core.OutputTraceDest.File && logFile != null)
            {
                ApplicationBarMenuItem appBarConsole = new ApplicationBarMenuItem(AppResources.ConsoleMenu);
                appBarConsole.Click += console_Click_1;
                ApplicationBar.MenuItems.Add(appBarConsole);
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
    }
}