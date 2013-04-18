using Linphone.Agents;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Phone.Media.Devices;
using Windows.Phone.Networking.Voip;

namespace Linphone.Model
{
    /// <summary>
    /// Utility class to handle most of the LinphoneCore (and more globally the C++/CX API) methods calls.
    /// </summary>
    public sealed class LinphoneManager : LinphoneCoreListener
    {
        #region Network state management
        private bool lastNetworkState;
        private void OnNetworkStatusChanged(object sender, NetworkNotificationEventArgs e)
        {
            if (lastNetworkState != DeviceNetworkInformation.IsNetworkAvailable)
            {
                lastNetworkState = DeviceNetworkInformation.IsNetworkAvailable;
                Debug.WriteLine("[LinphoneManager] Network state changed:" + (lastNetworkState ? "Available" : "Unavailable"));
                LinphoneCore.SetNetworkReachable(lastNetworkState);
            }
        }
        #endregion

        #region Class properties
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
        /// Quick accessor for the LinphoneCoreFactory object through the Oop server.
        /// </summary>
        public LinphoneCoreFactory LinphoneCoreFactory
        {
            get
            {
                return server.LinphoneCoreFactory;
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

        private RegistrationState _lastKnownState;
        /// <summary>
        /// Used to set the default registration state on the status bar when the view is changed.
        /// </summary>
        public RegistrationState LastKnownState {
            get
            {
                if (isLinphoneRunning && LinphoneCore.GetDefaultProxyConfig() != null) 
                {
                    _lastKnownState = LinphoneCore.GetDefaultProxyConfig().GetState();
                }
                return _lastKnownState;
            }

            set
            {
                _lastKnownState = value;
            }
        }

        /// <summary>
        /// Simple listener to notify pages' viewmodel when a call ends or starts
        /// </summary>
        public CallControllerListener CallListener { get; set; }
        #endregion

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
        /// Used to know when linphoneCore has been initialized
        /// </summary>
        public bool isLinphoneRunning = false;

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
                TimeSpan timeout = Debugger.IsAttached ? indefiniteWait : fifteenSecs;
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
            if (BackgroundManager.Instance.OopServer != null)
            {
                server = BackgroundManager.Instance.OopServer;
            }
            else
            {
                server = (Server)WindowsRuntimeMarshal.GetActivationFactory(typeof(Server)).ActivateInstance();
                BackgroundManager.Instance.OopServer = server;
            }

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
            isLinphoneRunning = false;
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

        #region Linphone Core init
        /// <summary>
        /// Creates a new LinphoneCore (if not created yet) using a LinphoneCoreFactory.
        /// </summary>
        public async void InitLinphoneCore()
        {
            if ((server.LinphoneCoreFactory != null) && (server.LinphoneCore != null))
            {
                // Reconnect the listeners when coming back from background mode
                Logger.Msg("[LinphoneManager] LinphoneCore alread created, skipping");

                server.LinphoneCore.CoreListener = this;
                isLinphoneRunning = true;
                return;
            }

            Logger.Msg("[LinphoneManager] Creating LinphoneCore");
            await SettingsManager.InstallConfigFile();
            LpConfig config = server.LinphoneCoreFactory.CreateLpConfig(SettingsManager.GetConfigPath(), SettingsManager.GetFactoryConfigPath());
            ConfigureLogger();
            server.LinphoneCoreFactory.CreateLinphoneCore(this, config);
            server.LinphoneCore.SetRootCA("Assets/rootca.pem");
            Logger.Msg("[LinphoneManager] LinphoneCore created");

            AudioRoutingManager.GetDefault().AudioEndpointChanged += AudioEndpointChanged;
            CallController.MuteRequested += MuteRequested;
            CallController.UnmuteRequested += UnmuteRequested;

            if (LinphoneCore.GetDefaultProxyConfig() != null)
            {
                string host, token;
                host = ((App)App.Current).PushChannelUri.Host;
                token = ((App)App.Current).PushChannelUri.AbsolutePath;
                LinphoneCore.GetDefaultProxyConfig().SetContactParameters("app-id=" + host + ";pn-type=wp;pn-tok=" + token + ";pn-msg-str=IM_MSG;pn-call-str=IC_MSG;pn-call-snd=ring.caf;pn-msg-snd=msg.caf");
            }

            lastNetworkState = DeviceNetworkInformation.IsNetworkAvailable;
            LinphoneCore.SetNetworkReachable(lastNetworkState);
            DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(OnNetworkStatusChanged);

            isLinphoneRunning = true;
        }

        /// <summary>
        /// Configures the Logger
        /// </summary>
        public void ConfigureLogger()
        {
            // To have the debug output in the debugger use the following commented configure and set your debugger to native mode
            //server.BackgroundModeLogger.Configure(SettingsManager.isDebugEnabled, OutputTraceDest.Debugger, "");
            // Else output the debug traces to a file
            ApplicationSettingsManager appSettings = new ApplicationSettingsManager();
            appSettings.Load();
            server.BackgroundModeLogger.Configure(appSettings.DebugEnabled, appSettings.LogDestination, appSettings.LogOption);
            server.LinphoneCoreFactory.OutputTraceListener = server.BackgroundModeLogger;
            server.LinphoneCoreFactory.SetLogLevel(appSettings.LogLevel);
            Logger.Instance.TraceListener = server.BackgroundModeLogger;
        }
        #endregion

        #region CallLogs
        private List<CallLog> _history;

        /// <summary>
        /// Gets the latest called address or number
        /// </summary>
        /// <returns>null if there isn't any</returns>
        public string GetLastCalledNumber()
        {
            foreach (LinphoneCallLog log in LinphoneManager.Instance.LinphoneCore.GetCallLogs())
            {
                if (log.GetDirection() == CallDirection.Outgoing)
                {
                    return log.GetTo().AsStringUriOnly();
                }
            }
            return null;
        }

        /// <summary>
        /// Get the calls' history.
        /// </summary>
        /// <returns>A list of CallLogs, each one representing a type of calls (All, Missed, ...)</returns>
        public List<CallLog> GetCallsHistory()
        {
            _history = new List<CallLog>();

            if (LinphoneCore.GetCallLogs() != null)
            {
                foreach (LinphoneCallLog log in LinphoneCore.GetCallLogs())
                {
                    string from = log.GetFrom().GetDisplayName();
                    if (from.Length == 0)
                        from = log.GetFrom().AsStringUriOnly().Replace("sip:", "");

                    string to = log.GetTo().GetDisplayName();
                    if (to.Length == 0)
                        to = log.GetTo().AsStringUriOnly().Replace("sip:", "");

                    bool isMissed = log.GetStatus() == LinphoneCallStatus.Missed;
                    long startDate = log.GetStartDate();
                    CallLog callLog = new CallLog(log, from, to, log.GetDirection() == CallDirection.Incoming, isMissed, startDate);
                    _history.Add(callLog);
                }
            }

            return _history;
        }

        /// <summary>
        /// Remove one or many entries from the calls' history.
        /// </summary>
        /// <param name="logsToRemove">A list of CallLog to remove from history</param>
        /// <returns>A list of CallLogs, without the removed entries</returns>
        public void RemoveCallLogs(IEnumerable<CallLog> logsToRemove)
        {
            // When removing log from history, it will be removed from logsToRemove list too. 
            // Using foreach causing the app to crash on a InvalidOperationException, so we are using while
            for (int i = 0; i < logsToRemove.Count(); i++)
            {
                CallLog logToRemove = logsToRemove.ElementAt(i);
                LinphoneCore.RemoveCallLog(logToRemove.NativeLog as LinphoneCallLog);
            }
        }

        /// <summary>
        /// Remove all calls' history from LinphoneCore.
        /// </summary>
        /// <returns>An empty list</returns>
        public void ClearCallLogs()
        {
            LinphoneCore.ClearCallLogs();
        }
        #endregion

        #region Call Management
        /// <summary>
        /// Start a new call to a sip address.
        /// </summary>
        /// <param name="sipAddress">SIP address to call</param>
        public void NewOutgoingCall(String sipAddress)
        {
            LinphoneCall LCall = LinphoneCore.Invite(sipAddress);
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
        /// Enable or disable sound capture using the device microphone
        /// </summary>
        public void MuteMic(Boolean isMicMuted)
        {
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                if (isMicMuted)
                    CallController.NotifyMuted();
                else
                    CallController.NotifyUnmuted();

                if (CallListener != null)
                    CallListener.MuteStateChanged(isMicMuted);
            });

            LinphoneCore.MuteMic(isMicMuted);
        }

        private void UnmuteRequested(VoipCallCoordinator sender, MuteChangeEventArgs args)
        {
            Logger.Msg("[LinphoneManager] Unmute requested");
            MuteMic(true);
        }

        private void MuteRequested(VoipCallCoordinator sender, MuteChangeEventArgs args)
        {
            Logger.Msg("[LinphoneManager] Mute requested");
            MuteMic(false);
        }

        /// <summary>
        /// Pauses the current call if any and if it's running.
        /// </summary>
        public void PauseCurrentCall()
        {
            if (LinphoneCore.GetCallsNb() > 0)
            {
                LinphoneCall call = LinphoneCore.GetCurrentCall();
                LinphoneCore.PauseCall(call);
            }
        }

        /// <summary>
        /// Resume the current call if any and if it's paused.
        /// </summary>
        public void ResumeCurrentCall()
        {
            if (LinphoneCore.GetCallsNb() > 0)
            {
                LinphoneCall call = LinphoneCore.GetCurrentCall();
                LinphoneCore.ResumeCall(call);
            }
        }

        private void CallResumeRequested(VoipPhoneCall sender, CallStateChangeEventArgs args)
        {
            Logger.Msg("[LinphoneManager] Resume requested");
            ResumeCurrentCall();
        }

        private void CallHoldRequested(VoipPhoneCall sender, CallStateChangeEventArgs args)
        {
            Logger.Msg("[LinphoneManager] Pause requested");
            PauseCurrentCall();
        }
        #endregion

        #region Audio route handling
        private void AudioEndpointChanged(AudioRoutingManager sender, object args)
        {
            Logger.Msg("[LinphoneManager] AudioEndpointChanged:" + sender.GetAudioEndpoint().ToString());
        }

        public void EnableSpeaker(bool enable)
        {
            if (enable)
            {
                AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Speakerphone);
            }
            else
            {
                AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Earpiece);
            }
        }
        #endregion

