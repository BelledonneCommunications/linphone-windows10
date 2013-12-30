using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Agents
{
    /// <summary>
    /// Booleans to enable/disable features and custom strings
    /// </summary>
    public class Customs
    {
        public static bool IsTunnelEnabled = false;
        public static bool AddPasswordInContactsParams = false;
        public static bool UseCustomIncomingCallView = false;
        public static string PictureUploadScriptURL = "https://linphone.org:444/upload.php";
        public static string UserAgent = "LinphoneWP8";
        public static bool EnableG729 = false;
        public static bool EnableVideo = false;

        internal static bool DeclineCallWithBusyReason = false;
    }
}
