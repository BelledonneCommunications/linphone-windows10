#pragma once

#include "LinphoneCoreFactory.h"
#include <iostream>
#include <fstream>


namespace Linphone
{
	namespace Core
	{
		ref class Globals;

		public ref class BackgroundModeLogger sealed : OutputTraceListener
		{
		public:
			virtual void OutputTrace(OutputTraceLevel level, Platform::String^ msg);

		private:
			friend ref class Linphone::Core::Globals;

			BackgroundModeLogger();
			~BackgroundModeLogger();

			std::ofstream *stream;
			std::recursive_mutex lock;
		};
	}
}