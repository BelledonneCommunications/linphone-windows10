using Linphone.Core;
using Linphone.Resources;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    /// <summary>
    /// Utility class used to handle everything that's application setting related.
    /// </summary>
    public class SettingsManager
    {
        private Dictionary<String, String> dict;
        private Dictionary<String, String> changesDict;

        private const string ApplicationSection = "app";

        #region Constants settings names
        private const string UsernameKeyName = "Username";
        private const string PasswordKeyName = "Password";
        private const string DomainKeyName = "Domain";
        private const string ProxyKeyName = "Proxy";
        private const string OutboundProxyKeyName = "OutboundProxy";

        private const string DebugSettingKeyName = "Debug";
        private const string SIPTransportSettingKeyName = "SIPTransport";
        private const string SIPPortKeyName = "SIPPort";
        private const string SendDTFMsRFC2833KeyName = "SendDTFMsRFC2833";

        private const string AMRNBSettingKeyName = "CodecAMRNB";
        private const string AMRWBSettingKeyName = "CodecAMRWB";
        private const string Speex16SettingKeyName = "CodecSpeex16";
        private const string Speex8SettingKeyName = "CodecSpeex8";
        private const string PCMUSettingKeyName = "CodecPCMU";
        private const string PCMASettingKeyName = "CodecPCMA";
        private const string G722SettingKeyName = "CodecG722";
        private const string ILBCSettingKeyName = "CodecILBC";
        private const string SILK16SettingKeyName = "CodecSILK16";
        private const string GSMSettingKeyName = "CodecGSM";

        private const string TunnelServerKeyName = "TunnelServer";
        private const string TunnelPortKeyName = "TunnelPort";
        private const string TunnelModeKeyName = "TunnelMode";
        #endregion

        /// <summary>
        /// Public constructor.
        /// </summary>
        public SettingsManager()
        {
            dict = new Dictionary<String, String>();
            changesDict = new Dictionary<String, String>();
            Load();
        }

        /// <summary>
        /// Load the SIP account settings.
        /// </summary>
        private void LoadSIPAccount()
        {
            dict[UsernameKeyName] = "";
            dict[DomainKeyName] = "";
            dict[ProxyKeyName] = "";
            dict[PasswordKeyName] = "";
            dict[OutboundProxyKeyName] = false.ToString();
            LinphoneProxyConfig cfg = LinphoneManager.Instance.LinphoneCore.GetDefaultProxyConfig();
            if (cfg != null)
            {
                LinphoneAddress address = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress(cfg.GetIdentity());
                if (address != null)
                {
                    LinphoneAddress proxyAddress = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress(cfg.GetAddr());
                    int proxyPort = proxyAddress.GetPort();
                    dict[UsernameKeyName] = address.GetUserName();
                    dict[DomainKeyName] = address.GetDomain();
                    if (address.GetDomain() != proxyAddress.GetDomain())
                    {
                        dict[ProxyKeyName] = proxyAddress.GetDomain();
                    }
                    else if (proxyPort > 0)
                    {
                        dict[ProxyKeyName] = String.Format("{0}:{1}", proxyAddress.GetDomain(), proxyPort);
                    }
                    dict[OutboundProxyKeyName] = (cfg.GetRoute().Length > 0).ToString();
                    var authInfos = LinphoneManager.Instance.LinphoneCore.GetAuthInfos();
                    if (authInfos.Count > 0)
                    {
                        dict[PasswordKeyName] = ((LinphoneAuthInfo)authInfos[0]).GetPassword();
                    }
                }
            }
        }

        private String GetKeyNameForCodec(String mimeType, int clockRate)
        {
            Dictionary<Tuple<String, int>, String> map = new Dictionary<Tuple<String, int>, String>
            {
                { new Tuple<String, int>("amr", 8000), AMRNBSettingKeyName },
                { new Tuple<String, int>("amr", 16000), AMRWBSettingKeyName },
                { new Tuple<String, int>("speex", 16000), Speex16SettingKeyName },
                { new Tuple<String, int>("speex", 8000), Speex8SettingKeyName },
                { new Tuple<String, int>("pcmu", 8000), PCMUSettingKeyName },
                { new Tuple<String, int>("pcma", 8000), PCMASettingKeyName },
                { new Tuple<String, int>("g722", 8000), G722SettingKeyName },
                { new Tuple<String, int>("ilbc", 8000), ILBCSettingKeyName },
                { new Tuple<String, int>("silk", 16000), SILK16SettingKeyName },
                { new Tuple<String, int>("gsm", 8000), GSMSettingKeyName },
            };

            Tuple<String, int> key = new Tuple<String, int>(mimeType.ToLower(), clockRate);
            if (map.ContainsKey(key))
            {
                return map[key];
            }
            return null;
        }

        private void LoadCodecs(IList<Object> ptlist)
        {
            foreach (PayloadType pt in ptlist)
            {
                String keyname = GetKeyNameForCodec(pt.GetMimeType(), pt.GetClockRate());
                if (keyname != null)
                {
                    dict[keyname] = LinphoneManager.Instance.LinphoneCore.PayloadTypeEnabled(pt).ToString();
                }
                else
                {
                    Logger.Warn("Codec {0}/{1} supported by core is not shown in the settings view, disable it", pt.GetMimeType(), pt.GetClockRate());
                    LinphoneManager.Instance.LinphoneCore.EnablePayloadType(pt, false);
                }
            }
        }

        /// <summary>
        /// Load the codecs settings.
        /// </summary>
        private void LoadCodecs()
        {
            LoadCodecs(LinphoneManager.Instance.LinphoneCore.GetAudioCodecs());
        }

        /// <summary>
        /// Load the network settings.
        /// </summary>
        private void LoadNetwork()
        {
            Transports transports = LinphoneManager.Instance.LinphoneCore.GetSignalingTransportsPorts();
            String tname = AppResources.TransportUDP;
            int port = 5060;
            if (transports.UDP > 0)
            {
                tname = AppResources.TransportUDP;
                port = transports.UDP;
            }
            else if (transports.TCP > 0)
            {
                tname = AppResources.TransportTCP;
                port = transports.TCP;
            }
            else if (transports.TLS > 0)
            {
                tname = AppResources.TransportTLS;
                port = transports.TLS;
            }
            dict[SIPTransportSettingKeyName] = tname;
            dict[SIPPortKeyName] = port.ToString();
        }

        /// <summary>
        /// Load the settings.
        /// </summary>
        private void Load()
        {
            LoadSIPAccount();
            LoadCodecs();
            LoadNetwork();
            dict[DebugSettingKeyName] = LinphoneManager.Instance.LinphoneCore.GetConfig().GetBool(ApplicationSection, DebugSettingKeyName, false).ToString();
        }

        /// <summary>
        /// Static access to the debug enabled setting.
        /// </summary>
        public static bool isDebugEnabled
        {
            get
            {
                SettingsManager sm = new SettingsManager();
                return sm.DebugEnabled;
            }
        }

        private String Get(String Key)
        {
            if (dict.ContainsKey(Key))
            {
                return dict[Key];
            }
            else
            {
                return null;
            }
        }

        private String GetNew(String Key)
        {
            if (changesDict.ContainsKey(Key))
            {
                return changesDict[Key];
            }
            else if (dict.ContainsKey(Key))
            {
                return dict[Key];
            }
            return null;
        }

        private void Set(String Key, String Value)
        {
            if (dict.ContainsKey(Key))
            {
                if (dict[Key] != Value)
                {
                    changesDict[Key] = Value;
                }
            }
            else
            {
                changesDict[Key] = Value;
            }
        }

        private bool ValueChanged(String Key)
        {
            return changesDict.ContainsKey(Key);
        }

        /// <summary>
        /// Save the SIP account settings.
        /// </summary>
        private void SaveSIPAccount()
        {
            bool AccountChanged = ValueChanged(UsernameKeyName) || ValueChanged(PasswordKeyName) || ValueChanged(DomainKeyName)
                || ValueChanged(ProxyKeyName) || ValueChanged(OutboundProxyKeyName);

            if (AccountChanged)
            {
                LinphoneCore lc = LinphoneManager.Instance.LinphoneCore;
                LinphoneProxyConfig cfg = lc.GetDefaultProxyConfig();
                if (cfg != null)
                {
                    // TODO: Force unregister
                }

                String username = GetNew(UsernameKeyName);
                String password = GetNew(PasswordKeyName);
                String domain = GetNew(DomainKeyName);
                bool outboundProxy = Convert.ToBoolean(GetNew(OutboundProxyKeyName));
                lc.ClearAuthInfos();
                lc.ClearProxyConfigs();
                if ((username != null) && (username.Length > 0) && (domain != null) && (domain.Length > 0))
                {
                    String proxy = GetNew(ProxyKeyName);
                    if ((proxy != null) && (proxy.Length > 0))
                    {
                        proxy = String.Format("sip:{0}", proxy);
                    }
                    else
                    {
                        proxy = String.Format("sip:{0}", domain);
                    }

                    cfg = lc.CreateEmptyProxyConfig();
                    cfg.SetIdentity(username, username, domain);
                    cfg.SetProxy(proxy);
                    cfg.EnableRegister(true);
                    // Can't set string to null: http://stackoverflow.com/questions/12980915/exception-when-trying-to-read-null-string-in-c-sharp-winrt-component-from-winjs
                    var auth = lc.CreateAuthInfo(username, "", password, "", domain);
                    lc.AddAuthInfo(auth);

                    if (outboundProxy)
                    {
                        cfg.SetRoute(proxy);
                    }

                    lc.AddProxyConfig(cfg);
                    lc.SetDefaultProxyConfig(cfg);
                }
            }
        }

        private void SaveCodecs(IList<Object> ptlist)
        {
            foreach (PayloadType pt in ptlist)
            {
                String keyname = GetKeyNameForCodec(pt.GetMimeType(), pt.GetClockRate());
                if ((keyname != null) && ValueChanged(keyname))
                {
                    LinphoneManager.Instance.LinphoneCore.EnablePayloadType(pt, Convert.ToBoolean(GetNew(keyname)));
                }
            }
        }

        /// <summary>
        /// Save the codecs settings.
        /// </summary>
        private void SaveCodecs()
        {
            SaveCodecs(LinphoneManager.Instance.LinphoneCore.GetAudioCodecs());
        }

        /// <summary>
        /// Save the network settings.
        /// </summary>
        private void SaveNetwork()
        {
            if (ValueChanged(SIPTransportSettingKeyName))
            {
                Transports transports = LinphoneManager.Instance.LinphoneCore.GetSignalingTransportsPorts();
                int port = Convert.ToInt32(GetNew(SIPPortKeyName));
                if (GetNew(SIPTransportSettingKeyName) == AppResources.TransportUDP)
                {
                    transports.UDP = port;
                }
                else if (GetNew(SIPTransportSettingKeyName) == AppResources.TransportTCP)
                {
                    transports.TCP = port;
                }
                else if (GetNew(SIPTransportSettingKeyName) == AppResources.TransportTLS)
                {
                    transports.TLS = port;
                }
                LinphoneManager.Instance.LinphoneCore.SetSignalingTransportsPorts(transports);
            }
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            SaveSIPAccount();
            SaveCodecs();
            SaveNetwork();

            LinphoneCore lc = LinphoneManager.Instance.LinphoneCore;
            bool debugEnabled = Convert.ToBoolean(GetNew(DebugSettingKeyName));
            lc.GetConfig().SetBool(ApplicationSection, DebugSettingKeyName, debugEnabled);
            LinphoneManager.Instance.ConfigureLogger();
        }

        #region SIP Account
        /// <summary>
        /// SIP Account username setting (String).
        /// </summary>
        public string Username
        {
            get
            {
                return Get(UsernameKeyName);
            }
            set
            {
                Set(UsernameKeyName, value);
            }
        }

        /// <summary>
        /// SIP account password setting (String).
        /// </summary>
        public string Password
        {
            get
            {
                return Get(PasswordKeyName);
            }
            set
            {
                Set(PasswordKeyName, value);
            }
        }

        /// <summary>
        /// SIP account domain setting (String).
        /// </summary>
        public string Domain
        {
            get
            {
                return Get(DomainKeyName);
            }
            set
            {
                Set(DomainKeyName, value);
            }
        }

        /// <summary>
        /// SIP account proxy setting (String).
        /// </summary>
        public string Proxy
        {
            get
            {
                return Get(ProxyKeyName);
            }
            set
            {
                Set(ProxyKeyName, value);
            }
        }

        /// <summary>
        /// SIP account outbound proxy setting (Bool).
        /// </summary>
        public bool? OutboundProxy
        {
            get
            {
                return Convert.ToBoolean(Get(OutboundProxyKeyName));
            }
            set
            {
                Set(OutboundProxyKeyName, value.ToString());
            }
        }
        #endregion

        /// <summary>
        /// Debug enabled setting (Bool).
        /// </summary>
        public bool DebugEnabled
        {
            get
            {
                return Convert.ToBoolean(Get(DebugSettingKeyName));
            }
            set
            {
                Set(DebugSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Transport setting (UDP or TCP).
        /// </summary>
        public string Transport
        {
            get
            {
                return Get(SIPTransportSettingKeyName);
            }
            set
            {
                Set(SIPTransportSettingKeyName, value);
            }
        }

        /// <summary>
        /// DTMFs using RFC2833 setting (Bool).
        /// </summary>
        public bool? SendDTFMsRFC2833
        {
            get
            {
                return false;   // TODO
            }
            set
            {
                // TODO
            }
        }

        #region Codecs Settings
        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool AMRNB
        {
            get
            {
                return Convert.ToBoolean(Get(AMRNBSettingKeyName));
            }
            set
            {
                Set(AMRNBSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool AMRWB
        {
            get
            {
                return Convert.ToBoolean(Get(AMRWBSettingKeyName));
            }
            set
            {
                Set(AMRWBSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool Speex16
        {
            get
            {
                return Convert.ToBoolean(Get(Speex16SettingKeyName));
            }
            set
            {
                Set(Speex16SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool Speex8
        {
            get
            {
                return Convert.ToBoolean(Get(Speex8SettingKeyName));
            }
            set
            {
                Set(Speex8SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool PCMU
        {
            get
            {
                return Convert.ToBoolean(Get(PCMUSettingKeyName));
            }
            set
            {
                Set(PCMUSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool PCMA
        {
            get
            {
                return Convert.ToBoolean(Get(PCMASettingKeyName));
            }
            set
            {
                Set(PCMASettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool G722
        {
            get
            {
                return Convert.ToBoolean(Get(G722SettingKeyName));
            }
            set
            {
                Set(G722SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool ILBC
        {
            get
            {
                return Convert.ToBoolean(Get(ILBCSettingKeyName));
            }
            set
            {
                Set(ILBCSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool SILK16
        {
            get
            {
                return Convert.ToBoolean(Get(SILK16SettingKeyName));
            }
            set
            {
                Set(SILK16SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is this codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool GSM
        {
            get
            {
                return Convert.ToBoolean(Get(GSMSettingKeyName));
            }
            set
            {
                Set(GSMSettingKeyName, value.ToString());
            }
        }
        #endregion

        #region Tunnel Settings
        /// <summary>
        /// Tunnel server setting (String).
        /// </summary>
        public string TunnelServer
        {
            get
            {
                return "";  // TODO
            }
            set
            {
                // TODO
            }
        }

        /// <summary>
        /// Tunnel port setting (Integer).
        /// </summary>
        public string TunnelPort
        {
            get
            {
                return "";  // TODO
            }
            set
            {
                // TODO
            }
        }

        /// <summary>
        /// Tunnel mode setting (Auto, Disabled, 3G Only or Always).
        /// </summary>
        public string TunnelMode
        {
            get
            {
                return AppResources.TunnelModeDisabled;  // TODO
            }
            set
            {
                // TODO
            }
        }
        #endregion
    }
}
