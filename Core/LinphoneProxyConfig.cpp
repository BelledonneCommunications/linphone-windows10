#include "LinphoneProxyConfig.h"
#include "LinphoneCore.h"
#include "Server.h"
#include "Utils.h"

void Linphone::Core::LinphoneProxyConfig::Edit()
{
	gApiLock.Lock();
	linphone_proxy_config_edit(this->proxy_config);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::Done()
{
	gApiLock.Lock();
	linphone_proxy_config_done(this->proxy_config);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::SetIdentity(Platform::String^ displayname, Platform::String^ username, Platform::String^ domain)
{
	gApiLock.Lock();

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

	gApiLock.Unlock();
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetIdentity()
{
	gApiLock.Lock();
	Platform::String^ identity = Utils::cctops(linphone_proxy_config_get_identity(this->proxy_config));
	gApiLock.Unlock();
	return identity;
}

void Linphone::Core::LinphoneProxyConfig::SetProxy(Platform::String^ proxyUri)
{
	gApiLock.Lock();
	const char* cc = Utils::pstoccs(proxyUri);
	linphone_proxy_config_set_server_addr(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::EnableRegister(Platform::Boolean enable)
{
	gApiLock.Lock();
	linphone_proxy_config_enable_register(this->proxy_config, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegisterEnabled()
{
	gApiLock.Lock();
	Platform::Boolean enabled = (linphone_proxy_config_register_enabled(this->proxy_config) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::NormalizePhoneNumber(Platform::String^ phoneNumber)
{
	gApiLock.Lock();
	const char* cc = Utils::pstoccs(phoneNumber);
	char* result = (char*) malloc(phoneNumber->Length());
	int result_size = 0;
	linphone_proxy_config_normalize_number(this->proxy_config, cc, result, result_size);
	Platform::String^ val = Utils::cctops(result);
	delete(cc);
	delete(result);
	gApiLock.Unlock();
	return val;
}

void Linphone::Core::LinphoneProxyConfig::SetDialPrefix(Platform::String^ prefix)
{
	gApiLock.Lock();
	const char* cc = Utils::pstoccs(prefix);
	linphone_proxy_config_set_dial_prefix(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::SetDialEscapePlus(Platform::Boolean value)
{
	gApiLock.Lock();
	linphone_proxy_config_set_dial_escape_plus(this->proxy_config, value);
	gApiLock.Unlock();
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetAddr()
{
	gApiLock.Lock();
	Platform::String^ addr = Utils::cctops(linphone_proxy_config_get_addr(this->proxy_config));
	gApiLock.Unlock();
	return addr;
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetDomain()
{
	gApiLock.Lock();
	Platform::String^ domain = Utils::cctops(linphone_proxy_config_get_domain(this->proxy_config));
	gApiLock.Unlock();
	return domain;
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegistered()
{
	gApiLock.Lock();
	Platform::Boolean registered = (linphone_proxy_config_is_registered(this->proxy_config) == TRUE);
	gApiLock.Unlock();
	return registered;
}

void Linphone::Core::LinphoneProxyConfig::SetRoute(Platform::String^ routeUri)
{
	gApiLock.Lock();
	const char* cc = Utils::pstoccs(routeUri);
	linphone_proxy_config_set_route(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetRoute()
{
	gApiLock.Lock();
	Platform::String^ route = Utils::cctops(linphone_proxy_config_get_route(this->proxy_config));
	gApiLock.Unlock();
	return route;
}

void Linphone::Core::LinphoneProxyConfig::EnablePublish(Platform::Boolean enable)
{
	gApiLock.Lock();
	linphone_proxy_config_enable_publish(this->proxy_config, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsPublishEnabled()
{
	gApiLock.Lock();
	Platform::Boolean enabled = (linphone_proxy_config_publish_enabled(this->proxy_config) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

Linphone::Core::RegistrationState Linphone::Core::LinphoneProxyConfig::GetState()
{
	gApiLock.Lock();
	Linphone::Core::RegistrationState state = (Linphone::Core::RegistrationState)linphone_proxy_config_get_state(this->proxy_config);
	gApiLock.Unlock();
	return state;
}

void Linphone::Core::LinphoneProxyConfig::SetExpires(int delay)
{
	gApiLock.Lock();
	linphone_proxy_config_expires(this->proxy_config, delay);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::SetContactParameters(Platform::String^ params)
{
	gApiLock.Lock();
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_parameters(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromIso(Platform::String^ iso)
{
	gApiLock.Lock();
	const char* isochar = Linphone::Core::Utils::pstoccs(iso);
	int ccc = linphone_dial_plan_lookup_ccc_from_iso(isochar);
	delete(isochar);
	gApiLock.Unlock();
	return ccc;
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromE164(Platform::String^ e164)
{
	gApiLock.Lock();
	const char* e164char = Linphone::Core::Utils::pstoccs(e164);
	int ccc = linphone_dial_plan_lookup_ccc_from_e164(e164char);
	delete(e164char);
	gApiLock.Unlock();
	return ccc;
}

Linphone::Core::LinphoneProxyConfig::LinphoneProxyConfig()
{
	gApiLock.Lock();
	this->proxy_config = linphone_proxy_config_new();
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = new RefToPtrProxy<LinphoneProxyConfig^>(this);
	linphone_proxy_config_set_user_data(this->proxy_config, proxy);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneProxyConfig::LinphoneProxyConfig(::LinphoneProxyConfig* proxy_config)
{
	gApiLock.Lock();
	this->proxy_config = proxy_config;
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = new RefToPtrProxy<LinphoneProxyConfig^>(this);
	linphone_proxy_config_set_user_data(this->proxy_config, proxy);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneProxyConfig::~LinphoneProxyConfig()
{
	gApiLock.Lock();
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = reinterpret_cast< RefToPtrProxy<LinphoneProxyConfig^> *>(linphone_proxy_config_get_user_data(this->proxy_config));
	delete proxy;
	gApiLock.Unlock();
}
