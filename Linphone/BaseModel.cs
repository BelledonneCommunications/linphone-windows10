using Linphone.BackEnd;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Linphone
{
    public class BaseModel : CallControllerListener
    {
        public BaseModel()
        {

        }

        public BasePage Page { get; set; }

        public void NewCallStarted(string callerNumber)
        {
            this.Page.Dispatcher.BeginInvoke(() =>
            {
                this.Page.NavigationService.Navigate(new Uri("/Views/InCall.xaml?sip=" + callerNumber, UriKind.RelativeOrAbsolute));
            });
        }

        public virtual void OnNavigatedTo(NavigationEventArgs nee)
        {
            LinphoneManager.Instance.CallController.SetCallControllerListener(this);
        }

        public virtual void OnNavigatedFrom(NavigationEventArgs nee)
        {
            LinphoneManager.Instance.CallController.SetCallControllerListener(null);
        } 
    }
}
