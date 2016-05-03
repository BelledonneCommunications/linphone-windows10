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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using BelledonneCommunications.Linphone.Native;
using Linphone.Model;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Page displaying the audio settings and the audio codecs to let the user enable/disable them.
    /// </summary>
    public partial class AudioSettings : Page
    {
        private CodecsSettingsManager _settings = new CodecsSettingsManager();
        private bool saveSettingsOnLeave = true;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public AudioSettings()
        {
            this.InitializeComponent();

            _settings.Load();
            Speex16.IsOn = _settings.Speex16;
            Speex8.IsOn = _settings.Speex8;
            PCMU.IsOn = _settings.PCMU;
            PCMA.IsOn = _settings.PCMA;
            G722.IsOn = _settings.G722;
            //G729.IsEnabled = _settings.G729 && Customs.EnableG729;
            //G729.IsEnabled = Customs.EnableG729;
            ILBC.IsOn = _settings.ILBC;
            SILK16.IsOn = _settings.SILK16;
            GSM.IsOn = _settings.GSM;
            OPUS.IsOn = _settings.OPUS;
            ISAC.IsOn = _settings.Isac;
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //LinphoneManager.Instance.ECListener = this;
        }

        /// <summary>
        /// Method called when the user is navigation away from this page
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (saveSettingsOnLeave)
            {
                Save();
            }
            base.OnNavigatingFrom(e);
        }

        private void Save()
        {
            _settings.Speex16 = ToBool(Speex16.IsOn);
            _settings.Speex8 = ToBool(Speex8.IsOn);
            _settings.PCMU = ToBool(PCMU.IsOn);
            _settings.PCMA = ToBool(PCMA.IsOn);
            _settings.G722 = ToBool(G722.IsOn);
            //_settings.G729 = ToBool(G729.IsEnabled) && Customs.EnableG729;
            _settings.ILBC = ToBool(ILBC.IsOn);
            _settings.SILK16 = ToBool(SILK16.IsOn);
            _settings.GSM = ToBool(GSM.IsOn);
            _settings.OPUS = ToBool(OPUS.IsOn);
            _settings.Isac = ToBool(ISAC.IsOn);
            _settings.Save();
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            saveSettingsOnLeave = false;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private bool ToBool(bool? enabled)
        {
            if (!enabled.HasValue) enabled = false;
            return (bool)enabled;
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void ECCalibratorButton_Click_1(object sender, RoutedEventArgs e)
        {
            //ECCalibratorButton.IsEnabled = false;
            //ECCalibratorStatusButton.Content = AppResources.ECCalibrationInProgress;
            //LinphoneManager.Instance.LinphoneCore.StartEchoCalibration();
        }

        /// <summary>
        /// Actualises the echo calibrator listener when the pages changes.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs nea)
        {
            //LinphoneManager.Instance.ECListener = null;
        }

        /// <summary>
        /// Called when the echo calibrator returns its status.
        /// </summary>
        /// <param name="status">The status of the echo canceller calibrator</param>
        /// <param name="delayMs">The echo delay in milliseconds if the status is EcCalibratorStatus.Done</param>
        public void ECStatusNotified(EcCalibratorStatus status, int delayMs)
        {
            /*BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                if (status == EcCalibratorStatus.Done)
                {
                    ECCalibratorStatusButton.Content = String.Format(AppResources.ECCalibrationDone, delayMs);
                }
                else if (status == EcCalibratorStatus.DoneNoEcho)
                {
                    ECCalibratorStatusButton.Content = AppResources.ECCalibrationDoneNoEcho;
                }
                else if (status == EcCalibratorStatus.Failed)
                {
                    ECCalibratorStatusButton.Content = AppResources.ECCalibrationFailed;
                }
                else if (status == EcCalibratorStatus.InProgress)
                {
                    ECCalibratorStatusButton.Content = AppResources.ECCalibrationInProgress;
                }
                ECCalibratorButton.IsEnabled = true;
            });*/
        }
    }
}