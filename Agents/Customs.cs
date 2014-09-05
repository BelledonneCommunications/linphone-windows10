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
        public const bool AddPasswordInUriContactsParams = false;
        public const bool UseCustomIncomingCallView = false;
        public const string PictureUploadScriptURL = "https://linphone.org:444/upload.php";
        public const string UserAgent = "LinphoneWP8";
        public const bool EnableG729 = false;

        // Do not enable these for releases !!!
#if DEBUG
        public const bool AllowTCPRemote = true;
        public const bool ShowWizardUntilAccountConfigured = false;
#else
        public const bool AllowTCPRemote = false;
        public const bool ShowWizardUntilAccountConfigured = false;
#endif

        internal const bool DeclineCallWithBusyReason = false;
    }
}
