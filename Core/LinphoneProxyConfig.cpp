/*
LinphoneProxyConfig.cpp
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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

void Linphone::Core::LinphoneProxyConfig::ServerAddr::set(Platform::String^ proxyUri)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(proxyUri);
	linphone_proxy_config_set_server_addr(this->proxy_config, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::ServerAddr::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_server_addr(this->proxy_config));
}

Linphone::Core::Reason Linphone::Core::LinphoneProxyConfig::Error::get()
{
	API_LOCK;
	return (Linphone::Core::Reason)linphone_proxy_config_get_error(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::RegisterEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_proxy_config_enable_register(this->proxy_config, enable);
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::RegisterEnabled::get()
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

void Linphone::Core::LinphoneProxyConfig::DialPrefix::set(Platform::String^ prefix)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(prefix);
	linphone_proxy_config_set_dial_prefix(this->proxy_config, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::DialPrefix::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_dial_prefix(this->proxy_config));
}

void Linphone::Core::LinphoneProxyConfig::DialEscapePlus::set(Platform::Boolean value)
{
	API_LOCK;
	linphone_proxy_config_set_dial_escape_plus(this->proxy_config, value);
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::DialEscapePlus::get()
{
	API_LOCK;
	return (linphone_proxy_config_get_dial_escape_plus(this->proxy_config) == TRUE);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::Addr::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_addr(this->proxy_config));
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::Domain::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_domain(this->proxy_config));
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::IsRegistered::get()
{
	API_LOCK;
	return (linphone_proxy_config_is_registered(this->proxy_config) == TRUE);
}

void Linphone::Core::LinphoneProxyConfig::Route::set(Platform::String^ routeUri)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(routeUri);
	linphone_proxy_config_set_route(this->proxy_config, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::Route::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_route(this->proxy_config));
}

void Linphone::Core::LinphoneProxyConfig::PublishEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_proxy_config_enable_publish(this->proxy_config, enable);
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::PublishEnabled::get()
{
	API_LOCK;
	return (linphone_proxy_config_publish_enabled(this->proxy_config) == TRUE);
}

Linphone::Core::RegistrationState Linphone::Core::LinphoneProxyConfig::State::get()
{
	API_LOCK;
	return (Linphone::Core::RegistrationState)linphone_proxy_config_get_state(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::Expires::set(int delay)
{
	API_LOCK;
	linphone_proxy_config_set_expires(this->proxy_config, delay);
}

int Linphone::Core::LinphoneProxyConfig::Expires::get()
{
	API_LOCK;
	return linphone_proxy_config_get_expires(this->proxy_config);
}

void Linphone::Core::LinphoneProxyConfig::ContactParameters::set(Platform::String^ params)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_parameters(this->proxy_config, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::ContactParameters::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_contact_parameters(this->proxy_config));
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

void Linphone::Core::LinphoneProxyConfig::ContactUriParameters::set(Platform::String^ params)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_uri_parameters(this->proxy_config, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneProxyConfig::ContactUriParameters::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_contact_uri_parameters(this->proxy_config));
}

void Linphone::Core::LinphoneProxyConfig::AVPFEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_proxy_config_enable_avpf(this->proxy_config, enable);
}

Platform::Boolean Linphone::Core::LinphoneProxyConfig::AVPFEnabled::get()
{
	API_LOCK;
	return (linphone_proxy_config_avpf_enabled(this->proxy_config) == TRUE);
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
