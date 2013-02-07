#pragma once

#include "ApiLock.h"
#include "LinphoneCoreListener.h"

namespace Linphone
{
	namespace BackEnd
	{
		ref class Globals;
		ref class LinphoneCore;
		ref class LinphoneAuthInfo;
		ref class LinphoneAddress;

		public ref class LinphoneCoreFactory sealed
		{
		public:
			property Linphone::BackEnd::LinphoneCore^ LinphoneCore
            {
                Linphone::BackEnd::LinphoneCore^ get();
            }

			void SetDebugMode(Platform::Boolean enable, Platform::String^ tag);
			
			void CreateLinphoneCore(LinphoneCoreListener^ listener, Platform::String^ userConfig, Platform::String^ factoryConfig, Platform::Object^ userData);
			void CreateLinphoneCore(LinphoneCoreListener^ listener);

			LinphoneAuthInfo^ CreateAuthInfo(Platform::String^ username, Platform::String^ password, Platform::String^ realm);

			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName);
			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ address);

		private:
			friend ref class Linphone::BackEnd::Globals;

			Linphone::BackEnd::LinphoneCore^ linphoneCore;

			LinphoneCoreFactory();
			~LinphoneCoreFactory();
		};
	}
}