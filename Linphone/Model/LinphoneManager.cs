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

namespace Linphone.Model
{
    class LinphoneManager : CoreListener
    {
        private static LinphoneManager _instance = new LinphoneManager();
        public static LinphoneManager Instance { get { return _instance; } }

        public delegate void ChangedEventHandler(object sender, EventArgs e);

        public event ChangedEventHandler CallStateChanged;
        public event ChangedEventHandler RegistrationChanged;
        public event ChangedEventHandler MessageReceived;

        private Core _core;
        public bool isLinphoneRunning = false;

        private PushNotificationChannel channel;

        public class CallEventArgs : EventArgs
        {
            public CallEventArgs(Call c)
            {
                _call = c;
            }
            private Call _call;
            public Call call
            {
                get { return _call; }
            }
        }

        public class ProxyEventArgs : EventArgs
        {
            public ProxyEventArgs(ProxyConfig p)
            {
                _proxy = p;
            }
            private ProxyConfig _proxy;
            public ProxyConfig proxy
            {
                get { return _proxy; }
            }
        }

        public class MessageEventArgs : EventArgs
        {
            public MessageEventArgs(ChatMessage m)
            {
                _chatMessage = m;
            }
            private ChatMessage _chatMessage;
            public ChatMessage chatMessage
            {
                get { return _chatMessage; }
            }
        }

        public Core Core {
            get
            {
                if (_core == null)
                {
                    LpConfig config = new LpConfig(GetConfigPath(),GetFactoryConfigPath());
                    _core = new Core(this, config);
                }
                return _core;
            }
        }

        public CoreDispatcher CoreDispatcher { get; set; }

        public LinphoneManager()
        {
            LastKnownState = RegistrationState.None;
        }


