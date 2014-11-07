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
	API_LOCK;
	Utils::LinphoneCoreEnableLogCollection(LinphoneLogCollectionEnabledWithoutPreviousLogHandler);
	this->linphoneCore = ref new Linphone::Core::LinphoneCore(listener, config);
	this->linphoneCore->Init();
	API_UNLOCK;
}

Linphone::Core::LpConfig^ LinphoneCoreFactory::CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath)
{
	API_LOCK;
	Linphone::Core::LpConfig^ lpConfig = dynamic_cast<Linphone::Core::LpConfig^>(Utils::CreateLpConfig(configPath, factoryConfigPath));
	API_UNLOCK;
	return lpConfig;
}

Linphone::Core::LinphoneAuthInfo^ LinphoneCoreFactory::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
	API_LOCK;
	Linphone::Core::LinphoneAuthInfo^ authInfo = dynamic_cast<Linphone::Core::LinphoneAuthInfo^>(Utils::CreateLinphoneAuthInfo(username, userid, password, ha1, realm, domain));
	API_UNLOCK;
	return authInfo;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName)
{
	API_LOCK;
	Linphone::Core::LinphoneAddress^ address = CreateLinphoneAddress("sip:user@domain.com");
	address->UserName = username;
	address->Domain = domain;
	address->DisplayName = displayName;
	API_UNLOCK;
	return address;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ uri)
{
	API_LOCK;
	Linphone::Core::LinphoneAddress^ address = dynamic_cast<Linphone::Core::LinphoneAddress^>(Utils::CreateLinphoneAddressFromUri(Utils::pstoccs(uri)));
	API_UNLOCK;
	return address;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports()
{
	API_LOCK;
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports());
	API_UNLOCK;
	return transports;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports(int udp_port, int tcp_port, int tls_port)
{
	API_LOCK;
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports(udp_port, tcp_port, tls_port));
	API_UNLOCK;
	return transports;
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports(Linphone::Core::Transports^ t)
{
	API_LOCK;
	Linphone::Core::Transports^ transports = dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports(t));
	API_UNLOCK;
	return transports;
}

Linphone::Core::VideoPolicy^ LinphoneCoreFactory::CreateVideoPolicy()
{
	API_LOCK;
	Linphone::Core::VideoPolicy^ policy = dynamic_cast<Linphone::Core::VideoPolicy^>(Utils::CreateVideoPolicy());
	API_UNLOCK;
	return policy;
}

Linphone::Core::VideoPolicy^ LinphoneCoreFactory::CreateVideoPolicy(Platform::Boolean automaticallyInitiate, Platform::Boolean automaticallyAccept)
{
	API_LOCK;
	Linphone::Core::VideoPolicy^ policy = dynamic_cast<Linphone::Core::VideoPolicy^>(Utils::CreateVideoPolicy(automaticallyInitiate, automaticallyAccept));
	API_UNLOCK;
	return policy;
}

Linphone::Core::VideoSize^ LinphoneCoreFactory::CreateVideoSize(int width, int height)
{
	API_LOCK;
	Linphone::Core::VideoSize^ size = dynamic_cast<Linphone::Core::VideoSize^>(Utils::CreateVideoSize(width, height));
	API_UNLOCK;
	return size;
}

Linphone::Core::VideoSize^ LinphoneCoreFactory::CreateVideoSize(int width, int height, Platform::String^ name)
{
	API_LOCK;
	Linphone::Core::VideoSize^ size = dynamic_cast<Linphone::Core::VideoSize^>(Utils::CreateVideoSize(width, height, name));
	API_UNLOCK;
	return size;
}

void LinphoneCoreFactory::SetLogLevel(OutputTraceLevel logLevel)
{
	API_LOCK;
	Linphone::Core::LinphoneCore::SetLogLevel(logLevel);
	API_UNLOCK;
}

void LinphoneCoreFactory::ResetLogCollection()
{
	API_LOCK;
	Linphone::Core::LinphoneCore::ResetLogCollection();
	API_UNLOCK;
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