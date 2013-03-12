#include "LinphoneProxyConfig.h"
#include "LinphoneCore.h"
#include "Server.h"
#include "Utils.h"

void Linphone::Core::LinphoneProxyConfig::Edit()
{
	linphone_proxy_config_edit(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::Done()
{
	linphone_proxy_config_done(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::SetIdentity(Platform::String^ identity)
{
	const char* cc = Utils::pstoccs(identity);
	linphone_proxy_config_set_identity(this->proxy_config, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetIdentity()
{
	return Utils::cctops(linphone_proxy_config_get_identity(this->proxy_config));
}

void Linphone::Core::LinphoneProxyConfig::SetProxy(Platform::String^ proxyUri)
{
	const char* cc = Utils::pstoccs(proxyUri);
	linphone_proxy_config_set_server_addr(this->proxy_config, cc);
	delete(cc);
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
	return Utils::cctops(linphone_proxy_config_get_domain(this->proxy_config));
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegistered()
{
	return false;
}

void Linphone::Core::LinphoneProxyConfig::SetRoute(Platform::String^ routeUri)
{
	const char* cc = Utils::pstoccs(routeUri);
	linphone_proxy_config_set_route(this->proxy_config, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetRoute()
{
	return Utils::cctops(linphone_proxy_config_get_route(this->proxy_config));
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

Linphone::Core::LinphoneProxyConfig::LinphoneProxyConfig(::LinphoneCore *lc)
{
	this->proxy_config = linphone_core_create_proxy_config(lc);
}

Linphone::Core::LinphoneProxyConfig::~LinphoneProxyConfig()
{
	free(this->proxy_config);
}