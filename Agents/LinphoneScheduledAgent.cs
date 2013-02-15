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

                String message = System.Text.Encoding.UTF8.GetString(incomingCallTask.MessageBody, 0, incomingCallTask.MessageBody.Length);
                Debug.WriteLine("[LinphoneScheduledAgent] Received VoIP Incoming Call task with body {0}", message);

                XDocument doc = XDocument.Parse(message);
                String callerName = "", callerNumber = "";
                var incomingCallPN = from prop in doc.Descendants("IncomingCall") select prop.Element("Name").Value;
                callerName = incomingCallPN.First().ToString();
                incomingCallPN = from prop in doc.Descendants("IncomingCall") select prop.Element("Number").Value;
                callerNumber = incomingCallPN.First().ToString();

                Debug.WriteLine("[{0}] Incoming call from caller {1}, number {2}", "KeepAliveAgent", callerName, callerNumber);
                LinphoneCall call = new LinphoneCall(callerName, callerNumber);
                Globals.Instance.LinphoneCore.IncomingCall = call;

                base.NotifyComplete();
            }
            else
            {
                VoipKeepAliveTask keepAliveTask = task as VoipKeepAliveTask;
                if (keepAliveTask != null)
                {
                    this.isIncomingCallAgent = false;
                    Debug.WriteLine("[LinphoneSchedulerAgent] Calling NotifyComplete");

                    base.NotifyComplete();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown scheduled task type {0}", task.GetType()));
                }
            }
        }

        protected override void OnCancel()
        {
            Debug.WriteLine("[{0}] Cancel requested.", this.isIncomingCallAgent ? "IncomingCallAgent" : "KeepAliveAgent");

            base.NotifyComplete();
        }
    }
}
