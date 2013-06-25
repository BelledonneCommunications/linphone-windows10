using Linphone.Agents;
using Linphone.Controls;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
using Linphone.Resources;
using Linphone.Views;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Networking.Voip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using Windows.Phone.Media.Capture;
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
                if (lastNetworkState)
                {
                    NetworkSettingsManager.ConfigureTunnel();
                }
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

        /// <summary>
        /// Simple listener to notify the echo canceller calibration status
        /// </summary>
        public EchoCalibratorListener ECListener { get; set; }
        #endregion

        #region Background Process
        private bool BackgroundProcessConnected;

        // An event that indicates that the UI process is no longer connected to the background process 
        private EventWaitHandle uiDisconnectedEvent;

        // A proxy to the server object in the background agent host process 
        private Server server = null;

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
            BackgroundManager.Instance.OopServer = null;
            server = (Server)WindowsRuntimeMarshal.GetActivationFactory(typeof(Server)).ActivateInstance();
            BackgroundManager.Instance.OopServer = server;

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
        public async Task InitLinphoneCore()
        {
            if ((server.LinphoneCoreFactory != null) && (server.LinphoneCore != null))
            {
                // Reconnect the listeners when coming back from background mode
                Debug.WriteLine("[LinphoneManager] LinphoneCore already created, skipping");

                server.LinphoneCore.CoreListener = this;
                isLinphoneRunning = true;
                // Set user-agent because it is not set if coming back from background mode
                server.LinphoneCore.SetUserAgent(DefaultValues.UserAgent, XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value);
                return;
            }

            Debug.WriteLine("[LinphoneManager] Creating LinphoneCore");
            await SettingsManager.InstallConfigFile();
            LpConfig config = server.LinphoneCoreFactory.CreateLpConfig(SettingsManager.GetConfigPath(), SettingsManager.GetFactoryConfigPath());
            ConfigureLogger();
            server.LinphoneCoreFactory.CreateLinphoneCore(this, config);
            server.LinphoneCore.SetUserAgent(DefaultValues.UserAgent, XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value);
            server.LinphoneCore.SetRootCA("Assets/rootca.pem");
            Debug.WriteLine("[LinphoneManager] LinphoneCore created");

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

            if (LinphoneCore.IsVideoSupported())
            {
                DetectCameras();
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
            LinphoneCall call = LinphoneCore.GetCurrentCall();
            if (call != null)
            {
                LinphoneCore.TerminateCall(call);
            }
            else
            {
                foreach (LinphoneCall lCall in LinphoneCore.GetCalls())
                {
                    if (lCall.GetState() == LinphoneCallState.Paused)
                    {
                        LinphoneCore.TerminateCall(lCall);
                    }
                }
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
            foreach (LinphoneCall call in LinphoneCore.GetCalls()) {
                if (call.GetState() == LinphoneCallState.Paused)
                {
                    LinphoneCore.ResumeCall(call);
                }
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

        #region Video handling

        /// <summary>
        /// Enables disables video.
        /// </summary>
        /// <param name="enable">Wether to enable or disable video</param>
        /// <returns>true if the operation has been successful, false otherwise</returns>
        public bool EnableVideo(bool enable)
        {
            if (LinphoneCore.IsInCall())
            {
                LinphoneCall call = LinphoneCore.GetCurrentCall();
                if (enable != call.IsCameraEnabled())
                {
                    LinphoneCallParams parameters = call.GetCurrentParamsCopy();
                    parameters.EnableVideo(enable);
                    if (enable)
                    {
                        // TODO: Handle bandwidth limitation
                    }
                    LinphoneCore.UpdateCall(call, parameters);
                    return true;
                }
            }
            return false;
        }

        private int nbCameras = 0;
        private String frontCamera = null;
        private String backCamera = null;

        private void DetectCameras()
        {
            int nbCameras = 0;
            foreach (String device in LinphoneCore.GetVideoDevices())
            {
                if (device.EndsWith(CameraSensorLocation.Front.ToString()))
                {
                    frontCamera = device;
                    nbCameras++;
                }
                else if (device.EndsWith(CameraSensorLocation.Back.ToString()))
                {
                    backCamera = device;
                    nbCameras++;
                }
            }
            String currentDevice = LinphoneCore.GetVideoDevice();
            if ((currentDevice != frontCamera) && (currentDevice != backCamera))
            {
                if (frontCamera != null)
                {
                    LinphoneCore.SetVideoDevice(frontCamera);
                }
                else if (backCamera != null)
                {
                    LinphoneCore.SetVideoDevice(backCamera);
                }
            }
            this.nbCameras = nbCameras;
        }

        /// <summary>
        /// Gets the number of cameras available on the device (int).
        /// </summary>
        public int NumberOfCameras
        {
            get
            {
                return nbCameras;
            }
        }

        /// <summary>
        /// Toggles the camera used for video capture.
        /// </summary>
        public void ToggleCameras()
        {
            if (NumberOfCameras >= 2)
            {
                String currentDevice = LinphoneCore.GetVideoDevice();
                if (currentDevice == frontCamera)
                {
                    LinphoneCore.SetVideoDevice(backCamera);
                }
                else if (currentDevice == backCamera)
                {
                    LinphoneCore.SetVideoDevice(frontCamera);
                }
                if (LinphoneCore.IsInCall())
                {
                    LinphoneCall call = LinphoneCore.GetCurrentCall();
                    LinphoneCore.UpdateCall(call, null);
                }
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
                        CallListener.CallEnded(call);
                });
            }
            else if (state == LinphoneCallState.Paused || state == LinphoneCallState.PausedByRemote)
            {
                Logger.Msg("[LinphoneManager] Call paused");
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    if (CallListener != null)
                        CallListener.PauseStateChanged(call, true);
                });
            }
            else if (state == LinphoneCallState.StreamsRunning)
            {
                Logger.Msg("[LinphoneManager] Call running");
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    if (CallListener != null)
                        CallListener.PauseStateChanged(call, false);
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
            else if (state == LinphoneCallState.UpdatedByRemote)
            {
                Boolean videoAdded = false;
                VideoPolicy policy = LinphoneManager.Instance.LinphoneCore.GetVideoPolicy();
                LinphoneCallParams remoteParams = call.GetRemoteParams();
                LinphoneCallParams localParams = call.GetCurrentParamsCopy();
                if (!policy.AutomaticallyAccept && remoteParams.IsVideoEnabled() && !localParams.IsVideoEnabled() && !LinphoneManager.Instance.LinphoneCore.IsInConference())
                {
                    LinphoneManager.Instance.LinphoneCore.DeferCallUpdate(call);
                    videoAdded = true;
                }
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    if (CallListener != null)
                        CallListener.CallUpdatedByRemote(call, videoAdded);
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
        public void DTMFReceived(LinphoneCall call, Char dtmf)
        {

        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void EcCalibrationStatus(EcCalibratorStatus status, int delayMs)
        {
            Logger.Msg("[LinphoneManager] Echo canceller calibration status: " + status.ToString());
            if (status == EcCalibratorStatus.Done)
            {
                Logger.Msg("[LinphoneManager] Echo canceller delay: {0} ms", delayMs);
            }
            if (ECListener != null)
            {
                ECListener.ECStatusNotified(status, delayMs);
            }
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
        /// Listener to let a view to be notified by LinphoneManager when a new message arrives.
        /// </summary>
        public MessageReceivedListener MessageListener { get; set; }

        /// <summary>
        /// Custom message box to display incoming messages when not in chat view
        /// </summary>
        public MessageReceivedNotification MessageReceivedNotification { get; set; }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
            string sipAddress = message.GetFrom().AsStringUriOnly().Replace("sip:", "");
            Logger.Msg("[LinphoneManager] Message received from " + sipAddress + ": " + message.GetText());

            if (MessageListener != null && MessageListener.GetSipAddressAssociatedWithDisplayConversation() != null && MessageListener.GetSipAddressAssociatedWithDisplayConversation().Equals(sipAddress))
            {
                MessageListener.MessageReceived(message);
            }
            else
            {
                DateTime date = new DateTime();
                date = date.AddYears(1969); //Timestamp is calculated from 01/01/1970, and DateTime is initialized to 01/01/0001.
                date = date.AddSeconds(message.GetTime());
                date = date.Add(TimeZoneInfo.Local.GetUtcOffset(date));

                //TODO: Temp hack to remove
                string url = message.GetExternalBodyUrl();
                url = url.Replace("\"", "");
                ChatMessage msg = new ChatMessage { Message = message.GetText(), ImageURL = url, MarkedAsRead = false, IsIncoming = true, LocalContact = sipAddress, RemoteContact = "", Timestamp = (date.Ticks / TimeSpan.TicksPerSecond) };
                DatabaseManager.Instance.Messages.InsertOnSubmit(msg);
                DatabaseManager.Instance.SubmitChanges();

                //Displays the message as a popup
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    if (MessageReceivedNotification != null)
                    {
                        MessageReceivedNotification.Hide();
                    }

                    Popup messageNotif = new Popup();
                    messageNotif.Width = Application.Current.Host.Content.ActualWidth;
                    messageNotif.Height = Application.Current.Host.Content.ActualHeight;
                    messageNotif.VerticalOffset = 25;

                    MessageReceivedNotification = new MessageReceivedNotification(messageNotif, msg);
                    MessageReceivedNotification.ShowClicked += (sender, chatMessage) =>
                    {
                        BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/Chat.xaml?sip=" + chatMessage.Contact, UriKind.RelativeOrAbsolute));
                    };
                    messageNotif.Child = MessageReceivedNotification;
                    messageNotif.IsOpen = true;
                });
            }
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
            if (e.ContactFound != null)
            {
                Logger.Msg("[LinphoneManager] Contact found: " + e.ContactFound.DisplayName);
                ContactManager.ContactFound -= OnContactFound;

                // Store the contact name as display name for call logs
                if (LinphoneManager.Instance.LinphoneCore.GetCurrentCall() != null)
                {
                    LinphoneManager.Instance.LinphoneCore.GetCurrentCall().GetRemoteAddress().SetDisplayName(e.ContactFound.DisplayName);
                }
            }
        }
        #endregion
    }
}
