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
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::System::Threading;

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
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_clear_proxy_config(this->lc);
}

void Linphone::Core::LinphoneCore::AddProxyConfig(Linphone::Core::LinphoneProxyConfig^ proxyCfg)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_add_proxy_config(this->lc, proxyCfg->proxy_config);
}

void Linphone::Core::LinphoneCore::SetDefaultProxyConfig(Linphone::Core::LinphoneProxyConfig^ proxyCfg)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_set_default_proxy(this->lc, proxyCfg->proxy_config);
}

Linphone::Core::LinphoneProxyConfig^ Linphone::Core::LinphoneCore::GetDefaultProxyConfig()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	::LinphoneProxyConfig *proxy=NULL;
	linphone_core_get_default_proxy(this->lc,&proxy);
	Linphone::Core::LinphoneProxyConfig^ defaultProxy = reinterpret_cast<Linphone::Core::LinphoneProxyConfig^>(proxy);
	return defaultProxy;
}

Linphone::Core::LinphoneProxyConfig^ Linphone::Core::LinphoneCore::CreateEmptyProxyConfig()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	Linphone::Core::LinphoneProxyConfig^ proxyConfig = ref new Linphone::Core::LinphoneProxyConfig();
	return proxyConfig;
}

Windows::Foundation::Collections::IVector<Linphone::Core::LinphoneProxyConfig^>^ Linphone::Core::LinphoneCore::GetProxyConfigList() 
{
	return nullptr;
}

void Linphone::Core::LinphoneCore::ClearAuthInfos() 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_clear_all_auth_info(this->lc);
}

void Linphone::Core::LinphoneCore::AddAuthInfo(Linphone::Core::LinphoneAuthInfo^ info) 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_add_auth_info(this->lc, info->auth_info);
}

Linphone::Core::LinphoneAuthInfo^ Linphone::Core::LinphoneCore::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	Linphone::Core::LinphoneAuthInfo^ authInfo = ref new Linphone::Core::LinphoneAuthInfo(username, userid, password, ha1, realm);
	return authInfo;
}

void Linphone::Core::LinphoneCore::Destroy() 
{
	IterateTimer->Cancel();
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCore::InterpretURL(Platform::String^ destination) 
{
	return nullptr;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::Invite(Platform::String^ destination) 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	const char *cc = Linphone::Core::Utils::pstoccs(destination);
	::LinphoneCall *call = linphone_core_invite(this->lc, cc);
	delete(cc);
	
	if(call != NULL)
	{
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
		Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
		return lCall;
	}
	return nullptr;
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
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_terminate_call(this->lc, call->call);
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::GetCurrentCall() 
{
	::LinphoneCall *call = linphone_core_get_current_call(this->lc);
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
	Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
	return lCall;
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
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_accept_call(this->lc, call->call);
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

void AddLogToVector(void* nLog, void* vector)
{
	::LinphoneCallLog *cl = (LinphoneCallLog*)nLog;
	Linphone::Core::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Core::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ logs = (list) ? list->Ref() : nullptr;

	Linphone::Core::LinphoneCallLog^ log = (Linphone::Core::LinphoneCallLog^)Linphone::Core::Utils::CreateLinphoneCallLog(cl);
	logs->Append(log);
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetCallLogs() 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	IVector<Object^>^ logs = ref new Vector<Object^>();

	const MSList* logslist = linphone_core_get_call_logs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *logsptr = new RefToPtrProxy<IVector<Object^>^>(logs);
	ms_list_for_each2(logslist, AddLogToVector, logsptr);

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
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_set_network_reachable(this->lc, isReachable);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsNetworkReachable() 
{
	return linphone_core_is_network_reachable(this->lc);
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
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_mute_mic(this->lc, isMuted);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMicMuted() 
{
	return linphone_core_is_mic_muted(this->lc);
}

void Linphone::Core::LinphoneCore::SendDTMF(char16 number) 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_send_dtmf(this->lc, number);
}

void Linphone::Core::LinphoneCore::PlayDTMF(char16 number, int duration) 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_play_dtmf(this->lc, number, duration);
}

void Linphone::Core::LinphoneCore::StopDTMF() 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_stop_dtmf(this->lc);
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate, int channels) 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	const char* type = Linphone::Core::Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, channels);
	delete type;
	return ref new Linphone::Core::PayloadType(pt);
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate) 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	const char* type = Linphone::Core::Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, 1);
	delete type;
	return ref new Linphone::Core::PayloadType(pt);
}

