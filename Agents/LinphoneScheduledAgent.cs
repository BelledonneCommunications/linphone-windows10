using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Scheduler;
using Linphone.BackEnd;
using Linphone.BackEnd.OutOfProcess;
using System.Diagnostics;

namespace Linphone
{
    class LinphoneScheduledAgent : ScheduledTaskAgent
    {
        // Indicates if this agent instance is handling an incoming call or not
        private bool isIncomingCallAgent;

        // Strings used in tracing
        private const string keepAliveAgentId = "KeepAliveAgent";
        private const string incomingCallAgentId = "IncomingCallAgent";

        public LinphoneScheduledAgent()
        {

        }

        protected override void OnInvoke(ScheduledTask task)
        {
            Debug.WriteLine("[LinphoneScheduledAgent] ScheduledAgentImpl has been invoked with argument of type {0}.", task.GetType());

            Globals.Instance.StartServer(RegistrationHelper.OutOfProcServerClassNames);

            VoipHttpIncomingCallTask incomingCallTask = task as VoipHttpIncomingCallTask;
            if (incomingCallTask != null)
            {
                isIncomingCallAgent = true;
            }
            else
            {
                VoipKeepAliveTask keepAliveTask = task as VoipKeepAliveTask;
                if (keepAliveTask != null)
                {
                    isIncomingCallAgent = false;
                }
            }
        }

        private void OnIncomingCallDialogDismissed()
        {
            Debug.WriteLine("[IncomingCallAgent] Incoming call processing is now complete.");

            base.NotifyComplete();
        }

        protected override void OnCancel()
        {
            Debug.WriteLine("[{0}] Cancel requested.", this.isIncomingCallAgent ? LinphoneScheduledAgent.incomingCallAgentId : LinphoneScheduledAgent.keepAliveAgentId);

            base.NotifyComplete();
        }
    }
}
