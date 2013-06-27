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

namespace Linphone.Views
{
    /// <summary>
    /// Displays on full screen received or sent pictures
    /// </summary>
    public partial class FullScreenPicture : PhoneApplicationPage
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        public FullScreenPicture()
        {
            InitializeComponent();
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
                string uri = NavigationContext.QueryString["uri"];
                BitmapImage image = Utils.ReadImageFromIsolatedStorage(uri);
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
    }
}