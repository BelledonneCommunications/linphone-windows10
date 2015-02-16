/*
InitManager.cs
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
            server.LinphoneCore.RootCA = "Assets/rootca.pem";
            server.LinphoneCore.ChatDatabasePath = GetChatDatabasePath();
            server.LinphoneCore.SetUserAgent(Customs.UserAgent, Linphone.Version.Number);
        }
    }
}
