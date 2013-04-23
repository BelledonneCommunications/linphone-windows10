#pragma once

#include "LinphoneCoreFactory.h"
#include <mutex>

namespace Linphone
{
	namespace Core
	{
		ref class Globals;
		class BackgroundModeLoggerPrivate;

		public ref class BackgroundModeLogger sealed : OutputTraceListener
		{
		public:
			virtual void OutputTrace(OutputTraceLevel level, Platform::String^ msg);
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