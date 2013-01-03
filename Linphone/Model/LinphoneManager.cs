using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public class LinphoneManager
    {
        private static LinphoneManager _instance = new LinphoneManager();
        public static LinphoneManager Instance
        {
            get { return _instance; }
        }

        private List<CallLogs> _history;

        /// <summary>
        /// Set the debug value for liblinphone
        /// </summary>
        /// <param name="enable">true to enable debug traces, false to disable them</param>
        /// <returns></returns>
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

        }
    }
}
