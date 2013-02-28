#include "LinphoneProxyConfig.h"
#include "LinphoneCore.h"
#include "Server.h"

void Linphone::Core::LinphoneProxyConfig::Edit()
{

}

void Linphone::Core::LinphoneProxyConfig::Done()
{

}

void Linphone::Core::LinphoneProxyConfig::SetIdentity(Platform::String^ identity)
{

}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetIdentity()
{
	return nullptr;
}

void Linphone::Core::LinphoneProxyConfig::SetProxy(Platform::String^ proxyUri)
{

}

void Linphone::Core::LinphoneProxyConfig::EnableRegister(Platform::Boolean enable)
{

}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegisterEnabled()
{
	return false;
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::NormalizePhoneNumber(Platform::String^ phoneNumber)
{
	return nullptr;
}

void Linphone::Core::LinphoneProxyConfig::SetDialPrefix(Platform::String^ prefix)
{

}

void Linphone::Core::LinphoneProxyConfig::SetDialEscapePlus(Platform::Boolean value)
{

}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetDomain()
{
	return nullptr;
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegistered()
{
	return false;
}

void Linphone::Core::LinphoneProxyConfig::SetRoute(Platform::String^ routeUri)
{

}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetRoute()
{
	return nullptr;
}

void Linphone::Core::LinphoneProxyConfig::EnablePublish(Platform::Boolean enable)
{

}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsPublishEnabled()
{
	return false;
}

Linphone::Core::RegistrationState Linphone::Core::LinphoneProxyConfig::GetState()
{
	return Linphone::Core::RegistrationState::RegistrationNone;
}

void Linphone::Core::LinphoneProxyConfig::SetExpires(int delay)
{

}

void Linphone::Core::LinphoneProxyConfig::SetContactParameters(Platform::String^ params)
{

}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromIso(Platform::String^ iso)
{
	return -1;
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromE164(Platform::String^ e164)
{
	return -1;
}
