using Linphone.Core;
using Linphone.Core.OutOfProcess;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Scheduler;
using System;
using System.Diagnostics;
using Windows.Storage;

namespace Linphone.Agents
{
    public class LinphoneScheduledAgent : ScheduledTaskAgent, LinphoneCoreListener
    {
        // Indicates if this agent instance is handling an incoming call or not
        private bool isIncomingCallAgent;

        public LinphoneScheduledAgent()
        {

        }

        protected override void OnInvoke(ScheduledTask task)
        {
            Debug.WriteLine("[LinphoneScheduledAgent] ScheduledAgentImpl has been invoked with argument of type {0}.", task.GetType());
            AgentHost.OnAgentStarted();
            VoipHttpIncomingCallTask incomingCallTask = task as VoipHttpIncomingCallTask;
            if (incomingCallTask != null)
            {
                this.isIncomingCallAgent = true;
                Debug.WriteLine("[IncomingCallAgent] Received VoIP Incoming Call task");

                BackgroundManager.Instance.OopServer.CallController.IncomingCallViewDismissed = OnIncomingCallDialogDismissed;
                BackgroundManager.Instance.InitLinphoneCore();
            }
            else
            {
                VoipKeepAliveTask keepAliveTask = task as VoipKeepAliveTask;
                if (keepAliveTask != null)
                {
                    this.isIncomingCallAgent = false;
                    Debug.WriteLine("[KeepAliveAgent] Keep Alive task");

                    if (DeviceNetworkInformation.IsNetworkAvailable)
                    {
                        var server = BackgroundManager.Instance.OopServer;
                        LpConfig config = server.LinphoneCoreFactory.CreateLpConfig(ApplicationData.Current.LocalFolder.Path + "\\linphonerc", "Assets/linphonerc-factory");
                        server.LinphoneCoreFactory.CreateLinphoneCore(this, config);
                        server.LinphoneCore.SetRootCA("Assets/rootca.pem");
                        server.LinphoneCore.SetNetworkReachable(true);
                        Debug.WriteLine("[KeepAliveAgent] Linphone Core created");

                        PushManager pushManager = new PushManager();
                        if (server.LinphoneCore.GetDefaultProxyConfig() != null && pushManager.PushChannelUri != null)
                        {
                            string host, token;
                            host = pushManager.PushChannelUri.Host;
                            token = pushManager.PushChannelUri.AbsolutePath;
                            server.LinphoneCore.GetDefaultProxyConfig().SetContactParameters("app-id=" + host + ";pn-type=wp;pn-tok=" + token + ";pn-msg-str=IM_MSG;pn-call-str=IC_MSG;pn-call-snd=ring.caf;pn-msg-snd=msg.caf");
                            Debug.WriteLine("[KeepAliveAgent] Added push parameters to contact");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("[KeepAliveAgent] Not connected, can't refresh register");
                        base.NotifyComplete();
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown scheduled task type {0}", task.GetType()));
                }
            }
        }

        // This method is called when the incoming call processing is complete to kill the background process if needed
        private void OnIncomingCallDialogDismissed()
        {
            Debug.WriteLine("[IncomingCallAgent] Incoming call processing is now complete.");
            BackgroundManager.Instance.OopServer.CallController.IncomingCallViewDismissed = null;
            base.NotifyComplete();
        } 

        protected override void OnCancel()
        {
            Debug.WriteLine("[{0}] Cancel requested.", this.isIncomingCallAgent ? "IncomingCallAgent" : "KeepAliveAgent");

            base.NotifyComplete();
        }

        #region LinphoneCoreListener Callbacks
        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void AuthInfoRequested(string realm, string username)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void GlobalState(GlobalState state, string message)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void CallState(LinphoneCall call, LinphoneCallState state)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void RegistrationState(LinphoneProxyConfig config, RegistrationState state, string message)
        {
            if (config == null)
                return;

            Debug.WriteLine("[KeepAliveAgent] Registration state changed: " + state.ToString() + ", message=" + message + " for identity " + config.GetIdentity());
            if (state == Linphone.Core.RegistrationState.RegistrationOk)
            {
                Debug.WriteLine("[KeepAliveAgent] Notify complete");
                base.NotifyComplete();
            }
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void DTMFReceived(LinphoneCall call, Char dtmf)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void EcCalibrationStatus(EcCalibratorStatus status, int delayMs)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void CallEncryptionChanged(LinphoneCall call, bool encrypted, string authenticationToken)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void CallStatsUpdated(LinphoneCall call, LinphoneCallStats stats)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
        }
        #endregion
    }
}
