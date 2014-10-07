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

void LinphoneCoreFactory::CreateLinphoneCore(Linphone::Core::LinphoneCoreListener^ listener)
{
	CreateLinphoneCore(listener, nullptr);
}

void LinphoneCoreFactory::CreateLinphoneCore(Linphone::Core::LinphoneCoreListener^ listener, Linphone::Core::LpConfig^ config)
{
	TRACE; gApiLock.Lock();
	Utils::LinphoneCoreEnableLogCollection(true);
	this->linphoneCore = ref new Linphone::Core::LinphoneCore(listener, config);
	this->linphoneCore->Init();
	gApiLock.Unlock();
}

Linphone::Core::LpConfig^ LinphoneCoreFactory::CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LpConfig^ lpConfig = dynamic_cast<Linphone::Core::LpConfig^>(Utils::CreateLpConfig(configPath, factoryConfigPath));
	gApiLock.Unlock();
	return lpConfig;
}

Linphone::Core::LinphoneAuthInfo^ LinphoneCoreFactory::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneAuthInfo^ authInfo = dynamic_cast<Linphone::Core::LinphoneAuthInfo^>(Utils::CreateLinphoneAuthInfo(username, userid, password, ha1, realm, domain));
	gApiLock.Unlock();
	return authInfo;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneAddress^ address = CreateLinphoneAddress("sip:user@domain.com");
	address->SetUserName(username);
	address->SetDomain(domain);
	address->SetDisplayName(displayName);
	gApiLock.Unlock();
	return address;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ uri)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneAddress^ address = dynamic_cast<Linphone::Core::LinphoneAddress^>(Utils::CreateLinphoneAddressFromUri(Utils::pstoccs(uri)));
	gApiLock.Unlock();
	return address;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports());
	gApiLock.Unlock();
	return transports;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports(int udp_port, int tcp_port, int tls_port)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports(udp_port, tcp_port, tls_port));
	gApiLock.Unlock();
	return transports;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports(Linphone::Core::Transports^ t)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports(t));
	gApiLock.Unlock();
	return transports;
}

Linphone::Core::VideoPolicy^ LinphoneCoreFactory::CreateVideoPolicy()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::VideoPolicy^ policy = dynamic_cast<Linphone::Core::VideoPolicy^>(Utils::CreateVideoPolicy());
	gApiLock.Unlock();
	return policy;
}

Linphone::Core::VideoPolicy^ LinphoneCoreFactory::CreateVideoPolicy(Platform::Boolean automaticallyInitiate, Platform::Boolean automaticallyAccept)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::VideoPolicy^ policy = dynamic_cast<Linphone::Core::VideoPolicy^>(Utils::CreateVideoPolicy(automaticallyInitiate, automaticallyAccept));
	gApiLock.Unlock();
	return policy;
}

Linphone::Core::VideoSize^ LinphoneCoreFactory::CreateVideoSize(int width, int height)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::VideoSize^ size = dynamic_cast<Linphone::Core::VideoSize^>(Utils::CreateVideoSize(width, height));
	gApiLock.Unlock();
	return size;
}

Linphone::Core::VideoSize^ LinphoneCoreFactory::CreateVideoSize(int width, int height, Platform::String^ name)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::VideoSize^ size = dynamic_cast<Linphone::Core::VideoSize^>(Utils::CreateVideoSize(width, height, name));
	gApiLock.Unlock();
	return size;
}

void LinphoneCoreFactory::SetLogLevel(OutputTraceLevel logLevel)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCore::SetLogLevel(logLevel);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneCore^ LinphoneCoreFactory::LinphoneCore::get()
{
	return this->linphoneCore;
}

void LinphoneCoreFactory::Destroy()
{
	this->linphoneCore->Destroy();
	delete this->linphoneCore;
	this->linphoneCore = nullptr;
}

LinphoneCoreFactory::LinphoneCoreFactory() :
	linphoneCore(nullptr)
{

}

LinphoneCoreFactory::~LinphoneCoreFactory()
{
	
}