#include "LinphoneCallLog.h"
#include "LinphoneAddress.h"
#include "Enums.h"
#include "Server.h"
#include "Globals.h"
#include "LinphoneCoreFactory.h"

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCallLog::From::get()
{
	API_LOCK;
	return (Linphone::Core::LinphoneAddress^)Linphone::Core::Utils::CreateLinphoneAddress((void*)linphone_call_log_get_from(this->callLog));
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCallLog::To::get()
{
	API_LOCK;
	return (Linphone::Core::LinphoneAddress^)Linphone::Core::Utils::CreateLinphoneAddress((void*)linphone_call_log_get_to(this->callLog));
}

Linphone::Core::CallDirection Linphone::Core::LinphoneCallLog::Direction::get()
{
	API_LOCK;
	return (Linphone::Core::CallDirection)linphone_call_log_get_dir(this->callLog);
}

Linphone::Core::LinphoneCallStatus Linphone::Core::LinphoneCallLog::Status::get()
{
	API_LOCK;
	return (Linphone::Core::LinphoneCallStatus)linphone_call_log_get_status(this->callLog);
}

int64 Linphone::Core::LinphoneCallLog::StartDate::get()
{
	API_LOCK;
	return linphone_call_log_get_start_date(this->callLog);
}

int Linphone::Core::LinphoneCallLog::Duration::get()
{
	API_LOCK;
	return linphone_call_log_get_duration(this->callLog);
}

Platform::String^ Linphone::Core::LinphoneCallLog::CallId::get()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_call_log_get_call_id(this->callLog));
}

Platform::Boolean Linphone::Core::LinphoneCallLog::VideoEnabled::get()
{
	API_LOCK;
	return (linphone_call_log_video_enabled(this->callLog) == TRUE);
}

Linphone::Core::LinphoneCallLog::LinphoneCallLog(::LinphoneCallLog *cl) :
	callLog(cl)
{
	API_LOCK;
	RefToPtrProxy<LinphoneCallLog^> *log = new RefToPtrProxy<LinphoneCallLog^>(this);
	linphone_call_log_set_user_pointer(this->callLog, log);
}

Linphone::Core::LinphoneCallLog::~LinphoneCallLog()
{
	API_LOCK;
	RefToPtrProxy<LinphoneCallLog^> *log = reinterpret_cast< RefToPtrProxy<LinphoneCallLog^> *>(linphone_call_log_get_user_pointer(this->callLog));
	delete log;
}