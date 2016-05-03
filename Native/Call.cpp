/*
Call.cpp
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
#include "Call.h"
#include "CallLog.h"
#include "CallParams.h"
#include "CallStats.h"


using namespace BelledonneCommunications::Linphone::Native;
//using namespace Windows::Phone::Networking::Voip;


CallStats^ Call::AudioStats::get()
{
	API_LOCK;
	return (CallStats^) Utils::CreateCallStats(this->call, (int)MediaType::Audio);
}

Platform::String^ Call::AuthenticationToken::get()
{
	API_LOCK;
	return Utils::cctops(linphone_call_get_authentication_token(this->call));
}

Platform::Boolean Call::AuthenticationTokenVerified::get()
{
	API_LOCK;
	return (linphone_call_get_authentication_token_verified(this->call) == TRUE);
}

void Call::AuthenticationTokenVerified::set(Platform::Boolean verified)
{
	API_LOCK;
	linphone_call_set_authentication_token_verified(this->call, verified);
}

float Call::AverageQuality::get()
{
	API_LOCK;
	return linphone_call_get_average_quality(this->call);
}

CallLog^ Call::CallLog::get()
{
	API_LOCK;
	return (BelledonneCommunications::Linphone::Native::CallLog^) Utils::GetCallLog(linphone_call_get_call_log(this->call));
}

Platform::Boolean Call::CameraEnabled::get()
{
	API_LOCK;
	return (linphone_call_camera_enabled(this->call) == TRUE);
}

void Call::CameraEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_camera(this->call, enable);
}

CallParams^ Call::CurrentParams::get()
{
	API_LOCK;
	return (CallParams^) Utils::GetCallParams((void *)linphone_call_get_current_params(this->call));
}

float Call::CurrentQuality::get()
{
	API_LOCK;
	return linphone_call_get_current_quality(this->call);
}

CallDirection Call::Direction::get()
{
	API_LOCK;
	return (CallDirection)linphone_call_get_dir(this->call);
}

int Call::Duration::get()
{
	API_LOCK;
	return linphone_call_get_duration(this->call);
}

Platform::Boolean Call::EchoCancellationEnabled::get()
{
	API_LOCK;
	return (linphone_call_echo_cancellation_enabled(this->call) == TRUE);
}

void Call::EchoCancellationEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_echo_cancellation(this->call, enable);
}

Platform::Boolean Call::EchoLimiterEnabled::get()
{
	API_LOCK;
	return (linphone_call_echo_limiter_enabled(this->call) == TRUE);
}

void Call::EchoLimiterEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_echo_limiter(this->call, enable);
}

Platform::Boolean Call::IsInConference::get()
{
	API_LOCK;
	return (linphone_call_is_in_conference(this->call) == TRUE);
}

Platform::Boolean Call::MediaInProgress::get()
{
	API_LOCK;
	return (linphone_call_media_in_progress(this->call) == TRUE);
}

Platform::Object^ Call::NativeVideoWindowId::get()
{
	API_LOCK;
	void *id = linphone_call_get_native_video_window_id(this->call);
	if (id == NULL) return nullptr;
	RefToPtrProxy<Platform::Object^> *proxy = reinterpret_cast<RefToPtrProxy<Platform::Object^>*>(id);
	Platform::Object^ nativeWindowId = (proxy) ? proxy->Ref() : nullptr;
	return nativeWindowId;
}

void Call::NativeVideoWindowId::set(Platform::Object^ value)
{
	API_LOCK;
	RefToPtrProxy<Platform::Object^> *nativeWindowId = new RefToPtrProxy<Platform::Object^>(value);
	linphone_call_set_native_video_window_id(this->call, nativeWindowId);
}

float Call::PlayVolume::get()
{
	API_LOCK;
	return linphone_call_get_play_volume(this->call);
}

Reason Call::Reason::get()
{
	API_LOCK;
	return (BelledonneCommunications::Linphone::Native::Reason)linphone_call_get_reason(this->call);
}

Platform::String^ Call::RemoteContact::get()
{
	API_LOCK;
	return Utils::cctops(linphone_call_get_remote_contact(this->call));
}

Address^ Call::RemoteAddress::get()
{
	API_LOCK;
	const ::LinphoneAddress *addr = linphone_call_get_remote_address(this->call);
	return (Address^)Utils::CreateAddress((void*)addr);
}

CallParams^ Call::RemoteParams::get()
{
	API_LOCK;
	return (CallParams^) Utils::GetCallParams((void *)linphone_call_get_remote_params(this->call));
}

Platform::String^ Call::RemoteUserAgent::get()
{
	API_LOCK;
	return Utils::cctops(linphone_call_get_remote_user_agent(this->call));
}

CallState Call::State::get()
{
	API_LOCK;
	return (CallState)linphone_call_get_state(this->call);
}

CallStats^ Call::VideoStats::get()
{
	API_LOCK;
	return (CallStats^) Utils::CreateCallStats(this->call, (int)MediaType::Video);
}

void Call::SendVFURequest()
{
	API_LOCK;
	linphone_call_send_vfu_request(this->call);
}

#if 0
void Call::CallContext::set(Windows::Phone::Networking::Voip::VoipPhoneCall^ cc)
{
	API_LOCK;
	this->callContext = cc;
}

Platform::Object^ Call::CallStartTimeFromContext::get()
{
	API_LOCK;
	Platform::Object^ result = nullptr;
	try {
		if (this->callContext != nullptr) {
			result = this->callContext->StartTime;
		}
	}
	catch (Platform::COMException^ ex) {
	}
	return result;
}

Windows::Phone::Networking::Voip::VoipPhoneCall^ Call::CallContext::get()
{
	return this->callContext;
}
#endif

Call::Call(::LinphoneCall *call)
	: call(call)
{
	API_LOCK;
	RefToPtrProxy<Call^> *proxy = new RefToPtrProxy<Call^>(this);
	linphone_call_ref(this->call);
	linphone_call_set_user_data(this->call, proxy);
}

Call::~Call()
{
	API_LOCK;
	linphone_call_unref(call);
	RefToPtrProxy<Call^> *proxy = reinterpret_cast< RefToPtrProxy<Call^> *>(linphone_call_get_user_data(this->call));
	delete proxy;
}
