using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public class DatabaseManager : DataContext
    {
        // Specify the connection string as a static.
        public static string DBConnectionString = "Data Source=isostore:/ToDo.sdf";

        private static DatabaseManager singleton;
        /// <summary>
        /// Static instance of the class.
        /// </summary>
        public static DatabaseManager Instance
        {
            get
            {
                if (DatabaseManager.singleton == null)
                    DatabaseManager.singleton = new DatabaseManager(DBConnectionString);

                return DatabaseManager.singleton;
            }
        }

        DatabaseManager(string connectionString)
            : base(connectionString)
        { }

        /// <summary>
        /// Database table which contains the received and sent messages
        /// </summary>
        public Table<ChatMessage> Messages;
    }

    /// <summary>
    /// Object used to store/retrieve chat messages in/from the database
    /// </summary>
    [Table]
    public class ChatMessage : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private int messageID;

        /// <summary>
        /// Identifies each message with a unique value
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int MessageID
        {
            get
            {
                return messageID;
            }
            set
            {
                if (messageID != value)
                {
                    NotifyPropertyChanging("MessageID");
                    messageID = value;
                    NotifyPropertyChanged("MessageID");
                }
            }
        }

        private string _localContact;

        /// <summary>
        /// SIP address of the sender if the chat message is incoming, else empty 
        /// </summary>
        [Column(CanBeNull=false)]
        public string LocalContact
        {
            get
            {
                return _localContact;
            }
            set
            {
                if (_localContact != value)
                {
                    NotifyPropertyChanging("LocalContact");
                    _localContact = value;
                    NotifyPropertyChanged("LocalContact");
                }
            }
        }

        private string _remoteContact;

        /// <summary>
        /// SIP address at wich the message was sent, else empty (if incoming)
        /// </summary>
        [Column(CanBeNull = false)]
        public string RemoteContact
        {
            get
            {
                return _remoteContact;
            }
            set
            {
                if (_remoteContact != value)
                {
                    NotifyPropertyChanging("RemoteContact");
                    _remoteContact = value;
                    NotifyPropertyChanged("RemoteContact");
                }
            }
        }

        private bool _isIncoming;

        /// <summary>
        /// True if the message has been received, false if it has been sent 
        /// </summary>
        [Column]
        public bool IsIncoming
        {
            get
            {
                return _isIncoming;
            }
            set
            {
                if (_isIncoming != value)
                {
                    NotifyPropertyChanging("IsIncoming");
                    _isIncoming = value;
                    NotifyPropertyChanged("IsIncoming");
                }
            }
        }

        private string _message;

        /// <summary>
        /// The text contained in the message, can be empty
        /// </summary>
        [Column]
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    NotifyPropertyChanging("Message");
                    _message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }

        private string _imageURL;

        /// <summary>
        /// URL of the image to download (incoming only) or filename of the image in the isolated storage
        /// </summary>
        [Column]
        public string ImageURL
        {
            get
            {
                return _imageURL;
            }
            set
            {
                if (_imageURL != value)
                {
                    NotifyPropertyChanging("ImageURL");
                    _imageURL = value;
                    NotifyPropertyChanged("ImageURL");
                }
            }
        }

        private long _timestamp;

        /// <summary>
        /// Timestamp at which the message has been received/sent, in seconds since January 1st, 1970
        /// </summary>
        [Column]
        public long Timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                if (_timestamp != value)
                {
                    NotifyPropertyChanging("Timestamp");
                    _timestamp = value;
                    NotifyPropertyChanged("Timestamp");
                }
            }
        }

        private bool _markedAsRead;

        /// <summary>
        /// True is the message has been read, otherwise false
        /// </summary>
        [Column]
        public bool MarkedAsRead
        {
            get
            {
                return _markedAsRead;
            }
            set
            {
                if (_markedAsRead != value)
                {
                    NotifyPropertyChanging("MarkedAsRead");
                    _markedAsRead = value;
                    NotifyPropertyChanged("MarkedAsRead");
                }
            }
        }

        private int _status;

        /// <summary>
        /// Stores the LinphoneChatMessageState (Idle, InProgress, Delivered, NotDelivered)
        /// </summary>
        [Column]
        public int Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    NotifyPropertyChanging("Status");
                    _status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// Version column aids update performance.
        /// </summary>
        [Column(IsVersion = true)]
        private Binary _version;

        /// <summary>
        /// Returns the local contact if the message is incoming, else returns the remote contact
        /// </summary>
        public string Contact
        {
            get
            {
                if (IsIncoming)
                    return LocalContact;
                return RemoteContact;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
