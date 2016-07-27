/*
AuthInfo.cpp
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

#include "AuthInfo.h"
#include "Utils.h"

using namespace BelledonneCommunications::Linphone::Native;

Platform::String^ AuthInfo::Ha1::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_ha1(this->auth_info));
}

void AuthInfo::Ha1::set(Platform::String^ ha1)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(ha1);
	linphone_auth_info_set_ha1(this->auth_info, cc);
	delete(cc);
}

Platform::String^ AuthInfo::Passwd::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_passwd(this->auth_info));
}

void AuthInfo::Passwd::set(Platform::String^ password)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(password);
	linphone_auth_info_set_passwd(this->auth_info, cc);
	delete(cc);
}

Platform::String^ AuthInfo::Realm::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_realm(this->auth_info));
}

void AuthInfo::Realm::set(Platform::String^ realm)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(realm);
	linphone_auth_info_set_realm(this->auth_info, cc);
	delete(cc);
}

Platform::String^ AuthInfo::Userid::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_userid(this->auth_info));
}

void AuthInfo::Userid::set(Platform::String^ userid)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(userid);
	linphone_auth_info_set_userid(this->auth_info, cc);
	delete(cc);
}

Platform::String^ AuthInfo::Username::get()
{
	API_LOCK;
	return Utils::cctops(linphone_auth_info_get_username(this->auth_info));
}

void AuthInfo::Username::set(Platform::String^ username)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(username);
	linphone_auth_info_set_username(this->auth_info, cc);
	delete(cc);
}

AuthInfo::AuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
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

AuthInfo::AuthInfo(::LinphoneAuthInfo *auth_info)
	: auth_info(auth_info)
{
}

AuthInfo::~AuthInfo()
{
}