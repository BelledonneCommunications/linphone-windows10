#include "LinphoneAuthInfo.h"
#include "Server.h"
#include "Utils.h"

Platform::String^ Linphone::Core::LinphoneAuthInfo::GetUsername()
{
	return nullptr;
}

void Linphone::Core::LinphoneAuthInfo::SetUsername(Platform::String^ username)
{

}

Platform::String^ Linphone::Core::LinphoneAuthInfo::GetPassword()
{
	return nullptr;
}

void Linphone::Core::LinphoneAuthInfo::SetPassword(Platform::String^ password)
{

}

Platform::String^ Linphone::Core::LinphoneAuthInfo::GetRealm()
{
	return nullptr;
}

void Linphone::Core::LinphoneAuthInfo::SetRealm(Platform::String^ realm)
{

}

Linphone::Core::LinphoneAuthInfo::LinphoneAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm)
{
	const char* cc_username = Utils::pstoccs(username);
	const char* cc_password = Utils::pstoccs(password);
	const char* cc_realm = Utils::pstoccs(realm);
	const char* cc_userid = Utils::pstoccs(userid);
	const char* cc_ha1 = Utils::pstoccs(ha1);
	this->auth_info = linphone_auth_info_new(cc_username, cc_userid, cc_password, cc_ha1, cc_realm);
	delete(cc_username);
	delete(cc_userid);
	delete(cc_password);
	delete(cc_ha1);
	delete(cc_realm);
}

Linphone::Core::LinphoneAuthInfo::~LinphoneAuthInfo()
{
	
}