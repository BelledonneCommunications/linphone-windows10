using Microsoft.Phone.Networking.Voip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public class LinphoneManager
    {
        private static LinphoneManager singleton;
        public static LinphoneManager Instance
        {
            get
            {
                if (LinphoneManager.singleton == null)
                    LinphoneManager.singleton = new LinphoneManager();

                return LinphoneManager.singleton;
            }
        }

        private List<CallLogs> _history;
        private bool BackgroundProcessConnected;

        public void ConnectBackgroundProcessToInterface()
        {
            if (BackgroundProcessConnected)
            {
                Debug.WriteLine("[LinphoneManager] Background process already connected to interface");
                return;
            }

            int backgroundProcessID;
            try
            {
                //VoipBackgroundProcess.Launch(out backgroundProcessID);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[LinphoneManager] Error launching VoIP background process. Exception: " + e.Message);
                throw;
            }

            BackgroundProcessConnected = true;
            Debug.WriteLine("[LinphoneManager] Background process connected to interface");
        }

        public void DisconnectBackgroundProcessFromInterface()
        {
            if (!BackgroundProcessConnected)
            {
                Debug.WriteLine("[LinphoneManager] Background process not connected to interface yet");
                return;
            }

            BackgroundProcessConnected = false;
            Debug.WriteLine("[LinphoneManager] Background process disconnected from interface");
        }

        /// <summary>
        /// Set the debug value for liblinphone
        /// </summary>
        /// <param name="enable">true to enable debug traces, false to disable them</param>
        public void EnableDebug(bool enable)
        {

        }

        /// <summary>
        /// Get the calls' history
        /// </summary>
        /// <returns>A list of CallLogs, each one representing a type of calls (All, Missed, ...)</returns>
        public List<CallLogs> GetCallsHistory()
        {
            _history = new List<CallLogs>();

            ObservableCollection<CallLog> calls = new ObservableCollection<CallLog>();
            ObservableCollection<CallLog> missedCalls = new ObservableCollection<CallLog>();

            CallLog logA = new CallLog("sip:cotcot@sip.linphone.org", "sip:miaou@sip.linphone.org", true, false);
            CallLog logB = new CallLog("sip:cotcot@sip.linphone.org", "sip:miaou@sip.linphone.org", true, true);
            CallLog logC = new CallLog("sip:cotcot@sip.linphone.org", "sip:miaou@sip.linphone.org", false, false);
            CallLog logD = new CallLog("sip:cotcot@sip.linphone.org", "sip:miaou@sip.linphone.org", true, true);
            CallLog logE = new CallLog("sip:cotcot@sip.linphone.org", "sip:miaou@sip.linphone.org", false, false);
            CallLog logF = new CallLog("sip:cotcot@sip.linphone.org", "sip:miaou@sip.linphone.org", false, false);
            CallLog logG = new CallLog("sip:cotcot@sip.linphone.org", "sip:miaou@sip.linphone.org", true, true);
            CallLog logH = new CallLog("sip:cotcot@sip.linphone.org", "sip:miaou@sip.linphone.org", false, false);

            calls.Add(logA);
            calls.Add(logB);
            calls.Add(logC);
            calls.Add(logD);
            calls.Add(logE);
            calls.Add(logF);
            calls.Add(logG);
            calls.Add(logH);
            missedCalls.Add(logB);
            missedCalls.Add(logD);
            missedCalls.Add(logG);

            CallLogs all = new CallLogs("All", calls);
            _history.Add(all);

            CallLogs missed = new CallLogs("Missed", missedCalls);
            _history.Add(missed);

            return _history;
        }

        /// <summary>
        /// Remove one or many entries from the calls' history
        /// </summary>
        /// <param name="logsToRemove">A list of CallLog to remove from history</param>
        /// <returns>A list of CallLogs, without the removed entries</returns>
        public List<CallLogs> RemoveCallLogs(IEnumerable<CallLog> logsToRemove)
        {
            // When removing log from history, it will be removed from logsToRemove list too. 
            // Using foreach causing the app to crash on a InvalidOperationException, so we are using while
            while (logsToRemove.Count() > 0)
            {
                CallLog logToRemove = logsToRemove.First();
                foreach (CallLogs logs in _history)
                {
                    logs.Calls.Remove(logToRemove);
                }
            }

            return _history;
        }

        /// <summary>
        /// Remove all calls' history from LinphoneCore
        /// </summary>
        public void ClearCallLogs()
        {

        }

        /// <summary>
        /// Start a new call to a sip address
        /// </summary>
        /// <param name="sipAddress">SIP address to call</param>
        public void NewOutgoingCall(String sipAddress)
        {
            //TileManager tileManager = TileManager.Instance;
            //tileManager.UpdateTileWithMissedCalls(new Random().Next(10));
        }

        /// <summary>
        /// Stops the current call if any
        /// </summary>
        public void EndCurrentCall()
        {

        }


        /// <summary>
        /// Pauses the current call if any and if it's running
        /// </summary>
        public void PauseCurrentCall()
        {

        }

        /// <summary>
        /// Resume the current call if any and if it's paused
        /// </summary>
        public void ResumeCurrentCall()
        {

        }

        /// <summary>
        /// Hang up the current call if any
        /// </summary>
        public void HangUp()
        {

        }
    }
}
