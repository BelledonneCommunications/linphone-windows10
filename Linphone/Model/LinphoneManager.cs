/*
LinphoneManager.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using System.Diagnostics;
using Linphone;
using System.IO;
using Windows.Storage;
using Windows.Phone.Media.Devices;
using Windows.Networking.PushNotifications;
using Windows.ApplicationModel.Calls;
using LinphoneTasks;
using Windows.Networking.Connectivity;
using System.Text;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.System.Threading;
using Linphone.Views;
using Windows.Media.Audio;
using System.Threading.Tasks;
using System.Threading;// Mutex
using Windows.UI.Xaml.Controls;//SwapChainPanel


using System.Runtime.InteropServices;
using EGLNativeWindowType = System.IntPtr;
using OpenGlFunctions = System.IntPtr;
using GLuint = System.UInt32;

namespace Linphone.Model {
    class LinphoneManager{
        private static LinphoneManager _instance = new LinphoneManager();
        public static LinphoneManager Instance {
            get {
                return _instance;
            }
        }

        private Core _core;
        private CoreListener _coreListener;
        public bool isLinphoneRunning = false;
        public static bool mRequestShowVideo = false;
        public static SwapChainPanel mainVideoPanel = null;
        public static SwapChainPanel previewVideoPanel = null;



        public static void StartVideoStream(SwapChainPanel main, SwapChainPanel preview) {
            mainVideoPanel = main;
            previewVideoPanel = preview;
            mRequestShowVideo = true;// Use to initialize surface in Iterate thread
        }
        public static void StopVideoStream()
        {
            mainVideoPanel = null;
            previewVideoPanel = null;
            mRequestShowVideo = true;// Use to initialize surface in Iterate thread
        }
        private PushNotificationChannel channel;

        #region LinphoneCore and initialization

        public Core Core {
            get {
                if (_core == null) {
                    Linphone.LoggingService.Instance.LogLevel = Linphone.LogLevel.Debug;
                    ConfigurePaths();
                   
                    _core = Factory.Instance.CreateCore(GetConfigPath(), GetFactoryConfigPath(), IntPtr.Zero);
                    _coreListener = _core.Listener;
                    coreListenerInit();
                    _core.Start();
                }
                return _core;
            }
        }

        private void coreListenerInit() {
            if (_coreListener == null)
                return;
            _coreListener.OnMessageReceived = this.OnMessageReceived;
            _coreListener.OnIsComposingReceived = this.IsComposingReceived;
            //_coreListener.Requ = this.AuthInfoRequested;
            _coreListener.OnCallEncryptionChanged = this.CallEncryptionChanged;
            _coreListener.OnCallStateChanged = this.CallStateChanged;
            _coreListener.OnCallStatsUpdated = this.CallStatsUpdated;
            _coreListener.OnDtmfReceived = this.DtmfReceived;
            _coreListener.OnGlobalStateChanged = this.GlobalStateChanged;
            _coreListener.OnLogCollectionUploadProgressIndication = this.LogCollectionUploadProgressIndication;
            _coreListener.OnLogCollectionUploadStateChanged = this.LogCollectionUploadStateChanged;
            _coreListener.OnRegistrationStateChanged = this.RegistrationStateChanged;
            _coreListener.OnTransferStateChanged = this.TransferStateChanged;
        }

        public CoreDispatcher CoreDispatcher {
            get; set;
        }

        public CoreListener getCoreListener() {
            return this._coreListener;
        }

        public LinphoneManager() {
            Init();
        }

        private async void Init() {
            var vcc = VoipCallCoordinator.GetDefault();
            var entryPoint = typeof(PhoneCallTask).FullName;
            var status = await vcc.ReserveCallResourcesAsync(entryPoint);
        }

        public async void InitPushNotifications() {
            var internetProfile = NetworkInformation.GetInternetConnectionProfile();
            if (internetProfile != null) {
                try
                {
                    channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                    AddPushInformationsToContactParams();
                } catch{
                    Debug.WriteLine("[LinphoneManager] Cannot use Notification \r\n");
                }
            }
        }
        public String GetChatDatabasePath() {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, "chat.db");
        }

        public String GetDefaultConfigPath() {
            return Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "linphonerc");
        }

        public String GetConfigPath() {
            FileInfo fInfo = new FileInfo(Path.Combine(ApplicationData.Current.LocalFolder.Path, "linphonerc"));
            if (fInfo.Exists) {
                return Path.Combine(ApplicationData.Current.LocalFolder.Path, "linphonerc");
            } else {
                return null;
            }
        }

        public String GetFactoryConfigPath() {
            return Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "linphonerc-factory");
        }

        public String GetRootCaPath() {
            return Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "Linphone", "rootca.pem");
        }

        public String GetCertificatesPath() {
            return ApplicationData.Current.LocalFolder.Path;
        }

        
        public void CleanMemory(IntPtr context)
        {
            if (context != IntPtr.Zero)
                Marshal.FreeHGlobal(context);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct ContextInfo
        {
            public EGLNativeWindowType window;
            public GLuint width;
            public GLuint height;
            public OpenGlFunctions functions;
        };

        public void CreateRenderSurface(SwapChainPanel panel, bool isPreview, bool freeOldMemory)
        {// Need to convert C# object into C++. Warning to memory leak
            IntPtr oldData = IntPtr.Zero;// Used to release memory after assignation
            ContextInfo c;
            if (panel != null)
                c.window = Marshal.GetIUnknownForObject(panel);
            else
                c.window = IntPtr.Zero;
            c.functions = IntPtr.Zero;
            c.width = 0;
            c.height = 0;
            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(c));
            Marshal.StructureToPtr(c, pnt, false);
            if (isPreview)
            {
                oldData = LinphoneManager.Instance.Core.NativePreviewWindowId;
                LinphoneManager.Instance.Core.NativePreviewWindowId = pnt;
            }
            else
            {
                oldData = LinphoneManager.Instance.Core.NativeVideoWindowId;
                LinphoneManager.Instance.Core.NativeVideoWindowId = pnt;
            }
            if (freeOldMemory && oldData != IntPtr.Zero)
                CleanMemory(oldData);
        }
        private object renderLock = new Object();
        public void InitLinphoneCore() {
            LinphoneManager.Instance.Core.RootCa = GetRootCaPath();
            LinphoneManager.Instance.Core.UserCertificatesPath = GetCertificatesPath();

            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1)) {
                AudioRoutingManager.GetDefault().AudioEndpointChanged += AudioEndpointChanged;
            }

            if (LinphoneManager.Instance.Core.VideoSupported()) {
                LinphoneManager.Instance.Core.VideoDisplayFilter = "MSOGL";
                LinphoneManager.Instance.Core.VideoCaptureEnabled = true;
                CreateRenderSurface(null, true, false);
                CreateRenderSurface(null, false, false);
                DetectCameras();
            }
            LinphoneManager.Instance.Core.UsePreviewWindow(true);
            LinphoneManager.Instance.Core.SetUserAgent("LinphoneW10", Core.Version);
            if (LinphoneManager.Instance.Core.Config != null) {
                EnableLogCollection(
                    (LinphoneManager.Instance.Core.Config.GetInt("app", "LogLevel", (int)LogCollectionState.Disabled) == 1) ? true : false);
            }
            InitPushNotifications();
            isLinphoneRunning = true;
            TimeSpan period = TimeSpan.FromMilliseconds(40);
            ThreadPoolTimer.CreatePeriodicTimer((source) => {
            CoreDispatcher.RunIdleAsync((args) =>
              {
                    Core.Iterate();
                        if(mRequestShowVideo)
                        {
                          CreateRenderSurface(previewVideoPanel, true, true);
                          CreateRenderSurface(mainVideoPanel, false, true);
                          mRequestShowVideo = false;
                        }
                        lock (renderLock)
                        {
                            LinphoneManager.Instance.Core.PreviewOglRender();
                            if (LinphoneManager.Instance.Core.CurrentCall != null)
                                LinphoneManager.Instance.Core.CurrentCall.OglRender();
                        }
                });
            }, period);
        }

        private void ConfigurePaths() {
            string packagePath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            string assetsPath = packagePath + "\\Assets";
            Factory.Instance.TopResourcesDir = assetsPath;
            Factory.Instance.DataResourcesDir = assetsPath;
            Factory.Instance.SoundResourcesDir = assetsPath + "\\sounds\\linphone";
            Factory.Instance.RingResourcesDir = Factory.Instance.SoundResourcesDir + "\\rings";
            Factory.Instance.ImageResourcesDir = assetsPath + "\\images";
            Factory.Instance.MspluginsDir = ".";
        }

        public void EnableLogCollection(bool enable) {
            Linphone.Core.EnableLogCollection(enable ? LogCollectionState.Enabled : LogCollectionState.Disabled);
            if (enable) Linphone.LoggingService.Instance.LogLevel = Linphone.LogLevel.Debug;
            Linphone.Core.SetLogCollectionPath(ApplicationData.Current.LocalFolder.Path);
        }


        public void AddPushInformationsToContactParams() {
            if (Core.DefaultProxyConfig != null && channel != null) {
                Uri pushUri = new Uri(channel.Uri);
                string host = null, token = null;

                host = pushUri.Host;
                token = pushUri.OriginalString;

                if (host == null || token == null) {
                    return;
                }

                byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(token);
                String tokenB64 = Convert.ToBase64String(toEncodeAsBytes);

                if (Core.DefaultProxyConfig != null) {
                    Core.DefaultProxyConfig.Edit();
                    Core.DefaultProxyConfig.ContactUriParameters = "app-id=" + host + ";pn-type=w10;pn-tok=" + tokenB64;
                    Core.DefaultProxyConfig.Done();
                }
            }
        }

        public String getCoreVersion() {
            return Core.Version;
        }

        /*public void ConfigureLog(OutputTraceLevel level) {
            Core.LogLevel = level;
        }*/

        public void resetLogCollection() {
            Core.ResetLogCollection();
        }

        public bool isMobileVersion() {
            return ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1);
        }

