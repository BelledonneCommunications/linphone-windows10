#pragma once

#include "LinphoneCoreFactory.h"
#include <mutex>

// Do not treat doxygen documentation as XML
#pragma warning(push)
#pragma warning(disable : 4635)
#include "coreapi\linphonecore.h"
#pragma warning(pop)

namespace Linphone
{
	namespace Core
	{
		ref class Globals;

		/// <summary>
		/// Object to log the application traces while in background mode.
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

		private:
			friend ref class Linphone::Core::Globals;

			BackgroundModeLogger();
			~BackgroundModeLogger();
		};
	}
}