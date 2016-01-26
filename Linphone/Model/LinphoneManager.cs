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
using System.Text;
using System.Threading.Tasks;
using BelledonneCommunications.Linphone.Native;
using System.ComponentModel;
using Windows.UI.Core;

namespace Linphone.Model
{
    class LinphoneManager : CoreListener, INotifyPropertyChanged
    {
        private static LinphoneManager _instance = new LinphoneManager();
        public static LinphoneManager Instance { get { return _instance; } }

        private Core _core;
        private string _callStateText = "No call";
        private string _registrationStateText = "Not registered";

        public event PropertyChangedEventHandler PropertyChanged;

        public Core Core {
            get
            {
                if (_core == null)
                {
                    _core = new Core(this);
                }
                return _core;
            }
        }

        public CoreDispatcher Dispatcher { get; set; }

        public LinphoneManager()
        {
        }

        public string CallStateText
        {
            get
            {
                return _callStateText;
            }
            set
            {
                _callStateText = value;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CallStateText"));
                });
            }
        }

        public string RegistrationStateText {
            get
            {
                return _registrationStateText;
            }
            set
            {
                _registrationStateText = value;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                {
                     PropertyChanged(this, new PropertyChangedEventArgs("RegistrationStateText"));
                 });
            }
        }

        void CoreListener.AuthInfoRequested(string realm, string username, string domain)
        {
            throw new NotImplementedException();
        }

        void CoreListener.CallEncryptionChanged(Call call, bool encrypted, string authenticationToken)
        {
            throw new NotImplementedException();
        }

        void CoreListener.CallStateChanged(Call call, CallState state, string message)
        {
            CallStateText = message;
        }

        void CoreListener.CallStatsUpdated(Call call, CallStats stats)
        {
            System.Diagnostics.Debug.WriteLine("CallStatsUpdated");
        }

        void CoreListener.DtmfReceived(Call call, char dtmf)
        {
            throw new NotImplementedException();
        }

        void CoreListener.GlobalStateChanged(GlobalState state, string message)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("GlobalStateChanged: {0} [{1}]", state, message));
        }

        void CoreListener.IsComposingReceived(ChatRoom room)
        {
            throw new NotImplementedException();
        }

        void CoreListener.LogCollectionUploadProgressIndication(int offset, int total)
        {
            throw new NotImplementedException();
        }

        void CoreListener.LogCollectionUploadStateChanged(LogCollectionUploadState state, string info)
        {
            throw new NotImplementedException();
        }

        void CoreListener.MessageReceived(ChatRoom room, ChatMessage message)
        {
            throw new NotImplementedException();
        }

        void CoreListener.RegistrationStateChanged(ProxyConfig config, RegistrationState state, string message)
        {
            RegistrationStateText = message;
        }
    }
}