#endregion

#region CallLogs
        private List<CallLogModel> _history;

        public List<CallLogModel> GetCallsHistory() {
            _history = new List<CallLogModel>();

            if (Core.CallLogs != null) {
                foreach (CallLog log in Core.CallLogs) {
                    string from = log.FromAddress.DisplayName;
                    if (from == null || from.Length == 0) {
                        Address fromAddress = log.FromAddress;
                        from = fromAddress.AsStringUriOnly();
                    }

                    string to = log.ToAddress.DisplayName;
                    if (to == null || to.Length == 0) {
                        Address toAddress = log.ToAddress;
                        to = toAddress.AsStringUriOnly();
                    }

                    bool isMissed = log.Status == CallStatus.Missed;
                    long startDate = log.StartDate;
                    CallLogModel callLog = new CallLogModel(log, from, to, log.Dir == CallDir.Incoming, isMissed, startDate);
                    _history.Add(callLog);
                }
            }

            return _history;
        }

        public string GetLastCalledNumber() {
            foreach (CallLog log in LinphoneManager.Instance.Core.CallLogs) {
                if (log.Dir == CallDir.Outgoing) {
                    return log.ToAddress.AsStringUriOnly();
                }
            }
            return null;
        }

        public void RemoveCallLogs(IEnumerable<CallLogModel> logsToRemove) {
            for (int i = 0 ; i < logsToRemove.Count() ; i++) {
                CallLogModel logToRemove = logsToRemove.ElementAt(i);
                Core.RemoveCallLog(logToRemove.NativeLog as CallLog);
            }
        }

        public void ClearCallLogs() {
            Core.ClearCallLogs();
        }
