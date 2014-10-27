#include "LinphoneProxyConfig.h"
#include "LinphoneCore.h"
#include "Server.h"
#include "Utils.h"

void Linphone::Core::LinphoneProxyConfig::Edit()
{
	TRACE; gApiLock.Lock();
	linphone_proxy_config_edit(this->proxy_config);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::Done()
{
	TRACE; gApiLock.Lock();
	linphone_proxy_config_done(this->proxy_config);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::SetIdentity(Platform::String^ displayname, Platform::String^ username, Platform::String^ domain)
{
	TRACE; gApiLock.Lock();

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
	TRACE; gApiLock.Lock();
	Platform::String^ identity = Utils::cctops(linphone_proxy_config_get_identity(this->proxy_config));
	gApiLock.Unlock();
	return identity;
}

void Linphone::Core::LinphoneProxyConfig::SetProxy(Platform::String^ proxyUri)
{
	TRACE; gApiLock.Lock();
	const char* cc = Utils::pstoccs(proxyUri);
	linphone_proxy_config_set_server_addr(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

Linphone::Core::Reason Linphone::Core::LinphoneProxyConfig::Error::get()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::Reason reason = (Linphone::Core::Reason)linphone_proxy_config_get_error(this->proxy_config);
	gApiLock.Unlock();
	return reason;
}

void Linphone::Core::LinphoneProxyConfig::EnableRegister(Platform::Boolean enable)
{
	TRACE; gApiLock.Lock();
	linphone_proxy_config_enable_register(this->proxy_config, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegisterEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_proxy_config_register_enabled(this->proxy_config) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::NormalizePhoneNumber(Platform::String^ phoneNumber)
{
	TRACE; gApiLock.Lock();
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
	TRACE; gApiLock.Lock();
	const char* cc = Utils::pstoccs(prefix);
	linphone_proxy_config_set_dial_prefix(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::SetDialEscapePlus(Platform::Boolean value)
{
	TRACE; gApiLock.Lock();
	linphone_proxy_config_set_dial_escape_plus(this->proxy_config, value);
	gApiLock.Unlock();
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetAddr()
{
	TRACE; gApiLock.Lock();
	Platform::String^ addr = Utils::cctops(linphone_proxy_config_get_addr(this->proxy_config));
	gApiLock.Unlock();
	return addr;
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetDomain()
{
	TRACE; gApiLock.Lock();
	Platform::String^ domain = Utils::cctops(linphone_proxy_config_get_domain(this->proxy_config));
	gApiLock.Unlock();
	return domain;
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegistered()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean registered = (linphone_proxy_config_is_registered(this->proxy_config) == TRUE);
	gApiLock.Unlock();
	return registered;
}

void Linphone::Core::LinphoneProxyConfig::SetRoute(Platform::String^ routeUri)
{
	TRACE; gApiLock.Lock();
	const char* cc = Utils::pstoccs(routeUri);
	linphone_proxy_config_set_route(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetRoute()
{
	TRACE; gApiLock.Lock();
	Platform::String^ route = Utils::cctops(linphone_proxy_config_get_route(this->proxy_config));
	gApiLock.Unlock();
	return route;
}

void Linphone::Core::LinphoneProxyConfig::EnablePublish(Platform::Boolean enable)
{
	TRACE; gApiLock.Lock();
	linphone_proxy_config_enable_publish(this->proxy_config, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsPublishEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_proxy_config_publish_enabled(this->proxy_config) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

Linphone::Core::RegistrationState Linphone::Core::LinphoneProxyConfig::GetState()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::RegistrationState state = (Linphone::Core::RegistrationState)linphone_proxy_config_get_state(this->proxy_config);
	gApiLock.Unlock();
	return state;
}

void Linphone::Core::LinphoneProxyConfig::SetExpires(int delay)
{
	TRACE; gApiLock.Lock();
	linphone_proxy_config_expires(this->proxy_config, delay);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneProxyConfig::SetContactParameters(Platform::String^ params)
{
	TRACE; gApiLock.Lock();
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_parameters(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromIso(Platform::String^ iso)
{
	TRACE; gApiLock.Lock();
	const char* isochar = Linphone::Core::Utils::pstoccs(iso);
	int ccc = linphone_dial_plan_lookup_ccc_from_iso(isochar);
	delete(isochar);
	gApiLock.Unlock();
	return ccc;
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromE164(Platform::String^ e164)
{
	TRACE; gApiLock.Lock();
	const char* e164char = Linphone::Core::Utils::pstoccs(e164);
	int ccc = linphone_dial_plan_lookup_ccc_from_e164(e164char);
	delete(e164char);
	gApiLock.Unlock();
	return ccc;
}

void Linphone::Core::LinphoneProxyConfig::SetContactUriParameters(Platform::String^ params)
{
	TRACE; gApiLock.Lock();
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_uri_parameters(this->proxy_config, cc);
	delete(cc);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneProxyConfig::LinphoneProxyConfig()
{
	TRACE; gApiLock.Lock();
	this->proxy_config = linphone_proxy_config_new();
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = new RefToPtrProxy<LinphoneProxyConfig^>(this);
	linphone_proxy_config_set_user_data(this->proxy_config, proxy);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneProxyConfig::LinphoneProxyConfig(::LinphoneProxyConfig* proxy_config)
{
	TRACE; gApiLock.Lock();
	this->proxy_config = proxy_config;
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = new RefToPtrProxy<LinphoneProxyConfig^>(this);
	linphone_proxy_config_set_user_data(this->proxy_config, proxy);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneProxyConfig::~LinphoneProxyConfig()
{
	TRACE; gApiLock.Lock();
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = reinterpret_cast< RefToPtrProxy<LinphoneProxyConfig^> *>(linphone_proxy_config_get_user_data(this->proxy_config));
	delete proxy;
	gApiLock.Unlock();
}
