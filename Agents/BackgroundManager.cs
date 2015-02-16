/*
BackgroundManager.cs
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
            OopServer.LinphoneCore.NetworkReachable = true;
            OopServer.LinphoneCore.IterateEnabled = true;
        }
    }
}
