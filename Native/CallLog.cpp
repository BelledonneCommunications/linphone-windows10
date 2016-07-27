/*
CallLog.cpp
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#include "Address.h"
#include "ApiLock.h"
#include "CallLog.h"
#include "Enums.h"

using namespace BelledonneCommunications::Linphone::Native;

Platform::String^ CallLog::CallId::get()
{
	API_LOCK;
	return Utils::cctops(linphone_call_log_get_call_id(this->callLog));
}

CallDirection CallLog::Direction::get()
{
	API_LOCK;
	return (CallDirection)linphone_call_log_get_dir(this->callLog);
}

int CallLog::Duration::get()
{
	API_LOCK;
	return linphone_call_log_get_duration(this->callLog);
}

Address^ CallLog::FromAddress::get()
{
	API_LOCK;
	return (Address^)Utils::CreateAddress((void*)linphone_call_log_get_from_address(this->callLog));
}

Platform::Boolean CallLog::IsVideoEnabled::get()
{
	API_LOCK;
	return (linphone_call_log_video_enabled(this->callLog) == TRUE);
}

int64 CallLog::StartDate::get()
{
	API_LOCK;
	return linphone_call_log_get_start_date(this->callLog);
}

CallStatus CallLog::Status::get()
{
	API_LOCK;
	return (CallStatus)linphone_call_log_get_status(this->callLog);
}

Address^ CallLog::ToAddress::get()
{
	API_LOCK;
	return (Address^)Utils::CreateAddress((void*)linphone_call_log_get_to_address(this->callLog));
}

CallLog::CallLog(::LinphoneCallLog *cl)
	: callLog(cl)
{
	API_LOCK;
	RefToPtrProxy<CallLog^> *log = new RefToPtrProxy<CallLog^>(this);
	linphone_call_log_ref(this->callLog);
	linphone_call_log_set_user_data(this->callLog, log);
}

CallLog::~CallLog()
{
	API_LOCK;
	if (this->callLog != nullptr) {
		linphone_call_log_unref(this->callLog);
	}
	RefToPtrProxy<CallLog^> *log = reinterpret_cast<RefToPtrProxy<CallLog^> *>(linphone_call_log_get_user_data(this->callLog));
	delete log;
}
