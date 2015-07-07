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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

#include "ApiLock.h"
#include "Address.h"

Platform::String^ Linphone::Native::Address::DisplayName::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_address_get_display_name(this->address));
}

void Linphone::Native::Address::DisplayName::set(Platform::String^ name)
{
	API_LOCK;
	const char *cc = Linphone::Native::Utils::pstoccs(name);
	linphone_address_set_display_name(this->address, cc);
	delete(cc);
}

Platform::String^ Linphone::Native::Address::Domain::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_address_get_domain(this->address));
}

void Linphone::Native::Address::Domain::set(Platform::String^ domain)
{
	API_LOCK;
	const char *cc = Linphone::Native::Utils::pstoccs(domain);
	linphone_address_set_domain(this->address, cc);
	delete(cc);
}

int Linphone::Native::Address::Port::get()
{
	API_LOCK;
	return linphone_address_get_port(this->address);
}

void Linphone::Native::Address::Port::set(int port)
{
	API_LOCK;
	linphone_address_set_port(this->address, port);
}

Platform::String^ Linphone::Native::Address::Scheme::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_address_get_scheme(this->address));
}

Linphone::Native::Transport Linphone::Native::Address::Transport::get()
{
	API_LOCK;
	::LinphoneTransportType transport = linphone_address_get_transport(this->address);
	switch (transport)
	{
	default:
	case LinphoneTransportUdp:
		return Linphone::Native::Transport::UDP;
	case LinphoneTransportTcp:
		return Linphone::Native::Transport::TCP;
	case LinphoneTransportTls:
		return Linphone::Native::Transport::TLS;
	case LinphoneTransportDtls:
		return Linphone::Native::Transport::DTLS;
	}
}

void Linphone::Native::Address::Transport::set(Linphone::Native::Transport transport)
{
	API_LOCK;
	::LinphoneTransportType transportType = LinphoneTransportUdp;
	if (transport == Linphone::Native::Transport::TCP)
	{
		transportType = LinphoneTransportTcp;
	}
	else if (transport == Linphone::Native::Transport::TLS)
	{
		transportType = LinphoneTransportTls;
	}
	else if (transport == Linphone::Native::Transport::DTLS)
	{
		transportType = LinphoneTransportDtls;
	}

	linphone_address_set_transport(this->address, transportType);
}

Platform::String^ Linphone::Native::Address::UserName::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_address_get_username(this->address));
}

void Linphone::Native::Address::UserName::set(Platform::String^ username)
{
	API_LOCK;
	const char *cc = Linphone::Native::Utils::pstoccs(username);
	linphone_address_set_username(this->address, cc);
	delete(cc);
}

Platform::String^ Linphone::Native::Address::AsString()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_address_as_string(this->address));
}

Platform::String^ Linphone::Native::Address::AsStringUriOnly()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_address_as_string_uri_only(this->address));
}

void Linphone::Native::Address::Clean()
{
	API_LOCK;
	linphone_address_clean(this->address);
}

Platform::String^ Linphone::Native::Address::ToString()
{
	return this->AsString();
}

Linphone::Native::Address::Address(::LinphoneAddress *addr) :
	address(addr)
{
	linphone_address_ref(this->address);
}

Linphone::Native::Address::Address(const char *uri)
{
	this->address = linphone_address_new(uri);
	if (this->address != nullptr) {
		linphone_address_ref(this->address);
	}
}

Linphone::Native::Address::~Address()
{
	if (this->address != nullptr) {
		linphone_address_unref(this->address);
	}
}