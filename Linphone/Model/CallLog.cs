using Linphone.Resources;
using System;
using System.Windows.Media.Imaging;

namespace Linphone.Model
{
    /// <summary>
    /// Object representing a call log.
    /// Keeps a reference to a C++/CX CallLog object and provides some useful methods.
    /// </summary>
    public class CallLog
    {
        private static BitmapImage _incomingIcon = new BitmapImage(new Uri("/Assets/call_status_incoming.png", UriKind.Relative));
        private static BitmapImage _outgoingIcon = new BitmapImage(new Uri("/Assets/call_status_outgoing.png", UriKind.Relative));
        private static BitmapImage _missedIcon = new BitmapImage(new Uri("/Assets/call_status_missed.png", UriKind.Relative));

        private Object _nativeLog;
        /// <summary>
        /// C++/CX call log object.
        /// </summary>
        public Object NativeLog
        {
            get
            {
                return _nativeLog;
            }
            set
            {
                _nativeLog = value;
            }
        }

        private String _from;
        /// <summary>
        /// Call initiator name or address.
        /// </summary>
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
        /// <summary>
        /// Call receiver name or address.
        /// </summary>
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
        /// <summary>
        /// For the user, has the call been outgoing or incoming.
        /// </summary>
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
        /// <summary>
        /// Indicated whether or not the call has been missed by the user.
        /// </summary>
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
        
        /// <summary>
        /// Returns a BitmapImage representing the status of the call (incoming, outgoing or missed).
        /// </summary>
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

        /// <summary>
        /// Returns a string representing the status of the call (incoming, outgoing or missed).
        /// </summary>
        public String StatusText
        {
            get
            {
                if (_isIncoming)
                {
                    if (_isMissed)
                        return AppResources.HistoryMissed;
                    else
                        return AppResources.HistoryIncoming;
                }
                else
                    return AppResources.HistoryOutgoing;
            }
        }

        /// <summary>
        /// Named displayed for the caller/callee.
        /// </summary>
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

        private DateTime _startDate;
        /// <summary>
        /// Details text about call
        /// </summary>
        public String DetailsText
        {
            get
            {
                return _startDate.ToShortDateString() + ", " + _startDate.ToShortTimeString();
            }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public CallLog(Object nativeLog, String from, String to, bool isIncoming, bool isMissed, long startDate)
        {
            _nativeLog = nativeLog;
            _from = from;
            _to = to;
            _isIncoming = isIncoming;
            _isMissed = isMissed;

            _startDate = new DateTime();
            _startDate = _startDate.AddYears(1969); //Timestamp is calculated from 01/01/1970, and DateTime is initialized to 01/01/0001
            _startDate = _startDate.AddSeconds(startDate);
            _startDate = _startDate.Add(TimeZoneInfo.Local.GetUtcOffset(_startDate));
        }
    }
}
