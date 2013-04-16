using Linphone.Core;
using System;


namespace Linphone.Model
{
    /// <summary>
    /// Class to handle the logging of the application
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// The trace listener to which to send the messages to be logged
        /// </summary>
        public OutputTraceListener TraceListener { get;  set; }

        private Logger()
        {
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

        /// <summary>
        /// Write a message to the logs
        /// </summary>
        /// <param name="level">The trace level of the message to be written</param>
        /// <param name="msg">The message to be written</param>
        private void Write(OutputTraceLevel level, String msg)
        {
            if (TraceListener != null)
            {
                TraceListener.OutputTrace(level, msg);
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
        /// Write a debug message with a format and some arguments
        /// </summary>
        /// <param name="fmt">The format of the message to be written</param>
        /// <param name="args">The arguments to fill the format with</param>
        public static void Dbg(String fmt, params object[] args)
        {
            Logger.Instance.Write(OutputTraceLevel.Debug, String.Format(fmt, args));
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
        /// Write a standard message with a format and some arguments
        /// </summary>
        /// <param name="fmt">The format of the message to be written</param>
        /// <param name="args">The arguments to fill the format with</param>
        public static void Msg(String fmt, params object[] args)
        {
            Logger.Instance.Write(OutputTraceLevel.Message, String.Format(fmt, args));
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
        /// Write a warning message with a format and some arguments
        /// </summary>
        /// <param name="fmt">The format of the message to be written</param>
        /// <param name="args">The arguments to fill the format with</param>
        public static void Warn(String fmt, params object[] args)
        {
            Logger.Instance.Write(OutputTraceLevel.Warning, String.Format(fmt, args));
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
        /// Write an error message with a format and some arguments
        /// </summary>
        /// <param name="fmt">The format of the message to be written</param>
        /// <param name="args">The arguments to fill the format with</param>
        public static void Err(String fmt, params object[] args)
        {
            Logger.Instance.Write(OutputTraceLevel.Error, String.Format(fmt, args));
        }
    }
}
