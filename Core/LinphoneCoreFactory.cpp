#include "LinphoneCoreFactory.h"
#include "LinphoneCore.h"
#include "LinphoneCoreListener.h"
#include "Server.h"

using namespace Linphone::Core;

void LinphoneCoreFactory::SetDebugMode(Platform::Boolean enable, Platform::String^ tag)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
}

void LinphoneCoreFactory::CreateLinphoneCore(LinphoneCoreListener^ listener, Platform::String^ userConfig, Platform::String^ factoryConfig, Platform::Object^ userData)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 
	this->linphoneCore = ref new Linphone::Core::LinphoneCore();
}

void LinphoneCoreFactory::CreateLinphoneCore(LinphoneCoreListener^ listener)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 
	this->linphoneCore = ref new Linphone::Core::LinphoneCore();
}

LinphoneAuthInfo^ LinphoneCoreFactory::CreateAuthInfo(Platform::String^ username, Platform::String^ password, Platform::String^ realm)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 
	return nullptr;
}

LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 
	return nullptr;
}

LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ address)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 
	return nullptr;
}

LinphoneCore^ LinphoneCoreFactory::LinphoneCore::get()
{
	return this->linphoneCore;
}

LinphoneCoreFactory::LinphoneCoreFactory() :
	linphoneCore(nullptr)
{

}

LinphoneCoreFactory::~LinphoneCoreFactory()
{

}