#pragma once

#include "LinphoneCoreFactory.h"

using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

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

			StorageFile^ storageFile;
			DataWriter^ dataWriter;
		};
	}
}