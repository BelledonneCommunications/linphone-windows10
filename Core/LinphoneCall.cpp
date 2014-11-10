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
	return (Linphone::Core::LinphoneCallState)linphone_call_get_state(this->call);
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCall::GetRemoteAddress()
{
	API_LOCK;
	const ::LinphoneAddress *addr = linphone_call_get_remote_address(this->call);
	return (Linphone::Core::LinphoneAddress^)Linphone::Core::Utils::CreateLinphoneAddress((void*)addr);
}

Linphone::Core::CallDirection Linphone::Core::LinphoneCall::GetDirection()
{
	API_LOCK;
	return (Linphone::Core::CallDirection)linphone_call_get_dir(this->call);
}

Linphone::Core::LinphoneCallLog^ Linphone::Core::LinphoneCall::GetCallLog()
{
	API_LOCK;
	return (Linphone::Core::LinphoneCallLog^) Linphone::Core::Utils::CreateLinphoneCallLog(linphone_call_get_call_log(this->call));
}

Linphone::Core::LinphoneCallStats^ Linphone::Core::LinphoneCall::GetAudioStats()
{
	API_LOCK;
	return (Linphone::Core::LinphoneCallStats^) Linphone::Core::Utils::CreateLinphoneCallStats(this->call, (int)Linphone::Core::MediaType::Audio);
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCall::GetRemoteParams()
{
	API_LOCK;
	Linphone::Core::LinphoneCallParams^ params = nullptr;
	if (linphone_call_get_remote_params(this->call) != nullptr) {
		params = (Linphone::Core::LinphoneCallParams^) Linphone::Core::Utils::CreateLinphoneCallParams(linphone_call_params_copy(linphone_call_get_remote_params(this->call)));
	}
	return params;
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCall::GetCurrentParamsCopy()
{
	API_LOCK;
	return (Linphone::Core::LinphoneCallParams^) Linphone::Core::Utils::CreateLinphoneCallParams(linphone_call_params_copy(linphone_call_get_current_params(this->call)));
}

void Linphone::Core::LinphoneCall::EnableEchoCancellation(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_echo_cancellation(this->call, enable);
}

Platform::Boolean Linphone::Core::LinphoneCall::IsEchoCancellationEnabled()
{
	API_LOCK;
	return (linphone_call_echo_cancellation_enabled(this->call) == TRUE);
}

void Linphone::Core::LinphoneCall::EnableEchoLimiter(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_echo_limiter(this->call, enable);
}

Platform::Boolean Linphone::Core::LinphoneCall::IsEchoLimiterEnabled()
{
	API_LOCK;
	return (linphone_call_echo_limiter_enabled(this->call) == TRUE);
}

int Linphone::Core::LinphoneCall::GetDuration()
{
	API_LOCK;
	return linphone_call_get_duration(this->call);
}

float Linphone::Core::LinphoneCall::GetCurrentQuality()
{
	API_LOCK;
	return linphone_call_get_current_quality(this->call);
}

float Linphone::Core::LinphoneCall::GetAverageQuality()
{
	API_LOCK;
	return linphone_call_get_average_quality(this->call);
}

Platform::String^ Linphone::Core::LinphoneCall::GetAuthenticationToken()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_call_get_authentication_token(this->call));
}

Platform::Boolean Linphone::Core::LinphoneCall::IsAuthenticationTokenVerified()
{
	API_LOCK;
	return (linphone_call_get_authentication_token_verified(this->call) == TRUE);
}

void Linphone::Core::LinphoneCall::SetAuthenticationTokenVerified(Platform::Boolean verified)
{
	API_LOCK;
	linphone_call_set_authentication_token_verified(this->call, verified);
}

Platform::Boolean Linphone::Core::LinphoneCall::IsInConference()
{
	API_LOCK;
	return (linphone_call_is_in_conference(this->call) == TRUE);
}

float Linphone::Core::LinphoneCall::GetPlayVolume()
{
	API_LOCK;
	return linphone_call_get_play_volume(this->call);
}

Platform::String^ Linphone::Core::LinphoneCall::GetRemoteUserAgent()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_call_get_remote_user_agent(this->call));
}

Platform::String^ Linphone::Core::LinphoneCall::GetRemoteContact()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_call_get_remote_contact(this->call));
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
	return (linphone_call_camera_enabled(this->call) == TRUE);
}

void Linphone::Core::LinphoneCall::EnableCamera(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_enable_camera(this->call, enable);
}

Linphone::Core::LinphoneCallStats^ Linphone::Core::LinphoneCall::GetVideoStats()
{
	API_LOCK;
	return (Linphone::Core::LinphoneCallStats^) Linphone::Core::Utils::CreateLinphoneCallStats(this->call, (int)Linphone::Core::MediaType::Video);
}

void Linphone::Core::LinphoneCall::SendVFURequest()
{
	API_LOCK;
	linphone_call_send_vfu_request(this->call);
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
}

Linphone::Core::LinphoneCall::~LinphoneCall()
{
	API_LOCK;
	linphone_call_unref(call);
	RefToPtrProxy<LinphoneCall^> *proxy = reinterpret_cast< RefToPtrProxy<LinphoneCall^> *>(linphone_call_get_user_data(this->call));
	delete proxy;
}
