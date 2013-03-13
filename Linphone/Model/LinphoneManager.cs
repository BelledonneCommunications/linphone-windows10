using Microsoft.Phone.Networking.Voip;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
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
using Windows.Phone.Networking.Voip;
using System.Windows.Media.Imaging;

namespace Linphone.Model
{
    /// <summary>
    /// Utility class to handle most of the LinphoneCore (and more globally the C++/CX API) methods calls.
    /// </summary>
    public sealed class LinphoneManager : LinphoneCoreListener, OutputTraceListener
    {
        private LinphoneManager()
        {
            LastKnownState = Linphone.Core.RegistrationState.RegistrationNone;
        }

        private static LinphoneManager singleton;
        /// <summary>
        /// Static instance of the class.
        /// </summary>
        public static LinphoneManager Instance
        {
            get
            {
                if (LinphoneManager.singleton == null)
                    LinphoneManager.singleton = new LinphoneManager();

                return LinphoneManager.singleton;
            }
        }

        /// <summary>
        /// Quick accessor for the LinphoneCore object through the OoP server.
        /// </summary>
        public LinphoneCore LinphoneCore
        {
            get
            {
                return server.LinphoneCore;
            }
        }

        /// <summary>
        /// Call coordinator used to manage system VoIP call objects.
        /// </summary>
        public VoipCallCoordinator CallController
        {
            get
            {
                return VoipCallCoordinator.GetDefault();
            }
        }

        /// <summary>
        /// Used to set the default registration state on the status bar when the view is changed.
        /// </summary>
        public RegistrationState LastKnownState { get; set; }

        /// <summary>
        /// Simple listener to notify pages' viewmodel when a call ends or starts
        /// </summary>
        public CallControllerListener CallListener { get; set; }

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

        // A timer to invoke LinphoneCore.Iterate()
        private static Timer timer;

        /// <summary>
        /// Starts and connects the UI to the background process.
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
        /// Disconnects the UI from the background process.
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
        /// Creates and adds the LinphoneProxyConfig in LinphoneCore.
        /// </summary>
        public void InitProxyConfig()
        {
            server.LinphoneCore.ClearAuthInfos();
            server.LinphoneCore.ClearProxyConfigs();

            SettingsManager sm = new SettingsManager();
            if (sm.Username != null && sm.Username.Length > 0 && sm.Domain != null && sm.Domain.Length > 0)
            {
                var proxy = server.LinphoneCore.CreateEmptyProxyConfig();
                proxy.SetIdentity(sm.Username, sm.Username, sm.Domain);
                proxy.EnableRegister(true);
                server.LinphoneCore.AddProxyConfig(proxy);
                server.LinphoneCore.SetDefaultProxyConfig(proxy);

                // Can't set string to null: http://stackoverflow.com/questions/12980915/exception-when-trying-to-read-null-string-in-c-sharp-winrt-component-from-winjs
                var auth = server.LinphoneCore.CreateAuthInfo(sm.Username, "", sm.Password, "", sm.Domain);
                server.LinphoneCore.AddAuthInfo(auth);
            }
        }

        /// <summary>
        /// Creates a new LinphoneCore (if not created yet) using a LinphoneCoreFactory.
        /// </summary>
        public void InitLinphoneCore()
        {
            if (LinphoneCore != null)
                return;

            server.LinphoneCoreFactory.CreateLinphoneCore(this);

            InitProxyConfig();

            timer = new Timer(LinphoneCoreIterate, null, 1, 20);
            Debug.WriteLine("[LinphoneManager] LinphoneCore created");
        }

        private void LinphoneCoreIterate(object o)
        {
            // Handle exception that can occurs when app is quickly closed/restarted
            try
            {
                if (LinphoneCore == null && timer != null)
                {
                    timer.Dispose();
                    return;
                }

                LinphoneCore.Iterate();
            }
            catch (Exception)
            {
                timer.Dispose();
            }
        }

        /// <summary>
        /// Set the debug value for liblinphone.
        /// </summary>
        /// <param name="enable">true to enable debug traces, false to disable them</param>
        public void EnableDebug(bool enable)
        {
            server.LinphoneCoreFactory.SetDebugMode(enable, this);
        }

        #region CallLogs
        private List<CallLogs> _history;

        /// <summary>
        /// Get the calls' history.
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
                    if (from.Length == 0)
                        from = log.GetFrom().AsStringUriOnly();

