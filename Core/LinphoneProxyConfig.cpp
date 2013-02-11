#include "LinphoneProxyConfig.h"
#include "LinphoneCore.h"
#include "Server.h"

using namespace Linphone::Core;

void LinphoneProxyConfig::Edit()
{

}

void LinphoneProxyConfig::Done()
{

}

void LinphoneProxyConfig::SetIdentity(Platform::String^ identity)
{

}

Platform::String^ LinphoneProxyConfig::GetIdentity()
{
	return nullptr;
}

void LinphoneProxyConfig::SetProxy(Platform::String^ proxyUri)
{

}

void LinphoneProxyConfig::EnableRegister(Platform::Boolean enable)
{

}

Platform::Boolean LinphoneProxyConfig::IsRegisterEnabled()
{
	return false;
}

Platform::String^ LinphoneProxyConfig::NormalizePhoneNumber(Platform::String^ phoneNumber)
{
	return nullptr;
}

void LinphoneProxyConfig::SetDialPrefix(Platform::String^ prefix)
{

}

void LinphoneProxyConfig::SetDialEscapePlus(Platform::Boolean value)
{

}

Platform::String^ LinphoneProxyConfig::GetDomain()
{
	return nullptr;
}

Platform::Boolean LinphoneProxyConfig::IsRegistered()
{
	return false;
}

void LinphoneProxyConfig::SetRoute(Platform::String^ routeUri)
{

}

Platform::String^ LinphoneProxyConfig::GetRoute()
{
	return nullptr;
}

void LinphoneProxyConfig::EnablePublish(Platform::Boolean enable)
{

}

Platform::Boolean LinphoneProxyConfig::IsPublishEnabled()
{
	return false;
}

RegistrationState LinphoneProxyConfig::GetState()
{
	return RegistrationState::RegistrationNone;
}

void LinphoneProxyConfig::SetExpires(int delay)
{

}

void LinphoneProxyConfig::SetContactParameters(Platform::String^ params)
{

}

int LinphoneProxyConfig::LookupCCCFromIso(Platform::String^ iso)
{
	return -1;
}

int LinphoneProxyConfig::LookupCCCFromE164(Platform::String^ e164)
{
	return -1;
}
