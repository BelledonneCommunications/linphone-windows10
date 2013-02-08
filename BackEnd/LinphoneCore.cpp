#include "LinphoneCore.h"
#include "LinphoneAddress.h"
#include "LinphoneAuthInfo.h"
#include "LinphoneCall.h"
#include "LinphoneCallParams.h"
#include "LinphoneProxyConfig.h"
#include "LpConfig.h"
#include "OnlineStatus.h"
#include "PayloadType.h"
#include "Server.h"
#include "CallController.h"

using namespace Linphone::BackEnd;
using namespace Platform;

Transports::Transports()
{

}

Transports::Transports(Transports^ t) :
	udp(t->UDP),
	tcp(t->TCP),
	tls(t->TLS)
{

}

int Transports::UDP::get()
{
	return udp;
}

int Transports::TCP::get()
{
	return tcp;
}

int Transports::TLS::get()
{
	return tls;
}

Platform::String^ Transports::ToString()
{
	return "udp[" + udp + "] tcp[" + tcp + "] tls[" + tls + "]";
}

void LinphoneCore::ClearProxyConfigs()
{

}

void LinphoneCore::AddProxyConfig(LinphoneProxyConfig^ proxyCfg)
{

}

void LinphoneCore::SetDefaultProxyConfig(LinphoneProxyConfig^ proxyCfg)
{

}

LinphoneProxyConfig^ LinphoneCore::GetDefaultProxyConfig()
{
	return nullptr;
}

Windows::Foundation::Collections::IVector<LinphoneProxyConfig^>^ LinphoneCore::GetProxyConfigList() 
{
	return nullptr;
}

void LinphoneCore::ClearAuthInfos() 
{

}

void LinphoneCore::AddAuthInfo(LinphoneAuthInfo^ info) 
{

}

void LinphoneCore::Iterate() 
{

}

void LinphoneCore::Destroy() 
{

}

LinphoneAddress^ LinphoneCore::InterpretURL(Platform::String^ destination) 
{
	return nullptr;
}

LinphoneCall^ LinphoneCore::Invite(Platform::String^ destination) 
{
	return nullptr;
}

LinphoneCall^ LinphoneCore::InviteAddress(LinphoneAddress^ to) 
{
	return nullptr;
}

LinphoneCall^ LinphoneCore::InviteAddressWithParams(LinphoneAddress^ destination, LinphoneCallParams^ params) 
{
	return nullptr;
}

void LinphoneCore::TerminateCall(LinphoneCall^ call) 
{
	
}

LinphoneCall^ LinphoneCore::GetCurrentCall() 
{
	return nullptr;
}

LinphoneAddress^ LinphoneCore::GetRemoteAddress() 
{
	return nullptr;
}

Platform::Boolean LinphoneCore::IsInCall() 
{
	return false;
}

Platform::Boolean LinphoneCore::IsIncomingInvitePending() 
{
	return false;
}

void LinphoneCore::AcceptCall(LinphoneCall^ call) 
{

}

void LinphoneCore::AcceptCallWithParams(LinphoneCall^ call, LinphoneCallParams^ params) 
{

}

void LinphoneCore::AcceptCallUpdate(LinphoneCall^ call, LinphoneCallParams^ params) 
{

}

void LinphoneCore::DeferCallUpdate(LinphoneCall^ call) 
{

}

void LinphoneCore::UpdateCall(LinphoneCall^ call, LinphoneCallParams^ params) 
{

}

LinphoneCallParams^ LinphoneCore::CreateDefaultCallParameters() 
{
	return nullptr;
}

Windows::Foundation::Collections::IVector<LinphoneCallLog^>^ LinphoneCore::GetCallLogs() 
{
	return nullptr;
}

void LinphoneCore::ClearCallLogs() 
{

}

void LinphoneCore::RemoveCallLog(LinphoneCallLog^ log) 
{

}

void LinphoneCore::SetNetworkReachable(Platform::Boolean isReachable) 
{

}

Platform::Boolean LinphoneCore::IsNetworkReachable() 
{
	return false;
}

void LinphoneCore::SetPlaybackGain(float gain) 
{

}

float LinphoneCore::GetPlaybackGain() 
{
	return -1;
}

void LinphoneCore::SetPlayLevel(int level) 
{

}

int LinphoneCore::GetPlayLevel() 
{
	return -1;
}

void LinphoneCore::MuteMic(Platform::Boolean isMuted) 
{

}

Platform::Boolean LinphoneCore::IsMicMuted() 
{
	return false;
}

void LinphoneCore::EnableSpeaker(Platform::Boolean enable) 
{

}

Platform::Boolean LinphoneCore::IsSpeakerEnabled() 
{
	return false;
}

void LinphoneCore::SendDTMF(char16 number) 
{

}

void LinphoneCore::PlayDTMF(char16 number, int duration) 
{

}

void LinphoneCore::StopDTMF() 
{

}

PayloadType^ LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate, int channels) 
{
	return nullptr;
}

PayloadType^ LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate) 
{
	return nullptr;
}

void LinphoneCore::EnablePayloadType(PayloadType^ pt, Platform::Boolean enable) 
{

}

Windows::Foundation::Collections::IVector<PayloadType^>^ LinphoneCore::GetAudioCodecs() 
{
	return nullptr;
}

void LinphoneCore::EnableEchoCancellation(Platform::Boolean enable) 
{

}

Platform::Boolean LinphoneCore::IsEchoCancellationEnabled() 
{
	return false;
}

Platform::Boolean LinphoneCore::IsEchoLimiterEnabled() 
{
	return false;
}

