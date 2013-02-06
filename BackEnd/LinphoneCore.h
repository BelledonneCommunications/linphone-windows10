#pragma once

//#include "LinphoneProxyConfig.h";
//#include "LinphoneAuthInfo.h";

namespace Linphone
{
	namespace BackEnd
	{
		ref class LinphoneCoreFactory;

		public ref class LinphoneCore sealed
		{
		public:
			Platform::String^ ToString();

			/*void ClearProxyConfigs();
			void AddProxyConfig(LinphoneProxyConfig^ proxyCfg);
			void SetDefaultProxyConfig(LinphoneProxyConfig^ proxyCfg);
			LinphoneProxyConfig^ GetDefaultProxyConfig();

			void ClearAuthInfos();
			void AddAuthInfo(LinphoneAuthInfo^ info);*/

		private:
			friend ref class Linphone::BackEnd::LinphoneCoreFactory;

			LinphoneCore();
			~LinphoneCore();
		};
	}
}