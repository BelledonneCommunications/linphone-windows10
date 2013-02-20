using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Linphone.Model
{
    /// <summary>
    /// Class used to allow the app to be automatically launched when user click on a sip: link.
    /// </summary>
    class AssociationUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            string tempUri = HttpUtility.UrlDecode(uri.ToString());

            if (tempUri.StartsWith("/Protocol?encodedLaunchUri="))
            {
                tempUri = tempUri.Replace("/Protocol?encodedLaunchUri=", "");
                if (tempUri.Contains("sip:"))
                {
                    return new Uri("/Views/Dialer.xaml?sip=" + tempUri, UriKind.RelativeOrAbsolute);
                }
            }

            return uri;
        }
    }
}
