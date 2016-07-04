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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using BelledonneCommunications.Linphone.Native;
using Windows.UI.Core;
using System.Diagnostics;
using Linphone.Views;
using System.IO;
using Windows.Storage;
using Windows.Phone.Media.Devices;
using Windows.System.Profile;
using Windows.Networking.PushNotifications;
using Windows.ApplicationModel.Calls;
using LinphoneTasks;
using Windows.Networking.Connectivity;
using System.Text;
using Windows.Foundation.Metadata;

namespace Linphone.Model
{
    class LinphoneManager : CoreListener
    {
        private static LinphoneManager _instance = new LinphoneManager();
        public static LinphoneManager Instance { get { return _instance; } }

        private Core _core;
        public bool isLinphoneRunning = false;

        private PushNotificationChannel channel;

        #region LinphoneCore and initialization

        public Core Core {
            get
            {
                if (_core == null)
                {
                    EnableLogCollection(true);
                    LpConfig config = new LpConfig(GetConfigPath(),GetFactoryConfigPath());
                    _core = new Core(this, config);
                }
                return _core;
            }
        }

        public CoreDispatcher CoreDispatcher { get; set; }

        public LinphoneManager()
        {
            Init();
        }

        private async void Init()
        {
            var vcc = VoipCallCoordinator.GetDefault();
            var entryPoint = typeof(PhoneCallTask).FullName;
            var status = await vcc.ReserveCallResourcesAsync(entryPoint);
        }

