using Microsoft.Phone.Networking.Voip;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
using System.Diagnostics;

namespace Linphone.Agents
{
    public class LinphoneCallInProgressAgent : VoipCallInProgressAgent
    {
        public LinphoneCallInProgressAgent() : base()
        {
        }

        /// <summary>
        /// Called when the first call has started.
        /// </summary>
        protected override void OnFirstCallStarting()
        {
            Debug.WriteLine("[LinphoneCallInProgressAgent] The first call has started.");
            AgentHost.OnAgentStarted();
        }

        /// <summary>
        /// Called when the last call has ended.
        /// </summary>
        protected override void OnCancel()
        {
            Debug.WriteLine("[LinphoneCallInProgressAgent] The last call has ended. Calling NotifyComplete");
            base.NotifyComplete();
        }
    }
}