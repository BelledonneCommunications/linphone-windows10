#include "LinphoneCallLog.h"
#include "LinphoneAddress.h"
#include "Enums.h"
#include "Server.h"
#include "Globals.h"
#include "LinphoneCoreFactory.h"

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCallLog::GetFrom()
{
	return this->from;
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCallLog::GetTo()
{
	return this->to;
}

Linphone::Core::CallDirection Linphone::Core::LinphoneCallLog::GetDirection()
{
	return this->direction;
}

Linphone::Core::LinphoneCallStatus Linphone::Core::LinphoneCallLog::GetStatus()
{
	return this->status;
}

Platform::String^ Linphone::Core::LinphoneCallLog::GetStartDate()
{
	return nullptr;
}

int64 Linphone::Core::LinphoneCallLog::GetTimestamp()
{
	return -1;
}

int Linphone::Core::LinphoneCallLog::GetCallDuration()
{
	return -1;
}

int Linphone::Core::LinphoneCallLog::GetCallId()
{
	return -1;
}

Linphone::Core::LinphoneCallLog::LinphoneCallLog(Platform::String^ from, Platform::String^ to, Linphone::Core::LinphoneCallStatus status, Linphone::Core::CallDirection direction) :
	status(status),
	direction(direction)
{
	LinphoneCoreFactory^ lcf = Globals::Instance->LinphoneCoreFactory;
	this->from = lcf->CreateLinphoneAddress(from);
	this->to = lcf->CreateLinphoneAddress(to);
}

Linphone::Core::LinphoneCallLog::~LinphoneCallLog()
{

}