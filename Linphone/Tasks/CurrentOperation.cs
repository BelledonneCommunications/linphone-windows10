using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace LinphoneTasks {
    class CurrentOperation {
        public static BackgroundTaskDeferral PhoneCallTaskDeferral {
            set {
                lock (_lock) {
                    _phoneCallTaskDeferral = value;
                }
            }
            get {
                lock (_lock) {
                    return _phoneCallTaskDeferral;
                }
            }
        }

        private static Object _lock = new Object();
        private static BackgroundTaskDeferral _phoneCallTaskDeferral = null;
    }
}
