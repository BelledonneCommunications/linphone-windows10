using Microsoft.Phone.Networking.Voip;
using Linphone.BackEnd;
using Linphone.BackEnd.OutOfProcess;
using System.Diagnostics;

namespace Linphone.Agents
{
    public class LinphoneCallInProgressAgent : VoipCallInProgressAgent
    {
        public LinphoneCallInProgressAgent() : base()
        {
            Globals.Instance.StartServer(RegistrationHelper.OutOfProcServerClassNames);
        }

        protected override void OnFirstCallStarting()
        {
            Debug.WriteLine("[LinphoneCallInProgressAgent] The first call has started.");
        }

        protected override void OnCancel()
        {
            Debug.WriteLine("[LinphoneCallInProgressAgent] The last call has ended. Calling NotifyComplete");

            base.NotifyComplete();
        }
    }
}