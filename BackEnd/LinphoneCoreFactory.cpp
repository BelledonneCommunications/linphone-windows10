#include "LinphoneCoreFactory.h"

using namespace Linphone::BackEnd;

void LinphoneCoreFactory::CreateLinphoneCore()
{
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