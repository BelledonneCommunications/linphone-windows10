#include "LinphoneAddress.h"
#include "Server.h"

Platform::String^ Linphone::Core::LinphoneAddress::GetDisplayName()
{
	return this->displayName;
}

Platform::String^ Linphone::Core::LinphoneAddress::GetUserName()
{
	return this->username;
}

Platform::String^ Linphone::Core::LinphoneAddress::GetDomain()
{
	return this->domain;
}

int Linphone::Core::LinphoneAddress::GetPort()
{
	return -1;
}

void Linphone::Core::LinphoneAddress::SetDisplayName(Platform::String^ name)
{
	this->displayName = name;
}

void Linphone::Core::LinphoneAddress::SetUserName(Platform::String^ username)
{
	this->username = username;
}

void Linphone::Core::LinphoneAddress::SetDomain(Platform::String^ domain)
{
	this->domain = domain;
}

void Linphone::Core::LinphoneAddress::SetPort(int port)
{

}

void Linphone::Core::LinphoneAddress::Clean()
{

}

Platform::String^ Linphone::Core::LinphoneAddress::AsString()
{
	return this->address;
}

Platform::String^ Linphone::Core::LinphoneAddress::AsStringUriOnly()
{
	return this->address;
}

Platform::String^ Linphone::Core::LinphoneAddress::ToString()
{
	return this->address;
}

Linphone::Core::LinphoneAddress::LinphoneAddress(Platform::String^ address) :
	address(address)
{

}

Linphone::Core::LinphoneAddress::LinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName) :
	address(L"sip:" + username + "@" + domain),
	displayName(displayName),
	username(username),
	domain(domain)
{

}

Linphone::Core::LinphoneAddress::~LinphoneAddress()
{

}