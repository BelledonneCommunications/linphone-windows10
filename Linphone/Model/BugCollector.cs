using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Linphone.Model
{
    /// <summary>
    /// Collects and reports uncatched exceptions
    /// </summary>
    public class BugCollector
    {
        const string exceptionsFileName = "exceptions.log";
        const string logFileName = "Linphone.log";

        internal static void LogException(Exception e, string extra)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (TextWriter output = new StreamWriter(store.OpenFile(exceptionsFileName, FileMode.Append)))
                    {
                        output.WriteLine("-------------------------");
                        output.WriteLine("Type: {0}", extra);
                        output.WriteLine("Message: {0}", e.Message);
                        foreach (KeyValuePair<string, string> kvp in e.Data)
                        {
                            output.WriteLine("Data: Key= {0}, Value= {1}", kvp.Key, kvp.Value);
                        }
                        output.WriteLine("Stacktrace: {0}", e.StackTrace);
                        output.Flush();
                        output.Close();
                    }
                }
            }
            catch (Exception) { }
        }

        internal static bool HasExceptionToReport()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    return store.FileExists(exceptionsFileName);
                }
            }
            catch (Exception) { }

            return false;
        }

        internal static async void ReportExceptions()
        {
            try
            {
                string body = "";
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(exceptionsFileName))
                    {
                        using (TextReader input = new StreamReader(store.OpenFile(exceptionsFileName, FileMode.Open)))
                        {
                            body += input.ReadToEnd();
                            input.Close();
                        }
                    }
                    if (store.FileExists(logFileName))
                    {
                        body += "\r\n"; 
                        // Limit the amount of linphone logs to the last 50ko
                        string logs = await ReadLogs();
                        if (logs.Length > 50000)
                        {
                            logs = logs.Substring(logs.Length - 50000);
                        }
                        body += logs;
                    }
                }
                EmailComposeTask email = new EmailComposeTask();
                email.To = "linphone-wphone@belledonne-communications.com";
                email.Subject = "Exception report";
                email.Body = body;
                email.Show();
            }
            catch (Exception) { }
        }

        internal static async Task<string> ReadLogs()
        {
            ApplicationSettingsManager appSettings = new ApplicationSettingsManager();
            appSettings.Load();

            byte[] data;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(appSettings.LogOption);

            using (Stream s = await file.OpenStreamForReadAsync())
            {
                data = new byte[s.Length];
                await s.ReadAsync(data, 0, (int)s.Length);
            }

            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        internal static void DeleteFile()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    store.DeleteFile(exceptionsFileName);
                }
            }
            catch (Exception) { }
        }
    }
}
