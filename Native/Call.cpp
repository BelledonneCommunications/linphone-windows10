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


//using namespace Windows::Phone::Networking::Voip;


Linphone::Native::CallStats^ Linphone::Native::Call::AudioStats::get()
{
	API_LOCK;
	return (Linphone::Native::CallStats^) Linphone::Native::Utils::CreateLinphoneCallStats(this->call, (int)Linphone::Native::MediaType::Audio);
}

Platform::String^ Linphone::Native::Call::AuthenticationToken::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_call_get_authentication_token(this->call));
}

Platform::Boolean Linphone::Native::Call::AuthenticationTokenVerified::get()
{
	API_LOCK;
	return (linphone_call_get_authentication_token_verified(this->call) == TRUE);
}

void Linphone::Native::Call::AuthenticationTokenVerified::set(Platform::Boolean verified)
{
	API_LOCK;
	linphone_call_set_authentication_token_verified(this->call, verified);
}

float Linphone::Native::Call::AverageQuality::get()
{
	API_LOCK;
	return linphone_call_get_average_quality(this->call);
}

Linphone::Native::CallLog^ Linphone::Native::Call::CallLog::get()
{
	API_LOCK;
	return (Linphone::Native::CallLog^) Linphone::Native::Utils::CreateLinphoneCallLog(linphone_call_get_call_log(this->call));
}

Platform::Boolean Linphone::Native::Call::CameraEnabled::get()
{
	API_LOCK;
	return (linphone_call_camera_enabled(this->call) == TRUE);
}

void Linphone::Native::Call::CameraEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_camera(this->call, enable);
}

float Linphone::Native::Call::CurrentQuality::get()
{
	API_LOCK;
	return linphone_call_get_current_quality(this->call);
}

Linphone::Native::CallDirection Linphone::Native::Call::Direction::get()
{
	API_LOCK;
	return (Linphone::Native::CallDirection)linphone_call_get_dir(this->call);
}

int Linphone::Native::Call::Duration::get()
{
	API_LOCK;
	return linphone_call_get_duration(this->call);
}

Platform::Boolean Linphone::Native::Call::EchoCancellationEnabled::get()
{
	API_LOCK;
	return (linphone_call_echo_cancellation_enabled(this->call) == TRUE);
}

void Linphone::Native::Call::EchoCancellationEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_echo_cancellation(this->call, enable);
}

Platform::Boolean Linphone::Native::Call::EchoLimiterEnabled::get()
{
	API_LOCK;
	return (linphone_call_echo_limiter_enabled(this->call) == TRUE);
}

void Linphone::Native::Call::EchoLimiterEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_echo_limiter(this->call, enable);
}

Platform::Boolean Linphone::Native::Call::InConference::get()
{
	API_LOCK;
	return (linphone_call_is_in_conference(this->call) == TRUE);
}

float Linphone::Native::Call::PlayVolume::get()
{
	API_LOCK;
	return linphone_call_get_play_volume(this->call);
}

Linphone::Native::Reason Linphone::Native::Call::Reason::get()
{
	API_LOCK;
	return (Linphone::Native::Reason)linphone_call_get_reason(this->call);
}

Platform::String^ Linphone::Native::Call::RemoteContact::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_call_get_remote_contact(this->call));
}

Linphone::Native::Address^ Linphone::Native::Call::RemoteAddress::get()
{
	API_LOCK;
	const ::LinphoneAddress *addr = linphone_call_get_remote_address(this->call);
	return (Linphone::Native::Address^)Linphone::Native::Utils::CreateAddress((void*)addr);
}

Linphone::Native::CallParams^ Linphone::Native::Call::RemoteParams::get()
{
	API_LOCK;
	Linphone::Native::CallParams^ params = nullptr;
	if (linphone_call_get_remote_params(this->call) != nullptr) {
		params = (Linphone::Native::CallParams^) Linphone::Native::Utils::CreateLinphoneCallParams(linphone_call_params_copy(linphone_call_get_remote_params(this->call)));
	}
	return params;
}

Platform::String^ Linphone::Native::Call::RemoteUserAgent::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_call_get_remote_user_agent(this->call));
}

Linphone::Native::CallState Linphone::Native::Call::State::get()
{
	API_LOCK;
	return (Linphone::Native::CallState)linphone_call_get_state(this->call);
}

Linphone::Native::CallStats^ Linphone::Native::Call::VideoStats::get()
{
	API_LOCK;
	return (Linphone::Native::CallStats^) Linphone::Native::Utils::CreateLinphoneCallStats(this->call, (int)Linphone::Native::MediaType::Video);
}

Linphone::Native::CallParams^ Linphone::Native::Call::GetCurrentParamsCopy()
{
	API_LOCK;
	return (Linphone::Native::CallParams^) Linphone::Native::Utils::CreateLinphoneCallParams(linphone_call_params_copy(linphone_call_get_current_params(this->call)));
}

void Linphone::Native::Call::SendVFURequest()
{
	API_LOCK;
	linphone_call_send_vfu_request(this->call);
}

#if 0
void Linphone::Native::Call::CallContext::set(Windows::Phone::Networking::Voip::VoipPhoneCall^ cc)
{
	API_LOCK;
	this->callContext = cc;
}

Platform::Object^ Linphone::Native::Call::CallStartTimeFromContext::get()
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

Windows::Phone::Networking::Voip::VoipPhoneCall^ Linphone::Native::Call::CallContext::get()
{
	return this->callContext;
}
#endif

Linphone::Native::Call::Call(::LinphoneCall *call)
	: call(call)
{
	API_LOCK;
	RefToPtrProxy<Call^> *proxy = new RefToPtrProxy<Call^>(this);
	linphone_call_set_user_data(this->call, proxy);
}

Linphone::Native::Call::~Call()
{
	API_LOCK;
	linphone_call_unref(call);
	RefToPtrProxy<Call^> *proxy = reinterpret_cast< RefToPtrProxy<Call^> *>(linphone_call_get_user_data(this->call));
	delete proxy;
}