void Linphone::Core::LinphoneCore::EnablePayloadType(PayloadType^ pt, Platform::Boolean enable) 
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	::PayloadType *payload = pt->payload;
	linphone_core_enable_payload_type(this->lc, payload, enable);
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
	return linphone_core_pause_call(this->lc, call->call);
}

Platform::Boolean Linphone::Core::LinphoneCore::ResumeCall(LinphoneCall^ call) 
{
	return linphone_core_resume_call(this->lc, call->call);;
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
	return calls;
}

int Linphone::Core::LinphoneCore::GetCallsNb() 
{
	return linphone_core_get_calls_nb(this->lc);
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

Linphone::Core::LinphoneCoreListener^ Linphone::Core::LinphoneCore::CoreListener::get()
{
	return this->listener;
}

void Linphone::Core::LinphoneCore::CoreListener::set(LinphoneCoreListener^ listener)
{
	this->listener = listener;
}

void call_state_changed(::LinphoneCore *lc, ::LinphoneCall *call, ::LinphoneCallState cstate, const char *msg) 
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		Linphone::Core::LinphoneCallState state = (Linphone::Core::LinphoneCallState) cstate;
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
		Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr) {
			lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
		}

		listener->CallState(lCall, state);
	}
}

void registration_state_changed(::LinphoneCore *lc, ::LinphoneProxyConfig *cfg, ::LinphoneRegistrationState cstate, const char *msg)
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		Linphone::Core::RegistrationState state = (Linphone::Core::RegistrationState) cstate;
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneProxyConfig^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneProxyConfig^> *>(linphone_proxy_config_get_user_data(cfg));
		Linphone::Core::LinphoneProxyConfig^ config = (proxy) ? proxy->Ref() : nullptr;
		listener->RegistrationState(config, state, Linphone::Core::Utils::cctops(msg));
	}
}

void global_state_changed(::LinphoneCore *lc, ::LinphoneGlobalState gstate, const char *msg)
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		Linphone::Core::GlobalState state = (Linphone::Core::GlobalState) gstate;
		listener->GlobalState(state, Linphone::Core::Utils::cctops(msg));
	}
}

void auth_info_requested(LinphoneCore *lc, const char *realm, const char *username) 
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		listener->AuthInfoRequested(Linphone::Core::Utils::cctops(realm), Linphone::Core::Utils::cctops(username));
	}
}

Linphone::Core::LinphoneCore::LinphoneCore(LinphoneCoreListener^ coreListener) :
	listener(coreListener),
	lc(nullptr)
{

}

void Linphone::Core::LinphoneCore::Init()
{
	LinphoneCoreVTable *vtable = (LinphoneCoreVTable*) malloc(sizeof(LinphoneCoreVTable));
	memset (vtable, 0, sizeof(LinphoneCoreVTable));
	vtable->global_state_changed = global_state_changed;
	vtable->registration_state_changed = registration_state_changed;
	vtable->call_state_changed = call_state_changed;
	vtable->auth_info_requested = auth_info_requested;

	this->lc = linphone_core_new(vtable, NULL, "Assets/linphone_rc", NULL);
	
	//Disable all codecs
	MSList* codecs=ms_list_copy(linphone_core_get_audio_codecs(lc));
	MSList* codecs_it;
	for (codecs_it=codecs;codecs_it!=NULL;codecs_it=codecs_it->next) {
			linphone_core_enable_payload_type(lc,(::PayloadType*)codecs_it->data,0);
	}

	// Launch iterate timer
	TimeSpan period;
	period.Duration = 20 * 10000;
	IterateTimer = ThreadPoolTimer::CreatePeriodicTimer(
		ref new TimerElapsedHandler([this](ThreadPoolTimer^ source)
		{
			if (source == IterateTimer) {
				std::lock_guard<std::recursive_mutex> lock(g_apiLock);
				linphone_core_iterate(this->lc);
			}
		}), period);
}

Linphone::Core::LinphoneCore::~LinphoneCore()
{
	
}
