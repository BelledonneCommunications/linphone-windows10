using Linphone.Core;
using Linphone.Model;
using Linphone.Resources;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Linphone.Controls
{
    /// <summary>
    /// User control to be displayed on each page, giving the state of the account registration.
    /// </summary>
    public partial class StatusBar : UserControl
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public StatusBar()
        {
            InitializeComponent();
            RefreshStatus(LinphoneManager.Instance.LastKnownState);
        }

        /// <summary>
        /// Refresh the status icon reading the default proxy config state.
        /// </summary>
        public void RefreshStatus()
        {
            RegistrationState state;
            if (LinphoneManager.Instance.LinphoneCore.DefaultProxyConfig == null)
                state = RegistrationState.RegistrationNone;
            else
                state = LinphoneManager.Instance.LinphoneCore.DefaultProxyConfig.State;

            RefreshStatus(state);
        }

        /// <summary>
        /// Refresh the status icon given the registration state in param.
        /// </summary>
        public void RefreshStatus(RegistrationState state)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (state == RegistrationState.RegistrationOk) {
                    StatusLed.Source = new BitmapImage(new Uri("/Assets/led_connected.png", UriKind.Relative));
                    StatusText.Text = AppResources.Registered;
                }
                else if (state == RegistrationState.RegistrationInProgress) {
                    StatusLed.Source = new BitmapImage(new Uri("/Assets/led_inprogress.png", UriKind.Relative));
                    StatusText.Text = AppResources.RegistrationInProgress;
                }
                else if (state == RegistrationState.RegistrationFailed) {
                    StatusLed.Source = new BitmapImage(new Uri("/Assets/led_error.png", UriKind.Relative));
                    StatusText.Text = AppResources.RegistrationFailed;
                }
                else {
                    StatusLed.Source = new BitmapImage(new Uri("/Assets/led_disconnected.png", UriKind.Relative));
                    StatusText.Text = AppResources.Disconnected;
                }
            });
        }
    }
}
