/*
LinphoneAddress.cpp
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
#include "LinphoneAddress.h"
#include "Server.h"

Platform::String^ Linphone::Core::LinphoneAddress::DisplayName::get()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_address_get_display_name(this->address));
}

Platform::String^ Linphone::Core::LinphoneAddress::UserName::get()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_address_get_username(this->address));
}

Platform::String^ Linphone::Core::LinphoneAddress::Domain::get()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_address_get_domain(this->address));
}

int Linphone::Core::LinphoneAddress::Port::get()
{
	API_LOCK;
	return linphone_address_get_port(this->address);
}

Linphone::Core::LinphoneTransport Linphone::Core::LinphoneAddress::Transport::get()
{
	API_LOCK;
	LinphoneTransportType transport = linphone_address_get_transport(this->address);
	switch (transport)
	{
	case LinphoneTransportUdp:
		return LinphoneTransport::LinphoneTransportUDP;
	case LinphoneTransportTcp:
		return LinphoneTransport::LinphoneTransportTCP;
	case LinphoneTransportTls:
		return LinphoneTransport::LinphoneTransportTLS;
	case LinphoneTransportDtls:
		return LinphoneTransport::LinphoneTransportDTLS;
	default:
		return LinphoneTransport::LinphoneTransportUDP;
	}
}

Platform::String^ Linphone::Core::LinphoneAddress::Scheme::get()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_address_get_scheme(this->address));
}

void Linphone::Core::LinphoneAddress::DisplayName::set(Platform::String^ name)
{
	API_LOCK;
	const char *cc = Linphone::Core::Utils::pstoccs(name);
	linphone_address_set_display_name(this->address, cc);
	delete(cc);
}

void Linphone::Core::LinphoneAddress::UserName::set(Platform::String^ username)
{
	API_LOCK;
	const char *cc = Linphone::Core::Utils::pstoccs(username);
	linphone_address_set_username(this->address, cc);
	delete(cc);
}

void Linphone::Core::LinphoneAddress::Domain::set(Platform::String^ domain)
{
	API_LOCK;
	const char *cc = Linphone::Core::Utils::pstoccs(domain);
	linphone_address_set_domain(this->address, cc);
	delete(cc);
}

void Linphone::Core::LinphoneAddress::Port::set(int port)
{
	API_LOCK;
	linphone_address_set_port(this->address, port);
}

void Linphone::Core::LinphoneAddress::Transport::set(Linphone::Core::LinphoneTransport transport)
{
	API_LOCK;
	LinphoneTransportType transportType = LinphoneTransportUdp;
	if (transport == LinphoneTransport::LinphoneTransportTCP)
	{
		transportType = LinphoneTransportTcp;
	} 
	else if (transport == LinphoneTransport::LinphoneTransportTLS)
	{
		transportType = LinphoneTransportTls;
	} 
	else if (transport == LinphoneTransport::LinphoneTransportDTLS)
	{
		transportType = LinphoneTransportDtls;
	}

	linphone_address_set_transport(this->address, transportType);
}

void Linphone::Core::LinphoneAddress::Clean()
{
	API_LOCK;
	linphone_address_clean(this->address);
}

Platform::String^ Linphone::Core::LinphoneAddress::AsString()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_address_as_string(this->address));
}

Platform::String^ Linphone::Core::LinphoneAddress::AsStringUriOnly()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_address_as_string_uri_only(this->address));
}

Platform::String^ Linphone::Core::LinphoneAddress::ToString()
{
	return this->AsString();
}

Linphone::Core::LinphoneAddress::LinphoneAddress(::LinphoneAddress *addr) :
	address(addr)
{
}

Linphone::Core::LinphoneAddress::LinphoneAddress(const char *uri)
{
	this->address = linphone_address_new(uri);
}

Linphone::Core::LinphoneAddress::~LinphoneAddress()
{
}