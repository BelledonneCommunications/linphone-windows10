#include "LinphoneCall.h"
#include "LinphoneAddress.h"
#include "LinphoneCallStats.h"
#include "LinphoneCallLog.h"
#include "LinphoneCallParams.h"
#include "Server.h"
#include "ApiLock.h"
#include "LinphoneCoreFactory.h"
#include "Globals.h"

using namespace Linphone::Core;
using namespace Windows::Phone::Networking::Voip;

LinphoneCallState LinphoneCall::GetState()
{
	return LinphoneCallState::Error;
}

LinphoneAddress^ LinphoneCall::GetRemoteAddress()
{
	LinphoneCoreFactory^ lcf = Globals::Instance->LinphoneCoreFactory;
	return lcf->CreateLinphoneAddress(this->number);
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

void LinphoneCall::CallContext::set(Platform::Object^ cc)
{
	this->callContext = cc;
}

Platform::Object^  LinphoneCall::CallContext::get()
{
	return this->callContext;
}

LinphoneCall::LinphoneCall(Platform::String^ contact, Platform::String^ number) :
	contact(contact),
	number(number)
{
}