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
using System.Windows.Media.Imaging;
using Linphone.Resources;

namespace Linphone.Views
{
    /// <summary>
    /// Displays on full screen received or sent pictures
    /// </summary>
    public partial class FullScreenPicture : PhoneApplicationPage
    {
        private String _fileName;

        /// <summary>
        /// Public constructor
        /// </summary>
        public FullScreenPicture()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Check if the uri contains a sip address, if yes, it displays the matching chat history.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("uri"))
            {
                _fileName = NavigationContext.QueryString["uri"];
                BitmapImage image = Utils.ReadImageFromIsolatedStorage(_fileName);
                if (image != null)
                {
                    Image.Source = image;
                }
                else
                {
                    //TODO ?
                }
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarSave = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
            appBarSave.Text = AppResources.ContextMenuSave;
            ApplicationBar.Buttons.Add(appBarSave);
            appBarSave.Click += (sender, e) =>
            {
                bool result = Utils.SavePictureInMediaLibrary(_fileName);
                MessageBox.Show(result ? AppResources.FileSavingSuccess : AppResources.FileSavingFailure, AppResources.FileSaving, MessageBoxButton.OK);
            };
        }
    }
}