                    string to = log.GetTo().GetDisplayName();
                    if (to.Length == 0)
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
        /// Remove one or many entries from the calls' history.
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
        /// Remove all calls' history from LinphoneCore.
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
        /// Start a new call to a sip address.
        /// </summary>
        /// <param name="sipAddress">SIP address to call</param>
        public void NewOutgoingCall(String sipAddress)
        {
            VoipPhoneCall call;
            CallController.RequestNewOutgoingCall("/Linphone;component/Views/InCall.xaml?sip=" + sipAddress, sipAddress, "Linphone", VoipCallMedia.Audio, out call);
            call.NotifyCallActive();
            LinphoneCall LCall = LinphoneCore.Invite(sipAddress);
            LCall.CallContext = call;
        }

        /// <summary>
        /// Stops the current call if any.
        /// </summary>
        public void EndCurrentCall()
        {
            if (LinphoneCore.GetCallsNb() > 0)
            {
                LinphoneCall call = LinphoneCore.GetCurrentCall();
                LinphoneCore.TerminateCall(call);
            }
        }

        /// <summary>
        /// Pauses the current call if any and if it's running.
        /// </summary>
        public void PauseCurrentCall()
        {
            LinphoneCall call = LinphoneCore.GetCurrentCall();
            ((VoipPhoneCall)call.CallContext).NotifyCallHeld();
            LinphoneCore.PauseCall(call);
        }

        /// <summary>
        /// Resume the current call if any and if it's paused.
        /// </summary>
        public void ResumeCurrentCall()
        {
            if (LinphoneCore.GetCallsNb() > 0)
            {
                LinphoneCall call = LinphoneCore.GetCurrentCall();
                ((VoipPhoneCall)call.CallContext).NotifyCallActive();
                LinphoneCore.ResumeCall(call);
            }
        }
        #endregion

        #region LinphoneCoreListener Callbacks
        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void AuthInfoRequested(string realm, string username)
        {
            Debug.WriteLine("[LinphoneManager] Auth info requested: realm=" + realm + ", username=" + username);
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void GlobalState(GlobalState state, string message)
        {
            Debug.WriteLine("[LinphoneManager] Global state changed: " + state.ToString() + ", message=" + message);
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void CallState(LinphoneCall call, LinphoneCallState state)
        {
            Debug.WriteLine("[LinphoneManager] Call state changed: " + call.GetRemoteAddress().AsStringUriOnly() + " => " + state.ToString());
            if (state == LinphoneCallState.IncomingReceived)
            {
                String contact = call.GetRemoteContact();
                String number = call.GetRemoteAddress().AsStringUriOnly();
                Debug.WriteLine("[LinphoneManager] Incoming received: " + contact + " (" + number + ")");

                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    VoipPhoneCall vcall = null;
                    Uri contactUri = new Uri(server.Path + "\\Assets\\unknown.png", UriKind.Absolute);
                    Uri iconUri = new Uri(server.Path + "\\Assets\\pnicon.png", UriKind.Absolute);
                    Uri ringtoneUri = new Uri(server.Path + "\\Assets\\Sounds\\Ringtone.wma", UriKind.Absolute);

                    CallController.RequestNewIncomingCall("/Linphone;component/Views/InCall.xaml?sip=" + number, contact, number, contactUri, "Linphone", iconUri, "", ringtoneUri, VoipCallMedia.Audio, fifteenSecs, out vcall);
                    vcall.AnswerRequested += ((c, eventargs) =>
                        {
                            Debug.WriteLine("[LinphoneManager] Call accepted");

                            vcall.NotifyCallActive();
                            LinphoneCore.AcceptCall(call);
                        });
                    vcall.RejectRequested += ((c, eventargs) =>
                        {
                            Debug.WriteLine("[LinphoneManager] Call rejected");

                            LinphoneCore.TerminateCall(call);
                        });

                    call.CallContext = vcall;
                });
            }
            else if (state == LinphoneCallState.StreamsRunning)
            {
                Debug.WriteLine("[LinphoneManager] Call accepted and running");

                if (CallListener != null)
                    CallListener.NewCallStarted(call.GetRemoteAddress().AsStringUriOnly());
            }
            else if (state == LinphoneCallState.CallEnd || state == LinphoneCallState.Error)
            {
                Debug.WriteLine("[LinphoneManager] Call ended");
                ((VoipPhoneCall)call.CallContext).NotifyCallEnded();

                if (CallListener != null)
                    CallListener.CallEnded();
            }
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void RegistrationState(LinphoneProxyConfig config, RegistrationState state, string message)
        {
            Debug.WriteLine("[LinphoneManager] Registration state changed: " + state.ToString() + ", message=" + message);
            LastKnownState = state;
            if (BasePage.StatusBar != null)
                BasePage.StatusBar.RefreshStatusIcon(state);
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void DTMFReceived(LinphoneCall call, int dtmf)
        {

        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void EcCalibrationStatus(EcCalibratorStatus status, int delay_ms, object data)
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
        #endregion

        public void outputTrace(int level, String msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }
}