#endregion

#region Call Management
        public void PauseCurrentCall() {
            if (Core.CallsNb > 0) {
                Call call = Core.CurrentCall;
                call.Pause();
            }
        }

        public void ResumeCurrentCall() {
            foreach (Call call in Core.Calls) {
                if (call.State == CallState.Paused) {
                    call.Resume();
                }
            }
        }

        public async void NewOutgoingCall(String sipAddress) {
            // Workaround to pop the microphone permission window
            await openMicrophonePopup();

            Call LCall = Core.Invite(sipAddress);
        }

        public void EndCurrentCall() {
            Call call = Core.CurrentCall;
            if (call != null) {
                call.Terminate();
            } else {
                foreach (Call lCall in Core.Calls) {
                    if (lCall.State == CallState.Paused) {
                        lCall.Terminate();
                    }
                }
            }
        }

        private void ShowCallError(string message) {
            /*if (CallErrorNotification != null)
            {
                CallErrorNotification.Dismiss();
            }
            CallErrorNotification = new CustomMessageBox()
            {
                Caption = ResourceManager.GetString("CallError", CultureInfo.CurrentCulture),
                Message = message,
                RightButtonContent = AppResources.Close
            };
            CallErrorNotification.Show();*/
        }
#endregion

#region Audio Management

        private void AudioEndpointChanged(AudioRoutingManager sender, object args) {
            Debug.WriteLine("[LinphoneManager] AudioEndpointChanged:" + sender.GetAudioEndpoint().ToString() + "\r\n");
        }

        public bool SpeakerEnabled {
            get {
                return AudioRoutingManager.GetDefault().GetAudioEndpoint() == AudioRoutingEndpoint.Speakerphone;
            }
            set {
                if (value)
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Speakerphone);
                else
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Earpiece);
            }
        }

        public bool BluetoothEnabled {
            get {
                var audioRoute = AudioRoutingManager.GetDefault().GetAudioEndpoint();
                return audioRoute == AudioRoutingEndpoint.Bluetooth || audioRoute == AudioRoutingEndpoint.BluetoothWithNoiseAndEchoCancellation;
            }
            set {
                if (value)
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Bluetooth);
                else
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Earpiece);
            }
        }

        public bool IsBluetoothAvailable {
            get {
                return (AudioRoutingManager.GetDefault().AvailableAudioEndpoints & AvailableAudioRoutingEndpoints.Bluetooth) != 0;
            }
        }


