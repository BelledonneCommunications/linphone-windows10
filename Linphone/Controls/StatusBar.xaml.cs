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
using Linphone.Core;
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
            RefreshStatusIcon(LinphoneManager.Instance.LastKnownState);
        }

        /// <summary>
        /// Refresh the status icon reading the default proxy config state.
        /// </summary>
        public void RefreshStatusIcon()
        {
            RegistrationState state;
            if (LinphoneManager.Instance.LinphoneCore.GetDefaultProxyConfig() == null)
                state = RegistrationState.RegistrationNone;
            else
                state = LinphoneManager.Instance.LinphoneCore.GetDefaultProxyConfig().GetState();

            RefreshStatusIcon(state);
        }

        /// <summary>
        /// Refresh the status icon given the registration state in param.
        /// </summary>
        public void RefreshStatusIcon(RegistrationState state)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (state == RegistrationState.RegistrationOk)
                    Status.Source = new BitmapImage(new Uri("/Assets/led_connected.png", UriKind.Relative));
                else if (state == RegistrationState.RegistrationInProgress)
                    Status.Source = new BitmapImage(new Uri("/Assets/led_inprogress.png", UriKind.Relative));
                else if (state == RegistrationState.RegistrationFailed)
                    Status.Source = new BitmapImage(new Uri("/Assets/led_error.png", UriKind.Relative));
                else
                    Status.Source = new BitmapImage(new Uri("/Assets/led_disconnected.png", UriKind.Relative));
            });
        }
    }
}
