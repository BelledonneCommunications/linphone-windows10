#include "LinphoneCall.h"
#include "LinphoneAddress.h"
#include "LinphoneCallStats.h"
#include "LinphoneCallLog.h"
#include "LinphoneCallParams.h"
#include "Server.h"
#include "ApiLock.h"
#include "LinphoneCoreFactory.h"
#include "Globals.h"

using namespace Windows::Phone::Networking::Voip;

Linphone::Core::LinphoneCallState Linphone::Core::LinphoneCall::GetState()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCallState state = (Linphone::Core::LinphoneCallState)linphone_call_get_state(this->call);
	gApiLock.Unlock();
	return state;
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCall::GetRemoteAddress()
{
	TRACE; gApiLock.Lock();
	const ::LinphoneAddress *addr = linphone_call_get_remote_address(this->call);
	Linphone::Core::LinphoneAddress^ address = (Linphone::Core::LinphoneAddress^)Linphone::Core::Utils::CreateLinphoneAddress((void*)addr);
	gApiLock.Unlock();
	return address;
}

Linphone::Core::CallDirection Linphone::Core::LinphoneCall::GetDirection()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::CallDirection direction = (Linphone::Core::CallDirection)linphone_call_get_dir(this->call);
	gApiLock.Unlock();
	return direction;
}

Linphone::Core::LinphoneCallLog^ Linphone::Core::LinphoneCall::GetCallLog()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCallLog^ log = (Linphone::Core::LinphoneCallLog^) Linphone::Core::Utils::CreateLinphoneCallLog(linphone_call_get_call_log(this->call));
	gApiLock.Unlock();
	return log;
}

Linphone::Core::LinphoneCallStats^ Linphone::Core::LinphoneCall::GetAudioStats()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCallStats^ stats = (Linphone::Core::LinphoneCallStats^) Linphone::Core::Utils::CreateLinphoneCallStats(this->call, (int)Linphone::Core::MediaType::Audio);
	gApiLock.Unlock();
	return stats;
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCall::GetRemoteParams()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCallParams^ params = nullptr;
	if (linphone_call_get_remote_params(this->call) != nullptr) {
		params = (Linphone::Core::LinphoneCallParams^) Linphone::Core::Utils::CreateLinphoneCallParams(linphone_call_params_copy(linphone_call_get_remote_params(this->call)));
	}
	gApiLock.Unlock();
	return params;
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCall::GetCurrentParamsCopy()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCallParams^ params = (Linphone::Core::LinphoneCallParams^) Linphone::Core::Utils::CreateLinphoneCallParams(linphone_call_params_copy(linphone_call_get_current_params(this->call)));
	gApiLock.Unlock();
	return params;
}

