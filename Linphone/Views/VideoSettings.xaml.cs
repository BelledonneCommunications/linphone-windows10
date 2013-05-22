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

            _codecsSettings.Load();
            H264.IsChecked = _codecsSettings.H264;
        }

        private void cancel_Click_1(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private bool ToBool(bool? enabled)
        {
            if (!enabled.HasValue) enabled = false;
            return (bool)enabled;
        }

        private void save_Click_1(object sender, EventArgs e)
        {
            _codecsSettings.H264 = ToBool(H264.IsChecked);
            _codecsSettings.Save();

            _callSettings.VideoEnabled = ToBool(VideoEnabled.IsChecked);
            _callSettings.AutomaticallyInitiateVideo = ToBool(AutomaticallyInitiateVideo.IsChecked);
            _callSettings.AutomaticallyAcceptVideo = ToBool(AutomaticallyAcceptVideo.IsChecked);
            _callSettings.Save();

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