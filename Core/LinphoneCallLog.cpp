#include "LinphoneCallLog.h"
#include "LinphoneAddress.h"
#include "CallDirection.h"
#include "Server.h"

using namespace Linphone::Core;

LinphoneAddress^ LinphoneCallLog::GetFrom()
{
	return nullptr;
}

LinphoneAddress^ LinphoneCallLog::GetTo()
{
	return nullptr;
}

CallDirection LinphoneCallLog::GetDirection()
{
	return CallDirection::Incoming;
}

LinphoneCallStatus LinphoneCallLog::GetStatus()
{
	return LinphoneCallStatus::Aborted;
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
