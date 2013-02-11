#pragma once

namespace Linphone
{
	namespace Core
	{
		public ref class LinphoneAuthInfo sealed
		{
		public:
			Platform::String^ GetUsername();
			void SetUsername(Platform::String^ username);
			Platform::String^ GetPassword();
			void SetPassword(Platform::String^ password);
			Platform::String^ GetRealm();
			void SetRealm(Platform::String^ realm);
		};
	}
}