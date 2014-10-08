using Linphone.Core;
using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Windows.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Page displaying the video settings and the video codecs to let the user enable/disable them.
    /// </summary>
    public partial class VideoSettings : BasePage
    {
        private CallSettingsManager _callSettings = new CallSettingsManager();
        private CodecsSettingsManager _codecsSettings = new CodecsSettingsManager();
        private bool saveSettingsOnLeave = true;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public VideoSettings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            _callSettings.Load();
            VideoEnabled.IsChecked = _callSettings.VideoEnabled;
            AutomaticallyInitiateVideo.IsChecked = _callSettings.AutomaticallyInitiateVideo;
            AutomaticallyAcceptVideo.IsChecked = _callSettings.AutomaticallyAcceptVideo;
            SelfViewEnabled.IsChecked = _callSettings.SelfViewEnabled;

            _codecsSettings.Load();
            H264.IsChecked = _codecsSettings.H264;
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Create LinphoneCore if not created yet, otherwise do nothing
            LinphoneManager.Instance.InitLinphoneCore();
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
            _codecsSettings.H264 = ToBool(H264.IsChecked);
            _codecsSettings.Save();

            _callSettings.VideoEnabled = ToBool(VideoEnabled.IsChecked);
            _callSettings.AutomaticallyInitiateVideo = ToBool(AutomaticallyInitiateVideo.IsChecked);
            _callSettings.AutomaticallyAcceptVideo = ToBool(AutomaticallyAcceptVideo.IsChecked);
            _callSettings.SelfViewEnabled = ToBool(SelfViewEnabled.IsChecked);
            _callSettings.Save();
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            saveSettingsOnLeave = false;
            NavigationService.GoBack();
        }

        private bool ToBool(bool? enabled)
        {
            if (!enabled.HasValue) enabled = false;
            return (bool)enabled;
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarSave = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
            appBarSave.Text = AppResources.SaveSettings;
            ApplicationBar.Buttons.Add(appBarSave);
            appBarSave.Click += save_Click_1;

            ApplicationBarIconButton appBarCancel = new ApplicationBarIconButton(new Uri("/Assets/AppBar/cancel.png", UriKind.Relative));
            appBarCancel.Text = AppResources.CancelChanges;
            ApplicationBar.Buttons.Add(appBarCancel);
            appBarCancel.Click += cancel_Click_1;
        }
    }
}