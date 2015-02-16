/*
LinphoneForegroundLifeTimeAgent.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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

            // Changes the decline reason if needed.
            if (Customs.DeclineCallWithBusyReason)
            {
                Globals.Instance.CallController.DeclineReason = Reason.LinphoneReasonBusy;
            }
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