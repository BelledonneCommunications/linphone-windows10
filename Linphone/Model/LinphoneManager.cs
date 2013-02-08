using Microsoft.Phone.Networking.Voip;
using Linphone.BackEnd;
using Linphone.BackEnd.OutOfProcess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Linphone.Resources;

namespace Linphone.Model
{
    public sealed class LinphoneManager : LinphoneCoreListener
    {
        private static LinphoneManager singleton;
        public static LinphoneManager Instance
        {
            get
            {
                if (LinphoneManager.singleton == null)
                    LinphoneManager.singleton = new LinphoneManager();

                return LinphoneManager.singleton;
            }
        }

        public LinphoneCore LinphoneCore
        {
            get
            {
                return server.LinphoneCore;
            }
        }

        public CallController CallController
        {
            get
            {
                return server.CallController;
            }
        }

        #region Background Process
        private bool BackgroundProcessConnected;

        // An event that indicates that the UI process is no longer connected to the background process 
        private EventWaitHandle uiDisconnectedEvent;

        // A proxy to the server object in the background agent host process 
        private Server server;

        // A timespan representing fifteen seconds 
        private static readonly TimeSpan fifteenSecs = new TimeSpan(0, 0, 15);

        // A timespan representing an indefinite wait 
        private static readonly TimeSpan indefiniteWait = new TimeSpan(0, 0, 0, 0, -1);

        /// <summary>
        /// Starts and connects the UI to the background process
        /// </summary>
        public void ConnectBackgroundProcessToInterface()
        {
            if (BackgroundProcessConnected)
            {
                Debug.WriteLine("[LinphoneManager] Background process already connected to interface");
                return;
            }

            int backgroundProcessID;
            try
            {
                VoipBackgroundProcess.Launch(out backgroundProcessID);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[LinphoneManager] Error launching VoIP background process. Exception: " + e.Message);
                throw;
            }

            // Wait for the background process to become ready 
            string backgroundProcessReadyEventName = Globals.GetBackgroundProcessReadyEventName((uint)backgroundProcessID);
            using (EventWaitHandle backgroundProcessReadyEvent = new EventWaitHandle(initialState: false, mode: EventResetMode.ManualReset, name: backgroundProcessReadyEventName))
            {
                TimeSpan timeout = Debugger.IsAttached ? LinphoneManager.indefiniteWait : LinphoneManager.fifteenSecs;
                if (!backgroundProcessReadyEvent.WaitOne(timeout))
                {
                    // We timed out - something is wrong 
                    throw new InvalidOperationException(string.Format("The background process did not become ready in {0} milliseconds", timeout.Milliseconds));
                }
                else
                {
                    Debug.WriteLine("[LinphoneManager] Background process {0} is ready", backgroundProcessID);
                }
            }
            // The background process is now ready. 
            // It is possible that the background process now becomes "not ready" again, but the chances of this happening are slim, 
            // and in that case, the following statement would fail - so, at this point, we don't explicitly guard against this condition. 

            // Create an instance of the server in the background process. 
            server = (Server)WindowsRuntimeMarshal.GetActivationFactory(typeof(Server)).ActivateInstance();

            // Un-set an event that indicates that the UI process is disconnected from the background process. 
            // The background process waits for this event to get set before shutting down. 
            // This ensures that the background agent host process doesn't shut down while the UI process is connected to it. 
            string uiDisconnectedEventName = Globals.GetUiDisconnectedEventName((uint)backgroundProcessID);
            uiDisconnectedEvent = new EventWaitHandle(initialState: false, mode: EventResetMode.ManualReset, name: uiDisconnectedEventName);
            uiDisconnectedEvent.Reset();

            BackgroundProcessConnected = true;
            Debug.WriteLine("[LinphoneManager] Background process connected to interface");
        }

        /// <summary>
        /// Disconnects the UI from the background process
        /// </summary>
        public void DisconnectBackgroundProcessFromInterface()
        {
            if (!BackgroundProcessConnected)
            {
                Debug.WriteLine("[LinphoneManager] Background process not connected to interface yet");
                return;
            }

            BackgroundProcessConnected = false;
            Debug.WriteLine("[LinphoneManager] Background process disconnected from interface");

            // From this point onwards, it is no longer safe to use any objects in the background process, 
            // or for the background process to call back into this process. 
            server = null;

            // Lastly, set the event that indicates that the UI is no longer connected to the background process. 
            if (uiDisconnectedEvent == null)
                throw new InvalidOperationException("The ConnectUi method must be called before this method is called");

            uiDisconnectedEvent.Set();
            uiDisconnectedEvent.Dispose();
            uiDisconnectedEvent = null;
        }
        #endregion

        /// <summary>
        /// Creates a new LinphoneCore (if not created yet) using a LinphoneCoreFactory
        /// </summary>
        public void InitLinphoneCore()
        {
            if (LinphoneCore != null)
                return;

            server.LinphoneCoreFactory.CreateLinphoneCore(this);
        }

        /// <summary>
        /// Set the debug value for liblinphone
        /// </summary>
        /// <param name="enable">true to enable debug traces, false to disable them</param>
        public void EnableDebug(bool enable)
        {
            server.LinphoneCoreFactory.SetDebugMode(enable, AppResources.ApplicationTitle);
        }

