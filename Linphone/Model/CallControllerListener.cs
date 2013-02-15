using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public interface CallControllerListener
    {
        void NewCallStarted(string callerNumber);

        void CallEnded();
    }
}
