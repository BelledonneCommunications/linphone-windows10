/*
ProxyConfig.cpp
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

#include "Core.h"
#include "ProxyConfig.h"
#include "Utils.h"


Platform::String^ Linphone::Native::ProxyConfig::ContactParameters::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_contact_parameters(this->proxyConfig));
}

void Linphone::Native::ProxyConfig::ContactParameters::set(Platform::String^ params)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_parameters(this->proxyConfig, cc);
	delete(cc);
}

Platform::String^ Linphone::Native::ProxyConfig::ContactUriParameters::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_contact_uri_parameters(this->proxyConfig));
}

void Linphone::Native::ProxyConfig::ContactUriParameters::set(Platform::String^ params)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(params);
	linphone_proxy_config_set_contact_uri_parameters(this->proxyConfig, cc);
	delete(cc);
}

Platform::Boolean Linphone::Native::ProxyConfig::DialEscapePlus::get()
{
	API_LOCK;
	return (linphone_proxy_config_get_dial_escape_plus(this->proxyConfig) == TRUE);
}

void Linphone::Native::ProxyConfig::DialEscapePlus::set(Platform::Boolean value)
{
	API_LOCK;
	linphone_proxy_config_set_dial_escape_plus(this->proxyConfig, value);
}

Platform::String^ Linphone::Native::ProxyConfig::DialPrefix::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_dial_prefix(this->proxyConfig));
}

void Linphone::Native::ProxyConfig::DialPrefix::set(Platform::String^ prefix)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(prefix);
	linphone_proxy_config_set_dial_prefix(this->proxyConfig, cc);
	delete(cc);
}

Platform::String^ Linphone::Native::ProxyConfig::Domain::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_domain(this->proxyConfig));
}

Linphone::Native::Reason Linphone::Native::ProxyConfig::Error::get()
{
	API_LOCK;
	return (Linphone::Native::Reason)linphone_proxy_config_get_error(this->proxyConfig);
}

int Linphone::Native::ProxyConfig::Expires::get()
{
	API_LOCK;
	return linphone_proxy_config_get_expires(this->proxyConfig);
}

void Linphone::Native::ProxyConfig::Expires::set(int delay)
{
	API_LOCK;
	linphone_proxy_config_set_expires(this->proxyConfig, delay);
}

Platform::String^ Linphone::Native::ProxyConfig::Identity::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_identity(this->proxyConfig));
}

void Linphone::Native::ProxyConfig::Identity::set(Platform::String^ identity)
{
	API_LOCK;
	const char* cidentity = Utils::pstoccs(identity);
	linphone_proxy_config_set_identity(this->proxyConfig, cidentity);
	delete(cidentity);
}

Platform::Boolean Linphone::Native::ProxyConfig::IsAvpfEnabled::get()
{
	API_LOCK;
	return (linphone_proxy_config_avpf_enabled(this->proxyConfig) == TRUE);
}

void Linphone::Native::ProxyConfig::IsAvpfEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_proxy_config_enable_avpf(this->proxyConfig, enable);
}

Platform::Boolean Linphone::Native::ProxyConfig::IsPublishEnabled::get()
{
	API_LOCK;
	return (linphone_proxy_config_publish_enabled(this->proxyConfig) == TRUE);
}

void Linphone::Native::ProxyConfig::IsPublishEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_proxy_config_enable_publish(this->proxyConfig, enable);
}

Platform::Boolean Linphone::Native::ProxyConfig::IsRegistered::get()
{
	API_LOCK;
	return (linphone_proxy_config_is_registered(this->proxyConfig) == TRUE);
}

Platform::Boolean Linphone::Native::ProxyConfig::IsRegisterEnabled::get()
{
	API_LOCK;
	return (linphone_proxy_config_register_enabled(this->proxyConfig) == TRUE);
}

void Linphone::Native::ProxyConfig::IsRegisterEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_proxy_config_enable_register(this->proxyConfig, enable);
}

Platform::String^ Linphone::Native::ProxyConfig::Route::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_route(this->proxyConfig));
}

void Linphone::Native::ProxyConfig::Route::set(Platform::String^ routeUri)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(routeUri);
	linphone_proxy_config_set_route(this->proxyConfig, cc);
	delete(cc);
}

Platform::String^ Linphone::Native::ProxyConfig::ServerAddr::get()
{
	API_LOCK;
	return Utils::cctops(linphone_proxy_config_get_server_addr(this->proxyConfig));
}

void Linphone::Native::ProxyConfig::ServerAddr::set(Platform::String^ proxyUri)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(proxyUri);
	linphone_proxy_config_set_server_addr(this->proxyConfig, cc);
	delete(cc);
}

Linphone::Native::RegistrationState Linphone::Native::ProxyConfig::State::get()
{
	API_LOCK;
	return (Linphone::Native::RegistrationState)linphone_proxy_config_get_state(this->proxyConfig);
}

void Linphone::Native::ProxyConfig::Done()
{
	API_LOCK;
	linphone_proxy_config_done(this->proxyConfig);
}

void Linphone::Native::ProxyConfig::Edit()
{
	API_LOCK;
	linphone_proxy_config_edit(this->proxyConfig);
}

Platform::String^ Linphone::Native::ProxyConfig::NormalizeNumber(Platform::String^ phoneNumber)
{
	API_LOCK;
	const char* cc = Utils::pstoccs(phoneNumber);
	char* result = (char*) malloc(phoneNumber->Length());
	int result_size = 0;
	linphone_proxy_config_normalize_number(this->proxyConfig, cc, result, result_size);
	Platform::String^ val = Utils::cctops(result);
	delete(cc);
	free(result);
	return val;
}

int Linphone::Native::ProxyConfig::LookupCccFromE164(Platform::String^ e164)
{
	API_LOCK;
	const char* e164char = Linphone::Native::Utils::pstoccs(e164);
	int ccc = linphone_dial_plan_lookup_ccc_from_e164(e164char);
	delete(e164char);
	return ccc;
}

int Linphone::Native::ProxyConfig::LookupCccFromIso(Platform::String^ iso)
{
	API_LOCK;
	const char* isochar = Linphone::Native::Utils::pstoccs(iso);
	int ccc = linphone_dial_plan_lookup_ccc_from_iso(isochar);
	delete(isochar);
	return ccc;
}

Linphone::Native::ProxyConfig::ProxyConfig(::LinphoneProxyConfig* proxyConfig)
{
	API_LOCK;
	this->proxyConfig = proxyConfig;
	RefToPtrProxy<ProxyConfig^> *proxy = new RefToPtrProxy<ProxyConfig^>(this);
	linphone_proxy_config_ref(this->proxyConfig);
	linphone_proxy_config_set_user_data(this->proxyConfig, proxy);
}

Linphone::Native::ProxyConfig::~ProxyConfig()
{
	API_LOCK;
	RefToPtrProxy<ProxyConfig^> *proxy = reinterpret_cast< RefToPtrProxy<ProxyConfig^> *>(linphone_proxy_config_get_user_data(this->proxyConfig));
	delete proxy;
	linphone_proxy_config_unref(this->proxyConfig);
}
