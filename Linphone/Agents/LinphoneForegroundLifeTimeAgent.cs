using Microsoft.Phone.Networking.Voip;
using System.Diagnostics;

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
        }

        protected override void OnCancel()
        {
            Debug.WriteLine("[LinphoneForegroundLifeTimeAgent] The UI is leaving the foreground");

            base.NotifyComplete();
        }
    }
}