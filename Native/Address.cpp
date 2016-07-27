/*
Address.cpp
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#include "ApiLock.h"
#include "Address.h"

using namespace BelledonneCommunications::Linphone::Native;

Platform::String^ Address::DisplayName::get()
{
	API_LOCK;
	return Utils::cctops(linphone_address_get_display_name(this->address));
}

void Address::DisplayName::set(Platform::String^ name)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(name);
	linphone_address_set_display_name(this->address, cc);
	delete(cc);
}

Platform::String^ Address::Domain::get()
{
	API_LOCK;
	return Utils::cctops(linphone_address_get_domain(this->address));
}

void Address::Domain::set(Platform::String^ domain)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(domain);
	linphone_address_set_domain(this->address, cc);
	delete(cc);
}

int Address::Port::get()
{
	API_LOCK;
	return linphone_address_get_port(this->address);
}

void Address::Port::set(int port)
{
	API_LOCK;
	linphone_address_set_port(this->address, port);
}

Platform::String^ Address::Scheme::get()
{
	API_LOCK;
	return Utils::cctops(linphone_address_get_scheme(this->address));
}

Transport Address::Transport::get()
{
	API_LOCK;
	::LinphoneTransportType transport = linphone_address_get_transport(this->address);
	switch (transport)
	{
	default:
	case LinphoneTransportUdp:
		return BelledonneCommunications::Linphone::Native::Transport::UDP;
	case LinphoneTransportTcp:
		return BelledonneCommunications::Linphone::Native::Transport::TCP;
	case LinphoneTransportTls:
		return BelledonneCommunications::Linphone::Native::Transport::TLS;
	case LinphoneTransportDtls:
		return BelledonneCommunications::Linphone::Native::Transport::DTLS;
	}
}

void Address::Transport::set(BelledonneCommunications::Linphone::Native::Transport transport)
{
	API_LOCK;
	::LinphoneTransportType transportType = LinphoneTransportUdp;
	if (transport == BelledonneCommunications::Linphone::Native::Transport::TCP)
	{
		transportType = LinphoneTransportTcp;
	}
	else if (transport == BelledonneCommunications::Linphone::Native::Transport::TLS)
	{
		transportType = LinphoneTransportTls;
	}
	else if (transport == BelledonneCommunications::Linphone::Native::Transport::DTLS)
	{
		transportType = LinphoneTransportDtls;
	}

	linphone_address_set_transport(this->address, transportType);
}

Platform::String^ Address::UserName::get()
{
	API_LOCK;
	return Utils::cctops(linphone_address_get_username(this->address));
}

void Address::UserName::set(Platform::String^ username)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(username);
	linphone_address_set_username(this->address, cc);
	delete(cc);
}

Platform::String^ Address::AsString()
{
	API_LOCK;
	return Utils::cctops(linphone_address_as_string(this->address));
}

Platform::String^ Address::AsStringUriOnly()
{
	API_LOCK;
	return Utils::cctops(linphone_address_as_string_uri_only(this->address));
}

void Address::Clean()
{
	API_LOCK;
	linphone_address_clean(this->address);
}

Platform::String^ Address::ToString()
{
	return this->AsString();
}

Address::Address(::LinphoneAddress *addr) :
	address(addr)
{
	linphone_address_ref(this->address);
}

Address::Address(const char *uri)
{
	this->address = linphone_address_new(uri);
	if (this->address != nullptr) {
		linphone_address_ref(this->address);
	}
}

Address::~Address()
{
	if (this->address != nullptr) {
		linphone_address_unref(this->address);
	}
}