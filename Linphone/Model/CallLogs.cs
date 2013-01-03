using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Linphone.Model
{
    public class CallLogs
    {
        private String _title;
        public String Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public ObservableCollection<CallLog> Calls { get; set; }

        public CallLogs(String title, ObservableCollection<CallLog> calls)
        {
            _title = title;
            Calls = calls;
        }
    }

    public class CallLog
    {
        private static BitmapImage _incomingIcon = new BitmapImage(new Uri("/Assets/call_status_incoming.png", UriKind.Relative));
        private static BitmapImage _outgoingIcon = new BitmapImage(new Uri("/Assets/call_status_outgoing.png", UriKind.Relative));
        private static BitmapImage _missedIcon = new BitmapImage(new Uri("/Assets/call_status_missed.png", UriKind.Relative));

        private String _from;
        public String From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        private String _to;
        public String To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }

        private bool _isIncoming;
        public bool IsIncoming
        {
            get
            {
                return _isIncoming;
            }
            set
            {
                _isIncoming = value;
            }
        }

        private bool _isMissed;
        public bool IsMissed
        {
            get
            {
                return _isMissed;
            }
            set
            {
                _isMissed = value;
            }
        }

        public BitmapImage StatusIcon
        {
            get
            {
                if (_isIncoming)
                {
                    if (_isMissed)
                        return _missedIcon;
                    else
                        return _incomingIcon;
                }
                else
                    return _outgoingIcon;
            }
        }

        public String DisplayedName
        {
            get
            {
                if (_isIncoming)
                    return _from;
                else
                    return _to;
            }
        }

        public CallLog(String from, String to, bool isIncoming, bool isMissed)
        {
            _from = from;
            _to = to;
            _isIncoming = isIncoming;
            _isMissed = isMissed;
        }
    }
}
