#include "LinphoneAddress.h"
#include "Server.h"

Platform::String^ Linphone::Core::LinphoneAddress::GetDisplayName()
{
	return Linphone::Core::Utils::cctops(linphone_address_get_display_name(this->address));
}

Platform::String^ Linphone::Core::LinphoneAddress::GetUserName()
{
	return Linphone::Core::Utils::cctops(linphone_address_get_username(this->address));
}

Platform::String^ Linphone::Core::LinphoneAddress::GetDomain()
{
	return Linphone::Core::Utils::cctops(linphone_address_get_domain(this->address));
}

int Linphone::Core::LinphoneAddress::GetPort()
{
	return linphone_address_get_port(this->address);
}

Linphone::Core::LinphoneTransport Linphone::Core::LinphoneAddress::GetTransport()
{
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

void Linphone::Core::LinphoneAddress::SetDisplayName(Platform::String^ name)
{
	const char *cc = Linphone::Core::Utils::pstoccs(name);
	linphone_address_set_display_name(this->address, cc);
	delete(cc);
}

void Linphone::Core::LinphoneAddress::SetUserName(Platform::String^ username)
{
	const char *cc = Linphone::Core::Utils::pstoccs(username);
	linphone_address_set_username(this->address, cc);
	delete(cc);
}

void Linphone::Core::LinphoneAddress::SetDomain(Platform::String^ domain)
{
	const char *cc = Linphone::Core::Utils::pstoccs(domain);
	linphone_address_set_domain(this->address, cc);
	delete(cc);
}

void Linphone::Core::LinphoneAddress::SetPort(int port)
{
	linphone_address_set_port(this->address, port);
}

void Linphone::Core::LinphoneAddress::SetTransport(Linphone::Core::LinphoneTransport transport)
{
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
	linphone_address_clean(this->address);
}

Platform::String^ Linphone::Core::LinphoneAddress::AsString()
{
	return Linphone::Core::Utils::cctops(linphone_address_as_string(this->address));
}

Platform::String^ Linphone::Core::LinphoneAddress::AsStringUriOnly()
{
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