void Linphone::Core::LinphoneCall::EnableEchoCancellation(Platform::Boolean enable)
{
	TRACE; gApiLock.Lock();
	linphone_call_enable_echo_cancellation(this->call, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCall::IsEchoCancellationEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_call_echo_cancellation_enabled(this->call) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

void Linphone::Core::LinphoneCall::EnableEchoLimiter(Platform::Boolean enable)
{
	TRACE; gApiLock.Lock();
	linphone_call_enable_echo_limiter(this->call, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCall::IsEchoLimiterEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_call_echo_limiter_enabled(this->call) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

int Linphone::Core::LinphoneCall::GetDuration()
{
	TRACE; gApiLock.Lock();
	int duration = linphone_call_get_duration(this->call);
	gApiLock.Unlock();
	return duration;
}

float Linphone::Core::LinphoneCall::GetCurrentQuality()
{
	TRACE; gApiLock.Lock();
	float quality = linphone_call_get_current_quality(this->call);
	gApiLock.Unlock();
	return quality;
}

float Linphone::Core::LinphoneCall::GetAverageQuality()
{
	TRACE; gApiLock.Lock();
	float quality = linphone_call_get_average_quality(this->call);
	gApiLock.Unlock();
	return quality;
}

Platform::String^ Linphone::Core::LinphoneCall::GetAuthenticationToken()
{
	TRACE; gApiLock.Lock();
	Platform::String^ token = Linphone::Core::Utils::cctops(linphone_call_get_authentication_token(this->call));
	gApiLock.Unlock();
	return token;
}

Platform::Boolean Linphone::Core::LinphoneCall::IsAuthenticationTokenVerified()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean verified = (linphone_call_get_authentication_token_verified(this->call) == TRUE);
	gApiLock.Unlock();
	return verified;
}

void Linphone::Core::LinphoneCall::SetAuthenticationTokenVerified(Platform::Boolean verified)
{
	TRACE; gApiLock.Lock();
	linphone_call_set_authentication_token_verified(this->call, verified);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCall::IsInConference()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean inConference = (linphone_call_is_in_conference(this->call) == TRUE);
	gApiLock.Unlock();
	return inConference;
}

float Linphone::Core::LinphoneCall::GetPlayVolume()
{
	TRACE; gApiLock.Lock();
	float volume = linphone_call_get_play_volume(this->call);
	gApiLock.Unlock();
	return volume;
}

Platform::String^ Linphone::Core::LinphoneCall::GetRemoteUserAgent()
{
	TRACE; gApiLock.Lock();
	Platform::String^ userAgent = Linphone::Core::Utils::cctops(linphone_call_get_remote_user_agent(this->call));
	gApiLock.Unlock();
	return userAgent;
}

Platform::String^ Linphone::Core::LinphoneCall::GetRemoteContact()
{
	TRACE; gApiLock.Lock();
	Platform::String^ contact = Linphone::Core::Utils::cctops(linphone_call_get_remote_contact(this->call));
	gApiLock.Unlock();
	return contact;
}

void Linphone::Core::LinphoneCall::CallContext::set(Windows::Phone::Networking::Voip::VoipPhoneCall^ cc)
{
	this->callContext = cc;
}

Platform::Object^ Linphone::Core::LinphoneCall::GetCallStartTimeFromContext()
{
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

Platform::Boolean Linphone::Core::LinphoneCall::IsCameraEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_call_camera_enabled(this->call) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

void Linphone::Core::LinphoneCall::EnableCamera(Platform::Boolean enable)
{
	TRACE; gApiLock.Lock();
	linphone_call_enable_camera(this->call, enable);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneCallStats^ Linphone::Core::LinphoneCall::GetVideoStats()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCallStats^ stats = (Linphone::Core::LinphoneCallStats^) Linphone::Core::Utils::CreateLinphoneCallStats(this->call, (int)Linphone::Core::MediaType::Video);
	gApiLock.Unlock();
	return stats;
}

void Linphone::Core::LinphoneCall::SendVFURequest()
{
	TRACE; gApiLock.Lock();
	linphone_call_send_vfu_request(this->call);
	gApiLock.Unlock();
}

Windows::Phone::Networking::Voip::VoipPhoneCall^ Linphone::Core::LinphoneCall::CallContext::get()
{
	return this->callContext;
}

Linphone::Core::LinphoneCall::LinphoneCall(::LinphoneCall *call) :
	call(call)
{
	TRACE; gApiLock.Lock();
	RefToPtrProxy<LinphoneCall^> *proxy = new RefToPtrProxy<LinphoneCall^>(this);
	linphone_call_set_user_data(this->call, proxy);
	this->callContext = nullptr;
	gApiLock.Unlock();
}

Linphone::Core::LinphoneCall::~LinphoneCall()
{
	TRACE; gApiLock.Lock();
	this->callContext = nullptr;
	linphone_call_unref(call);
	RefToPtrProxy<LinphoneCall^> *proxy = reinterpret_cast< RefToPtrProxy<LinphoneCall^> *>(linphone_call_get_user_data(this->call));
	delete proxy;
	gApiLock.Unlock();
}
