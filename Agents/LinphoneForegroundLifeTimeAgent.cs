using Microsoft.Phone.Networking.Voip;
using System.Diagnostics;
using Linphone.BackEnd;
using System.Threading;
using Linphone.BackEnd.OutOfProcess;

namespace Linphone.Agents
{
    public class LinphoneForegroundLifeTimeAgent : VoipForegroundLifetimeAgent
    {
        public LinphoneForegroundLifeTimeAgent() : base()
        {

        }

        protected override void OnLaunched()
        {
            Debug.WriteLine("[LinphoneForegroundLifeTimeAgent] The UI has entered the foreground.");

            Globals.Instance.StartServer(RegistrationHelper.OutOfProcServerClassNames);
        }

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

            base.NotifyComplete();
        }
    }
}