#endregion

#region Chat
        public int GetUnreadMessageCount() {
            int nbUnreadMessages = 0;
            foreach (ChatRoom chatroom in Core.ChatRooms) {
                nbUnreadMessages += chatroom.UnreadMessagesCount;
            }
            return nbUnreadMessages;
        }

        public delegate void MessageReceivedEventHandler(ChatRoom room, ChatMessage message);
        public event MessageReceivedEventHandler MessageReceived;

        public MessageReceivedListener MessageListener {
            get; set;
        }
        public ToastNotification MessageReceivedNotification {
            get; set;
        }
        void OnMessageReceived(Core lc, ChatRoom room, ChatMessage message) {
            if (CoreDispatcher == null)
                return;
            if (MessageReceived != null) {
                MessageReceived(room, message);
            }

            Address fromAddress = message.FromAddress;
            string sipAddress = String.Format("{0}@{1}", fromAddress.Username, fromAddress.Domain);
            if (MessageListener != null && MessageListener.GetSipAddressAssociatedWithDisplayConversation() != null && MessageListener.GetSipAddressAssociatedWithDisplayConversation().Equals(sipAddress)) {
                MessageListener.MessageReceived(message);
            } else {
                string url = message.ExternalBodyUrl;
                //url = url.Replace("\"", "");

                if (MessageReceivedNotification != null) {
                    ToastNotificationManager.History.Clear();
                }

                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");

                toastTextElements[0].AppendChild(toastXml.CreateTextNode(sipAddress));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(message.TextContent));

                IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                ((XmlElement)toastNode).SetAttribute("launch", "chat ? sip = " + sipAddress);

                MessageReceivedNotification = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(MessageReceivedNotification);
            }
        }

        public void FileTransferProgressIndication(ChatMessage message, int offset, int total) {
            if (CoreDispatcher == null)
                return;
            Debug.WriteLine(String.Format("FileTransferProgressIndication: {0}/{1}", offset, total));
        }


        public ComposingReceivedListener ComposingListener {
            get; set;
        }
        void IsComposingReceived(Core lc, ChatRoom room) {
            {
                if (CoreDispatcher == null)
                    return;
                if (ComposingListener != null && room != null) {
                    string currentListenerSipAddress = ComposingListener.GetSipAddressAssociatedWithDisplayConversation();
                    Address peerAddress = room.PeerAddress;
                    string roomComposingSipAddress = String.Format("{0}@{1}", peerAddress.Username, peerAddress.Domain);

                    if (currentListenerSipAddress != null && roomComposingSipAddress.Equals(currentListenerSipAddress))
                        ComposingListener.ComposeReceived();
                }
            }
        }
#endregion

#region Video Management
        private List<String> _cameras;
        private int numberOfCameras = 0;

        public bool IsVideoAvailable {
            get {
                return Core.VideoSupported() && (Core.VideoDisplayEnabled|| Core.VideoCaptureEnabled);
            }
        }

        public bool EnableVideo(bool enable) {
            if (Core.InCall()) {
                Call call = Core.CurrentCall;
                CallParams parameters = call.CurrentParams;
                if (enable != parameters.VideoEnabled) {
                    parameters.VideoEnabled = enable;
                    if (enable) {
                        // TODO: Handle bandwidth limitation
                    }
                    call.Update(parameters);
                    return true;
                }
            }
            return false;
        }

        public int NumberOfCameras {
            get {
                return numberOfCameras;
            }
        }

        private void DetectCameras() {
            int nbCameras = 0;
            _cameras = new List<string>();
            foreach (string device in LinphoneManager.Instance.Core.VideoDevicesList) {
                if (!device.Contains("StaticImage")) {
                    _cameras.Add(device);
                    nbCameras++;
                }
            }
            numberOfCameras = nbCameras;
        }

        public void ToggleCameras() {
            if (NumberOfCameras >= 2) {
                String currentDevice = Core.VideoDevice;
                Core.VideoDevice = _cameras.ElementAt((_cameras.IndexOf(Core.VideoDevice) + 1) % _cameras.Count());

                if (Core.InCall()) {
                    Call call = Core.CurrentCall;
                    call.Update(null);
                }
            }
        }