        private RegistrationState _lastKnownState;
        /// <summary>
        /// Used to set the default registration state on the status bar when the view is changed.
        /// </summary>
        public RegistrationState LastKnownState
        {
            get
            {
                try
                {
                    if (isLinphoneRunning && LinphoneManager.Instance.Core.DefaultProxyConfig != null)
                    {
                        _lastKnownState = Core.DefaultProxyConfig.State;
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


        #region Linphone Core Initialization

        public async void InitPushNotifications()
        {
            channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

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
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, "linphonerc");
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

            if(AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                AudioRoutingManager.GetDefault().AudioEndpointChanged += AudioEndpointChanged;
            }

            if (LinphoneManager.Instance.Core.IsVideoSupported)
            {
                DetectCameras();
            }

            LinphoneManager.Instance.Core.SetUserAgent("LinphoneW10", Core.Version);
            //server.LinphoneCore.NetworkReachable = lastNetworkState;
            // DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(OnNetworkStatusChanged);
            // ConfigureTunnel();
            InitPushNotifications();
            isLinphoneRunning = true;            
            LinphoneManager.Instance.Core.IsIterateEnabled = true;
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

        #endregion

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

        private String frontCamera = null;
        private String backCamera = null;

        public bool IsVideoAvailable
        {
            get
            {
                return Core.IsVideoSupported && (Core.IsVideoDisplayEnabled || Core.IsVideoCaptureEnabled);
            }
        }

        public int NumberOfCameras
        {
            get
            {
                return Core.VideoDevices.Count;
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

        /// <summary>
        /// Enable or disable sound capture using the device microphone
        /// </summary>
        public void MuteMic(Boolean isMicMuted)
        {
            if (BaseModel.UIDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            BaseModel.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (Core.CallsNb > 0)
                {
                    Core.IsMicEnabled = isMicMuted;
                    if (CallListener != null)
                        CallListener.MuteStateChanged(isMicMuted);
                }
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        }


        /// <summary>
        /// Pauses the current call if any and if it's running.
        /// </summary>
        public void PauseCurrentCall()
        {
            if (Core.CallsNb > 0)
            {
                Call call = Core.CurrentCall;
                Core.PauseCall(call);
            }
        }

        /// <summary>
        /// Resume the current call if any and if it's paused.
        /// </summary>
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

        private void AudioEndpointChanged(AudioRoutingManager sender, object args)
        {
           Debug.WriteLine("[LinphoneManager] AudioEndpointChanged:" + sender.GetAudioEndpoint().ToString() + "\r\n");
        }

        /// <summary>
        /// Property that handles the audio routing between the speaker and the earpiece.
        /// </summary>
        public bool SpeakerEnabled
        {
            get
            {
                //AudioRoutingManager
                //
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
                //var audioRoute = AudioRoutingManager.GetDefault().GetAudioEndpoint();
                //return audioRoute == AudioRoutingEndpoint.Bluetooth || audioRoute == AudioRoutingEndpoint.BluetoothWithNoiseAndEchoCancellation;
                return false;
            }
            set
            {
                /*if (value)
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Bluetooth);
                else
                    AudioRoutingManager.GetDefault().SetAudioEndpoint(AudioRoutingEndpoint.Earpiece);*/
            }
        }

        /// <summary>
        /// Returns true if the Bluetooth audio route is available
        /// </summary>
        public bool IsBluetoothAvailable
        {
            get
            {
                // return (AudioRoutingManager.GetDefault().AvailableAudioEndpoints & AvailableAudioRoutingEndpoints.Bluetooth) != 0;
                return false;
            }
        }

        public void NewOutgoingCall(String sipAddress)
        {
            Call LCall = Core.Invite(sipAddress);
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


        /// <summary>
        /// Enables disables video.
        /// </summary>
        /// <param name="enable">Wether to enable or disable video</param>
        /// <returns>true if the operation has been successful, false otherwise</returns>
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


        public void RemoveCallLogs(IEnumerable<CallLogModel> logsToRemove)
        {
            // When removing log from history, it will be removed from logsToRemove list too. 
            // Using foreach causing the app to crash on a InvalidOperationException, so we are using while
            for (int i = 0; i < logsToRemove.Count(); i++)
            {
                CallLogModel logToRemove = logsToRemove.ElementAt(i);
                Core.RemoveCallLog(logToRemove.NativeLog as CallLog);
            }
        }

        public CallControllerListener CallListener { get; set; }

        #region Listeners
        void CoreListener.AuthInfoRequested(string realm, string username, string domain)
        {
            //throw new NotImplementedException();
        }

        void CoreListener.CallEncryptionChanged(Call call, bool encrypted, string authenticationToken)
        {
            throw new NotImplementedException();
        }

        void CoreListener.CallStateChanged(Call call, CallState state, string message)
        {
            Debug.WriteLine("Call state " + message);

#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            CoreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (CallStateChanged != null)
                {
                    CallStateChanged(this, new CallEventArgs(call));
                }
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel

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
            throw new NotImplementedException();
        }

        void CoreListener.GlobalStateChanged(GlobalState state, string message)
        {
            Debug.WriteLine(String.Format("GlobalStateChanged: {0} [{1}]", state, message));
        }

        void CoreListener.IsComposingReceived(ChatRoom room)
        {
            Debug.WriteLine("Is composing ");
        }

        void CoreListener.LogCollectionUploadProgressIndication(int offset, int total)
        {
            throw new NotImplementedException();
        }

        void CoreListener.LogCollectionUploadStateChanged(LogCollectionUploadState state, string info)
        {
            throw new NotImplementedException();
        }

        public MessageReceivedListener MessageListener { get; set; }

        void CoreListener.MessageReceived(ChatRoom room, ChatMessage message)
        {
            if (CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MessageReceived != null)
                {
                    MessageReceived(this, new MessageEventArgs(message));
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
                  /*  DateTime date = new DateTime(message.Time * TimeSpan.TicksPerSecond, DateTimeKind.Utc).AddYears(1969).ToLocalTime();
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
                                BaseModel.CurrentPage.NavigationService.Navigate(new Uri("/Views/Chat.xaml?sip=" + Utils.ReplacePlusInUri(message.PeerAddress.AsStringUriOnly()), UriKind.RelativeOrAbsolute));
                                break;
                        }
                    };

                    MessageReceivedNotification.Show();*/
                }
            });
#pragma warning restore CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
        }

        void CoreListener.RegistrationStateChanged(ProxyConfig config, RegistrationState state, string message)
        {
            if (CoreDispatcher == null) return;
#pragma warning disable CS4014 // Dans la mesure où cet appel n'est pas attendu, l'exécution de la méthode actuelle continue avant la fin de l'appel
            CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if(RegistrationChanged != null)
                {
                    RegistrationChanged(this, new ProxyEventArgs(config));
                } 
            });
        }

        #endregion

        #region Call functions

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


        #endregion

    }
}
