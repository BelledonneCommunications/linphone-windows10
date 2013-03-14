#pragma once

#include "ApiLock.h"
#include "LinphoneCoreListener.h"

namespace Linphone
{
	namespace Core
	{
		ref class Globals;
		ref class LinphoneCore;
		ref class LinphoneAuthInfo;
		ref class LinphoneAddress;

		public interface class OutputTraceListener
		{
		public:
			void OutputTrace(int level, Platform::String^ msg);
		};

		public ref class LinphoneCoreFactory sealed
		{
		public:
			property LinphoneCore^ LinphoneCore
            {
                Linphone::Core::LinphoneCore^ get();
            }

			void SetDebugMode(Platform::Boolean enable, OutputTraceListener^ traceListener);
			
			void CreateLinphoneCore(LinphoneCoreListener^ listener, Platform::String^ userConfig, Platform::String^ factoryConfig, Platform::Object^ userData);
			void CreateLinphoneCore(LinphoneCoreListener^ listener);

			LinphoneAuthInfo^ CreateAuthInfo(Platform::String^ username, Platform::String^ password, Platform::String^ realm);

			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName);

			/// <summary>
			/// Constructs a LinphoneAddress object by parsing the user supplied address, given as a string.
			/// </summary>
			/// <param name="address">address, should be like "sip:joe@sip.linphone.org"</param>
			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ address);

		private:
			friend ref class Linphone::Core::Globals;

			Linphone::Core::LinphoneCore^ linphoneCore;

			LinphoneCoreFactory();
			~LinphoneCoreFactory();
		};
	}
}