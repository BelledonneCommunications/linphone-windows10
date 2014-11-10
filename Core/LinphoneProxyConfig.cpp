#include "LinphoneProxyConfig.h"
#include "LinphoneCore.h"
#include "Server.h"
#include "Utils.h"

void Linphone::Core::LinphoneProxyConfig::Edit()
{
	API_LOCK;
	linphone_proxy_config_edit(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::Done()
{
	API_LOCK;
	linphone_proxy_config_done(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::SetIdentity(Platform::String^ displayname, Platform::String^ username, Platform::String^ domain)
{
	API_LOCK;

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
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_identity(this->proxy_config));
}

void Linphone::Core::LinphoneProxyConfig::SetProxy(Platform::String^ proxyUri)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(proxyUri);
	linphone_proxy_config_set_server_addr(this->proxy_config, cc);
	delete(cc);
}

Linphone::Core::Reason Linphone::Core::LinphoneProxyConfig::Error::get()
{
	API_LOCK;
	return (Linphone::Core::Reason)linphone_proxy_config_get_error(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::EnableRegister(Platform::Boolean enable)
{
	API_LOCK;
	linphone_proxy_config_enable_register(this->proxy_config, enable);
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegisterEnabled()
{
	API_LOCK;
	return (linphone_proxy_config_register_enabled(this->proxy_config) == TRUE);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::NormalizePhoneNumber(Platform::String^ phoneNumber)
{
	API_LOCK;
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
	API_LOCK;
	const char* cc = Utils::pstoccs(prefix);
	linphone_proxy_config_set_dial_prefix(this->proxy_config, cc);
	delete(cc);
}

void Linphone::Core::LinphoneProxyConfig::SetDialEscapePlus(Platform::Boolean value)
{
	API_LOCK;
	linphone_proxy_config_set_dial_escape_plus(this->proxy_config, value);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetAddr()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_addr(this->proxy_config));
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetDomain()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_domain(this->proxy_config));
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegistered()
{
	API_LOCK;
	return (linphone_proxy_config_is_registered(this->proxy_config) == TRUE);
}

void Linphone::Core::LinphoneProxyConfig::SetRoute(Platform::String^ routeUri)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(routeUri);
	linphone_proxy_config_set_route(this->proxy_config, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::GetRoute()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_route(this->proxy_config));
}

void Linphone::Core::LinphoneProxyConfig::EnablePublish(Platform::Boolean enable)
{
	API_LOCK;
	linphone_proxy_config_enable_publish(this->proxy_config, enable);
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsPublishEnabled()
{
	API_LOCK;
	return (linphone_proxy_config_publish_enabled(this->proxy_config) == TRUE);
}

Linphone::Core::RegistrationState Linphone::Core::LinphoneProxyConfig::GetState()
{
	API_LOCK;
	return (Linphone::Core::RegistrationState)linphone_proxy_config_get_state(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::SetExpires(int delay)
{
	API_LOCK;
	linphone_proxy_config_expires(this->proxy_config, delay);
}

void Linphone::Core::LinphoneProxyConfig::SetContactParameters(Platform::String^ params)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_parameters(this->proxy_config, cc);
	delete(cc);
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromIso(Platform::String^ iso)
{
	API_LOCK;
	const char* isochar = Linphone::Core::Utils::pstoccs(iso);
	int ccc = linphone_dial_plan_lookup_ccc_from_iso(isochar);
	delete(isochar);
	return ccc;
}

int Linphone::Core::LinphoneProxyConfig::LookupCCCFromE164(Platform::String^ e164)
{
	API_LOCK;
	const char* e164char = Linphone::Core::Utils::pstoccs(e164);
	int ccc = linphone_dial_plan_lookup_ccc_from_e164(e164char);
	delete(e164char);
	return ccc;
}

void Linphone::Core::LinphoneProxyConfig::SetContactUriParameters(Platform::String^ params)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_uri_parameters(this->proxy_config, cc);
	delete(cc);
}

Linphone::Core::LinphoneProxyConfig::LinphoneProxyConfig()
{
	API_LOCK;
	this->proxy_config = linphone_proxy_config_new();
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = new RefToPtrProxy<LinphoneProxyConfig^>(this);
	linphone_proxy_config_set_user_data(this->proxy_config, proxy);
}

Linphone::Core::LinphoneProxyConfig::LinphoneProxyConfig(::LinphoneProxyConfig* proxy_config)
{
	API_LOCK;
	this->proxy_config = proxy_config;
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = new RefToPtrProxy<LinphoneProxyConfig^>(this);
	linphone_proxy_config_set_user_data(this->proxy_config, proxy);
}

Linphone::Core::LinphoneProxyConfig::~LinphoneProxyConfig()
{
	API_LOCK;
	RefToPtrProxy<LinphoneProxyConfig^> *proxy = reinterpret_cast< RefToPtrProxy<LinphoneProxyConfig^> *>(linphone_proxy_config_get_user_data(this->proxy_config));
	delete proxy;
}
