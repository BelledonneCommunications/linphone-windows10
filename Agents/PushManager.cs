using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Agents
{
    public class PushManager
    {
        public PushManager()
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }

        IsolatedStorageSettings _settings;

        #region Constants settings names
        private const string PushUriKeyName = "PushUri";
        #endregion

        #region methods
        private bool AddOrUpdate(string key, object value)
        {
            bool valueChanged = false;

            if (_settings.Contains(key))
            {
                if (!_settings[key].Equals(value))
                {
                    _settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                _settings.Add(key, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        private Uri GetValue(string key)
        {
            Uri value = null;

            if (_settings.Contains(key))
                value = (Uri)_settings[key];

            return value;
        }

        private void Save()
        {
            _settings.Save();
        }
        #endregion

        #region Accessors
        /// <summary>
        /// DTMFs using RFC2833 setting (bool).
        /// </summary>
        public Uri PushChannelUri
        {
            get
            {
                return GetValue(PushUriKeyName);
            }
            set
            {
                if (AddOrUpdate(PushUriKeyName, value))
                {
                    Save();
                }
            }
        }
        #endregion
    }
}
