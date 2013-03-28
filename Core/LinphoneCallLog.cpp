#include "LinphoneCallLog.h"
#include "LinphoneAddress.h"
#include "Enums.h"
#include "Server.h"
#include "Globals.h"
#include "LinphoneCoreFactory.h"

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCallLog::GetFrom()
{
	return (Linphone::Core::LinphoneAddress^)Linphone::Core::Utils::CreateLinphoneAddress((void*)linphone_call_log_get_from(this->callLog));
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCallLog::GetTo()
{
	return (Linphone::Core::LinphoneAddress^)Linphone::Core::Utils::CreateLinphoneAddress((void*)linphone_call_log_get_to(this->callLog));
}

Linphone::Core::CallDirection Linphone::Core::LinphoneCallLog::GetDirection()
{
	return (Linphone::Core::CallDirection)linphone_call_log_get_dir(this->callLog);
}

Linphone::Core::LinphoneCallStatus Linphone::Core::LinphoneCallLog::GetStatus()
{
	return (Linphone::Core::LinphoneCallStatus)linphone_call_log_get_status(this->callLog);
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

Linphone::Core::LinphoneCallLog::LinphoneCallLog(::LinphoneCallLog *cl) :
	callLog(cl)
{
	RefToPtrProxy<LinphoneCallLog^> *log = new RefToPtrProxy<LinphoneCallLog^>(this);
	linphone_call_log_set_user_pointer(this->callLog, log);
}

Linphone::Core::LinphoneCallLog::~LinphoneCallLog()
{
	RefToPtrProxy<LinphoneCallLog^> *log = reinterpret_cast< RefToPtrProxy<LinphoneCallLog^> *>(linphone_call_log_get_user_pointer(this->callLog));
	delete log;
}