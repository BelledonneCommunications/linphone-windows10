using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    /// <summary>
    /// Interface for a basic call (start, end) listener.
    /// </summary>
    public interface CallControllerListener
    {
        /// <summary>
        /// Called when a new call is started.
        /// </summary>
        void NewCallStarted(string callerNumber);

        /// <summary>
        /// Called when a call is ended.
        /// </summary>
        void CallEnded();
    }
}
