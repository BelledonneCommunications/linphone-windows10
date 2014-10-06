using Linphone.Model;
using Linphone.Resources;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Windows.Storage;

namespace Linphone.Views
{
    /// <summary>
    /// Displays the logs collected by LinphoneCore.
    /// </summary>
    public partial class Console : BasePage
    {
        private string logs;
        /// <summary>
        /// Public constructor
        /// </summary>
        public Console()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            Browser.Navigate(new Uri("/Views/logs.html", UriKind.RelativeOrAbsolute));
        }

        private async void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            logs = await ReadLogs();
            FormatAndDisplayLogs(logs);
        }

        private void FormatAndDisplayLogs(string logs)
        {
            logs = logs.Replace("\r\n", "\n");
            string[] lines = logs.Split('\n');
            bool insertNewLine = false;
            foreach (string line in lines)
            {
                if (line.Length == 0)
                {
                    insertNewLine = false;
                    Browser.InvokeScript("append_nl");
                }
                else
                {
                    if (insertNewLine == true)
                    {
                        Browser.InvokeScript("append_nl");
                    }
                    Browser.InvokeScript("append_text", line);
                    insertNewLine = true;
                }
            }
        }

        private async Task<string> ReadLogs()
        {
            ApplicationSettingsManager appSettings = new ApplicationSettingsManager();
            appSettings.Load();

            byte[] data;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(appSettings.LogOption);

            using (Stream s = await file.OpenStreamForReadAsync())
            {
                data = new byte[s.Length];
                await s.ReadAsync(data, 0, (int)s.Length);
            }

            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        private async void refresh_Click_1(object sender, EventArgs e)
        {
            Browser.InvokeScript("clean");
            logs = await ReadLogs();
            FormatAndDisplayLogs(logs);
        }

        private void email_Click_1(object sender, EventArgs e)
        {
            LinphoneManager.Instance.LinphoneCore.UploadLogCollection();
        }

        private void delete_Click_1(object sender, EventArgs e)
        {
            BugCollector.DeleteLinphoneLogFile();
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton appBarRefresh = new ApplicationBarIconButton(new Uri("/Assets/AppBar/refresh.png", UriKind.Relative));
            appBarRefresh.Text = AppResources.Refresh;
            ApplicationBar.Buttons.Add(appBarRefresh);
            appBarRefresh.Click += refresh_Click_1;

            ApplicationBarIconButton appBarEmail = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.email.png", UriKind.Relative));
            appBarEmail.Text = AppResources.SendEmail;
            ApplicationBar.Buttons.Add(appBarEmail);
            appBarEmail.Click += email_Click_1;

            ApplicationBarIconButton appBarDelete = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
            appBarDelete.Text = AppResources.DeleteLogs;
            ApplicationBar.Buttons.Add(appBarDelete);
            appBarDelete.Click += delete_Click_1;
        }
    }
}