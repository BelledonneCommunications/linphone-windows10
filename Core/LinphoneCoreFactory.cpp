#include "LinphoneAuthInfo.h"
#include "LinphoneCoreFactory.h"
#include "LinphoneCore.h"
#include "LinphoneCoreListener.h"
#include "LinphoneAddress.h"
#include "LpConfig.h"
#include "Server.h"
#include "Transports.h"
#include "VideoPolicy.h"
#include "VideoSize.h"

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
	Utils::LinphoneCoreEnableLogCollection(true);
	this->linphoneCore = ref new Linphone::Core::LinphoneCore(listener, config);
	this->linphoneCore->Init();
}

Linphone::Core::LpConfig^ LinphoneCoreFactory::CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath)
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::LpConfig^>(Utils::CreateLpConfig(configPath, factoryConfigPath));
}

Linphone::Core::LinphoneAuthInfo^ LinphoneCoreFactory::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::LinphoneAuthInfo^>(Utils::CreateLinphoneAuthInfo(username, userid, password, ha1, realm, domain));
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName)
{
	API_LOCK;
	Linphone::Core::LinphoneAddress^ address = CreateLinphoneAddress("sip:user@domain.com");
	address->UserName = username;
	address->Domain = domain;
	address->DisplayName = displayName;
	return address;
}

Linphone::Core::LinphoneAddress^ LinphoneCoreFactory::CreateLinphoneAddress(Platform::String^ uri)
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::LinphoneAddress^>(Utils::CreateLinphoneAddressFromUri(Utils::pstoccs(uri)));
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports()
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports());
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports(int udp_port, int tcp_port, int tls_port)
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports(udp_port, tcp_port, tls_port));
}

Linphone::Core::Transports^ LinphoneCoreFactory::CreateTransports(Linphone::Core::Transports^ t)
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::Transports^>(Utils::CreateTransports(t));
}

Linphone::Core::VideoPolicy^ LinphoneCoreFactory::CreateVideoPolicy()
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::VideoPolicy^>(Utils::CreateVideoPolicy());
}

Linphone::Core::VideoPolicy^ LinphoneCoreFactory::CreateVideoPolicy(Platform::Boolean automaticallyInitiate, Platform::Boolean automaticallyAccept)
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::VideoPolicy^>(Utils::CreateVideoPolicy(automaticallyInitiate, automaticallyAccept));
}

Linphone::Core::VideoSize^ LinphoneCoreFactory::CreateVideoSize(int width, int height)
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::VideoSize^>(Utils::CreateVideoSize(width, height));
}

Linphone::Core::VideoSize^ LinphoneCoreFactory::CreateVideoSize(int width, int height, Platform::String^ name)
{
	API_LOCK;
	return dynamic_cast<Linphone::Core::VideoSize^>(Utils::CreateVideoSize(width, height, name));
}

void LinphoneCoreFactory::SetLogLevel(OutputTraceLevel logLevel)
{
	API_LOCK;
	Linphone::Core::LinphoneCore::SetLogLevel(logLevel);
}

void LinphoneCoreFactory::ResetLogCollection()
{
	API_LOCK;
	Linphone::Core::LinphoneCore::ResetLogCollection();
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