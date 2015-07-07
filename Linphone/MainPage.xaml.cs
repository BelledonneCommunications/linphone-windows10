using Windows.UI.Xaml.Controls;
using Linphone.Native;
using Linphone.Model;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Linphone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LinphoneManager.Instance.Dispatcher = Dispatcher;
            LinphoneManager.Instance.Core.IterateEnabled = true;
        }

        private void CallButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string contact = ContactTextBox.Text;
            if (contact.Length > 0)
            {
                if (!contact.StartsWith("sip:"))
                {
                    contact = string.Format("sip:{0}", contact);
                }
                Core core = LinphoneManager.Instance.Core;
                Address address = core.CreateAddress(contact);
                if (address != null)
                {
                    core.InviteAddress(address);
                }
            }
        }

        private void RegisterButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if ((UsernameTextBox.Text.Length > 0) && (ServerTextBox.Text.Length > 0))
            {
                Core core = LinphoneManager.Instance.Core;
                ProxyConfig proxy = core.CreateProxyConfig();
                AuthInfo authInfo = core.CreateAuthInfo(UsernameTextBox.Text, "", PasswordBox.Password, "", "", "");
                core.AddAuthInfo(authInfo);
                proxy.Identity = string.Format("sip:{0}@{1}", UsernameTextBox.Text, ServerTextBox.Text);
                proxy.ServerAddr = ServerTextBox.Text;
                proxy.RegisterEnabled = true;
                core.AddProxyConfig(proxy);
                core.DefaultProxyConfig = proxy;
            }
        }
    }
}
