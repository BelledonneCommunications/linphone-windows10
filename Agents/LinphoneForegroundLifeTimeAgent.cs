using Microsoft.Phone.Networking.Voip;
using System.Diagnostics;
using Linphone.Core;
using System.Threading;
using Linphone.Core.OutOfProcess;

namespace Linphone.Agents
{
    public class LinphoneForegroundLifeTimeAgent : VoipForegroundLifetimeAgent
    {
        public LinphoneForegroundLifeTimeAgent() : base()
        {

        }

        /// <summary>
        /// Called when the app is in foreground (when it starts or when it's resumed)
        /// </summary>
        protected override void OnLaunched()
        {
            Debug.WriteLine("[LinphoneForegroundLifeTimeAgent] The UI has entered the foreground.");
            AgentHost.OnAgentStarted();
            //Force the callController to be initialized now to avoid creating it at the last moment (workaroud outgoing call crash after app started)
            Globals.Instance.CallController.IncomingCallViewDismissed = null;
        }

        /// <summary>
        /// Called when the app is in background
        /// </summary>
        protected override void OnCancel()
        {
            Debug.WriteLine("[LinphoneForegroundLifeTimeAgent] The UI is leaving the foreground");

            uint currentProcessId = Globals.GetCurrentProcessId();
            string backgroundProcessReadyEventName = Globals.GetBackgroundProcessReadyEventName(currentProcessId);
            using (EventWaitHandle backgroundProcessReadyEvent = new EventWaitHandle(initialState: false, mode: EventResetMode.ManualReset, name: backgroundProcessReadyEventName))
            {
                backgroundProcessReadyEvent.WaitOne();
                Debug.WriteLine("[LinphoneForegroundLifeTimeAgent] Background process {0} is ready", currentProcessId);
            }

            Debug.WriteLine("[LinphoneForegroundLifeTimeAgent] NotifyComplete"); 
            base.NotifyComplete();
        }
    }
}