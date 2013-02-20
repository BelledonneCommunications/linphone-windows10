#include "LinphoneCoreFactory.h"
#include "LinphoneCore.h"
#include "LinphoneCoreListener.h"
#include "LinphoneAddress.h"
#include "Server.h"

using namespace Linphone::Core;

void LinphoneCoreFactory::SetDebugMode(Platform::Boolean enable, Platform::String^ tag)
{
}

void LinphoneCoreFactory::CreateLinphoneCore(Linphone::Core::LinphoneCoreListener^ listener, Platform::String^ userConfig, Platform::String^ factoryConfig, Platform::Object^ userData)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 
	this->linphoneCore = ref new Linphone::Core::LinphoneCore(listener);
}

void LinphoneCoreFactory::CreateLinphoneCore(Linphone::Core::LinphoneCoreListener^ listener)
{
	this->linphoneCore = ref new Linphone::Core::LinphoneCore(listener);
}

LinphoneAuthInfo^ LinphoneCoreFactory::CreateAuthInfo(Platform::String^ username, Platform::String^ password, Platform::String^ realm)
{
	return nullptr;
}

LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName)
{
	return ref new LinphoneAddress(username, domain, displayName);
}

LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ address)
{
	return ref new LinphoneAddress(address);
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