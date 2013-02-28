#include "LinphoneCore.h"
#include "LinphoneAddress.h"
#include "LinphoneAuthInfo.h"
#include "LinphoneCall.h"
#include "LinphoneCallLog.h"
#include "LinphoneCallParams.h"
#include "LinphoneProxyConfig.h"
#include "LinphoneCoreListener.h"
#include "LpConfig.h"
#include "PayloadType.h"
#include "Server.h"
#include "Enums.h"
#include "ApiLock.h"
#include <collection.h>

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;

Linphone::Core::Transports::Transports()
{

}

Linphone::Core::Transports::Transports(Linphone::Core::Transports^ t) :
	udp(t->UDP),
	tcp(t->TCP),
	tls(t->TLS)
{

}

int Linphone::Core::Transports::UDP::get()
{
	return udp;
}

int Linphone::Core::Transports::TCP::get()
{
	return tcp;
}

int Linphone::Core::Transports::TLS::get()
{
	return tls;
}

Platform::String^ Linphone::Core::Transports::ToString()
{
	return "udp[" + udp + "] tcp[" + tcp + "] tls[" + tls + "]";
}


void Linphone::Core::LinphoneCore::SetContext(Platform::Object^ object)
{

}

void Linphone::Core::LinphoneCore::ClearProxyConfigs()
{

}

void Linphone::Core::LinphoneCore::AddProxyConfig(Linphone::Core::LinphoneProxyConfig^ proxyCfg)
{
	this->proxyCfgAdded = true;
}

void Linphone::Core::LinphoneCore::SetDefaultProxyConfig(Linphone::Core::LinphoneProxyConfig^ proxyCfg)
{

}

Linphone::Core::LinphoneProxyConfig^ Linphone::Core::LinphoneCore::GetDefaultProxyConfig()
{
	return nullptr;
}

Windows::Foundation::Collections::IVector<Linphone::Core::LinphoneProxyConfig^>^ Linphone::Core::LinphoneCore::GetProxyConfigList() 
{
	return nullptr;
}

void Linphone::Core::LinphoneCore::ClearAuthInfos() 
{

}

void Linphone::Core::LinphoneCore::AddAuthInfo(Linphone::Core::LinphoneAuthInfo^ info) 
{

}

void Linphone::Core::LinphoneCore::Iterate() 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	if (this->incomingcall != nullptr && this->listener != nullptr)
	{
		this->listener->CallState(this->incomingcall, Linphone::Core::LinphoneCallState::IncomingReceived);
		this->call = incomingcall;
		this->incomingcall = nullptr;
	}
	else if (this->call != nullptr && this->callAccepted && this->listener != nullptr)
	{
		this->callAccepted = false;
		this->listener->CallState(this->call, Linphone::Core::LinphoneCallState::Connected);
		this->callConnected = true;
	}
	else if (this->call != nullptr && this->callConnected && this->listener != nullptr)
	{
		this->callConnected = false;
		this->listener->CallState(this->call, Linphone::Core::LinphoneCallState::StreamsRunning);
	}
	else if (this->call != nullptr && this->callEnded && this->listener != nullptr)
	{
		this->callEnded = false;
		this->listener->CallState(this->call, Linphone::Core::LinphoneCallState::CallEnd);
		this->call = nullptr;
	}

	if (this->listener != nullptr && this->startup)
	{
		this->listener->GlobalState(Linphone::Core::GlobalState::GlobalStartup, L"");
		this->startup = false;
		this->on = true;
	}
	else if (this->listener != nullptr && this->on)
	{
		this->listener->GlobalState(Linphone::Core::GlobalState::GlobalOn, L"");
		this->on = false;
	}

	if (this->listener != nullptr && this->proxyCfgAdded && !this->on)
	{
		this->listener->RegistrationState(nullptr, Linphone::Core::RegistrationState::RegistrationInProgress, L"");
		this->proxyCfgAdded = false;
		this->proxyCfgRegistered = true;
	}
	else if (this->listener != nullptr && this->proxyCfgRegistered)
	{
		this->listener->RegistrationState(nullptr, Linphone::Core::RegistrationState::RegistrationOk, L"");
		this->proxyCfgRegistered = false;
	}
}

