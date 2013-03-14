#include "LinphoneCoreFactory.h"
#include "LinphoneCore.h"
#include "LinphoneCoreListener.h"
#include "LinphoneAddress.h"
#include "Server.h"

using namespace Linphone::Core;
using namespace Platform;

#define MAX_TRACE_SIZE		1024
#define MAX_SUITE_NAME_SIZE	128

static OutputTraceListener^ sTraceListener;

static void nativeOutputTraceHandler(int lev, const char *fmt, va_list args)
{
	if (sTraceListener) {
		wchar_t wstr[MAX_TRACE_SIZE];
		std::string str;
		str.resize(MAX_TRACE_SIZE);
		vsnprintf((char *)str.c_str(), MAX_TRACE_SIZE, fmt, args);
		mbstowcs(wstr, str.c_str(), sizeof(wstr));
		String^ msg = ref new String(wstr);
		sTraceListener->OutputTrace(lev, msg);
	}
}

static void LinphoneNativeOutputTraceHandler(OrtpLogLevel lev, const char *fmt, va_list args)
{
	char fmt2[MAX_TRACE_SIZE];
	snprintf(fmt2, MAX_TRACE_SIZE, "%s\n", fmt);
	nativeOutputTraceHandler((int)lev, fmt2, args);
}

void LinphoneCoreFactory::SetDebugMode(Platform::Boolean enable, OutputTraceListener^ traceListener)
{
	sTraceListener = traceListener;
}

void LinphoneCoreFactory::CreateLinphoneCore(Linphone::Core::LinphoneCoreListener^ listener, Platform::String^ userConfig, Platform::String^ factoryConfig, Platform::Object^ userData)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 
	linphone_core_enable_logs_with_cb(LinphoneNativeOutputTraceHandler);
	this->linphoneCore = ref new Linphone::Core::LinphoneCore(listener);
	this->linphoneCore->Init();
}

void LinphoneCoreFactory::CreateLinphoneCore(Linphone::Core::LinphoneCoreListener^ listener)
{
	LinphoneCoreFactory::CreateLinphoneCore(listener, nullptr, nullptr, nullptr);
}

Linphone::Core::LinphoneAuthInfo^ LinphoneCoreFactory::CreateAuthInfo(Platform::String^ username, Platform::String^ password, Platform::String^ realm)
{
	return nullptr;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName)
{
	return ref new LinphoneAddress(username, domain, displayName);
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ address)
{
	return ref new LinphoneAddress(address);
}

Linphone::Core::LinphoneCore^ LinphoneCoreFactory::LinphoneCore::get()
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