#endregion

#region Listeners
        public CallControllerListener CallListener {
            get; set;
        }

        void AuthInfoRequested(Core lc, string realm, string username, string domain) {
        }

        void CallEncryptionChanged(Core lc, Call call, bool encrypted, string authenticationToken) {
        }

        public delegate void CallStateChangedEventHandler(Call call, CallState state);
        public event CallStateChangedEventHandler CallStateChangedEvent;

        private async Task openMicrophonePopup() {
            AudioGraphSettings settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            AudioGraph audioGraph = result.Graph;

            CreateAudioDeviceInputNodeResult resultNode = await audioGraph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Media);
            AudioDeviceInputNode deviceInputNode = resultNode.DeviceInputNode;

            deviceInputNode.Dispose();
            audioGraph.Dispose();
        }

        void CallStateChanged(Core lc, Call call, CallState state, string message) {
            if (CallStateChangedEvent != null) {
                CallStateChangedEvent(call, state);
            }

            // Workaround to make windows pop the microphone permission window
            if (state == CallState.IncomingReceived) {
                openMicrophonePopup();
            }

            if (state == CallState.OutgoingProgress) {
                Debug.WriteLine("[LinphoneManager] Outgoing progress\r\n");

                if (CallListener != null && Core.CallsNb > 0) {
                    string sipAddress = call.RemoteAddress.AsStringUriOnly();
                    CallListener.NewCallStarted(sipAddress);
                }
            }

            if (state == CallState.IncomingReceived) {
                Debug.WriteLine("[LinphoneManager] Incoming received\r\n");
                if (CallListener != null && Core.CallsNb > 0) {
                    CallListener.CallIncoming(call);
                }
            } else if (state == CallState.StreamsRunning) {
                Debug.WriteLine("[LinphoneManager] Stream Running\r\n");
                if (CallListener != null && Core.CallsNb > 0) {
                    CallListener.PauseStateChanged(call, false, false);
                }
            } else if (state == CallState.UpdatedByRemote) {
                bool remoteVideo = call.RemoteParams.VideoEnabled;
                bool localVideo = call.CurrentParams.VideoEnabled;
                bool autoAcceptCameraPolicy = Core.VideoActivationPolicy.AutomaticallyAccept;
                if (remoteVideo && !localVideo && !autoAcceptCameraPolicy) {
                    call.DeferUpdate();
                }
                Debug.WriteLine("[LinphoneManager] Update call\r\n");
            } else if (state == CallState.End || state == CallState.Error) {
                Debug.WriteLine(String.Format("[LinphoneManager] Call ended: {0}\r\n", message));
                if (CallListener != null)
                    CallListener.CallEnded(call);
                string text;
                switch (call.Reason) {
                    case Reason.None:
                    case Reason.NotAnswered:
                        break;
                    case Reason.Declined:
                        if (call.Dir == CallDir.Outgoing) {
                            //ShowCallError(text.Replace("#address#", call.RemoteAddress.UserName));
                        }
                        break;
                    case Reason.NotFound:
                        //text = ResourceManager.GetString("CallErrorNotFound", CultureInfo.CurrentCulture);
                        //ShowCallError(text.Replace("#address#", call.RemoteAddress.UserName));
                        break;
                    case Reason.Busy:
                        //text = ResourceManager.GetString("CallErrorBusy", CultureInfo.CurrentCulture);
                        //ShowCallError(text.Replace("#address#", call.RemoteAddress.UserName));
                        break;
                    case Reason.NotAcceptable:
                        //ShowCallError(ResourceManager.GetString("CallErrorNotAcceptable", CultureInfo.CurrentCulture));
                        break;
                    case Reason.IOError:
                        //ShowCallError(ResourceManager.GetString("CallErrorIOError", CultureInfo.CurrentCulture));
                        break;
                    default:
                        //ShowCallError(ResourceManager.GetString("CallErrorUnknown", CultureInfo.CurrentCulture));
                        break;
                }
            }
        }

        void CallStatsUpdated(Core lc, Call call, CallStats stats) {
            //System.Diagnostics.Debug.WriteLine("CallStatsUpdated");
        }

        void DtmfReceived(Core lc, Call call, int dtmf) {
            //throw new NotImplementedException();
        }

        void GlobalStateChanged(Core lc, GlobalState state, string message) {
            Debug.WriteLine(String.Format("GlobalStateChanged: {0} [{1}]", state, message));
        }

        public delegate void LogUploadProgressIndicationEventHandler(int offset, int total);
        public event LogUploadProgressIndicationEventHandler LogUploadProgressIndicationEH;

        void LogCollectionUploadProgressIndication(Core lc, long offset, long total) {
        }

        void LogCollectionUploadStateChanged(Core lc, CoreLogCollectionUploadState state, string info) {
            if (CoreDispatcher == null)
                return;
            if (state == CoreLogCollectionUploadState.Delivered) {
                BugCollector.ReportExceptions(info);
            } else if (state == CoreLogCollectionUploadState.NotDelivered) {
                Debug.WriteLine("[LinphoneManager] Logs upload error: " + info);
            }
        }

        public delegate void RegistrationStateChangedEventHandler(ProxyConfig config, RegistrationState state, string message);
        public event RegistrationStateChangedEventHandler RegistrationChanged;

        void RegistrationStateChanged(Core lc, ProxyConfig config, RegistrationState state, string message) {
            if (CoreDispatcher == null)
                return;
            if (RegistrationChanged != null) {
                RegistrationChanged(config, state, message);
            }
        }

        void TransferStateChanged(Core lc, Call call, CallState state) {
            //throw new NotImplementedException();
        }

        public EchoCalibratorListener ECListener {
            get; set;
        }

        public void EcCalibrationStatus(EcCalibratorStatus status, int delayMs) {
            Debug.WriteLine("[LinphoneManager] Echo canceller calibration status: " + status.ToString() + "\r\n");
            if (status == EcCalibratorStatus.Done) {
                Debug.WriteLine("[LinphoneManager] Echo canceller delay: {0} ms\r\n", delayMs);
            }
            if (ECListener != null) {
                ECListener.ECStatusNotified(status, delayMs);
            }
        }

