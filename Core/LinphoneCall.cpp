#include "LinphoneCall.h"
#include "LinphoneAddress.h"
#include "LinphoneCallStats.h"
#include "LinphoneCallLog.h"
#include "LinphoneCallParams.h"
#include "Server.h"
#include "ApiLock.h"

using namespace Linphone::Core;

LinphoneCallState LinphoneCall::GetState()
{
	return LinphoneCallState::Error;
}

LinphoneAddress^ LinphoneCall::GetRemoteAddress()
{
	return ref new LinphoneAddress(this->number);
}

CallDirection LinphoneCall::GetDirection()
{
	return CallDirection::Incoming;
}

LinphoneCallLog^ LinphoneCall::GetCallLog()
{
	return nullptr;
}

LinphoneCallStats^ LinphoneCall::GetAudioStats()
{
	return nullptr;
}

LinphoneCallParams^ LinphoneCall::GetRemoteParams()
{
	return nullptr;
}

LinphoneCallParams^ LinphoneCall::GetCurrentParamsCopy()
{
	return nullptr;
}

void LinphoneCall::EnableEchoCancellation(Platform::Boolean enable)
{

}

Platform::Boolean LinphoneCall::IsEchoCancellationEnabled()
{
	return false;
}

void LinphoneCall::EnableEchoLimiter(Platform::Boolean enable)
{

}

Platform::Boolean LinphoneCall::IsEchoLimiterEnabled()
{
	return false;
}

int LinphoneCall::GetDuration()
{
	return -1;
}

float LinphoneCall::GetCurrentQuality()
{
	return -1;
}

float LinphoneCall::GetAverageQuality()
{
	return -1;
}

Platform::String^ LinphoneCall::GetAuthenticationToken()
{
	return nullptr;
}

Platform::Boolean LinphoneCall::IsAuthenticationTokenVerified()
{
	return false;
}

void LinphoneCall::SetAuthenticationTokenVerified(Platform::Boolean verified)
{

}

Platform::Boolean LinphoneCall::IsInConference()
{
	return false;
}

float LinphoneCall::GetPlayVolume()
{
	return -1;
}

Platform::String^ LinphoneCall::GetRemoteUserAgent()
{
	return nullptr;
}

Platform::String^ LinphoneCall::GetRemoteContact()
{
	return this->contact;
}

void LinphoneCall::CallContext::set(Windows::Phone::Networking::Voip::VoipPhoneCall^ cc)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	this->callContext = cc;
}

Windows::Phone::Networking::Voip::VoipPhoneCall^  LinphoneCall::CallContext::get()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	return this->callContext;
}

LinphoneCall::LinphoneCall(Platform::String^ contact, Platform::String^ number) :
	contact(contact),
	number(number)
{
}