#include "LinphoneAddress.h"
#include "Server.h"

using namespace Linphone::Core;

Platform::String^ LinphoneAddress::GetDisplayName()
{
	return nullptr;
}

Platform::String^ LinphoneAddress::GetUserName()
{
	return nullptr;
}

Platform::String^ LinphoneAddress::GetDomain()
{
	return nullptr;
}

int LinphoneAddress::GetPort()
{
	return -1;
}

void LinphoneAddress::SetDisplayName(Platform::String^ name)
{

}

void LinphoneAddress::SetUserName(Platform::String^ username)
{

}

void LinphoneAddress::SetDomain(Platform::String^ domain)
{

}

void LinphoneAddress::SetPort(int port)
{

}

void LinphoneAddress::Clean()
{

}

Platform::String^ LinphoneAddress::AsString()
{
	return nullptr;
}

Platform::String^ LinphoneAddress::AsStringUriOnly()
{
	return nullptr;
}

Platform::String^ LinphoneAddress::ToString()
{
	return nullptr;
}
