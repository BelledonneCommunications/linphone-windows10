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

void Linphone::Core::LinphoneProxyConfig::SetIdentity(Platform::String^ displayname, Platform::String^ username, Platform::String^ domain)
{
	const char* cc_username = Utils::pstoccs(username);
	const char* cc_domain = Utils::pstoccs(domain);
	const char* cc_displayname = Utils::pstoccs(displayname);
	::LinphoneAddress *addr = linphone_address_new(NULL);
	linphone_address_set_username(addr, cc_username);
	linphone_address_set_domain(addr, cc_domain);
	linphone_address_set_display_name(addr, cc_displayname);
	linphone_proxy_config_set_identity(this->proxy_config, linphone_address_as_string(addr));
	linphone_address_destroy(addr);
	delete(cc_username);
	delete(cc_domain);
	delete(cc_displayname);
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
	linphone_proxy_config_enable_register(this->proxy_config, enable);
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegisterEnabled()
{
	return linphone_proxy_config_register_enabled(this->proxy_config);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::NormalizePhoneNumber(Platform::String^ phoneNumber)
{
	const char* cc = Utils::pstoccs(phoneNumber);
	char* result = (char*) malloc(phoneNumber->Length());
	int result_size = 0;
	linphone_proxy_config_normalize_number(this->proxy_config, cc, result, result_size);
	Platform::String^ val = Utils::cctops(result);
	delete(cc);
	delete(result);
	return val;
}

void Linphone::Core::LinphoneProxyConfig::SetDialPrefix(Platform::String^ prefix)
{
	const char* cc = Utils::pstoccs(prefix);
	linphone_proxy_config_set_dial_prefix(this->proxy_config, cc);
	delete(cc);
}

void Linphone::Core::LinphoneProxyConfig::SetDialEscapePlus(Platform::Boolean value)
{
	linphone_proxy_config_set_dial_escape_plus(this->proxy_config, value);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetDomain()
{
	return Utils::cctops(linphone_proxy_config_get_domain(this->proxy_config));
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegistered()
{
	return linphone_proxy_config_is_registered(this->proxy_config);
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
	linphone_proxy_config_enable_publish(this->proxy_config, enable);
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
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_parameters(this->proxy_config, cc);
	delete(cc);
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromIso(Platform::String^ iso)
{
	return -1;
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromE164(Platform::String^ e164)
{
	return -1;
}

Linphone::Core::LinphoneProxyConfig::LinphoneProxyConfig()
{
	this->proxy_config = linphone_proxy_config_new();
	linphone_proxy_config_set_user_data(this->proxy_config, Linphone::Core::Utils::GetRawPointer(this));
}

Linphone::Core::LinphoneProxyConfig::~LinphoneProxyConfig()
{
	delete(this->proxy_config);
}