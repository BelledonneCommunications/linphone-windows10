#include "LinphoneAddress.h"
#include "Server.h"

using namespace Linphone::Core;

Platform::String^ LinphoneAddress::GetDisplayName()
{
	return this->displayName;
}

Platform::String^ LinphoneAddress::GetUserName()
{
	return this->username;
}

Platform::String^ LinphoneAddress::GetDomain()
{
	return this->domain;
}

int LinphoneAddress::GetPort()
{
	return -1;
}

void LinphoneAddress::SetDisplayName(Platform::String^ name)
{
	this->displayName = name;
}

void LinphoneAddress::SetUserName(Platform::String^ username)
{
	this->username = username;
}

void LinphoneAddress::SetDomain(Platform::String^ domain)
{
	this->domain = domain;
}

void LinphoneAddress::SetPort(int port)
{

}

void LinphoneAddress::Clean()
{

}

Platform::String^ LinphoneAddress::AsString()
{
	return this->address;
}

Platform::String^ LinphoneAddress::AsStringUriOnly()
{
	return this->address;
}

Platform::String^ LinphoneAddress::ToString()
{
	return this->address;
}

LinphoneAddress::LinphoneAddress(Platform::String^ address) :
	address(address)
{

}

LinphoneAddress::LinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName) :
	address(L"sip:" + username + "@" + domain),
	displayName(displayName),
	username(username),
	domain(domain)
{

}

LinphoneAddress::~LinphoneAddress()
{

}