        #region CallLogs
        private List<CallLogs> _history;

        /// <summary>
        /// Get the calls' history
        /// </summary>
        /// <returns>A list of CallLogs, each one representing a type of calls (All, Missed, ...)</returns>
        public List<CallLogs> GetCallsHistory()
        {
            _history = new List<CallLogs>();

            ObservableCollection<CallLog> calls = new ObservableCollection<CallLog>();
            ObservableCollection<CallLog> missedCalls = new ObservableCollection<CallLog>();

            if (LinphoneCore.GetCallLogs() != null)
            {
                foreach (LinphoneCallLog log in LinphoneCore.GetCallLogs())
                {
                    string from = log.GetFrom().GetDisplayName();
                    if (from == null)
                        from = log.GetFrom().AsStringUriOnly();

                    string to = log.GetTo().GetDisplayName();
                    if (to == null)
                        to = log.GetTo().AsStringUriOnly();

                    bool isMissed = log.GetStatus() == LinphoneCallStatus.Missed;

                    CallLog callLog = new CallLog(log, from, to, log.GetDirection() == CallDirection.Incoming, isMissed);

                    calls.Add(callLog);
                    if (isMissed)
                        missedCalls.Add(callLog);
                }
            }

            CallLogs all = new CallLogs("All", calls);
            _history.Add(all);

            CallLogs missed = new CallLogs("Missed", missedCalls);
            _history.Add(missed);

            return _history;
        }

        /// <summary>
        /// Remove one or many entries from the calls' history
        /// </summary>
        /// <param name="logsToRemove">A list of CallLog to remove from history</param>
        /// <returns>A list of CallLogs, without the removed entries</returns>
        public List<CallLogs> RemoveCallLogs(IEnumerable<CallLog> logsToRemove)
        {
            // When removing log from history, it will be removed from logsToRemove list too. 
            // Using foreach causing the app to crash on a InvalidOperationException, so we are using while
            while (logsToRemove.Count() > 0)
            {
                CallLog logToRemove = logsToRemove.First();
                LinphoneCore.RemoveCallLog(logToRemove.NativeLog as LinphoneCallLog);
            }

            return _history;
        }

        /// <summary>
        /// Remove all calls' history from LinphoneCore
        /// </summary>
        /// <returns>An empty list</returns>
        public List<CallLogs> ClearCallLogs()
        {
            LinphoneCore.ClearCallLogs();

            _history = new List<CallLogs>();

            ObservableCollection<CallLog> calls = new ObservableCollection<CallLog>();
            ObservableCollection<CallLog> missedCalls = new ObservableCollection<CallLog>();

            CallLogs all = new CallLogs("All", calls);
            _history.Add(all);

            CallLogs missed = new CallLogs("Missed", missedCalls);
            _history.Add(missed);

            return _history;
        }

        /// <summary>
        /// Start a new call to a sip address
        /// </summary>
        /// <param name="sipAddress">SIP address to call</param>
        public void NewOutgoingCall(String sipAddress)
        {
            CallController.NewOutgoingCall(sipAddress, "");
            LinphoneCore.Invite(sipAddress);
        }

        /// <summary>
        /// Stops the current call if any
        /// </summary>
        public void EndCurrentCall()
        {
            if (LinphoneCore.GetCallsNb() > 0)
            {
                LinphoneCall call = LinphoneCore.GetCalls().First();
                LinphoneCore.TerminateCall(call);
            }
            CallController.EndCurrentCall();
        }

        /// <summary>
        /// Pauses the current call if any and if it's running
        /// </summary>
        public void PauseCurrentCall()
        {
            LinphoneCall call = LinphoneCore.GetCurrentCall();
            LinphoneCore.PauseCall(call);
        }

        /// <summary>
        /// Resume the current call if any and if it's paused
        /// </summary>
        public void ResumeCurrentCall()
        {
            if (LinphoneCore.GetCallsNb() > 0)
            {
                LinphoneCall call = LinphoneCore.GetCalls().First();
                LinphoneCore.ResumeCall(call);
            }
        }
        #endregion

        #region LinphoneCoreListener Callbacks
        public void GlobalState(GlobalState state, string message)
        {

        }

        public void CallState(LinphoneCall call, LinphoneCallState state)
        {
            if (state == LinphoneCallState.CallEnd ||
                state == LinphoneCallState.Error ||
                state == LinphoneCallState.Released)
            {
                CallController.EndCurrentCall();
            }
        }

        public void RegistrationState(LinphoneProxyConfig config, RegistrationState state, string message)
        {

        }

        public void DTMFReceived(LinphoneCall call, int dtmf)
        {

        }

        public void EcCalibrationStatus(EcCalibratorStatus status, int delay_ms, object data)
        {

        }

        public void CallEncryptionChanged(LinphoneCall call, bool encrypted, string authenticationToken)
        {

        }

        public void CallStatsUpdated(LinphoneCall call, LinphoneCallStats stats)
        {

        }
        #endregion
    }
}
