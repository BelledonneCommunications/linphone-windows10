#pragma once

namespace Linphone
{
	namespace Core
	{
		public ref class LinphoneAddress sealed
		{
		public:
			Platform::String^ GetDisplayName();
			Platform::String^ GetUserName();
			Platform::String^ GetDomain();
			int GetPort();

			void SetDisplayName(Platform::String^ name);
			void SetUserName(Platform::String^ username);
			void SetDomain(Platform::String^ domain);
			void SetPort(int port);

			void Clean();
			Platform::String^ AsString();
			Platform::String^ AsStringUriOnly();
			Platform::String^ ToString();
		};
	}
}