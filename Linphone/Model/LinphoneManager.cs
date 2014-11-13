using Linphone.Agents;
using Linphone.Controls;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
using Linphone.Resources;
using Linphone.Views;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
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
                LinphoneCore.NetworkReachable = lastNetworkState;
            }
        }
        #endregion

        #region Class properties
        private ResourceManager ResourceManager;

        private LinphoneManager()
        {
            LastKnownState = Linphone.Core.RegistrationState.RegistrationNone;
            ResourceManager = new ResourceManager("Linphone.Resources.AppResources", typeof(AppResources).Assembly);
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
                    if (isLinphoneRunning && LinphoneCore.DefaultProxyConfig != null)
                    {
                        _lastKnownState = LinphoneCore.DefaultProxyConfig.State;
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
        private static readonly TimeSpan twoSecs = new TimeSpan(0, 0, 2);

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
            TileManager.Instance.RemoveCountOnTile();
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
                server = BackgroundManager.Instance.OopServer;
            }
            catch (Exception)
            {
                // Wait for the background process to become ready 
                string backgroundProcessReadyEventName = Globals.GetBackgroundProcessReadyEventName((uint)backgroundProcessID);
                using (EventWaitHandle backgroundProcessReadyEvent = new EventWaitHandle(initialState: false, mode: EventResetMode.ManualReset, name: backgroundProcessReadyEventName))
                {
                    TimeSpan timeout = twoSecs;
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
                server = BackgroundManager.Instance.OopServer;
            }
            
            // Un-set an event that indicates that the UI process is disconnected from the background process. 
            // The background process waits for this event to get set before shutting down. 
            // This ensures that the background agent host process doesn't shut down while the UI process is connected to it. 
            string uiDisconnectedEventName = Globals.GetUiDisconnectedEventName((uint)backgroundProcessID);
            uiDisconnectedEvent = new EventWaitHandle(initialState: false, mode: EventResetMode.ManualReset, name: uiDisconnectedEventName);
            uiDisconnectedEvent.Reset();

            BackgroundProcessConnected = true;
            Debug.WriteLine("[LinphoneManager] Background process connected to interface");

            // Create LinphoneCore if not created yet, otherwise do nothing
            InitLinphoneCore();
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
                if (LinphoneCore.CallsNb == 0)
                {
                    LinphoneCore.NetworkReachable = false; // To prevent the app from sending an unregister to the server
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
        public void InitLinphoneCore()
        {
            if (server.LinphoneCoreFactory != null && server.LinphoneCore != null)
            {
                // Reconnect the listeners when coming back from background mode
                Logger.Dbg("[LinphoneManager] LinphoneCore already created, skipping");

                server.LinphoneCore.CoreListener = this;
                // Set user-agent because it is not set if coming back from background mode
                try
                {
                    server.LinphoneCore.SetUserAgent(Customs.UserAgent, Linphone.Version.Number);
                    isLinphoneRunning = true;
                    return;
                } catch {
                    // It happens server.LinphoneCore is available but the real core behind is broken, we'll catch this here and force recreate a new core
                    Logger.Dbg("[LinphoneManager] Exception happened while setting the UA, force creation of a new LinphoneCore");
                }
            }

            Debug.WriteLine("[LinphoneManager] Creating LinphoneCore");
            InitManager.CreateLinphoneCore(server, this, LogLevel);
            Logger.Dbg("[LinphoneManager] LinphoneCore created");

            AudioRoutingManager.GetDefault().AudioEndpointChanged += AudioEndpointChanged;
            CallController.MuteRequested += MuteRequested;
            CallController.UnmuteRequested += UnmuteRequested;

            if (server.LinphoneCore.VideoSupported)
            {
                DetectCameras();
            }

            server.LinphoneCore.SetUserAgent(Customs.UserAgent, Linphone.Version.Number);
            AddPushInformationsToContactParams();

            lastNetworkState = DeviceNetworkInformation.IsNetworkAvailable;
            server.LinphoneCore.NetworkReachable = lastNetworkState;
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
            if (server.LinphoneCore.DefaultProxyConfig != null)
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
                    Logger.Warn("Can't set the PN params: {0} {1}\r\n", host, token);
                    return;
                }

                if (Customs.AddPasswordInUriContactsParams)
                {
                    SIPAccountSettingsManager sip = new SIPAccountSettingsManager();
                    sip.Load();
                    server.LinphoneCore.DefaultProxyConfig.ContactUriParameters = "pwd=" + sip.Password + ";app-id=" + host + ";pn-type=wp;pn-tok=" + token;
                }
                else
                {
                    server.LinphoneCore.DefaultProxyConfig.ContactUriParameters = "app-id=" + host + ";pn-type=wp;pn-tok=" + token;
                }
            }
        }

        public OutputTraceLevel LogLevel
        {
            get
            {
                ApplicationSettingsManager appSettings = new ApplicationSettingsManager();
                appSettings.Load();
                return appSettings.LogLevel;
            }
        }

        /// <summary>
        /// Configures the Logger
        /// </summary>
        public void ConfigureLogger()
        {
            InitManager.ConfigureLogger(server, LogLevel);
        }

        /// <summary>
        /// Configures the Tunnel using the given mode
        /// </summary>
        /// <param name="mode">mode to apply</param>
        public static void ConfigureTunnel(String mode)
        {
            if (LinphoneManager.Instance.LinphoneCore.TunnelAvailable)
            {
                Tunnel tunnel = LinphoneManager.Instance.LinphoneCore.Tunnel;
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
            foreach (LinphoneCallLog log in LinphoneManager.Instance.LinphoneCore.CallLogs)
            {
                if (log.Direction == CallDirection.Outgoing)
                {
                    return log.To.AsStringUriOnly();
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

            if (LinphoneCore.CallLogs != null)
            {
                foreach (LinphoneCallLog log in LinphoneCore.CallLogs)
                {
                    string from = log.From.DisplayName;
                    if (from.Length == 0)
                    {
                        LinphoneAddress fromAddress = log.From;
                        from = String.Format("{0}@{1}", fromAddress.UserName, fromAddress.Domain);
                    }

                    string to = log.To.DisplayName;
                    if (to.Length == 0)
                    {
                        LinphoneAddress toAddress = log.To;
                        to = String.Format("{0}@{1}", toAddress.UserName, toAddress.Domain);
                    }

                    bool isMissed = log.Status == LinphoneCallStatus.Missed;
                    long startDate = log.StartDate;
                    CallLog callLog = new CallLog(log, from, to, log.Direction == CallDirection.Incoming, isMissed, startDate);
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
            LinphoneCall call = LinphoneCore.CurrentCall;
            if (call != null)
            {
                LinphoneCore.TerminateCall(call);
            }
            else
            {
                foreach (LinphoneCall lCall in LinphoneCore.Calls)
                {
                    if (lCall.State == LinphoneCallState.Paused)
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
            if (BaseModel.UIDispatcher == null) return;
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                if (LinphoneCore.CallsNb > 0)
                {
                    LinphoneCore.MicMuted = isMicMuted;
                    if (CallListener != null)
                        CallListener.MuteStateChanged(isMicMuted);
                }
            });
        }

        private void UnmuteRequested(VoipCallCoordinator sender, MuteChangeEventArgs args)
        {
            Debug.WriteLine("[LinphoneManager] Unmute requested");
            MuteMic(true);
        }

        private void MuteRequested(VoipCallCoordinator sender, MuteChangeEventArgs args)
        {
            Debug.WriteLine("[LinphoneManager] Mute requested");
            MuteMic(false);
        }

        /// <summary>
        /// Pauses the current call if any and if it's running.
        /// </summary>
        public void PauseCurrentCall()
        {
            if (LinphoneCore.CallsNb > 0)
            {
                LinphoneCall call = LinphoneCore.CurrentCall;
                LinphoneCore.PauseCall(call);
            }
        }

        /// <summary>
        /// Resume the current call if any and if it's paused.
        /// </summary>
        public void ResumeCurrentCall()
        {
            foreach (LinphoneCall call in LinphoneCore.Calls) {
                if (call.State == LinphoneCallState.Paused)
                {
                    LinphoneCore.ResumeCall(call);
                }
            }
        }

        private void CallResumeRequested(VoipPhoneCall sender, CallStateChangeEventArgs args)
        {
            Logger.Msg("[LinphoneManager] Resume requested\r\n");
            ResumeCurrentCall();
        }

        private void CallHoldRequested(VoipPhoneCall sender, CallStateChangeEventArgs args)
        {
            Logger.Msg("[LinphoneManager] Pause requested\r\n");
            PauseCurrentCall();
        }
        #endregion

        #region Audio route handling
        private void AudioEndpointChanged(AudioRoutingManager sender, object args)
        {
            Logger.Msg("[LinphoneManager] AudioEndpointChanged:" + sender.GetAudioEndpoint().ToString() + "\r\n");
        }

        /// <summary>
        /// Property that handles the audio routing between the speaker and the earpiece.
        /// </summary>
        public bool SpeakerEnabled
        {
            get
            {
                return AudioRoutingManager.GetDefault().GetAudioEndpoint() == AudioRoutingEndpoint.Speakerphone;
            }
            set
            {
                if (value)
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Speakerphone);
                else
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Earpiece);
            }
        }

        /// <summary>
        /// Property that handles the audio routing between the speaker and the earpiece.
        /// </summary>
        public bool BluetoothEnabled
        {
            get
            {
                var audioRoute = AudioRoutingManager.GetDefault().GetAudioEndpoint();
                return audioRoute == AudioRoutingEndpoint.Bluetooth || audioRoute == AudioRoutingEndpoint.BluetoothWithNoiseAndEchoCancellation;
            }
            set
            {
                if (value)
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Bluetooth);
                else
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Earpiece);
            }
        }

        /// <summary>
        /// Returns true if the Bluetooth audio route is available
        /// </summary>
        public bool IsBluetoothAvailable
        {
            get
            {
                return (AudioRoutingManager.GetDefault().AvailableAudioEndpoints & AvailableAudioRoutingEndpoints.Bluetooth) != 0;
            }
        }
        #endregion

        #region Video handling
        /// <summary>
        /// Returns true if the video is available
        /// </summary>
        public bool IsVideoAvailable
        {
            get
            {
                return LinphoneCore.VideoSupported && (LinphoneCore.VideoDisplayEnabled || LinphoneCore.VideoCaptureEnabled);
            }
        }

        /// <summary>
        /// Enables disables video.
        /// </summary>
        /// <param name="enable">Wether to enable or disable video</param>
        /// <returns>true if the operation has been successful, false otherwise</returns>
        public bool EnableVideo(bool enable)
        {
            if (LinphoneCore.InCall)
            {
                LinphoneCall call = LinphoneCore.CurrentCall;
                LinphoneCallParams parameters = call.GetCurrentParamsCopy();
                if (enable != parameters.VideoEnabled)
                {
                    parameters.VideoEnabled = enable;
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

        private String frontCamera = null;
        private String backCamera = null;

        private void DetectCameras()
        {
            int nbCameras = 0;
            foreach (String device in LinphoneCore.VideoDevices)
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
            String currentDevice = LinphoneCore.VideoDevice;
            if ((currentDevice != frontCamera) && (currentDevice != backCamera))
            {
                if (frontCamera != null)
                {
                    LinphoneCore.VideoDevice = frontCamera;
                }
                else if (backCamera != null)
                {
                    LinphoneCore.VideoDevice = backCamera;
                }
            }
        }

        /// <summary>
        /// Gets the number of cameras available on the device (int).
        /// </summary>
        public int NumberOfCameras
        {
            get
            {
                return LinphoneCore.VideoDevices.Count;
            }
        }

        /// <summary>
        /// Toggles the camera used for video capture.
        /// </summary>
        public void ToggleCameras()
        {
            if (NumberOfCameras >= 2)
            {
                String currentDevice = LinphoneCore.VideoDevice;
                if (currentDevice == frontCamera)
                {
                    LinphoneCore.VideoDevice = backCamera;
                }
                else if (currentDevice == backCamera)
                {
                    LinphoneCore.VideoDevice = frontCamera;
                }
                if (LinphoneCore.InCall)
                {
                    LinphoneCall call = LinphoneCore.CurrentCall;
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
            Logger.Msg("[LinphoneManager] Auth info requested: realm=" + realm + ", username=" + username + ", domain=" + domain + "\r\n");
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void GlobalState(GlobalState state, string message)
        {
            Logger.Msg("[LinphoneManager] Global state changed: " + state.ToString() + ", message=" + message + "\r\n");
        }

        public delegate void CallStateChangedEventHandler(LinphoneCall call, LinphoneCallState state);
        public event CallStateChangedEventHandler CallStateChanged;

        private void ShowCallError(string message)
        {
            if (CallErrorNotification != null)
            {
                CallErrorNotification.Dismiss();
            }
            CallErrorNotification = new CustomMessageBox()
            {
                Caption = ResourceManager.GetString("CallError", CultureInfo.CurrentCulture),
                Message = message,
                RightButtonContent = AppResources.Close
            };
            CallErrorNotification.Show();
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void CallState(LinphoneCall call, LinphoneCallState state, string message)
        {
            if (BaseModel.UIDispatcher == null) return;
            if (state == LinphoneCallState.OutgoingProgress)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Outgoing progress\r\n");
                    LookupForContact(call);

                    if (CallListener != null)
                    {
                        string sipAddress = call.RemoteAddress.AsStringUriOnly();
                        CallListener.NewCallStarted(sipAddress);
                    }
                });
            }
            else if (state == LinphoneCallState.IncomingReceived)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Incoming received\r\n"); 
                    if (false) //TODO: Find a proper way to let the user choose between the two.
                    {
                        BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/IncomingCall.xaml?sip=" + call.RemoteAddress.AsStringUriOnly(), UriKind.RelativeOrAbsolute));
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
                    Logger.Msg("[LinphoneManager] Connected\r\n");
                    if (CallListener != null)
                    {
                        string sipAddress = call.RemoteAddress.AsStringUriOnly();
                        CallListener.NewCallStarted(sipAddress);
                    }
                });
            }
            else if (state == LinphoneCallState.CallEnd || state == LinphoneCallState.Error)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg(String.Format("[LinphoneManager] Call ended: {0}\r\n", message));
                    if (CallListener != null)
                        CallListener.CallEnded(call);
                    string text;
                    switch (call.Reason)
                    {
                        case Reason.LinphoneReasonNone:
                        case Reason.LinphoneReasonNotAnswered:
                            break;
                        case Reason.LinphoneReasonDeclined:
                            if (call.Direction == CallDirection.Outgoing)
                            {
                                text = ResourceManager.GetString("CallErrorDeclined", CultureInfo.CurrentCulture);
                                ShowCallError(text.Replace("#address#", call.RemoteAddress.UserName));
                            }
                            break;
                        case Reason.LinphoneReasonNotFound:
                            text = ResourceManager.GetString("CallErrorNotFound", CultureInfo.CurrentCulture);
                            ShowCallError(text.Replace("#address#", call.RemoteAddress.UserName));
                            break;
                        case Reason.LinphoneReasonBusy:
                            text = ResourceManager.GetString("CallErrorBusy", CultureInfo.CurrentCulture);
                            ShowCallError(text.Replace("#address#", call.RemoteAddress.UserName));
                            break;
                        case Reason.LinphoneReasonNotAcceptable:
                            ShowCallError(ResourceManager.GetString("CallErrorNotAcceptable", CultureInfo.CurrentCulture));
                            break;
                        default:
                            ShowCallError(ResourceManager.GetString("CallErrorUnknown", CultureInfo.CurrentCulture));
                            break;
                    }
                });
            }
            else if (state == LinphoneCallState.Paused || state == LinphoneCallState.PausedByRemote)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Call paused\r\n");
                    bool pausedByRemote = state == LinphoneCallState.PausedByRemote;
                    if (CallListener != null)
                        CallListener.PauseStateChanged(call, !pausedByRemote, pausedByRemote);
                });
            }
            else if (state == LinphoneCallState.StreamsRunning)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Call running\r\n");
                    if (CallListener != null)
                        CallListener.PauseStateChanged(call, false, false);
                });
            }
            else if (state == LinphoneCallState.Released)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Logger.Msg("[LinphoneManager] Call released\r\n");
                });
            }
            else if (state == LinphoneCallState.UpdatedByRemote)
            {
                BaseModel.UIDispatcher.BeginInvoke(() =>
                {
                    Boolean videoAdded = false;
                    VideoPolicy policy = LinphoneManager.Instance.LinphoneCore.VideoPolicy;
                    LinphoneCallParams remoteParams = call.RemoteParams;
                    LinphoneCallParams localParams = call.GetCurrentParamsCopy();
                    if (!policy.AutomaticallyAccept && remoteParams.VideoEnabled && !localParams.VideoEnabled && !LinphoneManager.Instance.LinphoneCore.InConference)
                    {
                        LinphoneManager.Instance.LinphoneCore.DeferCallUpdate(call);
                        videoAdded = true;
                    }

                    if (CallListener != null)
                        CallListener.CallUpdatedByRemote(call, videoAdded);
                });
            }

            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                if (CallStateChanged != null)
                {
                    CallStateChanged(call, state);
                }
            });
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void RegistrationState(LinphoneProxyConfig config, RegistrationState state, string message)
        {
            if ((config == null) || BaseModel.UIDispatcher == null) return;
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                try
                {
                    Logger.Msg("[LinphoneManager] Registration state changed: " + state.ToString() + ", message=" + message + " for identity " + config.GetIdentity() + "\r\n");
                    LastKnownState = state;
                    if (BasePage.StatusBar != null)
                        BasePage.StatusBar.RefreshStatus(state);
                    if ((state == Core.RegistrationState.RegistrationFailed) && (config.Error == Reason.LinphoneReasonForbidden))
                    {
                        if (RegistrationFailedNotification != null)
                        {
                            RegistrationFailedNotification.Dismiss();
                        }
                        RegistrationFailedNotification = new CustomMessageBox()
                        {
                            Caption = ResourceManager.GetString("RegistrationFailedPopupTitle", CultureInfo.CurrentCulture),
                            Message = ResourceManager.GetString("RegistrationFailedForbidden", CultureInfo.CurrentCulture),
                            RightButtonContent = AppResources.Close
                        };
                        RegistrationFailedNotification.Show();
                    }
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
            Logger.Msg("[LinphoneManager] Echo canceller calibration status: " + status.ToString() + "\r\n");
            if (status == EcCalibratorStatus.Done)
            {
                Logger.Msg("[LinphoneManager] Echo canceller delay: {0} ms\r\n", delayMs);
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
        /// Custom message box to display call errors.
        /// </summary>
        public CustomMessageBox CallErrorNotification { get; set; }

        /// <summary>
        /// Custom message box to display registration failures.
        /// </summary>
        public CustomMessageBox RegistrationFailedNotification { get; set; }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void MessageReceived(LinphoneChatMessage message)
        {
            if (BaseModel.UIDispatcher == null) return;
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                LinphoneAddress fromAddress = message.From;
                string sipAddress = String.Format("{0}@{1}", fromAddress.UserName, fromAddress.Domain);
                Logger.Msg("[LinphoneManager] Message received from " + sipAddress + ": " + message.Text + "\r\n");

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
                    DateTime date = new DateTime(message.Time * TimeSpan.TicksPerSecond, DateTimeKind.Utc).AddYears(1969).ToLocalTime();
                    DateTime now = DateTime.Now;
                    string dateStr;
                    if (now.Year == date.Year && now.Month == date.Month && now.Day == date.Day)
                        dateStr = String.Format("{0:HH:mm}", date);
                    else if (now.Year == date.Year)
                        dateStr = String.Format("{0:ddd d MMM, HH:mm}", date);
                    else
                        dateStr = String.Format("{0:ddd d MMM yyyy, HH:mm}", date);

                    //TODO: Temp hack to remove
                    string url = message.ExternalBodyUrl;
                    url = url.Replace("\"", "");

                    //Displays the message as a popup
                    if (MessageReceivedNotification != null)
                    {
                        MessageReceivedNotification.Dismiss();
                    }

                    MessageReceivedNotification = new CustomMessageBox()
                    {
                        Title = dateStr,
                        Caption = url.Length > 0 ? AppResources.ImageMessageReceived : AppResources.MessageReceived,
                        Message = url.Length > 0 ? "" : message.Text,
                        LeftButtonContent = AppResources.Close,
                        RightButtonContent = AppResources.Show
                    };

                    MessageReceivedNotification.Dismissed += (s, e) =>
                    {
                        switch (e.Result)
                        {
                            case CustomMessageBoxResult.RightButton:
                                BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/Chat.xaml?sip=" + message.PeerAddress.AsStringUriOnly(), UriKind.RelativeOrAbsolute));
                                break;
                        }
                    };

                    MessageReceivedNotification.Show();
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
            if (BaseModel.UIDispatcher == null) return;
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                if (ComposingListener != null && room != null)
                {
                    string currentListenerSipAddress = ComposingListener.GetSipAddressAssociatedWithDisplayConversation();
                    LinphoneAddress peerAddress = room.PeerAddress;
                    string roomComposingSipAddress = String.Format("{0}@{1}", peerAddress.UserName, peerAddress.Domain);

                    if (currentListenerSipAddress != null && roomComposingSipAddress.Equals(currentListenerSipAddress))
                        ComposingListener.ComposeReceived();
                }
            });
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void FileTransferProgressIndication(LinphoneChatMessage message, int offset, int total)
        {
            Logger.Msg(String.Format("FileTransferProgressIndication: {0}/{1}", offset, total));
        }

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void LogUploadStatusChanged(LinphoneCoreLogCollectionUploadState state, string info)
        {
            if (BaseModel.UIDispatcher == null) return;
            BaseModel.UIDispatcher.BeginInvoke(() =>
            {
                if (state == LinphoneCoreLogCollectionUploadState.LinphoneCoreLogCollectionUploadStateDelivered)
                {
                    BugCollector.ReportExceptions(info);
                }
                else if (state == LinphoneCoreLogCollectionUploadState.LinphoneCoreLogCollectionUploadStateNotDelivered) 
                {
                    Logger.Err(String.Format("[LinphoneManager] Logs upload error: {0}", info));
                    var notif = new CustomMessageBox()
                    {
                        Caption = "Logfile upload failed",
                        Message = info,
                        RightButtonContent = AppResources.Close
                    };
                    notif.Show();
                }
            });
        }

        public delegate void LogUploadProgressIndicationEventHandler(int offset, int total);
        public event LogUploadProgressIndicationEventHandler LogUploadProgressIndicationEH;

        /// <summary>
        /// Callback for LinphoneCoreListener
        /// </summary>
        public void LogUploadProgressIndication(int offset, int total)
        {
            if (LogUploadProgressIndicationEH != null)
            {
                LogUploadProgressIndicationEH(offset, total);
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
            try
            {
                LinphoneAddress remoteAddress = call.RemoteAddress;
                if (remoteAddress.DisplayName.Length == 0)
                {
                    string sipAddress = String.Format("{0}@{1}", remoteAddress.UserName, remoteAddress.Domain);
                    Logger.Msg("[LinphoneManager] Display name null, looking for remote address in contact: " + sipAddress + "\r\n");

                    ContactManager.ContactFound += OnContactFound;
                    ContactManager.FindContact(sipAddress);
                }
                else
                {
                    Logger.Msg("[LinphoneManager] Display name found: " + call.RemoteAddress.DisplayName + "\r\n");
                }
            }
            catch 
            {
                Logger.Warn("[LinphoneManager] Exception occured while looking for contact...\r\n");
            }
        }

        /// <summary>
        /// Callback called when the search on a phone number for a contact has a match
        /// </summary>
        private void OnContactFound(object sender, ContactFoundEventArgs e)
        {
            if (e.ContactFound != null)
            {
                Logger.Msg("[LinphoneManager] Contact found: " + e.ContactFound.DisplayName + "\r\n");
                // Store the contact name as display name for call logs
                if (LinphoneManager.Instance.LinphoneCore.CurrentCall != null)
                {
                    LinphoneManager.Instance.LinphoneCore.CurrentCall.RemoteAddress.DisplayName = e.ContactFound.DisplayName;
                }
            }
            ContactManager.ContactFound -= OnContactFound;
        }
        #endregion
    }
}
