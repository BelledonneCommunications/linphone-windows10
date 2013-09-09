using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    /// <summary>
    /// Booleans to enable/disable features and custom strings
    /// </summary>
    public class Customs
    {
        internal static bool IsTunnelEnabled = false;
        internal static bool AddPasswordInContactsParams = false;
        internal static bool UseCustomIncomingCallView = false;
        internal static string PictureUploadScriptURL = "https://linphone.org:444/upload.php";
        internal static string UserAgent = "LinphoneWP8";
        internal static bool EnableG729 = false;
    }
}
