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
using Windows.Storage;

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

                Globals.Instance.CallController.IncomingCallViewDismissed = OnIncomingCallDialogDismissed;
                CreateLinphoneCore();
            }
            else
            {
                VoipKeepAliveTask keepAliveTask = task as VoipKeepAliveTask;
                if (keepAliveTask != null)
                {
                    this.isIncomingCallAgent = false;
                    Debug.WriteLine("[KeepAliveAgent] Keep Alive");

                    //CreateLinphoneCore();
                    //Globals.Instance.LinphoneCore.RefreshRegisters();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown scheduled task type {0}", task.GetType()));
                }
            }
        }

        private void CreateLinphoneCore()
        {
            // Initiate incoming call processing by creating the Linphone Core
            Globals.Instance.LinphoneCoreFactory.CreateLinphoneCore(null, ApplicationData.Current.LocalFolder.Path + "\\linphonerc", "Assets/linphonerc-factory");

            //Globals.Instance.BackgroundModeLogger.Configure(true, OutputTraceDest.TCPRemote, "192.168.0.217:38954");
            //Globals.Instance.LinphoneCoreFactory.OutputTraceListener = Globals.Instance.BackgroundModeLogger;

            //if (Globals.Instance.LinphoneCore.GetDefaultProxyConfig() != null)
            //{
            //    string host, token;
            //    host = ((App)App.Current).PushChannelUri.Host;
            //    token = ((App)App.Current).PushChannelUri.AbsolutePath;
            //    Globals.Instance.LinphoneCore.GetDefaultProxyConfig().SetContactParameters("app-id=" + host + ";pn-type=wp;pn-tok=" + token + ";pn-msg-str=IM_MSG;pn-call-str=IC_MSG;pn-call-snd=ring.caf;pn-msg-snd=msg.caf");
            //}
            Globals.Instance.LinphoneCore.SetNetworkReachable(true);
        }

        // This method is called when the incoming call processing is complete 
        private void OnIncomingCallDialogDismissed()
        {
            Debug.WriteLine("[IncomingCallAgent] Incoming call processing is now complete.");
            Globals.Instance.CallController.IncomingCallViewDismissed = null;
            base.NotifyComplete();
        } 

        protected override void OnCancel()
        {
            Debug.WriteLine("[{0}] Cancel requested.", this.isIncomingCallAgent ? "IncomingCallAgent" : "KeepAliveAgent");

            base.NotifyComplete();
        }
    }
}
