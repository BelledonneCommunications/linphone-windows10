#pragma once

//#include "LinphoneProxyConfig.h";
//#include "LinphoneAuthInfo.h";

namespace Linphone
{
	namespace BackEnd
	{
		public ref class LinphoneCore sealed
		{
		public:
			LinphoneCore();
			Platform::String^ ToString();

			/*void ClearProxyConfigs();
			void AddProxyConfig(LinphoneProxyConfig^ proxyCfg);
			void SetDefaultProxyConfig(LinphoneProxyConfig^ proxyCfg);
			LinphoneProxyConfig^ GetDefaultProxyConfig();

			void ClearAuthInfos();
			void AddAuthInfo(LinphoneAuthInfo^ info);*/

		private:
			~LinphoneCore();
		};
	}
}