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

        private Server oopServer;
        /// <summary>
        /// A proxy to the server object in the background agent host process 
        /// </summary>
        public Server OopServer
        {
            get {
               if (oopServer == null)
                    oopServer = (Server)WindowsRuntimeMarshal.GetActivationFactory(typeof(Server)).ActivateInstance();
               return oopServer;
            }

            set {
                oopServer = value;
            }
        }

        /// <summary>
        /// Creates a new LinphoneCore (if not created yet) using a LinphoneCoreFactory.
        /// </summary>
        public void InitLinphoneCore()
        {
            // Initiate incoming call processing by creating the Linphone Core
            InitManager.CreateLinphoneCore(OopServer, null, OutputTraceLevel.Message);
            OopServer.LinphoneCore.SetNetworkReachable(true);
            OopServer.LinphoneCore.IterateEnabled = true;
        }
    }
}