void Linphone::Core::LinphoneCore::Destroy() 
{

}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCore::InterpretURL(Platform::String^ destination) 
{
	return nullptr;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::Invite(Platform::String^ destination) 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	Linphone::Core::LinphoneCall^ call = ref new Linphone::Core::LinphoneCall("", destination);
	if (this->listener != nullptr)
		this->listener->CallState(call, Linphone::Core::LinphoneCallState::OutgoingInit);

	this->callAccepted = true;
	return call;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::InviteAddress(Linphone::Core::LinphoneAddress^ to) 
{
	return nullptr;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::InviteAddressWithParams(Linphone::Core::LinphoneAddress^ destination, LinphoneCallParams^ params) 
{
	return nullptr;
}

void Linphone::Core::LinphoneCore::TerminateCall(Linphone::Core::LinphoneCall^ call) 
{
	this->callEnded = true;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::GetCurrentCall() 
{
	return this->call;
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCore::GetRemoteAddress() 
{
	return nullptr;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsInCall() 
{
	return false;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsIncomingInvitePending() 
{
	return false;
}

void Linphone::Core::LinphoneCore::AcceptCall(Linphone::Core::LinphoneCall^ call) 
{
	this->call = call;
	this->callAccepted = true;
}

void Linphone::Core::LinphoneCore::AcceptCallWithParams(Linphone::Core::LinphoneCall^ call, Linphone::Core::LinphoneCallParams^ params) 
{

}

void Linphone::Core::LinphoneCore::AcceptCallUpdate(Linphone::Core::LinphoneCall^ call, LinphoneCallParams^ params) 
{

}

void Linphone::Core::LinphoneCore::DeferCallUpdate(Linphone::Core::LinphoneCall^ call) 
{

}

void Linphone::Core::LinphoneCore::UpdateCall(Linphone::Core::LinphoneCall^ call, Linphone::Core::LinphoneCallParams^ params) 
{

}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCore::CreateDefaultCallParameters() 
{
	return nullptr;
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetCallLogs() 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	IVector<Object^>^ logs = ref new Vector<Object^>();

	Linphone::Core::LinphoneCallLog^ log = ref new Linphone::Core::LinphoneCallLog(L"sip:waouf@sip.linphone.org", L"sip:miaou@sip.linphone.org", Linphone::Core::LinphoneCallStatus::Missed, Linphone::Core::CallDirection::Incoming);
	logs->Append(log);
	log = ref new Linphone::Core::LinphoneCallLog(L"sip:waouf@sip.linphone.org", L"sip:miaou@sip.linphone.org", Linphone::Core::LinphoneCallStatus::Success, Linphone::Core::CallDirection::Outgoing);
	logs->Append(log);
	log = ref new Linphone::Core::LinphoneCallLog(L"sip:cotcot@sip.linphone.org", L"sip:miaou@sip.linphone.org", Linphone::Core::LinphoneCallStatus::Success, Linphone::Core::CallDirection::Incoming);
	logs->Append(log);

	return logs;
}

void Linphone::Core::LinphoneCore::ClearCallLogs() 
{

}

void Linphone::Core::LinphoneCore::RemoveCallLog(Linphone::Core::LinphoneCallLog^ log) 
{

}

void Linphone::Core::LinphoneCore::SetNetworkReachable(Platform::Boolean isReachable) 
{

}

Platform::Boolean Linphone::Core::LinphoneCore::IsNetworkReachable() 
{
	return false;
}

void Linphone::Core::LinphoneCore::SetPlaybackGain(float gain) 
{

}

float Linphone::Core::LinphoneCore::GetPlaybackGain() 
{
	return -1;
}

void Linphone::Core::LinphoneCore::SetPlayLevel(int level) 
{

}

int Linphone::Core::LinphoneCore::GetPlayLevel() 
{
	return -1;
}

void Linphone::Core::LinphoneCore::MuteMic(Platform::Boolean isMuted) 
{

}

Platform::Boolean Linphone::Core::LinphoneCore::IsMicMuted() 
{
	return false;
}

void Linphone::Core::LinphoneCore::EnableSpeaker(Platform::Boolean enable) 
{

}

Platform::Boolean Linphone::Core::LinphoneCore::IsSpeakerEnabled() 
{
	return false;
}

void Linphone::Core::LinphoneCore::SendDTMF(char16 number) 
{

}

void Linphone::Core::LinphoneCore::PlayDTMF(char16 number, int duration) 
{

}

void Linphone::Core::LinphoneCore::StopDTMF() 
{

}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate, int channels) 
{
	return nullptr;
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate) 
{
	return nullptr;
}

void Linphone::Core::LinphoneCore::EnablePayloadType(PayloadType^ pt, Platform::Boolean enable) 
{

}

Windows::Foundation::Collections::IVector<Linphone::Core::PayloadType^>^ Linphone::Core::LinphoneCore::GetAudioCodecs() 
{
	return nullptr;
}

void Linphone::Core::LinphoneCore::EnableEchoCancellation(Platform::Boolean enable) 
{

}

Platform::Boolean Linphone::Core::LinphoneCore::IsEchoCancellationEnabled() 
{
	return false;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsEchoLimiterEnabled() 
{
	return false;
}

void Linphone::Core::LinphoneCore::StartEchoCalibration(Platform::Object^ data) 
{

}

void Linphone::Core::LinphoneCore::EnableEchoLimiter(Platform::Boolean enable) 
{

}

void Linphone::Core::LinphoneCore::SetSignalingTransportsPorts(Transports^ transports) 
{

}

Linphone::Core::Transports^ Linphone::Core::LinphoneCore::GetSignalingTransportsPorts() 
{
	return nullptr;
}

void Linphone::Core::LinphoneCore::EnableIPv6(Platform::Boolean enable) 
{

}

void Linphone::Core::LinphoneCore::SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status) 
{

}

void Linphone::Core::LinphoneCore::SetStunServer(Platform::String^ stun) 
{

}

Platform::String^ Linphone::Core::LinphoneCore::GetStunServer() 
{
	return nullptr;
}

void Linphone::Core::LinphoneCore::SetFirewallPolicy(FirewallPolicy policy) 
{

}

Linphone::Core::FirewallPolicy Linphone::Core::LinphoneCore::GetFirewallPolicy() 
{
	return FirewallPolicy::NoFirewall;
}

void Linphone::Core::LinphoneCore::SetRootCA(Platform::String^ path) 
{

}

void Linphone::Core::LinphoneCore::SetUploadBandwidth(int bw) 
{

}

void Linphone::Core::LinphoneCore::SetDownloadBandwidth(int bw) 
{

}

void Linphone::Core::LinphoneCore::SetDownloadPTime(int ptime) 
{

}

void Linphone::Core::LinphoneCore::SetUploadPTime(int ptime) 
{

}

void Linphone::Core::LinphoneCore::EnableKeepAlive(Platform::Boolean enable)
{

}

Platform::Boolean Linphone::Core::LinphoneCore::IsKeepAliveEnabled() 
{
	return false;
}

void Linphone::Core::LinphoneCore::SetPlayFile(Platform::String^ path) 
{

}

Platform::Boolean Linphone::Core::LinphoneCore::PauseCall(LinphoneCall^ call) 
{
	return false;
}

Platform::Boolean Linphone::Core::LinphoneCore::ResumeCall(LinphoneCall^ call) 
{
	return false;
}

Platform::Boolean Linphone::Core::LinphoneCore::PauseAllCalls() 
{
	return false;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsInConference() 
{
	return false;
}

Platform::Boolean Linphone::Core::LinphoneCore::EnterConference() 
{
	return false;
}

void Linphone::Core::LinphoneCore::LeaveConference() 
{

}

void Linphone::Core::LinphoneCore::AddToConference(LinphoneCall^ call) 
{

}

void Linphone::Core::LinphoneCore::AddAllToConference() 
{

}

void Linphone::Core::LinphoneCore::RemoveFromConference() 
{

}

void Linphone::Core::LinphoneCore::TerminateConference() 
{

}

int Linphone::Core::LinphoneCore::GetConferenceSize() 
{
	return -1;
}

void Linphone::Core::LinphoneCore::TerminateAllCalls() 
{

}

IVector<Linphone::Core::LinphoneCall^>^ Linphone::Core::LinphoneCore::GetCalls() 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	
	Vector<Linphone::Core::LinphoneCall^>^ calls = ref new Vector<Linphone::Core::LinphoneCall^>();
	calls->Append(this->call);
	return calls;
}

int Linphone::Core::LinphoneCore::GetCallsNb() 
{
	if (this->Call != nullptr)
		return 1;
	else
		return 0;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::FindCallFromUri(Platform::String^ uri) 
{
	return nullptr;
}

int Linphone::Core::LinphoneCore::GetMaxCalls() 
{
	return -1;
}

void Linphone::Core::LinphoneCore::SetMaxCalls(int max) 
{

}

Platform::Boolean Linphone::Core::LinphoneCore::IsMyself(Platform::String^ uri) 
{
	return false;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsSoundResourcesLocked() 
{
	return false;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMediaEncryptionSupported(MediaEncryption menc) 
{
	return false;
}

void Linphone::Core::LinphoneCore::SetMediaEncryption(MediaEncryption menc) 
{

}

Linphone::Core::MediaEncryption Linphone::Core::LinphoneCore::GetMediaEncryption() 
{
	return MediaEncryption::None;
}

void Linphone::Core::LinphoneCore::SetMediaEncryptionMandatory(Platform::Boolean yesno) 
{

}

Platform::Boolean Linphone::Core::LinphoneCore::IsMediaEncryptionMandatory() 
{
	return false;
}

void Linphone::Core::LinphoneCore::EnableTunnel(Platform::Boolean enable) 
{

}

void Linphone::Core::LinphoneCore::TunnelAutoDetect() 
{

}

void Linphone::Core::LinphoneCore::TunnelCleanServers() 
{

}

void Linphone::Core::LinphoneCore::TunnelSetHttpProxy(Platform::String^ host, int port, Platform::String^ username, Platform::String^ password) 
{

}

void Linphone::Core::LinphoneCore::TunnelAddServerAndMirror(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay) 
{

}

Platform::Boolean Linphone::Core::LinphoneCore::IsTunnelAvailable() 
{
	return false;
}

void Linphone::Core::LinphoneCore::SetUserAgent(Platform::String^ name, Platform::String^ version) 
{

}

void Linphone::Core::LinphoneCore::SetCPUCount(int count) 
{

}

int Linphone::Core::LinphoneCore::GetMissedCallsCount() 
{
	return -1;
}

void Linphone::Core::LinphoneCore::ResetMissedCallsCount() 
{

}

void Linphone::Core::LinphoneCore::RefreshRegisters() 
{

}

Platform::String^ Linphone::Core::LinphoneCore::GetVersion() 
{
	return nullptr;
}

void Linphone::Core::LinphoneCore::SetAudioPort(int port) 
{

}

void Linphone::Core::LinphoneCore::SetAudioPortRange(int minP, int maxP) 
{

}

void Linphone::Core::LinphoneCore::SetIncomingTimeout(int timeout) 
{

}

void Linphone::Core::LinphoneCore::SetInCallTimeout(int timeout) 
{

}

void Linphone::Core::LinphoneCore::SetMicrophoneGain(float gain) 
{

}

void Linphone::Core::LinphoneCore::SetPrimaryContact(Platform::String^ displayName, Platform::String^ userName) 
{

}

void Linphone::Core::LinphoneCore::SetUseSipInfoForDTMFs(Platform::Boolean use) 
{

}

void Linphone::Core::LinphoneCore::SetUseRFC2833ForDTMFs(Platform::Boolean use) 
{

}

Linphone::Core::LpConfig^ Linphone::Core::LinphoneCore::GetConfig() 
{
	return nullptr;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::Call::get()
{
	return this->call;
}

void Linphone::Core::LinphoneCore::Call::set(Linphone::Core::LinphoneCall^ call)
{
	this->call = call;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::IncomingCall::get()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	return this->incomingcall;
}

void Linphone::Core::LinphoneCore::IncomingCall::set(Linphone::Core::LinphoneCall^ call)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	this->incomingcall = call;
}

Linphone::Core::LinphoneCore::LinphoneCore(LinphoneCoreListener^ coreListener) :
	call(nullptr),
	incomingcall(nullptr),
	callAccepted(false),
	callEnded(false),
	callConnected(false),
	proxyCfgAdded(false),
	proxyCfgRegistered(false),
	startup(true),
	on(false),
	listener(coreListener)
{

}

Linphone::Core::LinphoneCore::~LinphoneCore()
{
	
}
