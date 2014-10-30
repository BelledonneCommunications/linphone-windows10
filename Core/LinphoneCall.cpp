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
	API_LOCK;
	Linphone::Core::LinphoneCallState state = (Linphone::Core::LinphoneCallState)linphone_call_get_state(this->call);
	API_UNLOCK;
	return state;
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCall::GetRemoteAddress()
{
	API_LOCK;
	const ::LinphoneAddress *addr = linphone_call_get_remote_address(this->call);
	Linphone::Core::LinphoneAddress^ address = (Linphone::Core::LinphoneAddress^)Linphone::Core::Utils::CreateLinphoneAddress((void*)addr);
	API_UNLOCK;
	return address;
}

Linphone::Core::CallDirection Linphone::Core::LinphoneCall::GetDirection()
{
	API_LOCK;
	Linphone::Core::CallDirection direction = (Linphone::Core::CallDirection)linphone_call_get_dir(this->call);
	API_UNLOCK;
	return direction;
}

Linphone::Core::LinphoneCallLog^ Linphone::Core::LinphoneCall::GetCallLog()
{
	API_LOCK;
	Linphone::Core::LinphoneCallLog^ log = (Linphone::Core::LinphoneCallLog^) Linphone::Core::Utils::CreateLinphoneCallLog(linphone_call_get_call_log(this->call));
	API_UNLOCK;
	return log;
}