#endregion

#region Tunnel
        public static void ConfigureTunnel(String mode) {
            if (LinphoneManager.Instance.Core.Tunnel != null) {
                Tunnel tunnel = LinphoneManager.Instance.Core.Tunnel;
                if (tunnel != null) {
                    /*if (mode == AppResources.TunnelModeDisabled)
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
                    }*/
                }
            }
        }

        public void ConfigureTunnel() {
            NetworkSettingsManager settings = new NetworkSettingsManager();
            settings.Load();
            ConfigureTunnel(settings.TunnelMode);
        }
#endregion

#region Contact Lookup
        private ContactsManager ContactManager {
            get {
                return ContactsManager.Instance;
            }
        }

        private void LookupForContact(Call call) {
            try {
                Address remoteAddress = call.RemoteAddress;
                if (remoteAddress.DisplayName.Length == 0) {
                    string sipAddress = String.Format("{0}@{1}", remoteAddress.Username, remoteAddress.Domain);

                    ContactManager.ContactFound += OnContactFound;
                    //ContactManager.FindContact(sipAddress);
                } else {
                    Debug.WriteLine("[LinphoneManager] Display name found: " + call.RemoteAddress.DisplayName + "\r\n");
                }
            } catch {
                Debug.WriteLine("[LinphoneManager] Exception occured while looking for contact...\r\n");
            }
        }

        private void OnContactFound(object sender, ContactFoundEventArgs e) {
            if (e.ContactFound != null) {
                Debug.WriteLine("[LinphoneManager] Contact found: " + e.ContactFound.ContactName + "\r\n");
                // Store the contact name as display name for call logs
                if (LinphoneManager.Instance.Core.CurrentCall != null) {
                    LinphoneManager.Instance.Core.CurrentCall.RemoteAddress.DisplayName = e.ContactFound.ContactName;
                }
            }
            ContactManager.ContactFound -= OnContactFound;
        }
#endregion
    }

    public interface EchoCalibratorListener {
        void ECStatusNotified(EcCalibratorStatus status, int delayMs);
    }

}

