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
using System.Threading.Tasks;

namespace Linphone.Views
{
    /// <summary>
    /// View to use instead of Windows Phone 8 VoIP incoming call view
    /// </summary>
    public partial class IncomingCall : PhoneApplicationPage
    {
        private String _callingNumber;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public IncomingCall()
        {
            InitializeComponent();

            if (!LinphoneManager.Instance.LinphoneCore.IsVideoSupported() || !LinphoneManager.Instance.LinphoneCore.IsVideoEnabled())
            {
                AnswerVideo.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Forbid the user to back when this view is visible
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }

        /// <summary>
        /// Remove this entry from the back stack to ensure the user won't navigate to it with the back button
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (!e.IsNavigationInitiator)
            {
                //if we leave the application, we consider it as a call rejection
                LinphoneManager.Instance.EndCurrentCall();
            }

            base.OnNavigatedFrom(e);
            NavigationService.RemoveBackEntry(); //To prevent a new click on back button to start again the incoming call view (simulate a back click)
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Searches for a matching contact using the current call address or number and display information if found.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs nee)
        {
            // Create LinphoneCore if not created yet, otherwise do nothing
            LinphoneManager.Instance.InitLinphoneCore();

            base.OnNavigatedTo(nee);

            if (NavigationContext.QueryString.ContainsKey("sip"))
            {
                _callingNumber = NavigationContext.QueryString["sip"];
                if (_callingNumber.StartsWith("sip:"))
                {
                    _callingNumber = _callingNumber.Substring(4);
                }
                // While we dunno if the number matches a contact one, we consider it won't and we display the phone number as username
                Contact.Text = _callingNumber;

                if (_callingNumber != null && _callingNumber.Length > 0)
                {
                    ContactManager cm = ContactManager.Instance;
                    cm.ContactFound += cm_ContactFound;
                    cm.FindContact(_callingNumber);
                }
            }
        }

        /// <summary>
        /// Callback called when the search on a phone number for a contact has a match
        /// </summary>
        private void cm_ContactFound(object sender, ContactFoundEventArgs e)
        {
            if (e.ContactFound != null)
            {
                Contact.Text = e.ContactFound.DisplayName;
                if (e.PhoneLabel != null)
                {
                    Number.Text = e.PhoneLabel + " : " + e.PhoneNumber;
                }
                else
                {
                    Number.Text = e.PhoneNumber;
                }
            }
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            LinphoneManager.Instance.LinphoneCore.AcceptCall(LinphoneManager.Instance.LinphoneCore.GetCurrentCall());
            NavigationService.Navigate(new Uri("/Views/InCall.xaml?sip=" + _callingNumber, UriKind.RelativeOrAbsolute));
        }

        private void AnswerVideo_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Decline_Click(object sender, RoutedEventArgs e)
        {
            LinphoneManager.Instance.EndCurrentCall();
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}