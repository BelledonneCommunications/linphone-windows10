using Linphone.Agents;
using Linphone.Controls;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
using Linphone.Resources;
using Linphone.Views;
using Microsoft.Phone.Controls;
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
using Windows.Phone.Devices.Notification;
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
                    ConfigureTunnel();
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
                try
                {
                    if (isLinphoneRunning && LinphoneCore.GetDefaultProxyConfig() != null)
                    {
                        _lastKnownState = LinphoneCore.GetDefaultProxyConfig().GetState();
                    }
                }
                catch { }
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
        private static readonly TimeSpan twentySecs = new TimeSpan(0, 0, 20);

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

            try
            {
                BackgroundManager.Instance.OopServer = null;
                server = (Server)WindowsRuntimeMarshal.GetActivationFactory(typeof(Server)).ActivateInstance();
            }
            catch (Exception)
            {
                // Wait for the background process to become ready 
                string backgroundProcessReadyEventName = Globals.GetBackgroundProcessReadyEventName((uint)backgroundProcessID);
                using (EventWaitHandle backgroundProcessReadyEvent = new EventWaitHandle(initialState: false, mode: EventResetMode.ManualReset, name: backgroundProcessReadyEventName))
                {
                    TimeSpan timeout = twentySecs;
                    if (!backgroundProcessReadyEvent.WaitOne(timeout))
                    {
                        // We timed out - something is wrong 
                        throw new InvalidOperationException(string.Format("The background process ({0}) did not become ready in {1} seconds", backgroundProcessID, timeout.Seconds));
                    }
                    else
                    {
                        Debug.WriteLine("[LinphoneManager] Background process {0} is ready", backgroundProcessID);
                    }
                }

                // The background process is now ready. 
                // It is possible that the background process now becomes "not ready" again, but the chances of this happening are slim, 
                // and in that case, the following statement would fail - so, at this point, we don't explicitly guard against this condition. 
                server = (Server)WindowsRuntimeMarshal.GetActivationFactory(typeof(Server)).ActivateInstance();
            }
            finally
            {
                // Create an instance of the server in the background process. 
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

            try
            {
                LinphoneCore.CoreListener = null;
                if (LinphoneCore.GetCallsNb() == 0)
                {
                    LinphoneCore.SetNetworkReachable(false); // To prevent the app from sending an unregister to the server
                    LinphoneCoreFactory.Destroy();
                    Debug.WriteLine("[LinphoneManager] LinphoneCore has been destroyed");
                }
            }
            catch (Exception)
            {
                // Catch "The RPC server is unavailable." exceptions that occur sometimes at this point.
                // This is to clarify why the access to the RPC server is not reliable when called application
                // deactivation handler...
            }

            isLinphoneRunning = false;
            BackgroundProcessConnected = false;
            Debug.WriteLine("[LinphoneManager] Background process disconnected from interface");

            // From this point onwards, it is no longer safe to use any objects in the background process, 
            // or for the background process to call back into this process.
            server = null;
            BackgroundManager.Instance.OopServer = null;

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
            if (server.LinphoneCoreFactory != null && server.LinphoneCore != null)
            {
                // Reconnect the listeners when coming back from background mode
                Debug.WriteLine("[LinphoneManager] LinphoneCore already created, skipping");

                server.LinphoneCore.CoreListener = this;
                // Set user-agent because it is not set if coming back from background mode
                try
                {
                    server.LinphoneCore.SetUserAgent(Customs.UserAgent, XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value);
                    isLinphoneRunning = true;
                    return;
                } catch {
                    // It happens server.LinphoneCore is available but the real core behind is broken, we'll catch this here and force recreate a new core
                    Debug.WriteLine("[LinphoneManager] Exception happened while setting the UA, force creation of a new LinphoneCore");
                }
                //TODO: Crash not fixed
            }

            Debug.WriteLine("[LinphoneManager] Creating LinphoneCore");
            await SettingsManager.InstallConfigFile();
            LpConfig config = server.LinphoneCoreFactory.CreateLpConfig(SettingsManager.GetConfigPath(), SettingsManager.GetFactoryConfigPath());
            ConfigureLogger();
            server.LinphoneCoreFactory.CreateLinphoneCore(this, config);
            server.LinphoneCore.SetRootCA("Assets/rootca.pem");
            Debug.WriteLine("[LinphoneManager] LinphoneCore created");

            AudioRoutingManager.GetDefault().AudioEndpointChanged += AudioEndpointChanged;
            CallController.MuteRequested += MuteRequested;
            CallController.UnmuteRequested += UnmuteRequested;

            if (server.LinphoneCore.IsVideoSupported())
            {
                DetectCameras();
            }

            server.LinphoneCore.SetUserAgent(Customs.UserAgent, XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value);
            AddPushInformationsToContactParams();

            lastNetworkState = DeviceNetworkInformation.IsNetworkAvailable;
            server.LinphoneCore.SetNetworkReachable(lastNetworkState);
            DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(OnNetworkStatusChanged);
            ConfigureTunnel();

            server.LinphoneCore.IterateEnabled = true;
            isLinphoneRunning = true;
        }

        /// <summary>
        /// Sets the push notif infos into proxy config contacts params
        /// </summary>
        public void AddPushInformationsToContactParams()
        {
            if (server.LinphoneCore.GetDefaultProxyConfig() != null)
            {
                string host = null, token = null;
                try
                {
                    host = ((App)App.Current).PushChannelUri.Host;
                    token = ((App)App.Current).PushChannelUri.AbsolutePath;
                }
                catch { }

                if (host == null || token == null)
                {
                    Logger.Warn("Can't set the PN params: {0} {1}", host, token);
                    return;
                }

                if (Customs.AddPasswordInUriContactsParams)
                {
                    SIPAccountSettingsManager sip = new SIPAccountSettingsManager();
                    sip.Load();
                    server.LinphoneCore.GetDefaultProxyConfig().SetContactUriParameters("pwd=" + sip.Password + ";app-id=" + host + ";pn-type=wp;pn-tok=" + token);
                }
                else
                {
                    server.LinphoneCore.GetDefaultProxyConfig().SetContactUriParameters("app-id=" + host + ";pn-type=wp;pn-tok=" + token);
                }
            }
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

        /// <summary>
        /// Configures the Tunnel using the given mode
        /// </summary>
        /// <param name="mode">mode to apply</param>
        public static void ConfigureTunnel(String mode)
        {
            if (LinphoneManager.Instance.LinphoneCore.IsTunnelAvailable())
            {
                Tunnel tunnel = LinphoneManager.Instance.LinphoneCore.GetTunnel();
                if (tunnel != null)
                {
                    if (mode == AppResources.TunnelModeDisabled)
                    {
                        tunnel.Enable(false);
                    }
                    else if (mode == AppResources.TunnelModeAlways)
                    {
                        tunnel.Enable(true);
                    }
                    else if (mode == AppResources.TunnelModeAuto)
                    {
                        tunnel.Enable(false);
                        tunnel.AutoDetect();
                    }
                    else if (mode == AppResources.TunnelMode3GOnly)
                    {
                        if (DeviceNetworkInformation.IsWiFiEnabled)
                        {
                            tunnel.Enable(false);
                        }
                        else if (DeviceNetworkInformation.IsCellularDataEnabled)
                        {
                            tunnel.Enable(true);
                        }
                        else
                        {
                            tunnel.Enable(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Configures the Tunnel using the current setting value
        /// </summary>
        public void ConfigureTunnel()
        {
            NetworkSettingsManager settings = new NetworkSettingsManager();
            settings.Load();
            ConfigureTunnel(settings.TunnelMode);
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
                try
                {
                    if (isMicMuted)
                        CallController.NotifyMuted();
                    else
                        CallController.NotifyUnmuted();
                }
                catch (Exception) { }

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

        /// <summary>
        /// Enables the speaker in the current call
        /// </summary>
        /// <param name="enable">true to enable, false to disable</param>
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
        public void AuthInfoRequested(string realm, string username, string domain)
        {
            Logger.Msg("[LinphoneManager] Auth info requested: realm=" + realm + ", username=" + username + ", domain=" + domain);
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
            if (state == LinphoneCallState.OutgoingProgress)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Outgoing progress");
                    LookupForContact(call);

                    if (CallListener != null)
                    {
                        string sipAddress = call.GetRemoteAddress().AsStringUriOnly();
                        CallListener.NewCallStarted(sipAddress);

                    }
                });
            }
            else if (state == LinphoneCallState.IncomingReceived)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Incoming received"); 
                    if (false) //TODO: Find a proper way to let the user choose between the two.
                    {
                        BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/IncomingCall.xaml?sip=" + call.GetRemoteAddress().AsStringUriOnly(), UriKind.RelativeOrAbsolute));
                        //Remove the current page from the back stack to avoid duplicating him after
                        BaseModel.CurrentPage.NavigationService.RemoveBackEntry();
                    }

                    LookupForContact(call);
                });
            }
            else if (state == LinphoneCallState.Connected)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Connected");
                    if (CallListener != null)
                    {
                        string sipAddress = call.GetRemoteAddress().AsStringUriOnly();
                        CallListener.NewCallStarted(sipAddress);
                    }
                });
            }
            else if (state == LinphoneCallState.CallEnd || state == LinphoneCallState.Error)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Call ended");
                    if (CallListener != null)
                        CallListener.CallEnded(call);
                });
            }
            else if (state == LinphoneCallState.Paused || state == LinphoneCallState.PausedByRemote)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Call paused");
                    bool pausedByRemote = state == LinphoneCallState.PausedByRemote;
                    if (CallListener != null)
                        CallListener.PauseStateChanged(call, !pausedByRemote, pausedByRemote);
                });
            }
            else if (state == LinphoneCallState.StreamsRunning)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Call running");
                    if (CallListener != null)
                        CallListener.PauseStateChanged(call, false, false);
                });
            }
            else if (state == LinphoneCallState.Released)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Call released");
                    //Update tile
                    UpdateLiveTile();
                });
            }
            else if (state == LinphoneCallState.UpdatedByRemote)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
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
            if (config == null)
                return;

            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                try
                {
                    Logger.Msg("[LinphoneManager] Registration state changed: " + state.ToString() + ", message=" + message + " for identity " + config.GetIdentity());
                    LastKnownState = state;
                    if (BasePage.StatusBar != null)
                        BasePage.StatusBar.RefreshStatus(state);
                }
                catch { }
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
        public CustomMessageBox MessageReceivedNotification { get; set; }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
            
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                string sipAddress = message.GetFrom().AsStringUriOnly().Replace("sip:", "");
                Logger.Msg("[LinphoneManager] Message received from " + sipAddress + ": " + message.GetText());

                //Vibrate
                ChatSettingsManager settings = new ChatSettingsManager();
                settings.Load();
                if ((bool)settings.VibrateOnIncomingMessage)
                {
                    VibrationDevice vibrator = VibrationDevice.GetDefault();
                    vibrator.Vibrate(TimeSpan.FromSeconds(1));
                }

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
                    long timestamp = (date.Ticks / TimeSpan.TicksPerSecond);

                    //TODO: Temp hack to remove
                    string url = message.GetExternalBodyUrl();
                    url = url.Replace("\"", "");

                    ChatMessage msg = new ChatMessage { Message = message.GetText(), ImageURL = url, MarkedAsRead = false, IsIncoming = true, LocalContact = sipAddress, RemoteContact = "", Timestamp = timestamp };
                    DatabaseManager.Instance.Messages.InsertOnSubmit(msg);
                    DatabaseManager.Instance.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);

                    //Displays the message as a popup
                    if (MessageReceivedNotification != null)
                    {
                        MessageReceivedNotification.Dismiss();
                    }

                    MessageReceivedNotification = new CustomMessageBox()
                    {
                        Caption = url.Length > 0 ? AppResources.ImageMessageReceived : AppResources.MessageReceived,
                        Message = url.Length > 0 ? "" : message.GetText(),
                        LeftButtonContent = AppResources.Show,
                        RightButtonContent = AppResources.Close
                    };

                    MessageReceivedNotification.Dismissed += (s, e) =>
                        {
                            switch (e.Result)
                            {
                                case CustomMessageBoxResult.LeftButton:
                                    BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/Chat.xaml?sip=" + msg.Contact, UriKind.RelativeOrAbsolute));
                                    break;
                            }
                        };

                    MessageReceivedNotification.Show();

                    //Update tile
                    UpdateLiveTile();
                }
            });
        }

        /// <summary>
        /// Listener to let a view to be notified by LinphoneManager when a composing is received.
        /// </summary>
        public ComposingReceivedListener ComposingListener { get; set; }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void ComposingReceived(LinphoneChatRoom room)
        {
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                if (ComposingListener != null && room != null)
                {
                    string currentListenerSipAddress = ComposingListener.GetSipAddressAssociatedWithDisplayConversation();
                    string roomComposingSipAddress = room.GetPeerAddress().AsStringUriOnly().Replace("sip:", "");

                    if (currentListenerSipAddress != null && roomComposingSipAddress.Equals(currentListenerSipAddress))
                        ComposingListener.ComposeReceived();
                }
            });
        }
        #endregion

        /// <summary>
        /// Updates the app tile to display the number of missed calls and unread chats.
        /// </summary>
        public void UpdateLiveTile()
        {
            int missedCalls = LinphoneCore.GetMissedCallsCount();
            int unreadChats = (from message in DatabaseManager.Instance.Messages where message.MarkedAsRead == false select message).ToList().Count;
            TileManager.Instance.UpdateTileWithMissedCallsAndUnreadMessages(missedCalls + unreadChats);
        }

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
            try
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
            catch 
            {
                Logger.Warn("[LinphoneManager] Execption occured while looking for contact...");
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
