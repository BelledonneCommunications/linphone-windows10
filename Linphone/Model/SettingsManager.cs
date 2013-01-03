using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public class SettingsManager
    {
        private IsolatedStorageSettings settings;

        const string DebugSettingKeyName = "Debug";

        public SettingsManager()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public static bool isDebugEnabled
        {
            get
            {
                SettingsManager sm = new SettingsManager();
                return sm.DebugEnabled != null;
            }
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            if (settings.Contains(Key))
            {
                if (settings[Key] != value)
                {
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }

        /// <summary>
        /// Property to get and set the Debug mode
        /// </summary>
        public bool? DebugEnabled
        {
            get
            {
                return GetValueOrDefault<bool?>(DebugSettingKeyName, false);
            }
            set
            {
                if (AddOrUpdateValue(DebugSettingKeyName, value))
                {
                    Save();
                }
            }
        }
    }
}
