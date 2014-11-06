using Linphone.Core;
using System;
using System.IO;
using Windows.Storage;


namespace Linphone.Agents
{
    /// <summary>
    /// Class to handle the initialization process of the application
    /// </summary>
    public static class InitManager
    {
        /// <summary>
        /// Get the path of the default config file stored in the package
        /// </summary>
        /// <returns>The path of the default config file</returns>
        public static String GetDefaultConfigPath()
        {
            return Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "linphonerc");
        }

        /// <summary>
        /// Get the path of the config file stored in the Isolated Storage
        /// </summary>
        /// <returns>The path of the config file</returns>
        public static String GetConfigPath()
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, "linphonerc");
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
        /// Get the path of the database file used to store chat messages stored in the Isolated Storage
        /// </summary>
        /// <returns>The path of the config file</returns>
        public static String GetChatDatabasePath()
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, "chat.db");
        }

        /// <summary>
        /// Configure the Logger
        /// </summary>
        /// <param name="server">The out-of-process server</param>
        /// <param name="level">The log level to use for the Logger</param>
        public static void ConfigureLogger(Linphone.Core.OutOfProcess.Server server, OutputTraceLevel level)
        {
            server.LinphoneCoreFactory.SetLogLevel(level);
            Logger.Instance.TraceListener = server.BackgroundModeLogger;
        }

        /// <summary>
        /// Create the linphone core and set some basic configuration values
        /// </summary>
        /// <param name="server">The out-of-process server</param>
        /// <param name="listener">The LinphoneCoreListener</param>
        /// <param name="level">The log level to use for the Logger</param>
        public static void CreateLinphoneCore(Linphone.Core.OutOfProcess.Server server, LinphoneCoreListener listener, OutputTraceLevel level)
        {
            LpConfig config = server.LinphoneCoreFactory.CreateLpConfig(GetConfigPath(), GetFactoryConfigPath());
            ConfigureLogger(server, level);
            server.LinphoneCoreFactory.CreateLinphoneCore(listener, config);
            server.LinphoneCore.SetRootCA("Assets/rootca.pem");
            server.LinphoneCore.SetChatDatabasePath(GetChatDatabasePath());
            server.LinphoneCore.SetUserAgent(Customs.UserAgent, Linphone.Version.Number);
        }
    }
}
