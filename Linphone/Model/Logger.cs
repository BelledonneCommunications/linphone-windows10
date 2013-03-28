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
        /// <summary>
        /// Type of outputs to write the logs to
        /// </summary>
        [Flags] public enum Output
        {
            /// <summary>Write to a file synchronously</summary>
            FILE_SYNCHRONOUS,
            /// <summary>Write to the standard debug output (Visual Studio output)</summary>
            DEBUG_WRITE
        };

        /// <summary>
        /// The outputs to which to write the logs
        /// </summary>
        public Output Outputs { get; set; }

        /// <summary>
        /// The name of the file to write the logs to if using the FILE_SYNCHRONOUS output
        /// </summary>
        public String Filename { get; set; }

        /// <summary>
        /// Whether the logger is enabled or not
        /// </summary>
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

        /// <summary>
        /// Write a message to the configured outputs
        /// </summary>
        /// <param name="msg">The message to be written</param>
        private void Write(OutputTraceLevel level, String msg)
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

        /// <summary>
        /// Write a debug message
        /// </summary>
        /// <param name="msg">The message to be written</param>
        public static void Dbg(String msg)
        {
            Logger.Instance.Write(OutputTraceLevel.Debug, msg);
        }

        /// <summary>
        /// Write a standard message
        /// </summary>
        /// <param name="msg">The message to be written</param>
        public static void Msg(String msg)
        {
            Logger.Instance.Write(OutputTraceLevel.Message, msg);
        }

        /// <summary>
        /// Write a warning message
        /// </summary>
        /// <param name="msg">The message to be written</param>
        public static void Warn(String msg)
        {
            Logger.Instance.Write(OutputTraceLevel.Warning, msg);
        }

        /// <summary>
        /// Write an error message
        /// </summary>
        /// <param name="msg">The message to be written</param>
        public static void Err(String msg)
        {
            Logger.Instance.Write(OutputTraceLevel.Error, msg);
        }

        /// <summary>
        /// Handler to get and output native traces
        /// </summary>
        public void OutputTrace(OutputTraceLevel level, String msg)
        {
            Write(level, msg);
        }
    }
}
