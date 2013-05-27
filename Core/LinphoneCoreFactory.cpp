#include "LinphoneAuthInfo.h"
#include "LinphoneCoreFactory.h"
#include "LinphoneCore.h"
#include "LinphoneCoreListener.h"
#include "LinphoneAddress.h"
#include "LpConfig.h"
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


void LinphoneCoreFactory::CreateLinphoneCore(Linphone::Core::LinphoneCoreListener^ listener)
{
	CreateLinphoneCore(listener, nullptr);
}

void LinphoneCoreFactory::CreateLinphoneCore(Linphone::Core::LinphoneCoreListener^ listener, Linphone::Core::LpConfig^ config)
{
	gApiLock.Lock();
	Utils::LinphoneCoreSetLogHandler(LinphoneNativeOutputTraceHandler);
	this->linphoneCore = ref new Linphone::Core::LinphoneCore(listener, config);
	this->linphoneCore->Init();
	gApiLock.Unlock();
}

Linphone::Core::LpConfig^ LinphoneCoreFactory::CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath)
{
	gApiLock.Lock();
	Linphone::Core::LpConfig^ lpConfig = dynamic_cast<Linphone::Core::LpConfig^>(Utils::CreateLpConfig(configPath, factoryConfigPath));
	gApiLock.Unlock();
	return lpConfig;
}

Linphone::Core::LinphoneAuthInfo^ LinphoneCoreFactory::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm)
{
	gApiLock.Lock();
	Linphone::Core::LinphoneAuthInfo^ authInfo = dynamic_cast<Linphone::Core::LinphoneAuthInfo^>(Utils::CreateLinphoneAuthInfo(username, userid, password, ha1, realm));
	gApiLock.Unlock();
	return authInfo;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName)
{
	gApiLock.Lock();
	Linphone::Core::LinphoneAddress^ address = CreateLinphoneAddress("sip:user@domain.com");
	address->SetUserName(username);
	address->SetDomain(domain);
	address->SetDisplayName(displayName);
	gApiLock.Unlock();
	return address;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ uri)
{
	gApiLock.Lock();
	Linphone::Core::LinphoneAddress^ address = dynamic_cast<Linphone::Core::LinphoneAddress^>(Utils::CreateLinphoneAddressFromUri(Utils::pstoccs(uri)));
	gApiLock.Unlock();
	return address;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports()
{
	gApiLock.Lock();
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports());
	gApiLock.Unlock();
	return transports;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports(int udp_port, int tcp_port, int tls_port)
{
	gApiLock.Lock();
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports(udp_port, tcp_port, tls_port));
	gApiLock.Unlock();
	return transports;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports(Linphone::Core::Transports^ t)
{
	gApiLock.Lock();
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports(t));
	gApiLock.Unlock();
	return transports;
}

Linphone::Core::VideoPolicy^ LinphoneCoreFactory::CreateVideoPolicy()
{
	gApiLock.Lock();
	Linphone::Core::VideoPolicy^ policy = dynamic_cast<Linphone::Core::VideoPolicy^>(Utils::CreateVideoPolicy());
	gApiLock.Unlock();
	return policy;
}

Linphone::Core::VideoPolicy^ LinphoneCoreFactory::CreateVideoPolicy(Platform::Boolean automaticallyInitiate, Platform::Boolean automaticallyAccept)
{
	gApiLock.Lock();
	Linphone::Core::VideoPolicy^ policy = dynamic_cast<Linphone::Core::VideoPolicy^>(Utils::CreateVideoPolicy(automaticallyInitiate, automaticallyAccept));
	gApiLock.Unlock();
	return policy;
}

Linphone::Core::VideoSize^ LinphoneCoreFactory::CreateVideoSize(int width, int height)
{
	gApiLock.Lock();
	Linphone::Core::VideoSize^ size = dynamic_cast<Linphone::Core::VideoSize^>(Utils::CreateVideoSize(width, height));
	gApiLock.Unlock();
	return size;
}

Linphone::Core::VideoSize^ LinphoneCoreFactory::CreateVideoSize(int width, int height, Platform::String^ name)
{
	gApiLock.Lock();
	Linphone::Core::VideoSize^ size = dynamic_cast<Linphone::Core::VideoSize^>(Utils::CreateVideoSize(width, height, name));
	gApiLock.Unlock();
	return size;
}

void LinphoneCoreFactory::SetLogLevel(OutputTraceLevel logLevel)
{
	gApiLock.Lock();
	Linphone::Core::LinphoneCore::SetLogLevel(logLevel);
	gApiLock.Unlock();
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