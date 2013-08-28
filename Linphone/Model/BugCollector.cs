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

        internal static void ReportExceptions()
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

                        EmailComposeTask email = new EmailComposeTask();
                        email.To = "linphone-wphone@belledonne-communications.com";
                        email.Subject = "Exception report";
                        email.Body = body;
                        email.Show();
                    }
                }
            }
            catch (Exception) { }
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
