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
                        OutputTraceLevel level = (OutputTraceLevel)config.GetInt("app", "LogLevel", 1); //1 is Message
                        server.LinphoneCoreFactory.SetLogLevel(level);
                        server.LinphoneCoreFactory.CreateLinphoneCore(this, config);
                        server.LinphoneCore.SetRootCA("Assets/rootca.pem");
                        server.LinphoneCore.SetChatDatabasePath(ApplicationData.Current.LocalFolder.Path + "\\chat.db");
                        server.LinphoneCore.SetNetworkReachable(true);
                        server.LinphoneCore.SetUserAgent("LinphoneWP8KeepAlive", Linphone.Version.Number);
                        server.LinphoneCore.IterateEnabled = true;
                        Debug.WriteLine("[KeepAliveAgent] Linphone Core created");
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

            TileManager.Instance.UpdateCount(BackgroundManager.Instance.OopServer.LinphoneCore.GetMissedCallsCount());

            if (BackgroundManager.Instance.OopServer.LinphoneCore.GetCallsNb() == 0)
            {
                BackgroundManager.Instance.OopServer.LinphoneCore.SetNetworkReachable(false); // To prevent the core from unregister
                BackgroundManager.Instance.OopServer.LinphoneCoreFactory.Destroy();
            }
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
        public void AuthInfoRequested(string realm, string username, string domain)
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
        public void CallState(LinphoneCall call, LinphoneCallState state, string message)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void RegistrationState(LinphoneProxyConfig config, RegistrationState state, string message)
        {
            if (state == Linphone.Core.RegistrationState.RegistrationOk || state == Linphone.Core.RegistrationState.RegistrationFailed)
            {
                Debug.WriteLine("[KeepAliveAgent] Notify complete (" + state.ToString() + ")");
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

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void ComposingReceived(LinphoneChatRoom room)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void FileTransferProgressIndication(LinphoneChatMessage message, int offset, int total)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void LogUploadStatusChanged(LinphoneCoreLogCollectionUploadState state, string info)
        {
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void LogUploadProgressChanged(int progress)
        {
        }
        #endregion
    }
}
