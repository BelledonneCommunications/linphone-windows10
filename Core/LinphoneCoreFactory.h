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
		ref class LpConfig;

		public interface class OutputTraceListener
		{
		public:
			void OutputTrace(OutputTraceLevel level, Platform::String^ msg);
		};

		public ref class LinphoneCoreFactory sealed
		{
		public:
			property LinphoneCore^ LinphoneCore
            {
                Linphone::Core::LinphoneCore^ get();
            }

			property OutputTraceListener^ OutputTraceListener
			{
				Linphone::Core::OutputTraceListener^ get();
				void set(Linphone::Core::OutputTraceListener^ listener);
			}

			void CreateLinphoneCore(LinphoneCoreListener^ listener);
			void CreateLinphoneCore(LinphoneCoreListener^ listener, LpConfig^ config);

			LpConfig^ CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath);

			LinphoneAuthInfo^ CreateAuthInfo(Platform::String^ username, Platform::String^ password, Platform::String^ realm);

			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName);

			/// <summary>
			/// Constructs a LinphoneAddress object by parsing the user supplied address, given as a string.
			/// </summary>
			/// <param name="uri">address, should be like "sip:joe@sip.linphone.org"</param>
			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ uri);

			void SetLogLevel(OutputTraceLevel logLevel);

		private:
			friend ref class Linphone::Core::Globals;

			Linphone::Core::LinphoneCore^ linphoneCore;
			Linphone::Core::OutputTraceListener^ outputTraceListener;

			LinphoneCoreFactory();
			~LinphoneCoreFactory();
		};
	}
}