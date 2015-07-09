/*
LpConfig.cpp
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
#include "LpConfig.h"

bool Linphone::Native::LpConfig::GetBool(Platform::String^ section, Platform::String^ key, bool defaultValue)
{
	return (GetInt(section, key, defaultValue) == TRUE);
}

void Linphone::Native::LpConfig::SetBool(Platform::String^ section, Platform::String^ key, bool value)
{
	SetInt(section, key, (int)value);
}

int Linphone::Native::LpConfig::GetInt(Platform::String^ section, Platform::String^ key, int defaultValue)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	int value = lp_config_get_int(this->config, ccSection, ccKey, defaultValue);
	delete(ccKey);
	delete(ccSection);
	return value;
}

void Linphone::Native::LpConfig::SetInt(Platform::String^ section, Platform::String^ key, int value)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	lp_config_set_int(this->config, ccSection, ccKey, value);
	delete(ccKey);
	delete(ccSection);
}

int64 Linphone::Native::LpConfig::GetInt64(Platform::String^ section, Platform::String^ key, int64 defaultValue)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	int64 value = lp_config_get_int64(this->config, ccSection, ccKey, defaultValue);
	delete(ccKey);
	delete(ccSection);
	return value;
}

void Linphone::Native::LpConfig::SetInt64(Platform::String^ section, Platform::String^ key, int64 value)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	lp_config_set_int64(this->config, ccSection, ccKey, value);
	delete(ccKey);
	delete(ccSection);
}

float Linphone::Native::LpConfig::GetFloat(Platform::String^ section, Platform::String^ key, float defaultValue)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	float value = lp_config_get_float(this->config, ccSection, ccKey, defaultValue);
	delete(ccKey);
	delete(ccSection);
	return value;
}

void Linphone::Native::LpConfig::SetFloat(Platform::String^ section, Platform::String^ key, float value)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	lp_config_set_float(this->config, ccSection, ccKey, value);
	delete(ccKey);
	delete(ccSection);
}

Platform::String^ Linphone::Native::LpConfig::GetString(Platform::String^ section, Platform::String^ key, Platform::String^ defaultValue)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	const char *ccDefaultValue = Linphone::Native::Utils::pstoccs(defaultValue);
	const char *ccvalue = lp_config_get_string(this->config, ccSection, ccKey, ccDefaultValue);
	Platform::String^ value = Linphone::Native::Utils::cctops(ccvalue);
	delete(ccDefaultValue);
	delete(ccKey);
	delete(ccSection);
	return value;
}

void Linphone::Native::LpConfig::SetString(Platform::String^ section, Platform::String^ key, Platform::String^ value)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	const char *ccValue = Linphone::Native::Utils::pstoccs(value);
	lp_config_set_string(this->config, ccSection, ccKey, ccValue);
	delete(ccValue);
	delete(ccKey);
	delete(ccSection);
}

Platform::Array<int>^ Linphone::Native::LpConfig::GetRange(Platform::String^ section, Platform::String^ key, const Platform::Array<int>^ defaultValue)
{
	API_LOCK;
	Platform::Array<int>^ range = ref new Platform::Array<int>(2);
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	lp_config_get_range(this->config, ccSection, ccKey, &range[0], &range[1], defaultValue[0], defaultValue[1]);
	delete(ccKey);
	delete(ccSection);
	return range;
}

void Linphone::Native::LpConfig::SetRange(Platform::String^ section, Platform::String^ key, const Platform::Array<int>^ value)
{
	API_LOCK;
	const char *ccSection = Linphone::Native::Utils::pstoccs(section);
	const char *ccKey = Linphone::Native::Utils::pstoccs(key);
	lp_config_set_range(this->config, ccSection, ccKey, value[0], value[1]);
	delete(ccKey);
	delete(ccSection);
}

Linphone::Native::LpConfig::LpConfig(::LpConfig *config) :
	config(config)
{
	lp_config_ref(config);
}

Linphone::Native::LpConfig::LpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath)
{
	API_LOCK;
	const char *ccConfigPath = Linphone::Native::Utils::pstoccs(configPath);
	const char *ccFactoryConfigPath = Linphone::Native::Utils::pstoccs(factoryConfigPath);
	this->config = lp_config_new_with_factory(ccConfigPath, ccFactoryConfigPath);
	delete(ccFactoryConfigPath);
	delete(ccConfigPath);
}

Linphone::Native::LpConfig::~LpConfig()
{
	if (this->config != nullptr) {
		lp_config_unref(this->config);
	}
}