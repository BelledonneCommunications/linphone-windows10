#include "ApiLock.h"
#include "LpConfig.h"
#include "Server.h"

bool Linphone::Core::LpConfig::GetBool(Platform::String^ section, Platform::String^ key, bool defaultValue)
{
	return GetInt(section, key, defaultValue);
}

void Linphone::Core::LpConfig::SetBool(Platform::String^ section, Platform::String^ key, bool value)
{
	SetInt(section, key, (int)value);
}

int Linphone::Core::LpConfig::GetInt(Platform::String^ section, Platform::String^ key, int defaultValue)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	int value = lp_config_get_int(this->config, ccSection, ccKey, defaultValue);
	delete(ccKey);
	delete(ccSection);
	return value;
}

void Linphone::Core::LpConfig::SetInt(Platform::String^ section, Platform::String^ key, int value)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	lp_config_set_int(this->config, ccSection, ccKey, value);
	delete(ccKey);
	delete(ccSection);
}

int Linphone::Core::LpConfig::GetInt64(Platform::String^ section, Platform::String^ key, int64 defaultValue)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	int64 value = lp_config_get_int64(this->config, ccSection, ccKey, defaultValue);
	delete(ccKey);
	delete(ccSection);
	return value;
}

void Linphone::Core::LpConfig::SetInt64(Platform::String^ section, Platform::String^ key, int64 value)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	lp_config_set_int64(this->config, ccSection, ccKey, value);
	delete(ccKey);
	delete(ccSection);
}

float Linphone::Core::LpConfig::GetFloat(Platform::String^ section, Platform::String^ key, float defaultValue)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	float value = lp_config_get_float(this->config, ccSection, ccKey, defaultValue);
	delete(ccKey);
	delete(ccSection);
	return value;
}

void Linphone::Core::LpConfig::SetFloat(Platform::String^ section, Platform::String^ key, float value)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	lp_config_set_float(this->config, ccSection, ccKey, value);
	delete(ccKey);
	delete(ccSection);
}

Platform::String^ Linphone::Core::LpConfig::GetString(Platform::String^ section, Platform::String^ key, Platform::String^ defaultValue)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	const char *ccDefaultValue = Linphone::Core::Utils::pstoccs(defaultValue);
	const char *value = lp_config_get_string(this->config, ccSection, ccKey, ccDefaultValue);
	delete(ccDefaultValue);
	delete(ccKey);
	delete(ccSection);
	return Linphone::Core::Utils::cctops(value);
}

void Linphone::Core::LpConfig::SetString(Platform::String^ section, Platform::String^ key, Platform::String^ value)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	const char *ccValue = Linphone::Core::Utils::pstoccs(value);
	lp_config_set_string(this->config, ccSection, ccKey, ccValue);
	delete(ccValue);
	delete(ccKey);
	delete(ccSection);
}

Platform::Array<int>^ Linphone::Core::LpConfig::GetRange(Platform::String^ section, Platform::String^ key, const Platform::Array<int>^ defaultValue)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	Platform::Array<int>^ range = ref new Platform::Array<int>(2);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	lp_config_get_range(this->config, ccSection, ccKey, &range[0], &range[1], defaultValue[0], defaultValue[1]);
	delete(ccKey);
	delete(ccSection);
	return range;
}

void Linphone::Core::LpConfig::SetRange(Platform::String^ section, Platform::String^ key, const Platform::Array<int>^ value)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	const char *ccSection = Linphone::Core::Utils::pstoccs(section);
	const char *ccKey = Linphone::Core::Utils::pstoccs(key);
	lp_config_set_range(this->config, ccSection, ccKey, value[0], value[1]);
	delete(ccKey);
	delete(ccSection);
}

Linphone::Core::LpConfig::LpConfig(::LpConfig *config) :
	config(config)
{

}

Linphone::Core::LpConfig::~LpConfig()
{

}