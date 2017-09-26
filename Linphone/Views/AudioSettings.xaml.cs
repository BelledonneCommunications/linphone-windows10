/*
AudioSettings.xaml.cs
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Linphone;
using Linphone.Model;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views {
    /// <summary>
    /// Page displaying the audio settings and the audio codecs to let the user enable/disable them.
    /// </summary>
    public partial class AudioSettings : Page, EchoCalibratorListener {
        private CodecsSettingsManager _settings = new CodecsSettingsManager();
        private bool saveSettingsOnLeave = true;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public AudioSettings() {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += back_Click;

            _settings.Load();
            Speex16.IsOn = _settings.Speex16;
            Speex8.IsOn = _settings.Speex8;
            PCMU.IsOn = _settings.PCMU;
            PCMA.IsOn = _settings.PCMA;
            //G722.IsOn = _settings.G722;
            G729.IsOn = _settings.G729;
            //G729.IsEnabled = Customs.EnableG729;
            GSM.IsOn = _settings.GSM;
            OPUS.IsOn = _settings.OPUS;
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            LinphoneManager.Instance.ECListener = this;
        }

        /// <summary>
        /// Method called when the user is navigation away from this page
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            if (saveSettingsOnLeave) {
                Save();
            }
            base.OnNavigatingFrom(e);
        }

        private void Save() {
            _settings.Speex16 = ToBool(Speex16.IsOn);
            _settings.Speex8 = ToBool(Speex8.IsOn);
            _settings.PCMU = ToBool(PCMU.IsOn);
            _settings.PCMA = ToBool(PCMA.IsOn);
            //_settings.G722 = ToBool(G722.IsOn);
            _settings.G729 = ToBool(G729.IsOn);
            _settings.GSM = ToBool(GSM.IsOn);
            _settings.OPUS = ToBool(OPUS.IsOn);
            _settings.Save();
        }

        private void cancel_Click_1(object sender, EventArgs e) {
            saveSettingsOnLeave = false;
            if (Frame.CanGoBack) {
                Frame.GoBack();
            }
        }

        private bool ToBool(bool? enabled) {
            if (!enabled.HasValue)
                enabled = false;
            return (bool)enabled;
        }

        private void save_Click_1(object sender, EventArgs e) {
            if (Frame.CanGoBack) {
                Frame.GoBack();
            }
        }

        private void ECCalibratorButton_Click_1(object sender, RoutedEventArgs e) {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            ECCalibratorButton.IsEnabled = false;
            ECCalibratorStatusButton.Text = loader.GetString("ECCalibrationInProgress");
            //LinphoneManager.Instance.Core.StartEchoCalibration(); TODO
        }

        /// <summary>
        /// Actualises the echo calibrator listener when the pages changes.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nea) {
            LinphoneManager.Instance.ECListener = null;
        }

        /// <summary>
        /// Called when the echo calibrator returns its status.
        /// </summary>
        /// <param name="status">The status of the echo canceller calibrator</param>
        /// <param name="delayMs">The echo delay in milliseconds if the status is EcCalibratorStatus.Done</param>
        public void ECStatusNotified(EcCalibratorStatus status, int delayMs) {

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            if (status == EcCalibratorStatus.Done) {
                ECCalibratorStatusButton.Text = String.Format(loader.GetString("ECCalibrationDone"), delayMs);
            } else if (status == EcCalibratorStatus.DoneNoEcho) {
                ECCalibratorStatusButton.Text = loader.GetString("ECCalibrationDoneNoEcho");
            } else if (status == EcCalibratorStatus.Failed) {
                ECCalibratorStatusButton.Text = loader.GetString("ECCalibrationFailed");
            } else if (status == EcCalibratorStatus.InProgress) {
                ECCalibratorStatusButton.Text = loader.GetString("ECCalibrationInProgress");
            }
            ECCalibratorButton.IsEnabled = true;
        }

        private void back_Click(object sender, BackRequestedEventArgs e) {
            if (Frame.CanGoBack) {
                e.Handled = true;
                Frame.GoBack();
            }
        }

    }
}