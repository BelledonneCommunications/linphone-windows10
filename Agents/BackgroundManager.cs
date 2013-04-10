using Microsoft.Phone.Networking.Voip;
using Linphone.Core;
using Linphone.Core.OutOfProcess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Phone.Networking.Voip;
using System.Windows.Media.Imaging;
using Windows.Phone.Media.Devices;
using System.Reflection;
using Microsoft.Phone.Net.NetworkInformation;
using Windows.Storage;
using Linphone.Agents;

namespace Linphone.Agents
{
    public class BackgroundManager
    {
        private static BackgroundManager singleton;
        /// <summary>
        /// Static instance of the class.
        /// </summary>
        public static BackgroundManager Instance
        {
            get
            {
                if (BackgroundManager.singleton == null)
                    BackgroundManager.singleton = new BackgroundManager();

                return BackgroundManager.singleton;
            }
        }

        // A proxy to the server object in the background agent host process 
        public Server OopServer;

        /// <summary>
        /// Creates a new LinphoneCore (if not created yet) using a LinphoneCoreFactory.
        /// </summary>
        public void InitLinphoneCore()
        {
            // Initiate incoming call processing by creating the Linphone Core

            if (OopServer == null)
                OopServer = (Server)WindowsRuntimeMarshal.GetActivationFactory(typeof(Server)).ActivateInstance();

            OopServer.LinphoneCoreFactory.CreateLinphoneCore(null, ApplicationData.Current.LocalFolder.Path + "\\linphonerc", "Assets/linphonerc-factory");

            //Globals.Instance.BackgroundModeLogger.Configure(true, OutputTraceDest.TCPRemote, "192.168.0.217:38954");
            //Globals.Instance.LinphoneCoreFactory.OutputTraceListener = Globals.Instance.BackgroundModeLogger;

            //if (Globals.Instance.LinphoneCore.GetDefaultProxyConfig() != null)
            //{
            //    string host, token;
            //    host = ((App)App.Current).PushChannelUri.Host;
            //    token = ((App)App.Current).PushChannelUri.AbsolutePath;
            //    Globals.Instance.LinphoneCore.GetDefaultProxyConfig().SetContactParameters("app-id=" + host + ";pn-type=wp;pn-tok=" + token + ";pn-msg-str=IM_MSG;pn-call-str=IC_MSG;pn-call-snd=ring.caf;pn-msg-snd=msg.caf");
            //}
            OopServer.LinphoneCore.SetNetworkReachable(true);
        }
    }
}
