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

        public Table<ChatMessage> Messages;
    }

    [Table]
    public class ChatMessage : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private int messageID;
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

        private long _timestamp;
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

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

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
