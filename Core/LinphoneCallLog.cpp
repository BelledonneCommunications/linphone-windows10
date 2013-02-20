#include "LinphoneCallLog.h"
#include "LinphoneAddress.h"
#include "Enums.h"
#include "Server.h"
#include "Globals.h"
#include "LinphoneCoreFactory.h"

using namespace Linphone::Core;

LinphoneAddress^ LinphoneCallLog::GetFrom()
{
	return this->from;
}

LinphoneAddress^ LinphoneCallLog::GetTo()
{
	return this->to;
}

CallDirection LinphoneCallLog::GetDirection()
{
	return this->direction;
}

LinphoneCallStatus LinphoneCallLog::GetStatus()
{
	return this->status;
}

Platform::String^ LinphoneCallLog::GetStartDate()
{
	return nullptr;
}

int64 LinphoneCallLog::GetTimestamp()
{
	return -1;
}

int LinphoneCallLog::GetCallDuration()
{
	return -1;
}

int LinphoneCallLog::GetCallId()
{
	return -1;
}

LinphoneCallLog::LinphoneCallLog(Platform::String^ from, Platform::String^ to, LinphoneCallStatus status, CallDirection direction) :
	status(status),
	direction(direction)
{
	LinphoneCoreFactory^ lcf = Globals::Instance->LinphoneCoreFactory;
	this->from = lcf->CreateLinphoneAddress(from);
	this->to = lcf->CreateLinphoneAddress(to);
}

LinphoneCallLog::~LinphoneCallLog()
{

}