#include "LinphoneCoreFactory.h"
#include "LinphoneCore.h"
#include "LinphoneCoreListener.h"
#include "LinphoneAddress.h"
#include "Server.h"

using namespace Linphone::Core;
using namespace Platform;

#define MAX_TRACE_SIZE		2048
#define MAX_SUITE_NAME_SIZE	128

static void nativeOutputTraceHandler(OutputTraceLevel lev, const char *fmt, va_list args)
{
	if (Globals::Instance->LinphoneCoreFactory->OutputTraceListener) {
		wchar_t wstr[MAX_TRACE_SIZE];
		std::string str;
		str.resize(MAX_TRACE_SIZE);
		size_t len = vsnprintf((char *)str.c_str(), MAX_TRACE_SIZE, fmt, args);
		if (len >= MAX_TRACE_SIZE) ((char *)str.c_str())[MAX_TRACE_SIZE - 1] = '\0';
		mbstowcs(wstr, str.c_str(), sizeof(wstr));
		String^ msg = ref new String(wstr);
		Globals::Instance->LinphoneCoreFactory->OutputTraceListener->OutputTrace(lev, msg);
	}
}

static void LinphoneNativeOutputTraceHandler(OrtpLogLevel lev, const char *fmt, va_list args)
{
	OutputTraceLevel level = OutputTraceLevel::Message;
	char fmt2[MAX_TRACE_SIZE];
	snprintf(fmt2, MAX_TRACE_SIZE, "%s\n", fmt);
	if (lev == ORTP_DEBUG) level = OutputTraceLevel::Debug;
	else if (lev == ORTP_MESSAGE) level = OutputTraceLevel::Message;
	else if (lev == ORTP_TRACE) level = OutputTraceLevel::Message;
	else if (lev == ORTP_WARNING) level = OutputTraceLevel::Warning;
	else if (lev == ORTP_ERROR) level = OutputTraceLevel::Error;
	else if (lev == ORTP_FATAL) level = OutputTraceLevel::Error;
	nativeOutputTraceHandler(level, fmt2, args);
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
	return nullptr;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ uri)
{
	return (Linphone::Core::LinphoneAddress^)Utils::CreateLinphoneAddressFromUri(Utils::pstoccs(uri));
}

Linphone::Core::LinphoneCore^ LinphoneCoreFactory::LinphoneCore::get()
{
	return this->linphoneCore;
}

Linphone::Core::OutputTraceListener^ LinphoneCoreFactory::OutputTraceListener::get()
{
	return this->outputTraceListener;
}

void LinphoneCoreFactory::OutputTraceListener::set(Linphone::Core::OutputTraceListener^ listener)
{
	this->outputTraceListener = listener;
}

LinphoneCoreFactory::LinphoneCoreFactory() :
	linphoneCore(nullptr)
{

}

LinphoneCoreFactory::~LinphoneCoreFactory()
{

}