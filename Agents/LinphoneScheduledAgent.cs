using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Scheduler;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Phone.Shell;
using Windows.Phone.Networking.Voip;

namespace Linphone.Agents
{
    public class LinphoneScheduledAgent : ScheduledTaskAgent
    {
        // Indicates if this agent instance is handling an incoming call or not
        private bool isIncomingCallAgent;

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
                this.isIncomingCallAgent = true;
                Debug.WriteLine("[IncomingCallAgent] Received VoIP Incoming Call task");

                TileManager.Instance.UpdateTileWithMissedCalls(Globals.Instance.LinphoneCore.GetMissedCallsCount());
            }
            else
            {
                VoipKeepAliveTask keepAliveTask = task as VoipKeepAliveTask;
                Debug.WriteLine("[KeepAliveAgent] Keep Alive");
                if (keepAliveTask != null)
                {
                    this.isIncomingCallAgent = false;
                    base.NotifyComplete();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown scheduled task type {0}", task.GetType()));
                }
            }
        }

        // This method is called when the incoming call processing is complete 
        private void OnIncomingCallDialogDismissed()
        {
            Debug.WriteLine("[IncomingCallAgent] Incoming call processing is now complete.");
            base.NotifyComplete();
        } 

        protected override void OnCancel()
        {
            Debug.WriteLine("[{0}] Cancel requested.", this.isIncomingCallAgent ? "IncomingCallAgent" : "KeepAliveAgent");

            base.NotifyComplete();
        }
    }
}
