using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Resources;
using Linphone.Model;

namespace Linphone.Views
{
    /// <summary>
    /// Page displaying each available codec along as a switch to let the user enabling/disabling them.
    /// </summary>
    public partial class CodecsSettings : BasePage
    {
        private CodecsSettingsManager _settings = new CodecsSettingsManager();

        /// <summary>
        /// Public constructor.
        /// </summary>
        public CodecsSettings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            _settings.Load();
            AMRNB.IsChecked = _settings.AMRNB;
            AMRWB.IsChecked = _settings.AMRWB;
            Speex16.IsChecked = _settings.Speex16;
            Speex8.IsChecked = _settings.Speex8;
            PCMU.IsChecked = _settings.PCMU;
            PCMA.IsChecked = _settings.PCMA;
            G722.IsChecked = _settings.G722;
            ILBC.IsChecked = _settings.ILBC;
            SILK16.IsChecked = _settings.SILK16;
            GSM.IsChecked = _settings.GSM;
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
            _settings.AMRNB = ToBool(AMRNB.IsChecked);
            _settings.AMRWB = ToBool(AMRWB.IsChecked);
            _settings.Speex16 = ToBool(Speex16.IsChecked);
            _settings.Speex8 = ToBool(Speex8.IsChecked);
            _settings.PCMU = ToBool(PCMU.IsChecked);
            _settings.PCMA = ToBool(PCMA.IsChecked);
            _settings.G722 = ToBool(G722.IsChecked);
            _settings.ILBC = ToBool(ILBC.IsChecked);
            _settings.SILK16 = ToBool(SILK16.IsChecked);
            _settings.GSM = ToBool(GSM.IsChecked);
            _settings.Save();

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