        #region LinphoneCoreListener Callbacks
        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void AuthInfoRequested(string realm, string username)
        {
            Logger.Msg("[LinphoneManager] Auth info requested: realm=" + realm + ", username=" + username);
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void GlobalState(GlobalState state, string message)
        {
            Logger.Msg("[LinphoneManager] Global state changed: " + state.ToString() + ", message=" + message);
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void CallState(LinphoneCall call, LinphoneCallState state)
        {
            string sipAddress = call.GetRemoteAddress().AsStringUriOnly();

            Logger.Msg("[LinphoneManager] Call state changed: " + sipAddress + " => " + state.ToString());
            if (state == LinphoneCallState.OutgoingProgress)
            {
                Logger.Msg("[LinphoneManager] Outgoing progress");
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    LookupForContact(call);

                    if (CallListener != null)
                        CallListener.NewCallStarted(sipAddress);
                });
            }
            else if (state == LinphoneCallState.IncomingReceived)
            {
                Logger.Msg("[LinphoneManager] Incoming received"); 
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    LookupForContact(call);
                });
            }
            else if (state == LinphoneCallState.Connected)
            {
                Logger.Msg("[LinphoneManager] Connected");
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    if (CallListener != null)
                        CallListener.NewCallStarted(sipAddress);
                });
            }
            else if (state == LinphoneCallState.CallEnd || state == LinphoneCallState.Error)
            {
                Logger.Msg("[LinphoneManager] Call ended");
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    if (CallListener != null)
                        CallListener.CallEnded();
                });
            }
            else if (state == LinphoneCallState.Paused || state == LinphoneCallState.PausedByRemote)
            {
                Logger.Msg("[LinphoneManager] Call paused");
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    if (CallListener != null)
                        CallListener.PauseStateChanged(true);
                });
            }
            else if (state == LinphoneCallState.StreamsRunning)
            {
                Logger.Msg("[LinphoneManager] Call running");
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    if (CallListener != null)
                        CallListener.PauseStateChanged(false);
                });
            }
            else if (state == LinphoneCallState.Released)
            {
                Logger.Msg("[LinphoneManager] Call released");
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    TileManager.Instance.UpdateTileWithMissedCalls(LinphoneCore.GetMissedCallsCount());
                });
            }
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void RegistrationState(LinphoneProxyConfig config, RegistrationState state, string message)
        {
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                Logger.Msg("[LinphoneManager] Registration state changed: " + state.ToString() + ", message=" + message + " for identity " + config.GetIdentity());
                LastKnownState = state;
                if (BasePage.StatusBar != null)
                    BasePage.StatusBar.RefreshStatus(state);
            });
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

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
            Logger.Msg("[LinphoneManager] Message received from " + message.GetFrom().AsStringUriOnly() + ": " + message.GetText());

            //ShellToast toast = new ShellToast();
            //toast.Content = message.GetText();
            //toast.Title = message.GetFrom().GetDisplayName().Length > 0 ? message.GetFrom().GetDisplayName() : message.GetFrom().GetUserName();
            //toast.NavigationUri = new Uri("/Views/Chat.xaml?sip=" + message.GetFrom().AsStringUriOnly(), UriKind.RelativeOrAbsolute);
            //toast.Show();
        }
        #endregion

        #region Contact Lookup
        private ContactManager ContactManager
        {
            get
            {
                return ContactManager.Instance;
            }
        }

        private void LookupForContact(LinphoneCall call)
        {
            string sipAddress = call.GetRemoteAddress().AsStringUriOnly();
            if (call.GetRemoteAddress().GetDisplayName().Length == 0)
            {
                if (sipAddress.StartsWith("sip:"))
                {
                    sipAddress = sipAddress.Substring(4);
                }
                Logger.Msg("[LinphoneManager] Display name null, looking for remote address in contact: " + sipAddress);

                ContactManager.ContactFound += OnContactFound;
                ContactManager.FindContact(sipAddress);
            }
            else
            {
                Logger.Msg("[LinphoneManager] Display name found: " + call.GetRemoteAddress().GetDisplayName());
            }
        }

        /// <summary>
        /// Callback called when the search on a phone number for a contact has a match
        /// </summary>
        private void OnContactFound(object sender, ContactFoundEventArgs e)
        {
            Logger.Msg("[LinphoneManager] Contact found: " + e.ContactFound.DisplayName);
            ContactManager.ContactFound -= OnContactFound;

            // Store the contact name as display name for call logs
            if (LinphoneManager.Instance.LinphoneCore.GetCurrentCall() != null)
            {
                LinphoneManager.Instance.LinphoneCore.GetCurrentCall().GetRemoteAddress().SetDisplayName(e.ContactFound.DisplayName);
            }
        }
        #endregion
    }
}
