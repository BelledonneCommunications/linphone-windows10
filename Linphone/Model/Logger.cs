using Linphone.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Linphone.Model
{
    /// <summary>
    /// Class to handle the logging of the application
    /// </summary>
    public sealed class Logger : OutputTraceListener
    {
        [Flags] public enum Output {
            FILE_SYNCHRONOUS,
            DEBUG_WRITE
        };

        public Output Outputs { get; set; }
        public String Filename { get; set; }
        public bool Enable { get; set; }

        private Logger()
        {
            Outputs = Output.FILE_SYNCHRONOUS;
            Filename = "Linphone.log";
        }

        private static Logger singleton;

        /// <summary>
        /// Static instance of the class.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (Logger.singleton == null)
                    Logger.singleton = new Logger();

                return Logger.singleton;
            }
        }

        private StorageFile storageFile;
        private StreamWriter streamWriter;

        private async Task<bool> CreateFileIfNeeded()
        {
            if (Filename == null) return false;
            if (streamWriter == null)
            {
                StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                storageFile = await localFolder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);
                streamWriter = new StreamWriter(storageFile.Path);
            }
            return true;
        }

        private async void WriteToFile(String msg)
        {
            bool fileExists = await CreateFileIfNeeded();
            if (fileExists)
            {
                streamWriter.WriteLine(msg);
                streamWriter.Flush();
            }
        }

        public void Write(String msg)
        {
            if (Outputs.HasFlag(Output.FILE_SYNCHRONOUS))
            {
                WriteToFile(msg);
            }
            if (Outputs.HasFlag(Output.DEBUG_WRITE))
            {
                Debug.WriteLine(msg);
            }
        }

        public static void WriteLine(string msg)
        {
            Logger.Instance.Write(msg);
        }

        /// <summary>
        /// Handler to get and output native traces
        /// </summary>
        public void OutputTrace(int level, String msg)
        {
            Write(msg);
        }
    }
}
