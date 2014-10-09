using Linphone.Agents;
using Linphone.Core;
using Linphone.Resources;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Linphone.Model
{
    /// <summary>
    /// Interface describing the methods that each setting manager must implement.
    /// </summary>
    public interface ISettingsManager
    {
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
    public class SettingsManager
    {
        protected Dictionary<String, String> dict;
        protected Dictionary<String, String> changesDict;
        protected const string ApplicationSection = "app";

        /// <summary>
        /// Public constructor.
        /// </summary>
        public SettingsManager()
        {
            dict = new Dictionary<String, String>();
            changesDict = new Dictionary<String, String>();
        }

        /// <summary>
        /// Install the default config file from the package to the Isolated Storage
        /// </summary>
        public static async Task InstallConfigFile()
        {
            StorageFile destFile = null;
            try
            {
                destFile = await ApplicationData.Current.LocalFolder.GetFileAsync("linphonerc");
            }
            catch (System.IO.FileNotFoundException)
            {
            }

            if (destFile == null)
            {
                StorageFile sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/linphonerc"));
                try
                {
                    destFile = await sourceFile.CopyAsync(ApplicationData.Current.LocalFolder, "linphonerc", NameCollisionOption.FailIfExists);
                } catch {
                    System.Diagnostics.Debug.WriteLine("An error occured while installing config file");
                }
            }
            if (destFile != null)
            {
                System.Diagnostics.Debug.WriteLine("Config file successfully installed");
            }
        }

        /// <summary>
        /// Get the path of the config file stored in the Isolated Storage
        /// </summary>
        /// <returns>The path of the config file</returns>
        public static String GetConfigPath()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            return localFolder.Path + "\\linphonerc";
        }

        /// <summary>
        /// Get the path of the factory config file stored in the package
        /// </summary>
        /// <returns>The path of the factory config file</returns>
        public static String GetFactoryConfigPath()
        {
            return "Assets/linphonerc-factory";
        }

        /// <summary>
        /// Get the value of a settings parameter from its name
        /// </summary>
        /// <param name="Key">The name of the settings parameter for which we want the value</param>
        /// <returns>The value of the settings parameter</returns>
        protected String Get(String Key)
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

        /// <summary>
        /// Get the changed value of a settings parameter from its name
        /// </summary>
        /// <param name="Key">The name of the settings parameter for which we want the changed value</param>
        /// <returns>The changed value of the settings parameter</returns>
        protected String GetNew(String Key)
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

        /// <summary>
        /// Set a new value for a settings parameter given its name
        /// </summary>
        /// <param name="Key">The name of the settings parameter to change</param>
        /// <param name="Value">The new value to be set for the settings parameter</param>
        protected void Set(String Key, String Value)
        {
            if (dict.ContainsKey(Key))
            {
                if (dict[Key] != Value || Value.Length == 0)
                {
                    changesDict[Key] = Value;
                }
            }
            else
            {
                changesDict[Key] = Value;
            }
        }

        /// <summary>
        /// Tell whether a settings parameter has been changed given its name
        /// </summary>
        /// <param name="Key">The name of the settings parameter</param>
        /// <returns>A boolean telling whether the settings parameter has been changed or not</returns>
        protected bool ValueChanged(String Key)
        {
            return changesDict.ContainsKey(Key);
        }
    }

    /// <summary>
    /// Utility class used to handle application settings.
    /// </summary>
    public class ApplicationSettingsManager : SettingsManager, ISettingsManager
    {
        internal const String LinphoneLogFileName = "Linphone.log";
        private LpConfig Config;

        #region Constants settings names
        private const string LogLevelKeyName = "LogLevel";
        private const string LogDestinationKeyName = "LogDestination";
        private const string LogOptionKeyName = "LogOption";
        #endregion

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ApplicationSettingsManager()
        {
            if (LinphoneManager.Instance.LinphoneCore == null)
            {
                Config = LinphoneManager.Instance.LinphoneCoreFactory.CreateLpConfig(GetConfigPath(), GetFactoryConfigPath());
            }
            else
            {
                Config = LinphoneManager.Instance.LinphoneCore.GetConfig();
            }
        }

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Load the application settings.
        /// </summary>
        public void Load()
        {
            dict[LogLevelKeyName] = Config.GetInt(ApplicationSection, LogLevelKeyName, (int)OutputTraceLevel.None).ToString();
            dict[LogDestinationKeyName] = Config.GetString(ApplicationSection, LogDestinationKeyName, OutputTraceDest.File.ToString());
            dict[LogOptionKeyName] = Config.GetString(ApplicationSection, LogOptionKeyName, LinphoneLogFileName);
        }

        /// <summary>
        /// Save the application settings.
        /// </summary>
        public async void Save()
        {
            if (ValueChanged(LogDestinationKeyName) || ValueChanged(LogOptionKeyName))
            {
                var logsDestination = GetNew(LogDestinationKeyName);
                Config.SetString(ApplicationSection, LogDestinationKeyName, logsDestination);

                if (OutputTraceDest.File.ToString().Equals(logsDestination))
                {
                    Config.SetString(ApplicationSection, LogOptionKeyName, LinphoneLogFileName);
                }
                else if (OutputTraceDest.TCPRemote.ToString().Equals(logsDestination))
                {
                    Config.SetString(ApplicationSection, LogOptionKeyName, GetNew(LogOptionKeyName));
                }
                LinphoneManager.Instance.ConfigureLogger();
            }
            else if (ValueChanged(LogLevelKeyName))
            {
                try
                {
                    Config.SetInt(ApplicationSection, LogLevelKeyName, Convert.ToInt32(GetNew(LogLevelKeyName)));
                    LinphoneManager.Instance.ConfigureLogger();
                    if ((Get(LogDestinationKeyName) == OutputTraceDest.File.ToString())
                        && (GetNew(LogLevelKeyName) == ((int)OutputTraceLevel.None).ToString()))
                    {
                        try
                        {
                            StorageFile logfile = await ApplicationData.Current.LocalFolder.GetFileAsync(Get(LogOptionKeyName));
                            await logfile.DeleteAsync();
                        }
                        catch
                        {
                            Logger.Warn("Failed deleting log file {0}", Get(LogOptionKeyName));
                        }
                    }
                }
                catch
                {
                    Logger.Warn("Failed setting the log level name {0}", Get(LogLevelKeyName));
                }
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Debug enabled setting (Bool).
        /// </summary>
        public bool DebugEnabled
        {
            get
            {
                return Convert.ToInt32(Get(LogLevelKeyName)) != (int)OutputTraceLevel.None;
            }
            set
            {
                if (value)
                {
                    Set(LogLevelKeyName, ((int)OutputTraceLevel.Message).ToString());
                }
                else
                {
                    Set(LogLevelKeyName, ((int)OutputTraceLevel.None).ToString());
                }
            }
        }

        /// <summary>
        /// Log level (OutputTraceLevel).
        /// </summary>
        public OutputTraceLevel LogLevel
        {
            get
            {
                return (OutputTraceLevel) Convert.ToInt32(Get(LogLevelKeyName));
            }
            set
            {
                Set(LogLevelKeyName, ((int)value).ToString());
            }
        }

        /// <summary>
        /// Log destination.
        /// </summary>
        public OutputTraceDest LogDestination
        {
            get
            {
                String dest = Get(LogDestinationKeyName);
                if (dest == OutputTraceDest.Debugger.ToString()) return OutputTraceDest.Debugger;
                else if (dest == OutputTraceDest.File.ToString()) return OutputTraceDest.File;
                else if (dest == OutputTraceDest.TCPRemote.ToString()) return OutputTraceDest.TCPRemote;
                return OutputTraceDest.None;
            }
            set
            {
                Set(LogDestinationKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Log option (filename if LogDestination is OutputTraceDest.File, host:port if LogDestination is OutputTraceDest.TCPRemote).
        /// </summary>
        public String LogOption
        {
            get
            {
                return Get(LogOptionKeyName);
            }
            set
            {
                Set(LogOptionKeyName, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle SIP account settings.
    /// </summary>
    public class SIPAccountSettingsManager : SettingsManager, ISettingsManager
    {
        #region Constants settings names
        private const string UsernameKeyName = "Username";
        private const string UserIdKeyName = "UserId";
        private const string PasswordKeyName = "Password";
        private const string DomainKeyName = "Domain";
        private const string ProxyKeyName = "Proxy";
        private const string OutboundProxyKeyName = "OutboundProxy";
        private const string DisplayNameKeyName = "DisplayName";
        private const string TransportKeyName = "Transport";

        private Dictionary<LinphoneTransport, string> EnumToTransport;
        private Dictionary<string, LinphoneTransport> TransportToEnum;
        #endregion

        public SIPAccountSettingsManager()
        {
            EnumToTransport = new Dictionary<LinphoneTransport, string>()
            {
                { LinphoneTransport.LinphoneTransportUDP, AppResources.TransportUDP },
                { LinphoneTransport.LinphoneTransportTCP, AppResources.TransportTCP },
                { LinphoneTransport.LinphoneTransportTLS, AppResources.TransportTLS }
            };

            TransportToEnum = new Dictionary<string, LinphoneTransport>()
            {
                { AppResources.TransportUDP, LinphoneTransport.LinphoneTransportUDP },
                { AppResources.TransportTCP, LinphoneTransport.LinphoneTransportTCP },
                { AppResources.TransportTLS, LinphoneTransport.LinphoneTransportTLS }
            };
        }

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Load the SIP account settings.
        /// </summary>
        public void Load()
        {
            dict[UsernameKeyName] = "";
            dict[UserIdKeyName] = "";
            dict[PasswordKeyName] = "";
            dict[DisplayNameKeyName] = "";
#if DEBUG
            dict[DomainKeyName] = "sip.linphone.org";
            dict[ProxyKeyName] = "sip:sip.linphone.org:5223;transport=tls";
            dict[OutboundProxyKeyName] = true.ToString();
            dict[TransportKeyName] = AppResources.TransportTLS;
#else
            dict[DomainKeyName] = "";
            dict[ProxyKeyName] = "";
            dict[OutboundProxyKeyName] = false.ToString();
            dict[TransportKeyName] = AppResources.TransportUDP;
#endif

            LinphoneProxyConfig cfg = LinphoneManager.Instance.LinphoneCore.GetDefaultProxyConfig();
            if (cfg != null)
            {
                LinphoneAddress address = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress(cfg.GetIdentity());
                if (address != null)
                {
                    LinphoneAddress proxyAddress = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress(cfg.GetAddr());
                    dict[ProxyKeyName] = proxyAddress.AsStringUriOnly();
                    dict[TransportKeyName] = EnumToTransport[proxyAddress.GetTransport()];
                    dict[UsernameKeyName] = address.GetUserName();
                    dict[DomainKeyName] = address.GetDomain();
                    dict[OutboundProxyKeyName] = (cfg.GetRoute().Length > 0).ToString();
                    var authInfos = LinphoneManager.Instance.LinphoneCore.GetAuthInfos();
                    if (authInfos.Count > 0)
                    {
                        LinphoneAuthInfo info = ((LinphoneAuthInfo)authInfos[0]);
                        dict[PasswordKeyName] = info.GetPassword();
                        dict[UserIdKeyName] = info.GetUserId();
                    }
                    dict[DisplayNameKeyName] = address.GetDisplayName();
                }
            }
        }

        /// <summary>
        /// Save the SIP account settings.
        /// </summary>
        public void Save()
        {
            bool AccountChanged = ValueChanged(UsernameKeyName) || ValueChanged(UserIdKeyName) || ValueChanged(PasswordKeyName) || ValueChanged(DomainKeyName)
                || ValueChanged(ProxyKeyName) || ValueChanged(OutboundProxyKeyName) || ValueChanged(DisplayNameKeyName) || ValueChanged(TransportKeyName);

            if (AccountChanged)
            {
                LinphoneCore lc = LinphoneManager.Instance.LinphoneCore;
                LinphoneProxyConfig cfg = lc.GetDefaultProxyConfig();
                if (cfg != null)
                {
                    cfg.Edit();
                    cfg.EnableRegister(false);
                    cfg.Done();

                    //Wait for unregister to complete
                    int timeout = 2000;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (true)
                    {
                        if (stopwatch.ElapsedMilliseconds >= timeout || cfg.GetState() == RegistrationState.RegistrationCleared || cfg.GetState() == RegistrationState.RegistrationNone)
                        {
                            break;
                        }
                        Thread.Sleep(1);
                    }
                }

                String username = GetNew(UsernameKeyName);
                String userid = GetNew(UserIdKeyName);
                String password = GetNew(PasswordKeyName);
                String domain = GetNew(DomainKeyName);
                String proxy = GetNew(ProxyKeyName);
                String displayname = GetNew(DisplayNameKeyName);
                String transport = GetNew(TransportKeyName);
                bool outboundProxy = Convert.ToBoolean(GetNew(OutboundProxyKeyName));
                lc.ClearAuthInfos();
                lc.ClearProxyConfigs();
                if ((username != null) && (username.Length > 0) && (domain != null) && (domain.Length > 0))
                {
                    if ((proxy != null) && (proxy.Length > 0))
                    {
                        // Check if proxy address is correct
                        LinphoneAddress test = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress(proxy);
                        if (test == null)
                        {
                            proxy = String.Format("sip:{0}", domain);
                        }

                        if (transport != null)
                        {
                            LinphoneAddress temp = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress(proxy);
                            if (temp != null)
                            {
                                temp.SetTransport(TransportToEnum[transport]);
                                proxy = temp.AsStringUriOnly();
                            }
                        }
                    }
                    else
                    {
                        proxy = String.Format("sip:{0}", domain);
                        if (transport != null)
                        {
                            LinphoneAddress temp = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress(proxy);
                            if (temp != null)
                            {
                                temp.SetTransport(TransportToEnum[transport]);
                                proxy = temp.AsStringUriOnly();
                            }
                        }
                    }

                    cfg = lc.CreateEmptyProxyConfig();
                    cfg.Edit();
                    if (displayname != null && displayname.Length > 0)
                    {
                        cfg.SetIdentity(displayname, username, domain);
                    }
                    else
                    {
                        cfg.SetIdentity(username, username, domain);
                    }

                    cfg.SetProxy(proxy);

                    if (outboundProxy)
                    {
                        cfg.SetRoute(proxy);
                    }
                    cfg.SetExpires(28800);

                    // Can't set string to null: http://stackoverflow.com/questions/12980915/exception-when-trying-to-read-null-string-in-c-sharp-winrt-component-from-winjs
                    if (userid == null)
                        userid = "";
                    if (password == null)
                        password = "";
                    var auth = lc.CreateAuthInfo(username, userid, password, "", "", domain);
                    lc.AddAuthInfo(auth);

                    lc.AddProxyConfig(cfg);
                    lc.SetDefaultProxyConfig(cfg);
                    LinphoneManager.Instance.AddPushInformationsToContactParams();
                    cfg.EnableRegister(true);
                    cfg.Done();
                }
            }
        }
        #endregion

        #region Accessors
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
        /// SIP Account userid setting (String).
        /// </summary>
        public string UserId
        {
            get
            {
                return Get(UserIdKeyName);
            }
            set
            {
                Set(UserIdKeyName, value);
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

        /// <summary>
        /// SIP account display name setting (String).
        /// </summary>
        public string DisplayName
        {
            get
            {
                return Get(DisplayNameKeyName);
            }
            set
            {
                Set(DisplayNameKeyName, value);
            }
        }

        /// <summary>
        /// Transport (String).
        /// </summary>
        public string Transport
        {
            get
            {
                return Get(TransportKeyName);
            }
            set
            {
                Set(TransportKeyName, value);
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle codecs settings.
    /// </summary>
    public class CodecsSettingsManager : SettingsManager, ISettingsManager
    {
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
        #endregion

        private String GetKeyNameForCodec(String mimeType, int clockRate)
        {
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
            };

            Tuple<String, int> key = new Tuple<String, int>(mimeType.ToLower(), clockRate);
            if (map.ContainsKey(key))
            {
                return map[key];
            }
            return null;
        }

        #region Implementation of the ISettingsManager interface
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
        public void Load()
        {
            LoadCodecs(LinphoneManager.Instance.LinphoneCore.GetAudioCodecs());
            LoadCodecs(LinphoneManager.Instance.LinphoneCore.GetVideoCodecs());
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
        public void Save()
        {
            SaveCodecs(LinphoneManager.Instance.LinphoneCore.GetAudioCodecs());
            SaveCodecs(LinphoneManager.Instance.LinphoneCore.GetVideoCodecs());
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Is AMR narrow band audio codec enabled or disabled ? (Boolean)
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
        /// Is AMR wideband audio codec enabled or disabled ? (Boolean)
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
        /// Is speex 16000Hz audio codec enabled or disabled ? (Boolean)
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
        /// Is speex 8000Hz audio codec enabled or disabled ? (Boolean)
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
        /// Is PCMU (G.711 ulaw) audio codec enabled or disabled ? (Boolean)
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
        /// Is PCMA (G.711 alaw) audio codec enabled or disabled ? (Boolean)
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
        /// Is G.722 audio codec enabled or disabled ? (Boolean)
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
        /// Is G.729 audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool G729
        {
            get
            {
                return Convert.ToBoolean(Get(G729SettingKeyName));
            }
            set
            {
                Set(G729SettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is iLBC audio codec enabled or disabled ? (Boolean)
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
        /// Is Silk 16000Hz audio codec enabled or disabled ? (Boolean)
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
        /// Is GSM audio codec enabled or disabled ? (Boolean)
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

        /// <summary>
        /// Is OPUS audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool OPUS
        {
            get
            {
                return Convert.ToBoolean(Get(OpusSettingKeyName));
            }
            set
            {
                Set(OpusSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is ISAC audio codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool Isac
        {
            get
            {
                return Convert.ToBoolean(Get(IsacSettingKeyName));
            }
            set
            {
                Set(IsacSettingKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Is H.264 video codec enabled or disabled ? (Boolean)
        /// </summary>
        public bool H264
        {
            get
            {
                return Convert.ToBoolean(Get(H264SettingKeyName));
            }
            set
            {
                Set(H264SettingKeyName, value.ToString());
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle call settings.
    /// </summary>
    public class CallSettingsManager : SettingsManager, ISettingsManager
    {
        #region Constants settings names
        private const string SendDTMFsRFC2833KeyName = "SendDTMFsRFC2833";
        private const string SendDTMFsSIPInfoKeyName = "SendDTMFsSIPInfo";
        private const string VideoEnabledKeyName = "VideoEnabled";
        private const string AutomaticallyInitiateVideoKeyName = "AutomaticallyInitiateVideo";
        private const string AutomaticallyAcceptVideoKeyName = "AutomaticallyAcceptVideo";
        private const string SelfViewEnabledKeyName = "SelfViewEnabled";
        #endregion

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Loads the call settings.
        /// </summary>
        public void Load()
        {
            dict[SendDTMFsRFC2833KeyName] = LinphoneManager.Instance.LinphoneCore.GetUseRFC2833ForDTMFs().ToString();
            dict[SendDTMFsSIPInfoKeyName] = LinphoneManager.Instance.LinphoneCore.GetUseSipInfoForDTMFs().ToString();
            dict[VideoEnabledKeyName] = LinphoneManager.Instance.LinphoneCore.IsVideoEnabled().ToString();
            VideoPolicy policy = LinphoneManager.Instance.LinphoneCore.GetVideoPolicy();
            dict[AutomaticallyInitiateVideoKeyName] = policy.AutomaticallyInitiate.ToString();
            dict[AutomaticallyAcceptVideoKeyName] = policy.AutomaticallyAccept.ToString();
            dict[SelfViewEnabledKeyName] = LinphoneManager.Instance.LinphoneCore.IsSelfViewEnabled().ToString();
        }

        /// <summary>
        /// Saves the call settings.
        /// </summary>
        public void Save()
        {
            if (ValueChanged(SendDTMFsRFC2833KeyName))
            {
                LinphoneManager.Instance.LinphoneCore.SetUseRFC2833ForDTMFs(Convert.ToBoolean(GetNew(SendDTMFsRFC2833KeyName)));
            }
            if (ValueChanged(SendDTMFsSIPInfoKeyName))
            {
                LinphoneManager.Instance.LinphoneCore.SetUseSipInfoForDTMFs(Convert.ToBoolean(GetNew(SendDTMFsSIPInfoKeyName)));
            }
            if (ValueChanged(VideoEnabledKeyName))
            {
                bool isVideoEnabled = Convert.ToBoolean(GetNew(VideoEnabledKeyName));
                LinphoneManager.Instance.LinphoneCore.EnableVideo(isVideoEnabled, isVideoEnabled);
            }
            if (ValueChanged(AutomaticallyInitiateVideoKeyName) || ValueChanged(AutomaticallyAcceptVideoKeyName))
            {
                VideoPolicy policy = LinphoneManager.Instance.LinphoneCoreFactory.CreateVideoPolicy(
                    Convert.ToBoolean(GetNew(AutomaticallyInitiateVideoKeyName)),
                    Convert.ToBoolean(GetNew(AutomaticallyAcceptVideoKeyName)));
                LinphoneManager.Instance.LinphoneCore.SetVideoPolicy(policy);
            }
            if (ValueChanged(SelfViewEnabledKeyName))
            {
                LinphoneManager.Instance.LinphoneCore.EnableSelfView(Convert.ToBoolean(GetNew(SelfViewEnabledKeyName)));
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// DTMFs using RFC2833 setting (bool).
        /// </summary>
        public bool? SendDTFMsRFC2833
        {
            get
            {
                return Convert.ToBoolean(Get(SendDTMFsRFC2833KeyName));
            }
            set
            {
                Set(SendDTMFsRFC2833KeyName, value.ToString());
            }
        }

        /// <summary>
        /// DTMFs using SIP INFO setting (bool).
        /// </summary>
        public bool? SendDTFMsSIPInfo
        {
            get
            {
                return Convert.ToBoolean(Get(SendDTMFsSIPInfoKeyName));
            }
            set
            {
                Set(SendDTMFsSIPInfoKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Video enabled setting (bool).
        /// </summary>
        public bool? VideoEnabled
        {
            get
            {
                return Convert.ToBoolean(Get(VideoEnabledKeyName));
            }
            set
            {
                Set(VideoEnabledKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Automatically initiate video on outgoing call setting (bool).
        /// </summary>
        public bool? AutomaticallyInitiateVideo
        {
            get
            {
                return Convert.ToBoolean(Get(AutomaticallyInitiateVideoKeyName));
            }
            set
            {
                Set(AutomaticallyInitiateVideoKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Automatically accept video on incoming call setting (bool).
        /// </summary>
        public bool? AutomaticallyAcceptVideo
        {
            get
            {
                return Convert.ToBoolean(Get(AutomaticallyAcceptVideoKeyName));
            }
            set
            {
                Set(AutomaticallyAcceptVideoKeyName, value.ToString());
            }
        }

        /// <summary>
        /// Display self view during calls (bool).
        /// </summary>
        public bool? SelfViewEnabled
        {
            get
            {
                return Convert.ToBoolean(Get(SelfViewEnabledKeyName));
            }
            set
            {
                Set(SelfViewEnabledKeyName, value.ToString());
            }
        }
        #endregion
    }

    /// <summary>
    /// Utility class to handle network settings.
    /// </summary>
    public class NetworkSettingsManager : SettingsManager, ISettingsManager
    {
        private LpConfig Config;
        private Dictionary<string, string> TunnelModeToString;
        private Dictionary<string, FirewallPolicy> FirewallPolicyToEnum;
        private Dictionary<string, MediaEncryption> MediaEncryptionToEnum;
        private Dictionary<string, string> StringToTunnelMode;
        private Dictionary<FirewallPolicy, string> EnumToFirewallPolicy;
        private Dictionary<MediaEncryption, string> EnumToMediaEncryption;

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
        public NetworkSettingsManager()
        {
            if (LinphoneManager.Instance.LinphoneCore == null)
            {
                Config = LinphoneManager.Instance.LinphoneCoreFactory.CreateLpConfig(GetConfigPath(), GetFactoryConfigPath());
            }
            else
            {
                Config = LinphoneManager.Instance.LinphoneCore.GetConfig();
            }
            TunnelModeToString = new Dictionary<string, string>()
            {
                { AppResources.TunnelMode3GOnly, "3gonly" },
                { AppResources.TunnelModeAlways, "always" },
                { AppResources.TunnelModeAuto, "auto" },
                { AppResources.TunnelModeDisabled, "disabled" }
            };
            StringToTunnelMode = new Dictionary<string, string>()
            {
                { "3gonly", AppResources.TunnelMode3GOnly },
                { "always", AppResources.TunnelModeAlways },
                { "auto", AppResources.TunnelModeAuto },
                { "disabled", AppResources.TunnelModeDisabled }
            };
            FirewallPolicyToEnum = new Dictionary<string, FirewallPolicy>()
            {
                { AppResources.FirewallPolicyNone, FirewallPolicy.NoFirewall },
                { AppResources.FirewallPolicyNat, FirewallPolicy.UseNatAddress },
                { AppResources.FirewallPolicyStun, FirewallPolicy.UseStun },
                { AppResources.FirewallPolicyIce, FirewallPolicy.UseIce }
            };
            EnumToFirewallPolicy = new Dictionary<FirewallPolicy, string>()
            {
                { FirewallPolicy.NoFirewall, AppResources.FirewallPolicyNone },
                { FirewallPolicy.UseNatAddress, AppResources.FirewallPolicyNat },
                { FirewallPolicy.UseStun, AppResources.FirewallPolicyStun },
                { FirewallPolicy.UseIce, AppResources.FirewallPolicyIce }
            };
            MediaEncryptionToEnum = new Dictionary<string, MediaEncryption>()
            {
                { AppResources.MediaEncryptionNone, MediaEncryption.None },
                { AppResources.MediaEncryptionSRTP, MediaEncryption.SRTP }
            };
            EnumToMediaEncryption = new Dictionary<MediaEncryption, string>()
            {
                { MediaEncryption.None, AppResources.MediaEncryptionNone },
                { MediaEncryption.SRTP, AppResources.MediaEncryptionSRTP }
            };
        }

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Load the network settings.
        /// </summary>
        public void Load()
        {
#if DEBUG
            dict[StunServerKeyName] = "stun.linphone.org";
            dict[FirewallPolicyKeyName] = EnumToFirewallPolicy[FirewallPolicy.UseIce];
            dict[MediaEncryptionKeyName] = EnumToMediaEncryption[MediaEncryption.SRTP];
#else
            dict[StunServerKeyName] = LinphoneManager.Instance.LinphoneCore.GetStunServer();
            dict[FirewallPolicyKeyName] = EnumToFirewallPolicy[LinphoneManager.Instance.LinphoneCore.GetFirewallPolicy()];
            dict[MediaEncryptionKeyName] = EnumToMediaEncryption[LinphoneManager.Instance.LinphoneCore.GetMediaEncryption()];
#endif

            // Load tunnel configuration
            dict[TunnelModeKeyName] = AppResources.TunnelModeDisabled;
            dict[TunnelServerKeyName] = "";
            dict[TunnelPortKeyName] = "";
            if (LinphoneManager.Instance.LinphoneCore.IsTunnelAvailable())
            {
                String mode = Config.GetString(ApplicationSection, TunnelModeKeyName, TunnelModeToString[AppResources.TunnelModeDisabled]);
                dict[TunnelModeKeyName] = StringToTunnelMode[mode];
                Tunnel tunnel = LinphoneManager.Instance.LinphoneCore.GetTunnel();
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
            }
        }

        /// <summary>
        /// Save the network settings.
        /// </summary>
        public void Save()
        {
            if (ValueChanged(StunServerKeyName))
                LinphoneManager.Instance.LinphoneCore.SetStunServer(GetNew(StunServerKeyName));

            if (ValueChanged(FirewallPolicyKeyName))
            {
                string firewallPolicy = GetNew(FirewallPolicyKeyName);
                LinphoneManager.Instance.LinphoneCore.SetFirewallPolicy(FirewallPolicyToEnum[firewallPolicy]);
            }

            if (ValueChanged(MediaEncryptionKeyName))
            {
                string mediaEncryption = GetNew(MediaEncryptionKeyName);
                LinphoneManager.Instance.LinphoneCore.SetMediaEncryption(MediaEncryptionToEnum[mediaEncryption]);
            }

            // Save tunnel configuration
            if (LinphoneManager.Instance.LinphoneCore.IsTunnelAvailable() && Customs.IsTunnelEnabled)
            {
                Boolean needTunnelReconfigure = false;
                if (ValueChanged(TunnelServerKeyName) || ValueChanged(TunnelPortKeyName))
                {
                    Tunnel tunnel = LinphoneManager.Instance.LinphoneCore.GetTunnel();
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
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Media encryption setting (String).
        /// </summary>
        public string MEncryption
        {
            get
            {
                return Get(MediaEncryptionKeyName);
            }
            set
            {
                Set(MediaEncryptionKeyName, value);
            }
        }

        /// <summary>
        /// Firewall policy setting (String).
        /// </summary>
        public string FWPolicy
        {
            get
            {
                return Get(FirewallPolicyKeyName);
            }
            set
            {
                Set(FirewallPolicyKeyName, value);
            }
        }

        /// <summary>
        /// Stun server setting (String).
        /// </summary>
        public string StunServer
        {
            get
            {
                return Get(StunServerKeyName);
            }
            set
            {
                Set(StunServerKeyName, value);
            }
        }

        /// <summary>
        /// Tunnel server setting (String).
        /// </summary>
        public string TunnelServer
        {
            get
            {
                return Get(TunnelServerKeyName);
            }
            set
            {
                Set(TunnelServerKeyName, value);
            }
        }

        /// <summary>
        /// Tunnel port setting (Integer).
        /// </summary>
        public string TunnelPort
        {
            get
            {
                return Get(TunnelPortKeyName);
            }
            set
            {
                Set(TunnelPortKeyName, value);
            }
        }

        /// <summary>
        /// Tunnel mode setting (Auto, Disabled, 3G Only or Always).
        /// </summary>
        public string TunnelMode
        {
            get
            {
                return Get(TunnelModeKeyName);
            }
            set
            {
                Set(TunnelModeKeyName, value);
            }
        }
        #endregion
    }
    
    /// <summary>
    /// Utility class to handle chat settings.
    /// </summary>
    public class ChatSettingsManager : SettingsManager, ISettingsManager
    {
        IsolatedStorageSettings _settings;

        #region Constants settings names
        private const string VibrateOnIncomingMessageKeyName = "VibrateOnIncomingMessage";
        private const string ScaleDownSentPicturesKeyName = "ScaleDownSentPictures";
        #endregion

        #region Implementation of the ISettingsManager interface
        /// <summary>
        /// Loads the call settings.
        /// </summary>
        public void Load()
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;

            string value;
            if (_settings.Contains(VibrateOnIncomingMessageKeyName))
                value = ((bool)_settings[VibrateOnIncomingMessageKeyName]).ToString();
            else
                value = true.ToString();
            dict[VibrateOnIncomingMessageKeyName] = value;

            if (_settings.Contains(ScaleDownSentPicturesKeyName))
                value = ((bool)_settings[ScaleDownSentPicturesKeyName]).ToString();
            else
                value = true.ToString();
            dict[ScaleDownSentPicturesKeyName] = value;
        }

        /// <summary>
        /// Saves the call settings.
        /// </summary>
        public void Save()
        {
            if (ValueChanged(VibrateOnIncomingMessageKeyName))
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
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// DTMFs using RFC2833 setting (bool).
        /// </summary>
        public bool? VibrateOnIncomingMessage
        {
            get
            {
                return Convert.ToBoolean(Get(VibrateOnIncomingMessageKeyName));
            }
            set
            {
                Set(VibrateOnIncomingMessageKeyName, value.ToString());
            }
        }
        public bool? ScaleDownSentPictures
        {
            get
            {
                return Convert.ToBoolean(Get(ScaleDownSentPicturesKeyName));
            }
            set 
            {
                Set(ScaleDownSentPicturesKeyName, value.ToString());
            }
        }
        #endregion
    }
}
