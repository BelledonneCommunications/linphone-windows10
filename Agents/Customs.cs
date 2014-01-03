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
        public const bool IsTunnelEnabled = false;
        public const bool AddPasswordInContactsParams = false;
        public const bool UseCustomIncomingCallView = false;
        public const string PictureUploadScriptURL = "https://linphone.org:444/upload.php";
        public const string UserAgent = "LinphoneWP8";
        public const bool EnableG729 = false;
        public const bool EnableVideo = false;
#if DEBUG
        public const bool AllowTCPRemote = true; // Do not enable this for releases !!!
#else
        public const bool AllowTCPRemote = false; // Do not enable this for releases !!!
#endif

        internal const bool DeclineCallWithBusyReason = false;
    }
}
