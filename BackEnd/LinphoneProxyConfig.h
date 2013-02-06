#pragma once

namespace Linphone
{
	namespace BackEnd
	{
		ref class RegistrationState;

		public ref class LinphoneProxyConfig sealed
		{
		public:
			void Edit();
			void Done();
			void SetIdentity(Platform::String^ identity);
			Platform::String^ GetIdentity();
			void SetProxy(Platform::String^ proxyUri);
			void EnableRegister(Platform::Boolean enable);
			Platform::Boolean IsRegisterEnabled();
			Platform::String^ NormalizePhoneNumber(Platform::String^ phoneNumber);
			void SetDialPrefix(Platform::String^ prefix);
			void SetDialEscapePlus(Platform::Boolean value);
			Platform::String^ GetDomain();
			Platform::Boolean IsRegistered();
			void SetRoute(Platform::String^ routeUri);
			Platform::String^ GetRoute();
			void EnablePublish(Platform::Boolean enable);
			Platform::Boolean IsPublishEnabled();
			RegistrationState^ GetState();
			void SetExpires(int delay);
			void SetContactParameters(Platform::String^ params);
			int LookupCCCFromIso(Platform::String^ iso);
			int LookupCCCFromE164(Platform::String^ e164);
		};
	}
}