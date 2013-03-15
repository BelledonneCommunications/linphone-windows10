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
	return Linphone::Core::LinphoneCallState::Error;
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCall::GetRemoteAddress()
{
	const ::LinphoneAddress *addr = linphone_call_get_remote_address(this->call);
	Linphone::Core::LinphoneAddress^ address = (Linphone::Core::LinphoneAddress^)Linphone::Core::Utils::CreateLinphoneAddress((void*)addr);
	return address;
}

Linphone::Core::CallDirection Linphone::Core::LinphoneCall::GetDirection()
{
	return Linphone::Core::CallDirection::Incoming;
}

Linphone::Core::LinphoneCallLog^ Linphone::Core::LinphoneCall::GetCallLog()
{
	return nullptr;
}

Linphone::Core::LinphoneCallStats^ Linphone::Core::LinphoneCall::GetAudioStats()
{
	return nullptr;
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCall::GetRemoteParams()
{
	return nullptr;
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCall::GetCurrentParamsCopy()
{
	return nullptr;
}

void Linphone::Core::LinphoneCall::EnableEchoCancellation(Platform::Boolean enable)
{

}

Platform::Boolean Linphone::Core::LinphoneCall::IsEchoCancellationEnabled()
{
	return false;
}

void Linphone::Core::LinphoneCall::EnableEchoLimiter(Platform::Boolean enable)
{

}

Platform::Boolean Linphone::Core::LinphoneCall::IsEchoLimiterEnabled()
{
	return false;
}

int Linphone::Core::LinphoneCall::GetDuration()
{
	return -1;
}

float Linphone::Core::LinphoneCall::GetCurrentQuality()
{
	return -1;
}

float Linphone::Core::LinphoneCall::GetAverageQuality()
{
	return -1;
}

Platform::String^ Linphone::Core::LinphoneCall::GetAuthenticationToken()
{
	return nullptr;
}

Platform::Boolean Linphone::Core::LinphoneCall::IsAuthenticationTokenVerified()
{
	return false;
}

void Linphone::Core::LinphoneCall::SetAuthenticationTokenVerified(Platform::Boolean verified)
{

}

Platform::Boolean Linphone::Core::LinphoneCall::IsInConference()
{
	return false;
}

float Linphone::Core::LinphoneCall::GetPlayVolume()
{
	return -1;
}

Platform::String^ Linphone::Core::LinphoneCall::GetRemoteUserAgent()
{
	return nullptr;
}

Platform::String^ Linphone::Core::LinphoneCall::GetRemoteContact()
{
	return Linphone::Core::Utils::cctops(linphone_call_get_remote_contact(this->call));
}

void Linphone::Core::LinphoneCall::CallContext::set(Platform::Object^ cc)
{
	this->callContext = cc;
}

Platform::Object^  Linphone::Core::LinphoneCall::CallContext::get()
{
	return this->callContext;
}

Linphone::Core::LinphoneCall::LinphoneCall(::LinphoneCall *call) :
	call(call)
{
	linphone_call_set_user_pointer(this->call, Linphone::Core::Utils::GetRawPointer(this));
}

Linphone::Core::LinphoneCall::~LinphoneCall()
{

}