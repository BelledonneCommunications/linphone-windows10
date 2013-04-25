#pragma once

#include "LinphoneCoreFactory.h"
#include <mutex>

namespace Linphone
{
	namespace Core
	{
		ref class Globals;
		class BackgroundModeLoggerPrivate;

		/// <summary>
		/// Object to log the application traces while in background mode.
		/// It can log to several destinations:
		///  - the debugger output (needs to run the application from Visual Studio)
		///  - a file (stored in the Isolated Storage space of the application)
		///  - remote computer using a TCP connection (needs an application to get the logs on the remote computer)
		/// </summary>
		public ref class BackgroundModeLogger sealed : OutputTraceListener
		{
		public:
			/// <summary>
			/// Method called to output a trace to the logs.
			/// </summary>
			/// <param name="level">The level of the trace to output</param>
			/// <param name="msg">The message to ouput</param>
			virtual void OutputTrace(OutputTraceLevel level, Platform::String^ msg);

			/// <summary>
			/// Configures the logger to enabled/disable logging and define the destination of the logs.
			/// If dest is OutputTraceDest::Debugger, the option is useless and can be null.
			/// If dest is OutputTraceDest::File, the option is the path of the file to write to.
			/// If dest is OutputTraceDest::TCPRemote, the option is the host and port of the computer to send the logs to (e.g. "192.168.0.17:38954")
			/// </summary>
			/// <param name="enable">Tells whether to enable or disable logging</param>
			/// <param name="dest">Tells the destination of the logs</param>
			/// <param name="option">Aditional configuration parameter that depends on the configured destination</param>
			void Configure(bool enable, OutputTraceDest dest, Platform::String^ option);

		private:
			friend ref class Linphone::Core::Globals;

			BackgroundModeLogger();
			~BackgroundModeLogger();

			std::recursive_mutex lock;
			bool enabled;
			OutputTraceDest dest;
			Platform::String^ filename;
			BackgroundModeLoggerPrivate *d;
		};
	}
}