Linphone::Core::LinphoneCallStats^ Linphone::Core::LinphoneCall::GetAudioStats()
{
	API_LOCK;
	Linphone::Core::LinphoneCallStats^ stats = (Linphone::Core::LinphoneCallStats^) Linphone::Core::Utils::CreateLinphoneCallStats(this->call, (int)Linphone::Core::MediaType::Audio);
	API_UNLOCK;
	return stats;
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCall::GetRemoteParams()
{
	API_LOCK;
	Linphone::Core::LinphoneCallParams^ params = nullptr;
	if (linphone_call_get_remote_params(this->call) != nullptr) {
		params = (Linphone::Core::LinphoneCallParams^) Linphone::Core::Utils::CreateLinphoneCallParams(linphone_call_params_copy(linphone_call_get_remote_params(this->call)));
	}
	API_UNLOCK;
	return params;
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCall::GetCurrentParamsCopy()
{
	API_LOCK;
	Linphone::Core::LinphoneCallParams^ params = (Linphone::Core::LinphoneCallParams^) Linphone::Core::Utils::CreateLinphoneCallParams(linphone_call_params_copy(linphone_call_get_current_params(this->call)));
	API_UNLOCK;
	return params;
}

void Linphone::Core::LinphoneCall::EnableEchoCancellation(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_echo_cancellation(this->call, enable);
	API_UNLOCK;
}

Platform::Boolean Linphone::Core::LinphoneCall::IsEchoCancellationEnabled()
{
	API_LOCK;
	Platform::Boolean enabled = (linphone_call_echo_cancellation_enabled(this->call) == TRUE);
	API_UNLOCK;
	return enabled;
}

void Linphone::Core::LinphoneCall::EnableEchoLimiter(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_echo_limiter(this->call, enable);
	API_UNLOCK;
}

Platform::Boolean Linphone::Core::LinphoneCall::IsEchoLimiterEnabled()
{
	API_LOCK;
	Platform::Boolean enabled = (linphone_call_echo_limiter_enabled(this->call) == TRUE);
	API_UNLOCK;
	return enabled;
}

int Linphone::Core::LinphoneCall::GetDuration()
{
	API_LOCK;
	int duration = linphone_call_get_duration(this->call);
	API_UNLOCK;
	return duration;
}

float Linphone::Core::LinphoneCall::GetCurrentQuality()
{
	API_LOCK;
	float quality = linphone_call_get_current_quality(this->call);
	API_UNLOCK;
	return quality;
}

float Linphone::Core::LinphoneCall::GetAverageQuality()
{
	API_LOCK;
	float quality = linphone_call_get_average_quality(this->call);
	API_UNLOCK;
	return quality;
}

Platform::String^ Linphone::Core::LinphoneCall::GetAuthenticationToken()
{
	API_LOCK;
	Platform::String^ token = Linphone::Core::Utils::cctops(linphone_call_get_authentication_token(this->call));
	API_UNLOCK;
	return token;
}

Platform::Boolean Linphone::Core::LinphoneCall::IsAuthenticationTokenVerified()
{
	API_LOCK;
	Platform::Boolean verified = (linphone_call_get_authentication_token_verified(this->call) == TRUE);
	API_UNLOCK;
	return verified;
}

void Linphone::Core::LinphoneCall::SetAuthenticationTokenVerified(Platform::Boolean verified)
{
	API_LOCK;
	linphone_call_set_authentication_token_verified(this->call, verified);
	API_UNLOCK;
}

Platform::Boolean Linphone::Core::LinphoneCall::IsInConference()
{
	API_LOCK;
	Platform::Boolean inConference = (linphone_call_is_in_conference(this->call) == TRUE);
	API_UNLOCK;
	return inConference;
}

float Linphone::Core::LinphoneCall::GetPlayVolume()
{
	API_LOCK;
	float volume = linphone_call_get_play_volume(this->call);
	API_UNLOCK;
	return volume;
}

Platform::String^ Linphone::Core::LinphoneCall::GetRemoteUserAgent()
{
	API_LOCK;
	Platform::String^ userAgent = Linphone::Core::Utils::cctops(linphone_call_get_remote_user_agent(this->call));
	API_UNLOCK;
	return userAgent;
}

Platform::String^ Linphone::Core::LinphoneCall::GetRemoteContact()
{
	API_LOCK;
	Platform::String^ contact = Linphone::Core::Utils::cctops(linphone_call_get_remote_contact(this->call));
	API_UNLOCK;
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
	API_LOCK;
	Platform::Boolean enabled = (linphone_call_camera_enabled(this->call) == TRUE);
	API_UNLOCK;
	return enabled;
}

void Linphone::Core::LinphoneCall::EnableCamera(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_camera(this->call, enable);
	API_UNLOCK;
}

Linphone::Core::LinphoneCallStats^ Linphone::Core::LinphoneCall::GetVideoStats()
{
	API_LOCK;
	Linphone::Core::LinphoneCallStats^ stats = (Linphone::Core::LinphoneCallStats^) Linphone::Core::Utils::CreateLinphoneCallStats(this->call, (int)Linphone::Core::MediaType::Video);
	API_UNLOCK;
	return stats;
}

void Linphone::Core::LinphoneCall::SendVFURequest()
{
	API_LOCK;
	linphone_call_send_vfu_request(this->call);
	API_UNLOCK;
}

Windows::Phone::Networking::Voip::VoipPhoneCall^ Linphone::Core::LinphoneCall::CallContext::get()
{
	return this->callContext;
}

Linphone::Core::Reason Linphone::Core::LinphoneCall::Reason::get()
{
	return (Linphone::Core::Reason)linphone_call_get_reason(this->call);
}

Linphone::Core::LinphoneCall::LinphoneCall(::LinphoneCall *call) :
	call(call)
{
	API_LOCK;
	RefToPtrProxy<LinphoneCall^> *proxy = new RefToPtrProxy<LinphoneCall^>(this);
	linphone_call_set_user_data(this->call, proxy);
	API_UNLOCK;
}

Linphone::Core::LinphoneCall::~LinphoneCall()
{
	API_LOCK;
	linphone_call_unref(call);
	RefToPtrProxy<LinphoneCall^> *proxy = reinterpret_cast< RefToPtrProxy<LinphoneCall^> *>(linphone_call_get_user_data(this->call));
	delete proxy;
	API_UNLOCK;
}
