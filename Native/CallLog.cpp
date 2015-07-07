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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

#include "Address.h"
#include "ApiLock.h"
#include "CallLog.h"
#include "Enums.h"


Platform::String^ Linphone::Native::CallLog::CallId::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_call_log_get_call_id(this->callLog));
}

Linphone::Native::CallDirection Linphone::Native::CallLog::Direction::get()
{
	API_LOCK;
	return (Linphone::Native::CallDirection)linphone_call_log_get_dir(this->callLog);
}

int Linphone::Native::CallLog::Duration::get()
{
	API_LOCK;
	return linphone_call_log_get_duration(this->callLog);
}

Linphone::Native::Address^ Linphone::Native::CallLog::From::get()
{
	API_LOCK;
	return (Linphone::Native::Address^)Linphone::Native::Utils::CreateAddress((void*)linphone_call_log_get_from(this->callLog));
}

int64 Linphone::Native::CallLog::StartDate::get()
{
	API_LOCK;
	return linphone_call_log_get_start_date(this->callLog);
}

Linphone::Native::CallStatus Linphone::Native::CallLog::Status::get()
{
	API_LOCK;
	return (Linphone::Native::CallStatus)linphone_call_log_get_status(this->callLog);
}

Linphone::Native::Address^ Linphone::Native::CallLog::To::get()
{
	API_LOCK;
	return (Linphone::Native::Address^)Linphone::Native::Utils::CreateAddress((void*)linphone_call_log_get_to(this->callLog));
}

Platform::Boolean Linphone::Native::CallLog::VideoEnabled::get()
{
	API_LOCK;
	return (linphone_call_log_video_enabled(this->callLog) == TRUE);
}

Linphone::Native::CallLog::CallLog(::LinphoneCallLog *cl)
	: callLog(cl)
{
	API_LOCK;
	RefToPtrProxy<CallLog^> *log = new RefToPtrProxy<CallLog^>(this);
	linphone_call_log_set_user_pointer(this->callLog, log);
}

Linphone::Native::CallLog::~CallLog()
{
	API_LOCK;
	RefToPtrProxy<CallLog^> *log = reinterpret_cast<RefToPtrProxy<CallLog^> *>(linphone_call_log_get_user_pointer(this->callLog));
	delete log;
}