        public async void InitPushNotifications()
        {
            var internetProfile = NetworkInformation.GetInternetConnectionProfile();
            if (internetProfile != null)
            {
                channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                AddPushInformationsToContactParams();
            }
        }
        public String GetChatDatabasePath()
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, "chat.db");
        }

        public String GetDefaultConfigPath()
        {
            return Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "linphonerc");
        }

        public String GetConfigPath()
        {
            FileInfo fInfo = new FileInfo(Path.Combine(ApplicationData.Current.LocalFolder.Path, "linphonerc"));
            if(fInfo.Exists)
            {
                return Path.Combine(ApplicationData.Current.LocalFolder.Path, "linphonerc");
            } else
            {
                return null;
            }
        }

        public String GetFactoryConfigPath()
        {
            return Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "linphonerc-factory");
        }

        public String GetRootCaPath()
        {
            return Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "rootca.pem");
        }

        public void InitLinphoneCore()
        {
            Core.LogLevel = OutputTraceLevel.Debug;
           
            LinphoneManager.Instance.Core.ChatDatabasePath = GetChatDatabasePath();
            LinphoneManager.Instance.Core.RootCa = GetRootCaPath();

            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1)) {
                AudioRoutingManager.GetDefault().AudioEndpointChanged += AudioEndpointChanged;
            }

            if (LinphoneManager.Instance.Core.IsVideoSupported)
            {
                DetectCameras();
            }

            LinphoneManager.Instance.Core.SetUserAgent("LinphoneW10", Core.Version);
            InitPushNotifications();
            isLinphoneRunning = true;            
            LinphoneManager.Instance.Core.IsIterateEnabled = true;
        }

        public void EnableLogCollection(bool enable)
        {
            Core.LogCollectionEnabled = enable ? LogCollectionState.EnabledWithoutPreviousLogHandler : LogCollectionState.Disabled;
            Core.LogCollectionPath = ApplicationData.Current.LocalFolder.Path;
        }


        public void AddPushInformationsToContactParams()
        {
            if (Core.DefaultProxyConfig != null && channel != null)
            {
                Uri pushUri = new Uri(channel.Uri);
                string host = null, token = null;

                host = pushUri.Host;
                token = pushUri.OriginalString;

                if (host == null || token == null)
                {
                    return;
                }

                byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(token);
                String tokenB64 = Convert.ToBase64String(toEncodeAsBytes);

                if (Core.DefaultProxyConfig != null)
                {
                    Core.DefaultProxyConfig.Edit();
                    Core.DefaultProxyConfig.ContactUriParameters = "app-id=" + host + ";pn-type=w10;pn-tok=" + tokenB64;
                    Core.DefaultProxyConfig.Done();
                }
            }
        }

        public String getCoreVersion()
        {
            return Core.Version;
        }

        public void ConfigureLog(OutputTraceLevel level)
        {
            Core.LogLevel = level;
        }

        public void resetLogCollection()
        {
            Core.ResetLogCollection();
        }

        #endregion

        #region CallLogs
        private List<CallLogModel> _history;

        public List<CallLogModel> GetCallsHistory()
        {
            _history = new List<CallLogModel>();

            if (Core.CallLogs != null)
            {
                foreach (CallLog log in Core.CallLogs)
                {
                    string from = log.FromAddress.DisplayName;
                    if (from.Length == 0)
                    {
                        Address fromAddress = log.FromAddress;
                        from = fromAddress.AsStringUriOnly();
                    }

                    string to = log.ToAddress.DisplayName;
                    if (to.Length == 0)
                    {
                        Address toAddress = log.ToAddress;
                        to = toAddress.AsStringUriOnly();
                    }

                    bool isMissed = log.Status == CallStatus.Missed;
                    long startDate = log.StartDate;
                    CallLogModel callLog = new CallLogModel(log, from, to, log.Direction == CallDirection.Incoming, isMissed, startDate);
                    _history.Add(callLog);
                }
            }

            return _history;
        }

        public string GetLastCalledNumber()
        {
            foreach (CallLog log in LinphoneManager.Instance.Core.CallLogs)
            {
                if (log.Direction == CallDirection.Outgoing)
                {
                    return log.ToAddress.AsStringUriOnly();
                }
            }
            return null;
        }

        public void RemoveCallLogs(IEnumerable<CallLogModel> logsToRemove)
        {
            for (int i = 0; i < logsToRemove.Count(); i++)
            {
                CallLogModel logToRemove = logsToRemove.ElementAt(i);
                Core.RemoveCallLog(logToRemove.NativeLog as CallLog);
            }
        }

        public void ClearCallLogs()
        {
            Core.ClearCallLogs();
        }
        #endregion

        #region Call Management
        public void PauseCurrentCall()
        {
            if (Core.CallsNb > 0)
            {
                Call call = Core.CurrentCall;
                Core.PauseCall(call);
            }
        }

        public void ResumeCurrentCall()
        {
            foreach (Call call in Core.Calls)
            {
                if (call.State == CallState.Paused)
                {
                    Core.ResumeCall(call);
                }
            }
        }

        public void NewOutgoingCall(String sipAddress)
        {
            Call LCall = Core.Invite(sipAddress);
        }

        public void EndCurrentCall()
        {
            Call call = Core.CurrentCall;
            if (call != null)
            {
                Core.TerminateCall(call);
            }
            else
            {
                foreach (Call lCall in Core.Calls)
                {
                    if (lCall.State == CallState.Paused)
                    {
                        Core.TerminateCall(lCall);
                    }
                }
            }
        }

        private void ShowCallError(string message)
        {
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

        private void AudioEndpointChanged(AudioRoutingManager sender, object args)
        {
            Debug.WriteLine("[LinphoneManager] AudioEndpointChanged:" + sender.GetAudioEndpoint().ToString() + "\r\n");
        }

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

        public bool IsBluetoothAvailable
        {
            get
            {
                return (AudioRoutingManager.GetDefault().AvailableAudioEndpoints & AvailableAudioRoutingEndpoints.Bluetooth) != 0;
            }
        }


        #endregion

        #region Chat
        public int GetUnreadMessageCount()
        {
            int nbUnreadMessages = 0;
            foreach(ChatRoom chatroom in Core.ChatRooms)
            {
                nbUnreadMessages += chatroom.UnreadMessageCount;
            }
            return nbUnreadMessages;
        }

        public delegate void MessageReceivedEventHandler(ChatRoom room, ChatMessage message);
        public event MessageReceivedEventHandler MessageReceived;

        public MessageReceivedListener MessageListener { get; set; }
        //public ToastNotification MessageReceivedNotification { get; set; }
        void CoreListener.MessageReceived(ChatRoom room, ChatMessage message)
        {
            if (CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MessageReceived != null)
                {
                    MessageReceived(room, message);
                }

                Address fromAddress = message.FromAddress;
                string sipAddress = String.Format("{0}@{1}", fromAddress.UserName, fromAddress.Domain);
                Debug.WriteLine("[LinphoneManager] Message received from " + sipAddress + ": " + message.Text + "\r\n");

                if (MessageListener != null && MessageListener.GetSipAddressAssociatedWithDisplayConversation() != null && MessageListener.GetSipAddressAssociatedWithDisplayConversation().Equals(sipAddress))
                {
                    MessageListener.MessageReceived(message);
                }
                else
                {
                    /*string dateStr = Utils.FormatDate(message.Time):
                      string url = message.ExternalBodyUrl;
                      url = url.Replace("\"", "");

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
                                  BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/Chat.xaml?sip=" + Utils.ReplacePlusInUri(message.PeerAddress.AsStringUriOnly()), UriKind.RelativeOrAbsolute));
                                  break;
                          }
                      };

                      MessageReceivedNotification.Show();*/
                }
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        }

        public void FileTransferProgressIndication(ChatMessage message, int offset, int total)
        {
            Debug.WriteLine(String.Format("FileTransferProgressIndication: {0}/{1}", offset, total));
        }


        public ComposingReceivedListener ComposingListener { get; set; }
        void CoreListener.IsComposingReceived(ChatRoom room)
        {
            {
                if (CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
                CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (ComposingListener != null && room != null)
                    {
                        string currentListenerSipAddress = ComposingListener.GetSipAddressAssociatedWithDisplayConversation();
                        Address peerAddress = room.PeerAddress;
                        string roomComposingSipAddress = String.Format("{0}@{1}", peerAddress.UserName, peerAddress.Domain);

                        if (currentListenerSipAddress != null && roomComposingSipAddress.Equals(currentListenerSipAddress))
                            ComposingListener.ComposeReceived();
                    }
                });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            }
        }
        #endregion

        #region Video Management
        private String frontCamera = null;
        private String backCamera = null;

        public bool IsVideoAvailable
        {
            get
            {
                return Core.IsVideoSupported && (Core.IsVideoDisplayEnabled || Core.IsVideoCaptureEnabled);
            }
        }

        public bool EnableVideo(bool enable)
        {
            if (Core.IsInCall)
            {
                Call call = Core.CurrentCall;
                CallParams parameters = call.CurrentParams;
                if (enable != parameters.IsVideoEnabled)
                {
                    parameters.IsVideoEnabled = enable;
                    if (enable)
                    {
                        // TODO: Handle bandwidth limitation
                    }
                    Core.UpdateCall(call, parameters);
                    return true;
                }
            }
            return false;
        }

        public int NumberOfCameras
        {
            get
            {
                return Core.VideoDevices.Count;
            }
        }

        private void DetectCameras()
        {
            int nbCameras = 0;
            foreach (String device in LinphoneManager.Instance.Core.VideoDevices)
            {
                if (device.Contains("Front"))
                {
                    frontCamera = device;
                    nbCameras++;
                }
                else if (device.Contains("Back"))
                {
                    backCamera = device;
                    nbCameras++;
                }

            }
            String currentDevice = LinphoneManager.Instance.Core.VideoDevice;
            if ((currentDevice != frontCamera) && (currentDevice != backCamera))
            {
                if (frontCamera != null)
                {
                    Core.VideoDevice = frontCamera;
                }
                else if (backCamera != null)
                {
                    Core.VideoDevice = backCamera;
                }
            }
        }

        public void ToggleCameras()
        {
            if (NumberOfCameras >= 2)
            {
                String currentDevice = Core.VideoDevice;
                if (currentDevice == frontCamera)
                {
                    Core.VideoDevice = backCamera;
                }
                else if (currentDevice == backCamera)
                {
                    Core.VideoDevice = frontCamera;
                }

                if (Core.IsInCall)
                {
                    Call call = Core.CurrentCall;
                    Core.UpdateCall(call, null);
                }
            }
        }
        #endregion

        #region Listeners
        public CallControllerListener CallListener { get; set; }
   
        void CoreListener.AuthInfoRequested(string realm, string username, string domain)
        {
        }

        void CoreListener.CallEncryptionChanged(Call call, bool encrypted, string authenticationToken)
        {
        }

        public delegate void CallStateChangedEventHandler(Call call, CallState state);
        public event CallStateChangedEventHandler CallStateChangedEvent;

        void CoreListener.CallStateChanged(Call call, CallState state, string message)
        {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            CoreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if(CallStateChangedEvent != null)
                {
                    CallStateChangedEvent(call, state);
                }
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            

            if (state == CallState.OutgoingProgress)
            {
                
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
                CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                   Debug.WriteLine("[LinphoneManager] Outgoing progress\r\n");
 
                    if (CallListener != null && Core.CallsNb > 0)
                    {
                        string sipAddress = call.RemoteAddress.AsStringUriOnly();
                        CallListener.NewCallStarted(sipAddress);
                    }
                });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            }

            if (state == CallState.IncomingReceived)
            {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
                CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Debug.WriteLine("[LinphoneManager] Incoming received\r\n");
                    if (CallListener != null && Core.CallsNb > 0)
                    {
                        CallListener.CallIncoming(call);
                    }

                });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            }

            else if (state == CallState.StreamsRunning)
            {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
                CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Debug.WriteLine("[LinphoneManager] Stream Running\r\n");
                    if (CallListener != null && Core.CallsNb > 0)
                    {
                        CallListener.PauseStateChanged(call, false, false);
                    }
                });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            }

            else if (state == CallState.UpdatedByRemote)
            {
                bool remoteVideo = call.RemoteParams.IsVideoEnabled;
                bool localVideo = call.CurrentParams.Copy().IsVideoEnabled;
                bool autoAcceptCameraPolicy = Core.VideoPolicy.AutomaticallyAccept;
                if (remoteVideo && !localVideo && !autoAcceptCameraPolicy)
                {
                    Core.DeferCallUpdate(call);
                }
               
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
                CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Debug.WriteLine("[LinphoneManager] Update call\r\n");
                   
                    
                });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            }


            else if (state == CallState.End || state == CallState.Error)
            {
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
                CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Debug.WriteLine(String.Format("[LinphoneManager] Call ended: {0}\r\n", message));
                    if (CallListener != null)
                        CallListener.CallEnded(call);
                    string text;
                    switch (call.Reason)
                    {
                        case Reason.None:
                        case Reason.NotAnswered:
                            break;
                        case Reason.Declined:
                            if (call.Direction == CallDirection.Outgoing)
                            {
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
                });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            }
        }

        void CoreListener.CallStatsUpdated(Call call, CallStats stats)
        {
            //System.Diagnostics.Debug.WriteLine("CallStatsUpdated");
        }

        void CoreListener.DtmfReceived(Call call, char dtmf)
        {
            //throw new NotImplementedException();
        }

        void CoreListener.GlobalStateChanged(GlobalState state, string message)
        {
            Debug.WriteLine(String.Format("GlobalStateChanged: {0} [{1}]", state, message));
        }

        public delegate void LogUploadProgressIndicationEventHandler(int offset, int total);
        public event LogUploadProgressIndicationEventHandler LogUploadProgressIndicationEH;

        void CoreListener.LogCollectionUploadProgressIndication(int offset, int total)
        {
            if (CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (LogUploadProgressIndicationEH != null)
                {
                    LogUploadProgressIndicationEH(offset, total);
                }
            });
        }

        void CoreListener.LogCollectionUploadStateChanged(LogCollectionUploadState state, string info)
        {
            if (CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (state == LogCollectionUploadState.Delivered)
                {
                    BugCollector.ReportExceptions(info);
                }
                else if (state == LogCollectionUploadState.NotDelivered)
                {
                    Debug.WriteLine("[LinphoneManager] Logs upload error: " + info);
                    /* var notif = new CustomMessageBox()
                     {
                         Caption = "Logfile upload failed",
                         Message = info,
                         RightButtonContent = AppResources.Close
                     };
                     notif.Show();*/
                }
            });
        }

        public delegate void RegistrationStateChangedEventHandler(ProxyConfig config, RegistrationState state, string message);
        public event RegistrationStateChangedEventHandler RegistrationChanged;

        void CoreListener.RegistrationStateChanged(ProxyConfig config, RegistrationState state, string message)
        {
            if (CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if(RegistrationChanged != null)
                {
                    RegistrationChanged(config, state, message);
                } 
            });
        }

        public EchoCalibratorListener ECListener { get; set; }

        public void EcCalibrationStatus(EcCalibratorStatus status, int delayMs)
        {
            Debug.WriteLine("[LinphoneManager] Echo canceller calibration status: " + status.ToString() + "\r\n");
            if (status == EcCalibratorStatus.Done)
            {
                Debug.WriteLine("[LinphoneManager] Echo canceller delay: {0} ms\r\n", delayMs);
            }
            CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (ECListener != null)
                {
                    ECListener.ECStatusNotified(status, delayMs);
                }
            });
        }

        #endregion

        #region Tunnel
        public static void ConfigureTunnel(String mode)
        {
            if (LinphoneManager.Instance.Core.Tunnel != null)
            {
                Tunnel tunnel = LinphoneManager.Instance.Core.Tunnel;
                if (tunnel != null)
                {
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

        public void ConfigureTunnel()
        {
            NetworkSettingsManager settings = new NetworkSettingsManager();
            settings.Load();
            ConfigureTunnel(settings.TunnelMode);
        }
        #endregion

        #region Contact Lookup
        private ContactsManager ContactManager
        {
            get
            {
                return ContactsManager.Instance;
            }
        }

        private void LookupForContact(Call call)
        {
            try
            {
                Address remoteAddress = call.RemoteAddress;
                if (remoteAddress.DisplayName.Length == 0)
                {
                    string sipAddress = String.Format("{0}@{1}", remoteAddress.UserName, remoteAddress.Domain);

                    ContactManager.ContactFound += OnContactFound;
                    //ContactManager.FindContact(sipAddress);
                }
                else
                {
                    Debug.WriteLine("[LinphoneManager] Display name found: " + call.RemoteAddress.DisplayName + "\r\n");
                }
            }
            catch
            {
                Debug.WriteLine("[LinphoneManager] Exception occured while looking for contact...\r\n");
            }
        }

        private void OnContactFound(object sender, ContactFoundEventArgs e)
        {
            if (e.ContactFound != null)
            {
                Debug.WriteLine("[LinphoneManager] Contact found: " + e.ContactFound.ContactName + "\r\n");
                // Store the contact name as display name for call logs
                if (LinphoneManager.Instance.Core.CurrentCall != null)
                {
                    LinphoneManager.Instance.Core.CurrentCall.RemoteAddress.DisplayName = e.ContactFound.ContactName;
                }
            }
            ContactManager.ContactFound -= OnContactFound;
        }
        #endregion
    }

    public interface EchoCalibratorListener
    {
        void ECStatusNotified(EcCalibratorStatus status, int delayMs);
    }

}

