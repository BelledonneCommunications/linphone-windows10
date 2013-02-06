#include "LinphoneCoreFactory.h"
#include "LinphoneCore.h"
#include "Server.h"

using namespace Linphone::BackEnd;

void LinphoneCoreFactory::CreateLinphoneCore()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 

	this->linphoneCore = ref new Linphone::BackEnd::LinphoneCore();
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