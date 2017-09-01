/*
SettingsManager.cs
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

using Linphone;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace Linphone.Model {
    /// <summary>
    /// Interface describing the methods that each setting manager must implement.
    /// </summary>
    public interface ISettingsManager {
        /// <summary>
        /// Load some settings.
        /// </summary>
        void Load();

        /// <summary>
        /// Save some settings.
        /// </summary>
        void Save();
    }

    /// <summary>
    /// Utility class used to handle everything that's application setting related.
    /// </summary>
    public class SettingsManager {
        protected Dictionary<String, String> dict;
        protected Dictionary<String, String> changesDict;
        protected const string ApplicationSection = "app";

        /// <summary>
        /// Public constructor.
        /// </summary>
        public SettingsManager() {
            dict = new Dictionary<String, String>();
            changesDict = new Dictionary<String, String>();
        }

        /// <summary>
        /// Install the default config file from the package to the Isolated Storage
        /// </summary>
        public static async void InstallConfigFile() {

            FileInfo fInfo = new FileInfo(Path.Combine(ApplicationData.Current.LocalFolder.Path, "linphonerc"));
            if (!fInfo.Exists) {
                try {
                    StorageFile file1 = await StorageFile.GetFileFromPathAsync(Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "linphonerc"));
                    await file1.CopyAsync(ApplicationData.Current.LocalFolder, "linphonerc");
                } catch (Exception e) {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Get the value of a settings parameter from its name
        /// </summary>
        /// <param name="Key">The name of the settings parameter for which we want the value</param>
        /// <returns>The value of the settings parameter</returns>
        protected String Get(String Key) {
            if (dict.ContainsKey(Key)) {
                return dict[Key];
            } else {
                return null;
            }
        }

        /// <summary>
        /// Get the changed value of a settings parameter from its name
        /// </summary>
        /// <param name="Key">The name of the settings parameter for which we want the changed value</param>
        /// <returns>The changed value of the settings parameter</returns>
        protected String GetNew(String Key) {
            if (changesDict.ContainsKey(Key)) {
                return changesDict[Key];
            } else if (dict.ContainsKey(Key)) {
                return dict[Key];
            }
            return null;
        }

        /// <summary>
        /// Set a new value for a settings parameter given its name
        /// </summary>
        /// <param name="Key">The name of the settings parameter to change</param>
        /// <param name="Value">The new value to be set for the settings parameter</param>
        protected void Set(String Key, String Value) {
            if (dict.ContainsKey(Key)) {
                if (dict[Key] != Value || Value.Length == 0) {
                    changesDict[Key] = Value;
                }
            } else {
                changesDict[Key] = Value;
            }
        }

        /// <summary>
        /// Tell whether a settings parameter has been changed given its name
        /// </summary>
        /// <param name="Key">The name of the settings parameter</param>
        /// <returns>A boolean telling whether the settings parameter has been changed or not</returns>
        protected bool ValueChanged(String Key) {
            return changesDict.ContainsKey(Key);
        }
    }

    /// <summary>
    /// Utility class used to handle application settings.
    /// </summary>
    public class ApplicationSettingsManager : SettingsManager, ISettingsManager {
        private Config Config;
        private LogCollectionState LogLevel;
        #region Constants settings names
        private const String LogLevelKeyName = "LogLevel";
        private const string VideoActiveWhenGoingToBackgroundKeyName = "VideoActiveWhenGoingToBackground";
        private const string VideoAutoAcceptWhenGoingToBackgroundKeyName = "VideoAutoAcceptWhenGoingToBackground";
        #endregion

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ApplicationSettingsManager() {
            if (LinphoneManager.Instance.Core == null) {
                Config = LinphoneManager.Instance.Core.CreateConfig(LinphoneManager.Instance.GetConfigPath());
            } else {
                Config = LinphoneManager.Instance.Core.Config;
            }
        }

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Load the application settings.
        /// </summary>
        public void Load() {
            LogLevelSetting = (LogCollectionState)(Config.GetInt(ApplicationSection, LogLevelKeyName, (int)LogCollectionState.Disabled));
            dict[VideoActiveWhenGoingToBackgroundKeyName] = Config.GetInt(ApplicationSection, VideoActiveWhenGoingToBackgroundKeyName, 0).ToString();
            dict[VideoAutoAcceptWhenGoingToBackgroundKeyName] = Config.GetInt(ApplicationSection, VideoAutoAcceptWhenGoingToBackgroundKeyName, 1).ToString();
        }

        /// <summary>
        /// Save the application settings.
        /// </summary>
        public async void Save() {
            if (ValueChanged(LogLevelKeyName)) {
                try {
                    Config.SetInt(ApplicationSection, LogLevelKeyName, (int)LogLevel);
                    LinphoneManager.Instance.EnableLogCollection((LogLevelSetting == LogCollectionState.Enabled) ? true: false);
                    Linphone.Core.EnableLogCollection(LogLevelSetting);
                    Linphone.Core.SetLogLevelMask((LogLevelSetting == LogCollectionState.Enabled) ? (uint)0xFF: 0x0);
                    //LinphoneManager.Instance.ConfigureLog(LogLevel);
                } catch {
                    // Core.LogLevel.Warn("Failed setting the log level name {0}", Get(LogLevelKeyName));
                }
            }
            if (ValueChanged(VideoActiveWhenGoingToBackgroundKeyName)) {
                Config.SetInt(ApplicationSection, VideoActiveWhenGoingToBackgroundKeyName, Convert.ToInt32(GetNew(VideoActiveWhenGoingToBackgroundKeyName)));
            }
            if (ValueChanged(VideoAutoAcceptWhenGoingToBackgroundKeyName)) {
                Config.SetInt(ApplicationSection, VideoAutoAcceptWhenGoingToBackgroundKeyName, Convert.ToInt32(GetNew(VideoAutoAcceptWhenGoingToBackgroundKeyName)));
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Debug enabled setting (Bool).
        /// </summary>
        public bool DebugEnabled {
            get {
                return LogLevelSetting == LogCollectionState.Enabled;
            }
            set {
                if (value) {
                    LogLevelSetting = LogCollectionState.Enabled;
                } else {
                    LogLevelSetting = LogCollectionState.Disabled;
                }
            }
        }

        /// <summary>
        /// Log level (OutputTraceLevel).
        /// </summary>
        public LogCollectionState LogLevelSetting {
            get {
                return LogLevel;
            }
            set {
                LogLevel = value;
                Set(LogLevelKeyName, ((int)value).ToString());
            }
        }

        /// <summary>
        /// Save if the video was active when going to background.
        /// </summary>
        public Boolean VideoActiveWhenGoingToBackground {
            get {
                return Convert.ToBoolean(Get(VideoActiveWhenGoingToBackgroundKeyName));
            }
            set {
                Set(VideoActiveWhenGoingToBackgroundKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Save the video auto accept policy when going to background.
        /// </summary>
        public Boolean VideoAutoAcceptWhenGoingToBackground {
            get {
                return Convert.ToBoolean(Get(VideoAutoAcceptWhenGoingToBackgroundKeyName));
            }
            set {
                Set(VideoAutoAcceptWhenGoingToBackgroundKeyName, value.ToString());
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle SIP account settings.
    /// </summary>
    public class SIPAccountSettingsManager : SettingsManager, ISettingsManager {
        #region Constants settings names
        private const string UsernameKeyName = "Username";
        private const string UserIdKeyName = "UserId";
        private const string PasswordKeyName = "Password";
        private const string DomainKeyName = "Domain";
        private const string ProxyKeyName = "Proxy";
        private const string OutboundProxyKeyName = "OutboundProxy";
        private const string DisplayNameKeyName = "DisplayName";
        private const string TransportKeyName = "Transport";
        private const string ExpireKeyName = "Expire";
        private const string AVPFKeyName = "AVPF";
        private const string Ice = "ICE";

        private Dictionary<TransportType, string> EnumToTransport;
        private Dictionary<string, TransportType> TransportToEnum;
        #endregion

        public SIPAccountSettingsManager() {
            EnumToTransport = new Dictionary<TransportType, string>()
             {
                { TransportType.Udp,  ResourceLoader.GetForCurrentView().GetString("TransportUDP") },
                { TransportType.Tcp, ResourceLoader.GetForCurrentView().GetString("TransportTCP") },
                { TransportType.Tls, ResourceLoader.GetForCurrentView().GetString("TransportTLS") }
            };

            TransportToEnum = new Dictionary<string, TransportType>()
            {
                { ResourceLoader.GetForCurrentView().GetString("TransportUDP"), TransportType.Udp },
                { ResourceLoader.GetForCurrentView().GetString("TransportTCP"), TransportType.Tcp },
                { ResourceLoader.GetForCurrentView().GetString("TransportTLS"), TransportType.Tls }
            };
        }

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Load the SIP account settings.
        /// </summary>
        public void Load() {
            dict[UsernameKeyName] = "";
            dict[UserIdKeyName] = "";
            dict[PasswordKeyName] = "";
            dict[DisplayNameKeyName] = "";
            dict[DomainKeyName] = "";
            dict[ProxyKeyName] = "";
            dict[OutboundProxyKeyName] = false.ToString();
            dict[TransportKeyName] = "UDP";
            dict[ExpireKeyName] = "";
            dict[AVPFKeyName] = false.ToString();
            dict[Ice] = false.ToString();

            ProxyConfig cfg = LinphoneManager.Instance.Core.DefaultProxyConfig;
            if (cfg != null) {
                Address address = LinphoneManager.Instance.Core.CreateAddress(cfg.IdentityAddress.AsStringUriOnly());
                if (address != null) {
                    Address proxyAddress = LinphoneManager.Instance.Core.CreateAddress(cfg.ServerAddr);
                    dict[ProxyKeyName] = proxyAddress.AsStringUriOnly();
                    dict[TransportKeyName] = EnumToTransport[proxyAddress.Transport];
                    dict[UsernameKeyName] = address.Username;
                    dict[DomainKeyName] = address.Domain;
                    dict[OutboundProxyKeyName] = (cfg.Route.Length > 0).ToString();
                    dict[ExpireKeyName] = String.Format("{0}", cfg.Expires);
                    AuthInfo authInfo = LinphoneManager.Instance.Core.FindAuthInfo(address.Domain, address.Username, address.Domain);
                    if (authInfo != null) {
                        dict[PasswordKeyName] = authInfo.Passwd;
                        dict[UserIdKeyName] = authInfo.Userid;
                    }
                    if (cfg.NatPolicy == null) cfg.NatPolicy = LinphoneManager.Instance.Core.CreateNatPolicy();
                    dict[Ice] = cfg.NatPolicy.IceEnabled.ToString();
                    dict[DisplayNameKeyName] = address.DisplayName;
                    dict[AVPFKeyName] = cfg.AvpfEnabled.ToString();
                }
            }
        }

        /// <summary>
        /// Save the SIP account settings.
        /// </summary>
        public void Save() {
            bool AccountChanged = ValueChanged(UsernameKeyName) || ValueChanged(UserIdKeyName) || ValueChanged(PasswordKeyName) || ValueChanged(DomainKeyName)
                || ValueChanged(ProxyKeyName) || ValueChanged(OutboundProxyKeyName) || ValueChanged(DisplayNameKeyName) || ValueChanged(TransportKeyName) || ValueChanged(ExpireKeyName);

            if (AccountChanged) {
                Core lc = LinphoneManager.Instance.Core;
                ProxyConfig cfg = lc.DefaultProxyConfig;
                if (cfg != null) {
                    cfg.Edit();
                    cfg.RegisterEnabled = false;
                    cfg.Done();

                    //Wait for unregister to complete
                    int timeout = 2000;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (true) {
                        if (stopwatch.ElapsedMilliseconds >= timeout || cfg.State == RegistrationState.Cleared || cfg.State == RegistrationState.None) {
                            break;
                        }
                        // Thread.Sleep(1);
                    }
                }

                String username = GetNew(UsernameKeyName);
                String userid = GetNew(UserIdKeyName);
                String password = GetNew(PasswordKeyName);
                String domain = GetNew(DomainKeyName);
                String proxy = GetNew(ProxyKeyName);
                String displayname = GetNew(DisplayNameKeyName);
                String transport = GetNew(TransportKeyName);
                String expires = GetNew(ExpireKeyName);
                bool avpf = Convert.ToBoolean(GetNew(AVPFKeyName));
                bool ice = Convert.ToBoolean(GetNew(Ice));

                bool outboundProxy = Convert.ToBoolean(GetNew(OutboundProxyKeyName));
                lc.ClearAllAuthInfo();
                lc.ClearProxyConfig();
                if ((username != null) && (username.Length > 0) && (domain != null) && (domain.Length > 0)) {
                    cfg = lc.CreateProxyConfig();
                    cfg.Edit();
                    if (displayname != null && displayname.Length > 0) {
                        cfg.IdentityAddress = Factory.Instance.CreateAddress("\"" + displayname + "\" " + "<sip:" + username + "@" + domain + ">");
                    } else {
                        cfg.IdentityAddress = Factory.Instance.CreateAddress("<sip:" + username + "@" + domain + ">");
                    }

                    if ((proxy == null) || (proxy.Length <= 0)) {
                        proxy = "sip:" + domain;
                    } else {
                        if (!proxy.StartsWith("sip:") && !proxy.StartsWith("<sip:")
                            && !proxy.StartsWith("sips:") && !proxy.StartsWith("<sips:")) {
                            proxy = "sip:" + proxy;
                        }
                    }


                    cfg.ServerAddr = proxy;

                    if (transport != null) {
                        Address proxyAddr = LinphoneManager.Instance.Core.CreateAddress(proxy);
                        if (proxyAddr != null) {
                            proxyAddr.Transport = TransportToEnum[transport];
                            cfg.ServerAddr = proxyAddr.AsStringUriOnly();
                        }
                    }
                    if (outboundProxy) {
                        cfg.Route = cfg.ServerAddr;
                    }

                    if (cfg.NatPolicy != null) {
                        cfg.NatPolicy.IceEnabled = ice;
                    }

                    int result = 0;
                    int.TryParse(expires, out result);
                    if (result != 0) {
                        cfg.Expires = result;
                    }

                    var auth = lc.CreateAuthInfo(username, userid, password, "", "", domain);
                    lc.AddAuthInfo(auth);

                    lc.AddProxyConfig(cfg);
                    lc.DefaultProxyConfig = cfg;
                    LinphoneManager.Instance.AddPushInformationsToContactParams();
                    //cfg.AvpfEnabled = avpf;
                    cfg.RegisterEnabled = true;
                    cfg.Done();
                }
            }
        }

        public void Delete() {
            ProxyConfig cfg = LinphoneManager.Instance.Core.DefaultProxyConfig;
            if (cfg != null) {
                LinphoneManager.Instance.Core.ClearProxyConfig();
                LinphoneManager.Instance.Core.ClearAllAuthInfo();
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// SIP Account username setting (String).
        /// </summary>
        public string Username {
            get {
                return Get(UsernameKeyName);
            }
            set {
                Set(UsernameKeyName, value);
            }
        }

        /// <summary>
        /// SIP Account userid setting (String).
        /// </summary>
        public string UserId {
            get {
                return Get(UserIdKeyName);
            }
            set {
                Set(UserIdKeyName, value);
            }
        }

        /// <summary>
        /// SIP account password setting (String).
        /// </summary>
        public string Password {
            get {
                return Get(PasswordKeyName);
            }
            set {
                Set(PasswordKeyName, value);
            }
        }

        /// <summary>
        /// SIP account domain setting (String).
        /// </summary>
        public string Domain {
            get {
                return Get(DomainKeyName);
            }
            set {
                Set(DomainKeyName, value);
            }
        }

        /// <summary>
        /// SIP account proxy setting (String).
        /// </summary>
        public string Proxy {
            get {
                return Get(ProxyKeyName);
            }
            set {
                Set(ProxyKeyName, value);
            }
        }

        /// <summary>
        /// SIP account outbound proxy setting (Bool).
        /// </summary>
        public bool? OutboundProxy {
            get {
                return Convert.ToBoolean(Get(OutboundProxyKeyName));
            }
            set {
                Set(OutboundProxyKeyName, value.ToString());
            }
        }

        /// <summary>
        /// SIP account display name setting (String).
        /// </summary>
        public string DisplayName {
            get {
                return Get(DisplayNameKeyName);
            }
            set {
                Set(DisplayNameKeyName, value);
            }
        }

        /// <summary>
        /// Transport (String).
        /// </summary>
        public string Transports {
            get {
                return Get(TransportKeyName);
            }
            set {
                Set(TransportKeyName, value);
            }
        }

        /// <summary>
        /// SIP account expires setting (String).
        /// </summary>
        public string Expires {
            get {
                return Get(ExpireKeyName);
            }
            set {
                Set(ExpireKeyName, value);
            }
        }

        /// <summary>
        /// AVPF activated for SIP account (Boolean).
        /// </summary>
        public bool? AVPF {
            get {
                return Convert.ToBoolean(Get(AVPFKeyName));
            }
            set {
                Set(AVPFKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Ice activated for SIP account (Boolean).
        /// </summary>
        public bool? ICE {
            get {
                return Convert.ToBoolean(Get(Ice));
            }
            set {
                Set(Ice, value.ToString());
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle codecs settings.
    /// </summary>
    public class CodecsSettingsManager : SettingsManager, ISettingsManager {
        #region Constants settings names
        private const string AMRNBSettingKeyName = "CodecAMRNB";
        private const string AMRWBSettingKeyName = "CodecAMRWB";
        private const string Speex16SettingKeyName = "CodecSpeex16";
        private const string Speex8SettingKeyName = "CodecSpeex8";
        private const string PCMUSettingKeyName = "CodecPCMU";
        private const string PCMASettingKeyName = "CodecPCMA";
        private const string G722SettingKeyName = "CodecG722";
        private const string G729SettingKeyName = "CodecG729";
        private const string ILBCSettingKeyName = "CodecILBC";
        private const string SILK16SettingKeyName = "CodecSILK16";
        private const string GSMSettingKeyName = "CodecGSM";
        private const string OpusSettingKeyName = "CodecOpus";
        private const string IsacSettingKeyName = "CodecIsac";
        private const string H264SettingKeyName = "CodecH264";
        private const string VP8SettingKeyName = "CodecVP8";
        #endregion

        private String GetKeyNameForCodec(String mimeType, int clockRate) {
            Dictionary<Tuple<String, int>, String> map = new Dictionary<Tuple<String, int>, String>
            {
                { new Tuple<String, int>("amr", 8000), AMRNBSettingKeyName },
                { new Tuple<String, int>("amr-wb", 16000), AMRWBSettingKeyName },
                { new Tuple<String, int>("speex", 16000), Speex16SettingKeyName },
                { new Tuple<String, int>("speex", 8000), Speex8SettingKeyName },
                { new Tuple<String, int>("pcmu", 8000), PCMUSettingKeyName },
                { new Tuple<String, int>("pcma", 8000), PCMASettingKeyName },
                { new Tuple<String, int>("g722", 8000), G722SettingKeyName },
                { new Tuple<String, int>("g729", 8000), G729SettingKeyName },
                { new Tuple<String, int>("ilbc", 8000), ILBCSettingKeyName },
                { new Tuple<String, int>("silk", 16000), SILK16SettingKeyName },
                { new Tuple<String, int>("gsm", 8000), GSMSettingKeyName },
                { new Tuple<String, int>("opus", 48000), OpusSettingKeyName },
                { new Tuple<String, int>("isac", 16000), IsacSettingKeyName },
                { new Tuple<String, int>("h264", 90000), H264SettingKeyName },
                { new Tuple<String, int>("vp8", 90000), VP8SettingKeyName },
            };

            Tuple<String, int> key = new Tuple<String, int>(mimeType.ToLower(), clockRate);
            if (map.ContainsKey(key)) {
                return map[key];
            }
            return null;
        }

        #region Implementation of the ISettingsManager interface
        private void LoadCodecs(IEnumerable<PayloadType> ptlist) {
            foreach (PayloadType pt in ptlist) {
                String keyname = GetKeyNameForCodec(pt.MimeType, pt.ClockRate);
                if (keyname != null) {
                    dict[keyname] = Convert.ToString(pt.Enabled());
                } else {
                    //Logger.Warn("Codec {0}/{1} supported by core is not shown in the settings view, disable it", pt.MimeType, pt.ClockRate);
                    pt.Enable(false);
                }
            }
        }

        /// <summary>
        /// Load the codecs settings.
        /// </summary>
        public void Load() {
            LoadCodecs(LinphoneManager.Instance.Core.AudioPayloadTypes);
            LoadCodecs(LinphoneManager.Instance.Core.VideoPayloadTypes);
        }

        private void SaveCodecs(IEnumerable<PayloadType> ptlist) {
            foreach (PayloadType pt in ptlist) {
                String keyname = GetKeyNameForCodec(pt.MimeType, pt.ClockRate);
                if ((keyname != null) && ValueChanged(keyname)) {
                    pt.Enable(Convert.ToBoolean(GetNew(keyname)));
                }
            }
        }

        /// <summary>
        /// Save the codecs settings.
        /// </summary>
        public void Save() {
            SaveCodecs(LinphoneManager.Instance.Core.AudioPayloadTypes);
            SaveCodecs(LinphoneManager.Instance.Core.VideoPayloadTypes);
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Is AMR narrow band audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool AMRNB {
            get {
                return Convert.ToBoolean(Get(AMRNBSettingKeyName));
            }
            set {
                Set(AMRNBSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is AMR wideband audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool AMRWB {
            get {
                return Convert.ToBoolean(Get(AMRWBSettingKeyName));
            }
            set {
                Set(AMRWBSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is speex 16000Hz audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool Speex16 {
            get {
                return Convert.ToBoolean(Get(Speex16SettingKeyName));
            }
            set {
                Set(Speex16SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is speex 8000Hz audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool Speex8 {
            get {
                return Convert.ToBoolean(Get(Speex8SettingKeyName));
            }
            set {
                Set(Speex8SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is PCMU (G.711 ulaw) audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool PCMU {
            get {
                return Convert.ToBoolean(Get(PCMUSettingKeyName));
            }
            set {
                Set(PCMUSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is PCMA (G.711 alaw) audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool PCMA {
            get {
                return Convert.ToBoolean(Get(PCMASettingKeyName));
            }
            set {
                Set(PCMASettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is G.722 audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool G722 {
            get {
                return Convert.ToBoolean(Get(G722SettingKeyName));
            }
            set {
                Set(G722SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is G.729 audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool G729 {
            get {
                return Convert.ToBoolean(Get(G729SettingKeyName));
            }
            set {
                Set(G729SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is iLBC audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool ILBC {
            get {
                return Convert.ToBoolean(Get(ILBCSettingKeyName));
            }
            set {
                Set(ILBCSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is Silk 16000Hz audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool SILK16 {
            get {
                return Convert.ToBoolean(Get(SILK16SettingKeyName));
            }
            set {
                Set(SILK16SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is GSM audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool GSM {
            get {
                return Convert.ToBoolean(Get(GSMSettingKeyName));
            }
            set {
                Set(GSMSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is OPUS audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool OPUS {
            get {
                return Convert.ToBoolean(Get(OpusSettingKeyName));
            }
            set {
                Set(OpusSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is ISAC audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool Isac {
            get {
                return Convert.ToBoolean(Get(IsacSettingKeyName));
            }
            set {
                Set(IsacSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is H.264 video codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool H264 {
            get {
                return Convert.ToBoolean(Get(H264SettingKeyName));
            }
            set {
                Set(H264SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is VP8 video codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool VP8 {
            get {
                return Convert.ToBoolean(Get(VP8SettingKeyName));
            }
            set {
                Set(VP8SettingKeyName, value.ToString());
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle call settings.
    /// </summary>
    public class CallSettingsManager : SettingsManager, ISettingsManager {
        #region Constants settings names
        private const string SendDTMFsRFC2833KeyName = "SendDTMFsRFC2833";
        private const string SendDTMFsSIPInfoKeyName = "SendDTMFsSIPInfo";
        private const string VideoEnabledKeyName = "VideoEnabled";
        private const string AutomaticallyInitiateVideoKeyName = "AutomaticallyInitiateVideo";
        private const string AutomaticallyAcceptVideoKeyName = "AutomaticallyAcceptVideo";
        private const string SelfViewEnabledKeyName = "SelfViewEnabled";
        private const string PreferredVideoSizeKeyName = "PreferredVideoSize";
        private const string DownloadBandwidthKeyName = "DownloadBandwidth";
        private const string UploadBandwidthKeyName = "UploadBandwidth";
        #endregion

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Loads the call settings.
        /// </summary>
        public void Load() {
            dict[SendDTMFsRFC2833KeyName] = LinphoneManager.Instance.Core.UseRfc2833ForDtmf.ToString();
            dict[SendDTMFsSIPInfoKeyName] = LinphoneManager.Instance.Core.UseInfoForDtmf.ToString();
            dict[VideoEnabledKeyName] = LinphoneManager.Instance.Core.VideoCaptureEnabled.ToString();
            VideoActivationPolicy policy = LinphoneManager.Instance.Core.VideoActivationPolicy;
            dict[AutomaticallyInitiateVideoKeyName] = policy.AutomaticallyInitiate.ToString();
            dict[AutomaticallyAcceptVideoKeyName] = policy.AutomaticallyAccept.ToString();
            dict[SelfViewEnabledKeyName] = LinphoneManager.Instance.Core.SelfViewEnabled.ToString();
            dict[PreferredVideoSizeKeyName] = LinphoneManager.Instance.Core.PreviewVideoDefinition.Name;
            dict[DownloadBandwidthKeyName] = LinphoneManager.Instance.Core.DownloadBandwidth.ToString();
            dict[UploadBandwidthKeyName] = LinphoneManager.Instance.Core.UploadBandwidth.ToString();
        }

        /// <summary>
        /// Saves the call settings.
        /// </summary>
        public void Save() {
            if (ValueChanged(SendDTMFsRFC2833KeyName)) {
                LinphoneManager.Instance.Core.UseRfc2833ForDtmf = Convert.ToBoolean(GetNew(SendDTMFsRFC2833KeyName));
            }
            if (ValueChanged(SendDTMFsSIPInfoKeyName)) {
                LinphoneManager.Instance.Core.UseInfoForDtmf = Convert.ToBoolean(GetNew(SendDTMFsSIPInfoKeyName));
            }
            if (ValueChanged(VideoEnabledKeyName)) {
                bool isVideoEnabled = Convert.ToBoolean(GetNew(VideoEnabledKeyName));
                LinphoneManager.Instance.Core.VideoCaptureEnabled = isVideoEnabled;
                LinphoneManager.Instance.Core.VideoDisplayEnabled = isVideoEnabled;
            }
            if (ValueChanged(AutomaticallyInitiateVideoKeyName) || ValueChanged(AutomaticallyAcceptVideoKeyName)) {
                VideoActivationPolicy policy = Factory.Instance.CreateVideoActivationPolicy();

                LinphoneManager.Instance.Core.VideoActivationPolicy = policy;
            }
            if (ValueChanged(SelfViewEnabledKeyName)) {
                LinphoneManager.Instance.Core.SelfViewEnabled = Convert.ToBoolean(GetNew(SelfViewEnabledKeyName));
            }
            if (ValueChanged(PreferredVideoSizeKeyName)) {
                LinphoneManager.Instance.Core.PreviewVideoDefinition = Factory.Instance.CreateVideoDefinitionFromName(GetNew(PreferredVideoSizeKeyName));
            }
            if (ValueChanged(DownloadBandwidthKeyName)) {
                LinphoneManager.Instance.Core.DownloadBandwidth = Convert.ToInt32(GetNew(DownloadBandwidthKeyName));
            }
            if (ValueChanged(UploadBandwidthKeyName)) {
                LinphoneManager.Instance.Core.UploadBandwidth = Convert.ToInt32(GetNew(UploadBandwidthKeyName));
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// DTMFs using RFC2833 setting (bool).
        /// </summary>
        public bool? SendDTFMsRFC2833 {
            get {
                return Convert.ToBoolean(Get(SendDTMFsRFC2833KeyName));
            }
            set {
                Set(SendDTMFsRFC2833KeyName, value.ToString());
            }
        }

        /// <summary>
        /// DTMFs using SIP INFO setting (bool).
        /// </summary>
        public bool? SendDTFMsSIPInfo {
            get {
                return Convert.ToBoolean(Get(SendDTMFsSIPInfoKeyName));
            }
            set {
                Set(SendDTMFsSIPInfoKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Video enabled setting (bool).
        /// </summary>
        public bool? VideoEnabled {
            get {
                return Convert.ToBoolean(Get(VideoEnabledKeyName));
            }
            set {
                Set(VideoEnabledKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Automatically initiate video on outgoing call setting (bool).
        /// </summary>
        public bool? AutomaticallyInitiateVideo {
            get {
                return Convert.ToBoolean(Get(AutomaticallyInitiateVideoKeyName));
            }
            set {
                Set(AutomaticallyInitiateVideoKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Automatically accept video on incoming call setting (bool).
        /// </summary>
        public bool? AutomaticallyAcceptVideo {
            get {
                return Convert.ToBoolean(Get(AutomaticallyAcceptVideoKeyName));
            }
            set {
                Set(AutomaticallyAcceptVideoKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Display self view during calls (bool).
        /// </summary>
        public bool? SelfViewEnabled {
            get {
                return Convert.ToBoolean(Get(SelfViewEnabledKeyName));
            }
            set {
                Set(SelfViewEnabledKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Preferred video size (String).
        /// </summary>
        public string PreferredVideoSize {
            get {
                return Get(PreferredVideoSizeKeyName);
            }
            set {
                Set(PreferredVideoSizeKeyName, value);
            }
        }

        /// <summary>
        /// Download bandwidth (int).
        /// </summary>
        public int DownloadBandwidth {
            get {
                return Convert.ToInt32(Get(DownloadBandwidthKeyName));
            }
            set {
                Set(DownloadBandwidthKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Upload bandwidth (int).
        /// </summary>
        public int UploadBandwidth {
            get {
                return Convert.ToInt32(Get(UploadBandwidthKeyName));
            }
            set {
                Set(UploadBandwidthKeyName, value.ToString());
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle network settings.
    /// </summary>
    public class NetworkSettingsManager : SettingsManager, ISettingsManager {
        private Config Config;
        private Dictionary<string, string> TunnelModeToString;
        private Dictionary<string, MediaEncryption> MediaEncryptionToEnum;
        private Dictionary<string, string> StringToTunnelMode;
        private Boolean IPV6Enabled;
        private Boolean Ice;
        public Dictionary<MediaEncryption, string> EnumToMediaEncryption;

        #region Constants settings names
        private const string MediaEncryptionKeyName = "MediaEncryption";
        private const string FirewallPolicyKeyName = "FirewallPolicy";
        private const string StunServerKeyName = "StunServer";
        private const string TunnelServerKeyName = "TunnelServer";
        private const string TunnelPortKeyName = "TunnelPort";
        private const string TunnelModeKeyName = "TunnelMode";
        #endregion

        /// <summary>
        /// Public constructor.
        /// </summary>
        public NetworkSettingsManager() {
            if (LinphoneManager.Instance.Core == null) {
                //  Config = LinphoneManager.Instance.Core.CreateConfig(InitManager.GetConfigPath(), InitManager.GetFactoryConfigPath());
            } else {
                Config = LinphoneManager.Instance.Core.Config;
            }

            if (LinphoneManager.Instance.isMobileVersion()) {
                TunnelModeToString = new Dictionary<string, string>() {
                    { ResourceLoader.GetForCurrentView().GetString("TunnelMode3GOnly"), "3gonly" },
                    { ResourceLoader.GetForCurrentView().GetString("TunnelModeAlways"), "always" },
                    { ResourceLoader.GetForCurrentView().GetString("TunnelModeAuto"), "auto" },
                    { ResourceLoader.GetForCurrentView().GetString("TunnelModeDisabled"), "disabled" }
                };
                StringToTunnelMode = new Dictionary<string, string>() {
                    { "3gonly", ResourceLoader.GetForCurrentView().GetString("TunnelMode3GOnly") },
                    { "always", ResourceLoader.GetForCurrentView().GetString("TunnelModeAlways") },
                    { "auto", ResourceLoader.GetForCurrentView().GetString("TunnelModeAuto") },
                    { "disabled", ResourceLoader.GetForCurrentView().GetString("TunnelModeDisabled") }
                };
            } else {
                TunnelModeToString = new Dictionary<string, string>() { };
                StringToTunnelMode = new Dictionary<string, string>() { };
            }
            MediaEncryptionToEnum = new Dictionary<string, MediaEncryption>()
            {
                { "None", MediaEncryption.None },
                { "SRTP", MediaEncryption.SRTP },
                { "ZRTP", MediaEncryption.ZRTP },
                { "DTLS", MediaEncryption.DTLS }
            };
            EnumToMediaEncryption = new Dictionary<MediaEncryption, string>()
            {
                { MediaEncryption.None, "None" },
                { MediaEncryption.SRTP, "SRTP" },
                { MediaEncryption.ZRTP, "ZRTP" },
                { MediaEncryption.DTLS, "DTLS" }
            };
        }

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Load the network settings.
        /// </summary>
        public void Load() {
            dict[StunServerKeyName] = LinphoneManager.Instance.Core.StunServer;
            dict[MediaEncryptionKeyName] = EnumToMediaEncryption[LinphoneManager.Instance.Core.MediaEncryption];
            IPV6Enabled = LinphoneManager.Instance.Core.Ipv6Enabled;
            // Load tunnel configuration
            //  dict[TunnelModeKeyName] = AppResources.TunnelModeDisabled;
            dict[TunnelServerKeyName] = "";
            dict[TunnelPortKeyName] = "";
            /*if (LinphoneManager.Instance.Core.TunnelAvailable)
            {
               // String mode = Config.GetString(ApplicationSection, TunnelModeKeyName, TunnelModeToString[AppResources.TunnelModeDisabled]);
             /*   dict[TunnelModeKeyName] = StringToTunnelMode[mode];
                Tunnel tunnel = LinphoneManager.Instance.LinphoneCore.Tunnel;
                if (tunnel != null)
                {
                    IList<Object> servers = tunnel.GetServers();
                    if (servers.Count > 0)
                    {
                        TunnelConfig conf = servers[0] as TunnelConfig;
                        dict[TunnelServerKeyName] = conf.Host;
                        dict[TunnelPortKeyName] = conf.Port.ToString();
                    }
                }
            }*/
        }

        /// <summary>
        /// Save the network settings.
        /// </summary>
        public void Save() {
            if (ValueChanged(StunServerKeyName))
                LinphoneManager.Instance.Core.StunServer = GetNew(StunServerKeyName);

            if (ValueChanged(MediaEncryptionKeyName)) {
                string mediaEncryption = GetNew(MediaEncryptionKeyName);
                LinphoneManager.Instance.Core.MediaEncryption = MediaEncryptionToEnum[mediaEncryption];
            }

            LinphoneManager.Instance.Core.Ipv6Enabled = IPV6Enabled;
            if (LinphoneManager.Instance.Core.NatPolicy != null)
                LinphoneManager.Instance.Core.NatPolicy.IceEnabled = Ice;

            // Save tunnel configuration
            /* if (LinphoneManager.Instance.Core.TunnelAvailable && Customs.IsTunnelEnabled)
             {
                 Boolean needTunnelReconfigure = false;
                 if (ValueChanged(TunnelServerKeyName) || ValueChanged(TunnelPortKeyName))
                 {
                     Tunnel tunnel = LinphoneManager.Instance.LinphoneCore.Tunnel;
                     if (tunnel != null)
                     {
                         tunnel.CleanServers();
                         String server = GetNew(TunnelServerKeyName);
                         Int32 port = 0;
                         try
                         {
                             port = Convert.ToInt32(GetNew(TunnelPortKeyName));
                         }
                         catch (System.FormatException) { }
                         if ((server.Length > 0) && (port > 0))
                         {
                             tunnel.AddServer(GetNew(TunnelServerKeyName), Convert.ToInt32(GetNew(TunnelPortKeyName)));
                         }
                         needTunnelReconfigure = true;
                     }
                 }
                 if (ValueChanged(TunnelModeKeyName))
                 {
                     String mode = GetNew(TunnelModeKeyName);
                     Config.SetString(ApplicationSection, TunnelModeKeyName, TunnelModeToString[mode]);
                     needTunnelReconfigure = true;
                 }
                 if (needTunnelReconfigure)
                     LinphoneManager.Instance.ConfigureTunnel();
             }*/
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Media encryption setting (String).
        /// </summary>
        public string MEncryption {
            get {
                return (Get(MediaEncryptionKeyName) != null) ? Get(MediaEncryptionKeyName) : "";
            }
            set {
                Set(MediaEncryptionKeyName, value);
            }
        }

        /// <summary>
        /// Firewall policy setting (Boolean).
        /// </summary>
        public Boolean FWPolicy {
            get {
                return Ice;
            }
            set {
                Ice = value;
            }
        }

        /// <summary>
        /// Stun server setting (String).
        /// </summary>
        public string StunServer {
            get {
                return (Get(StunServerKeyName) != null) ? Get(StunServerKeyName) : "";
            }
            set {
                Set(StunServerKeyName, value);
            }
        }

        /// <summary>
        /// Tunnel server setting (String).
        /// </summary>
        public string TunnelServer {
            get {
                return (Get(TunnelServerKeyName) != null) ? Get(TunnelServerKeyName) : "";
            }
            set {
                Set(TunnelServerKeyName, value);
            }
        }

        /// <summary>
        /// Tunnel port setting (Integer).
        /// </summary>
        public string TunnelPort {
            get {
                return (Get(TunnelPortKeyName) != null) ? Get(TunnelPortKeyName) : "";
            }
            set {
                Set(TunnelPortKeyName, value);
            }
        }

        /// <summary>
        /// Tunnel mode setting (Auto, Disabled, 3G Only or Always).
        /// </summary>
        public string TunnelMode {
            get {
                return (Get(TunnelModeKeyName) != null) ? Get(TunnelModeKeyName) : "";
            }
            set {
                Set(TunnelModeKeyName, value);
            }
        }

        public Boolean IPV6 {
            get {
                return IPV6Enabled;
            }
            set {
                IPV6Enabled = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle chat settings.
    /// </summary>
    public class ChatSettingsManager : SettingsManager, ISettingsManager {
        // IsolatedStorageSettings _settings;

        #region Constants settings names
        private const string VibrateOnIncomingMessageKeyName = "VibrateOnIncomingMessage";
        private const string ScaleDownSentPicturesKeyName = "ScaleDownSentPictures";
        #endregion

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Loads the call settings.
        /// </summary>
        public void Load() {
            // _settings = IsolatedStorageSettings.ApplicationSettings;

            // string value;
            /* if (_settings.Contains(VibrateOnIncomingMessageKeyName))
                 value = ((bool)_settings[VibrateOnIncomingMessageKeyName]).ToString();
             else
                 value = true.ToString();
             dict[VibrateOnIncomingMessageKeyName] = value;

             if (_settings.Contains(ScaleDownSentPicturesKeyName))
                 value = ((bool)_settings[ScaleDownSentPicturesKeyName]).ToString();
             else
                 value = true.ToString();
             dict[ScaleDownSentPicturesKeyName] = value;*/
        }

        /// <summary>
        /// Saves the call settings.
        /// </summary>
        public void Save() {
            /* if (ValueChanged(VibrateOnIncomingMessageKeyName))
             {
                 bool value = Convert.ToBoolean(GetNew(VibrateOnIncomingMessageKeyName));
                 if (_settings.Contains(VibrateOnIncomingMessageKeyName))
                     _settings[VibrateOnIncomingMessageKeyName] = value;
                 else
                     _settings.Add(VibrateOnIncomingMessageKeyName, value);
                 _settings.Save();
             }
             if (ValueChanged(ScaleDownSentPicturesKeyName))
             {
                 bool value = Convert.ToBoolean(GetNew(ScaleDownSentPicturesKeyName));
                 if (_settings.Contains(ScaleDownSentPicturesKeyName))
                     _settings[ScaleDownSentPicturesKeyName] = value;
                 else
                     _settings.Add(ScaleDownSentPicturesKeyName, value);
                 _settings.Save();
             }*/
        }
        #endregion

        #region Accessors
        /// <summary>
        /// DTMFs using RFC2833 setting (bool).
        /// </summary>
        public bool? VibrateOnIncomingMessage {
            get {
                return Convert.ToBoolean(Get(VibrateOnIncomingMessageKeyName));
            }
            set {
                Set(VibrateOnIncomingMessageKeyName, value.ToString());
            }
        }
        public bool? ScaleDownSentPictures {
            get {
                return Convert.ToBoolean(Get(ScaleDownSentPicturesKeyName));
            }
            set {
                Set(ScaleDownSentPicturesKeyName, value.ToString());
            }
        }
        #endregion
    }
}