void LinphoneCore::StartEchoCalibration(Platform::Object^ data) 
{

}

void LinphoneCore::EnableEchoLimiter(Platform::Boolean enable) 
{

}

void LinphoneCore::SetSignalingTransportsPorts(Transports^ transports) 
{

}

Transports^ LinphoneCore::GetSignalingTransportsPorts() 
{
	return nullptr;
}

void LinphoneCore::EnableIPv6(Platform::Boolean enable) 
{

}

void LinphoneCore::SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status) 
{

}

void LinphoneCore::SetStunServer(Platform::String^ stun) 
{

}

Platform::String^ LinphoneCore::GetStunServer() 
{
	return nullptr;
}

void LinphoneCore::SetFirewallPolicy(FirewallPolicy policy) 
{

}

FirewallPolicy LinphoneCore::GetFirewallPolicy() 
{
	return FirewallPolicy::NoFirewall;
}

void LinphoneCore::SetRootCA(Platform::String^ path) 
{

}

void LinphoneCore::SetUploadBandwidth(int bw) 
{

}

void LinphoneCore::SetDownloadBandwidth(int bw) 
{

}

void LinphoneCore::SetDownloadPTime(int ptime) 
{

}

void LinphoneCore::SetUploadPTime(int ptime) 
{

}

void LinphoneCore::EnableKeepAlive(Platform::Boolean enable)
{

}

Platform::Boolean LinphoneCore::IsKeepAliveEnabled() 
{
	return false;
}

void LinphoneCore::SetPlayFile(Platform::String^ path) 
{

}

Platform::Boolean LinphoneCore::PauseCall(LinphoneCall^ call) 
{
	return false;
}

Platform::Boolean LinphoneCore::ResumeCall(LinphoneCall^ call) 
{
	return false;
}

Platform::Boolean LinphoneCore::PauseAllCalls() 
{
	return false;
}

Platform::Boolean LinphoneCore::IsInConference() 
{
	return false;
}

Platform::Boolean LinphoneCore::EnterConference() 
{
	return false;
}

void LinphoneCore::LeaveConference() 
{

}

void LinphoneCore::AddToConference(LinphoneCall^ call) 
{

}

void LinphoneCore::AddAllToConference() 
{

}

void LinphoneCore::RemoveFromConference() 
{

}

void LinphoneCore::TerminateConference() 
{

}

int LinphoneCore::GetConferenceSize() 
{
	return -1;
}

void LinphoneCore::TerminateAllCalls() 
{

}

Windows::Foundation::Collections::IVector<LinphoneCall^>^ LinphoneCore::GetCalls() 
{
	return nullptr;
}

int LinphoneCore::GetCallsNb() 
{
	return -1;
}

LinphoneCall^ LinphoneCore::FindCallFromUri(Platform::String^ uri) 
{
	return nullptr;
}

int LinphoneCore::GetMaxCalls() 
{
	return -1;
}

void LinphoneCore::SetMaxCalls(int max) 
{

}

Platform::Boolean LinphoneCore::IsMyself(Platform::String^ uri) 
{
	return false;
}

Platform::Boolean LinphoneCore::IsSoundResourcesLocked() 
{
	return false;
}

Platform::Boolean LinphoneCore::IsMediaEncryptionSupported(MediaEncryption menc) 
{
	return false;
}

void LinphoneCore::SetMediaEncryption(MediaEncryption menc) 
{

}

MediaEncryption LinphoneCore::GetMediaEncryption() 
{
	return MediaEncryption::None;
}

void LinphoneCore::SetMediaEncryptionMandatory(Platform::Boolean yesno) 
{

}

Platform::Boolean LinphoneCore::IsMediaEncryptionMandatory() 
{
	return false;
}

void LinphoneCore::EnableTunnel(Platform::Boolean enable) 
{

}

void LinphoneCore::TunnelAutoDetect() 
{

}

void LinphoneCore::TunnelCleanServers() 
{

}

void LinphoneCore::TunnelSetHttpProxy(Platform::String^ host, int port, Platform::String^ username, Platform::String^ password) 
{

}

void LinphoneCore::TunnelAddServerAndMirror(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay) 
{

}

Platform::Boolean LinphoneCore::IsTunnelAvailable() 
{
	return false;
}

void LinphoneCore::SetUserAgent(Platform::String^ name, Platform::String^ version) 
{

}

void LinphoneCore::SetCPUCount(int count) 
{

}

int LinphoneCore::GetMissedCallsCount() 
{
	return -1;
}

void LinphoneCore::ResetMissedCallsCount() 
{

}

void LinphoneCore::RefreshRegisters() 
{

}

Platform::String^ LinphoneCore::GetVersion() 
{
	return nullptr;
}

void LinphoneCore::SetAudioPort(int port) 
{

}

void LinphoneCore::SetAudioPortRange(int minP, int maxP) 
{

}

void LinphoneCore::SetIncomingTimeout(int timeout) 
{

}

void LinphoneCore::SetInCallTimeout(int timeout) 
{

}

void LinphoneCore::SetMicrophoneGain(float gain) 
{

}

void LinphoneCore::SetPrimaryContact(Platform::String^ displayName, Platform::String^ userName) 
{

}

void LinphoneCore::SetUseSipInfoForDTMFs(Platform::Boolean use) 
{

}

void LinphoneCore::SetUseRFC2833ForDTMFs(Platform::Boolean use) 
{

}

LpConfig^ LinphoneCore::GetConfig() 
{
	return nullptr;
}

LinphoneCore::LinphoneCore()
{

}

LinphoneCore::~LinphoneCore()
{

}
