#include "LinphoneAuthInfo.h"
#include "Server.h"
#include "Utils.h"

Platform::String^ Linphone::Core::LinphoneAuthInfo::Username::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_username(this->auth_info));
}

void Linphone::Core::LinphoneAuthInfo::Username::set(Platform::String^ username)
{
	API_LOCK;
	const char *cc = Linphone::Core::Utils::pstoccs(username);
	linphone_auth_info_set_username(this->auth_info, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneAuthInfo::UserId::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_userid(this->auth_info));
}

void Linphone::Core::LinphoneAuthInfo::UserId::set(Platform::String^ userid)
{
	API_LOCK;
	const char *cc = Linphone::Core::Utils::pstoccs(userid);
	linphone_auth_info_set_userid(this->auth_info, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneAuthInfo::Password::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_passwd(this->auth_info));
}

void Linphone::Core::LinphoneAuthInfo::Password::set(Platform::String^ password)
{
	API_LOCK;
	const char *cc = Linphone::Core::Utils::pstoccs(password);
	linphone_auth_info_set_passwd(this->auth_info, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneAuthInfo::Realm::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_realm(this->auth_info));
}

void Linphone::Core::LinphoneAuthInfo::Realm::set(Platform::String^ realm)
{
	API_LOCK;
	const char *cc = Linphone::Core::Utils::pstoccs(realm);
	linphone_auth_info_set_realm(this->auth_info, cc);
	delete(cc);
}

Platform::String^ Linphone::Core::LinphoneAuthInfo::Ha1::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_ha1(this->auth_info));
}

void Linphone::Core::LinphoneAuthInfo::Ha1::set(Platform::String^ ha1)
{
	API_LOCK;
	const char *cc = Linphone::Core::Utils::pstoccs(ha1);
	linphone_auth_info_set_ha1(this->auth_info, cc);
	delete(cc);
}

Linphone::Core::LinphoneAuthInfo::LinphoneAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
	API_LOCK;
	const char* cc_username = Utils::pstoccs(username);
	const char* cc_password = Utils::pstoccs(password);
	const char* cc_realm = Utils::pstoccs(realm);
	const char* cc_userid = Utils::pstoccs(userid);
	const char* cc_ha1 = Utils::pstoccs(ha1);
	const char* cc_domain = Utils::pstoccs(domain);
	this->auth_info = linphone_auth_info_new(cc_username, cc_userid, cc_password, cc_ha1, cc_realm, cc_domain);
	delete(cc_username);
	delete(cc_userid);
	delete(cc_password);
	delete(cc_ha1);
	delete(cc_realm);
	delete(cc_domain);
}

Linphone::Core::LinphoneAuthInfo::LinphoneAuthInfo(::LinphoneAuthInfo *auth_info) :
	auth_info(auth_info)
{
}

Linphone::Core::LinphoneAuthInfo::~LinphoneAuthInfo()
{
}