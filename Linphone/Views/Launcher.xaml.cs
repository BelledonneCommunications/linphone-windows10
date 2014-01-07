using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Linphone.Agents;

namespace Linphone.Views
{
    public partial class Launcher : PhoneApplicationPage
    {
        public Launcher()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Customs.ShowWizardUntilAccountConfigured)
            {
                NavigationService.Navigate(new Uri("/Views/FirstLogin.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Dialer.xaml", UriKind.RelativeOrAbsolute));
            }
            NavigationService.RemoveBackEntry(); // Prevent a back to this screen
        }    }
}