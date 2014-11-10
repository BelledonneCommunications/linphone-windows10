#include "LinphoneCore.h"
#include "LinphoneAddress.h"
#include "LinphoneAuthInfo.h"
#include "LinphoneCall.h"
#include "LinphoneCallLog.h"
#include "LinphoneCallParams.h"
#include "LinphoneProxyConfig.h"
#include "LinphoneCoreListener.h"
#include "LinphoneChatRoom.h"
#include "LpConfig.h"
#include "PayloadType.h"
#include "CallController.h"
#include "Tunnel.h"
#include "Server.h"
#include "Enums.h"
#include "ApiLock.h"
#include <collection.h>

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Phone::Media::Devices;
using namespace Windows::Phone::Networking::Voip;
using namespace Windows::System::Threading;

Linphone::Core::Transports::Transports() :
	udp(5060),
	tcp(0),
	tls(0)
{
}

Linphone::Core::Transports::Transports(int udp_port, int tcp_port, int tls_port) :
	udp(udp_port),
	tcp(tcp_port),
	tls(tls_port)
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

void Linphone::Core::Transports::UDP::set(int value)
{
	this->udp = value;
	this->tcp = 0;
	this->tls = 0;
}

int Linphone::Core::Transports::TCP::get()
{
	return tcp;
}

void Linphone::Core::Transports::TCP::set(int value)
{
	this->udp = 0;
	this->tcp = value;
	this->tls = 0;
}

int Linphone::Core::Transports::TLS::get()
{
	return tls;
}

void Linphone::Core::Transports::TLS::set(int value)
{
	this->udp = 0;
	this->tcp = 0;
	this->tls = value;
}

Platform::String^ Linphone::Core::Transports::ToString()
{
	return "udp[" + udp + "] tcp[" + tcp + "] tls[" + tls + "]";
}



Linphone::Core::VideoPolicy::VideoPolicy() :
	automaticallyInitiate(true), automaticallyAccept(true)
{
}

Linphone::Core::VideoPolicy::VideoPolicy(bool automaticallyInitiate, bool automaticallyAccept) :
	automaticallyInitiate(automaticallyInitiate), automaticallyAccept(automaticallyAccept)
{
}

bool Linphone::Core::VideoPolicy::AutomaticallyInitiate::get()
{
	return automaticallyInitiate;
}

void Linphone::Core::VideoPolicy::AutomaticallyInitiate::set(bool value)
{
	automaticallyInitiate = value;
}

bool Linphone::Core::VideoPolicy::AutomaticallyAccept::get()
{
	return automaticallyAccept;
}

void Linphone::Core::VideoPolicy::AutomaticallyAccept::set(bool value)
{
	automaticallyAccept = value;
}



Linphone::Core::VideoSize::VideoSize(int width, int height) :
	width(width), height(height), name("")
{
}

Linphone::Core::VideoSize::VideoSize(int width, int height, Platform::String^ name) :
	width(width), height(height), name(name)
{
}

int Linphone::Core::VideoSize::Width::get()
{
	return width;
}

void Linphone::Core::VideoSize::Width::set(int value)
{
	width = value;
}

int Linphone::Core::VideoSize::Height::get()
{
	return height;
}

void Linphone::Core::VideoSize::Height::set(int value)
{
	height = value;
}

Platform::String^ Linphone::Core::VideoSize::Name::get()
{
	return name;
}

void Linphone::Core::VideoSize::Name::set(Platform::String^ value)
{
	name = value;
}



void Linphone::Core::LinphoneCore::SetLogLevel(OutputTraceLevel logLevel)
{
	int coreLogLevel = 0;
	if (logLevel == OutputTraceLevel::Error) {
		coreLogLevel = ORTP_ERROR | ORTP_FATAL;
	}
	else if (logLevel == OutputTraceLevel::Warning) {
		coreLogLevel = ORTP_WARNING | ORTP_ERROR | ORTP_FATAL;
	}
	else if (logLevel == OutputTraceLevel::Message) {
		coreLogLevel = ORTP_MESSAGE | ORTP_WARNING | ORTP_ERROR | ORTP_FATAL;
	}
	else if (logLevel == OutputTraceLevel::Debug) {
		coreLogLevel = ORTP_DEBUG | ORTP_MESSAGE | ORTP_WARNING | ORTP_ERROR | ORTP_FATAL;
	}
	Utils::LinphoneCoreSetLogLevel(coreLogLevel);
}

void Linphone::Core::LinphoneCore::ResetLogCollection()
{
	API_LOCK;
	linphone_core_reset_log_collection();
}

void Linphone::Core::LinphoneCore::ClearProxyConfigs()
{
	API_LOCK;
	linphone_core_clear_proxy_config(this->lc);
}

void Linphone::Core::LinphoneCore::AddProxyConfig(Linphone::Core::LinphoneProxyConfig^ proxyCfg)
{
	API_LOCK;
	linphone_core_add_proxy_config(this->lc, proxyCfg->proxy_config);
}

void Linphone::Core::LinphoneCore::SetDefaultProxyConfig(Linphone::Core::LinphoneProxyConfig^ proxyCfg)
{
	API_LOCK;
	linphone_core_set_default_proxy(this->lc, proxyCfg->proxy_config);
}

Linphone::Core::LinphoneProxyConfig^ Linphone::Core::LinphoneCore::GetDefaultProxyConfig()
{
	API_LOCK;
	LinphoneProxyConfig^ defaultProxy = nullptr;
	::LinphoneProxyConfig *proxy=NULL;
	linphone_core_get_default_proxy(this->lc,&proxy);
	if (proxy != nullptr) {
		defaultProxy = ref new Linphone::Core::LinphoneProxyConfig(proxy);
	}
	return defaultProxy;
}

Linphone::Core::LinphoneProxyConfig^ Linphone::Core::LinphoneCore::CreateEmptyProxyConfig()
{
	API_LOCK;
	LinphoneProxyConfig^ proxyConfig = nullptr;
	::LinphoneProxyConfig *proxy = linphone_core_create_proxy_config(this->lc);
	if (proxy != nullptr) {
		proxyConfig = ref new Linphone::Core::LinphoneProxyConfig(proxy);
	}
	return proxyConfig;
}

static void AddProxyConfigToVector(void *vProxyConfig, void *vector)
{
	::LinphoneProxyConfig *pc = (LinphoneProxyConfig *)vProxyConfig;
	Linphone::Core::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Core::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ proxyconfigs = (list) ? list->Ref() : nullptr;

	Linphone::Core::LinphoneProxyConfig^ proxyConfig = (Linphone::Core::LinphoneProxyConfig^)Linphone::Core::Utils::CreateLinphoneProxyConfig(pc);
	proxyconfigs->Append(proxyConfig);
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetProxyConfigList() 
{
	API_LOCK;
	IVector<Object^>^ proxyconfigs = ref new Vector<Object^>();
	const MSList *configList = linphone_core_get_proxy_config_list(this->lc);
	RefToPtrProxy<IVector<Object^>^> *proxyConfigPtr = new RefToPtrProxy<IVector<Object^>^>(proxyconfigs);
	ms_list_for_each2(configList, AddProxyConfigToVector, proxyConfigPtr);
	return proxyconfigs;
}

void Linphone::Core::LinphoneCore::ClearAuthInfos() 
{
	API_LOCK;
	linphone_core_clear_all_auth_info(this->lc);
}

void Linphone::Core::LinphoneCore::AddAuthInfo(Linphone::Core::LinphoneAuthInfo^ info) 
{
	API_LOCK;
	linphone_core_add_auth_info(this->lc, info->auth_info);
}

Linphone::Core::LinphoneAuthInfo^ Linphone::Core::LinphoneCore::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
	API_LOCK;
	Linphone::Core::LinphoneAuthInfo^ authInfo = ref new Linphone::Core::LinphoneAuthInfo(username, userid, password, ha1, realm, domain);
	return authInfo;
}

static void AddAuthInfoToVector(void *vAuthInfo, void *vector)
{
	::LinphoneAuthInfo *ai = (LinphoneAuthInfo *)vAuthInfo;
	Linphone::Core::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Core::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ authInfos = (list) ? list->Ref() : nullptr;

	Linphone::Core::LinphoneAuthInfo^ authInfo = (Linphone::Core::LinphoneAuthInfo^)Linphone::Core::Utils::CreateLinphoneAuthInfo(ai);
	authInfos->Append(authInfo);
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetAuthInfos()
{
	API_LOCK;
	IVector<Object^>^ authInfos = ref new Vector<Object^>();
	const MSList *authlist = linphone_core_get_auth_info_list(this->lc);
	RefToPtrProxy<IVector<Object^>^> *authInfosPtr = new RefToPtrProxy<IVector<Object^>^>(authInfos);
	ms_list_for_each2(authlist, AddAuthInfoToVector, authInfosPtr);
	return authInfos;
}

void Linphone::Core::LinphoneCore::Destroy() 
{
	API_LOCK;
	linphone_core_destroy(this->lc);
	IterateEnabled = false;
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCore::InterpretURL(Platform::String^ destination) 
{
	API_LOCK;
	const char* url = Linphone::Core::Utils::pstoccs(destination);
	Linphone::Core::LinphoneAddress^ addr = (Linphone::Core::LinphoneAddress^) Linphone::Core::Utils::CreateLinphoneAddress(linphone_core_interpret_url(this->lc, url));
	delete(url);
	return addr;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::Invite(Platform::String^ destination) 
{
	API_LOCK;
	Linphone::Core::LinphoneCall^ lCall = nullptr;
	const char *cc = Linphone::Core::Utils::pstoccs(destination);
	::LinphoneCall *call = linphone_core_invite(this->lc, cc);
	call = linphone_call_ref(call);
	delete(cc);
	if (call != NULL) {
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_data(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	return lCall;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::InviteAddress(Linphone::Core::LinphoneAddress^ destination) 
{
	API_LOCK;
	Linphone::Core::LinphoneCall^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_invite_address(this->lc, destination->address);
	call = linphone_call_ref(call);
	if (call != NULL) {
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_data(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Core::LinphoneCall^) Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	return lCall;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::InviteAddressWithParams(Linphone::Core::LinphoneAddress^ destination, LinphoneCallParams^ params) 
{
	API_LOCK;
	Linphone::Core::LinphoneCall^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_invite_address_with_params(this->lc, destination->address, params->params);
	call = linphone_call_ref(call);
	if (call != NULL) {
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Core::LinphoneCall^) Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	return lCall;
}

void Linphone::Core::LinphoneCore::TerminateCall(Linphone::Core::LinphoneCall^ call) 
{
	API_LOCK;
	linphone_core_terminate_call(this->lc, call->call);
}

void Linphone::Core::LinphoneCore::DeclineCall(Linphone::Core::LinphoneCall^ call, Linphone::Core::Reason reason)
{
	API_LOCK;
	linphone_core_decline_call(this->lc, call->call, (LinphoneReason)reason);
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::GetCurrentCall() 
{
	API_LOCK;
	Linphone::Core::LinphoneCall^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_get_current_call(this->lc);
	if (call != nullptr) {
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
	}
	return lCall;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsInCall() 
{
	API_LOCK;
	return (linphone_core_in_call(this->lc) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsIncomingInvitePending() 
{
	API_LOCK;
	return (linphone_core_inc_invite_pending(this->lc) == TRUE);
}

void Linphone::Core::LinphoneCore::AcceptCall(Linphone::Core::LinphoneCall^ call) 
{
	API_LOCK;
	linphone_core_accept_call(this->lc, call->call);
}

void Linphone::Core::LinphoneCore::AcceptCallWithParams(Linphone::Core::LinphoneCall^ call, Linphone::Core::LinphoneCallParams^ params) 
{
	API_LOCK;
	linphone_core_accept_call_with_params(this->lc, call->call, params->params);
}

void Linphone::Core::LinphoneCore::AcceptCallUpdate(Linphone::Core::LinphoneCall^ call, Linphone::Core::LinphoneCallParams^ params) 
{
	API_LOCK;
	linphone_core_accept_call_update(this->lc, call->call, params->params);
}

void Linphone::Core::LinphoneCore::DeferCallUpdate(Linphone::Core::LinphoneCall^ call) 
{
	API_LOCK;
	linphone_core_defer_call_update(this->lc, call->call);
}

void Linphone::Core::LinphoneCore::UpdateCall(Linphone::Core::LinphoneCall^ call, Linphone::Core::LinphoneCallParams^ params) 
{
	API_LOCK;
	if (params != nullptr) {
		linphone_core_update_call(this->lc, call->call, params->params);
	} else {
		linphone_core_update_call(this->lc, call->call, nullptr);
	}
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCore::CreateDefaultCallParameters() 
{
	API_LOCK;
	return (Linphone::Core::LinphoneCallParams^) Linphone::Core::Utils::CreateLinphoneCallParams(linphone_core_create_default_call_parameters(this->lc));
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
	API_LOCK;
	IVector<Object^>^ logs = ref new Vector<Object^>();
	const MSList* logslist = linphone_core_get_call_logs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *logsptr = new RefToPtrProxy<IVector<Object^>^>(logs);
	ms_list_for_each2(logslist, AddLogToVector, logsptr);
	return logs;
}

void Linphone::Core::LinphoneCore::ClearCallLogs() 
{
	API_LOCK;
	linphone_core_clear_call_logs(this->lc);
}

void Linphone::Core::LinphoneCore::RemoveCallLog(Linphone::Core::LinphoneCallLog^ log) 
{
	API_LOCK;
	linphone_core_remove_call_log(this->lc, log->callLog);
}

void Linphone::Core::LinphoneCore::SetNetworkReachable(Platform::Boolean isReachable) 
{
	API_LOCK;
	linphone_core_set_network_reachable(this->lc, isReachable);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsNetworkReachable() 
{
	API_LOCK;
	return (linphone_core_is_network_reachable(this->lc) == TRUE);
}

void Linphone::Core::LinphoneCore::SetMicrophoneGain(float gain) 
{
	API_LOCK;
	linphone_core_set_mic_gain_db(this->lc, gain);
}

void Linphone::Core::LinphoneCore::SetPlaybackGain(float gain) 
{
	API_LOCK;
	linphone_core_set_playback_gain_db(this->lc, gain);
}

float Linphone::Core::LinphoneCore::GetPlaybackGain() 
{
	API_LOCK;
	return linphone_core_get_playback_gain_db(this->lc);
}

void Linphone::Core::LinphoneCore::SetPlayLevel(int level) 
{
	API_LOCK;
	linphone_core_set_play_level(this->lc, level);
}

int Linphone::Core::LinphoneCore::GetPlayLevel() 
{
	API_LOCK;
	return linphone_core_get_play_level(this->lc);
}

void Linphone::Core::LinphoneCore::MuteMic(Platform::Boolean isMuted) 
{
	API_LOCK;
	linphone_core_mute_mic(this->lc, isMuted);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMicMuted() 
{
	API_LOCK;
	return (linphone_core_is_mic_muted(this->lc) == TRUE);
}

void Linphone::Core::LinphoneCore::SendDTMF(char16 number) 
{
	API_LOCK;
	char conv[4];
	if (wctomb(conv, number) == 1) {
		linphone_core_send_dtmf(this->lc, conv[0]);
	}
}

void Linphone::Core::LinphoneCore::PlayDTMF(char16 number, int duration) 
{
	API_LOCK;
	char conv[4];
	if (wctomb(conv, number) == 1) {
		linphone_core_play_dtmf(this->lc, conv[0], duration);
	}
}

void Linphone::Core::LinphoneCore::StopDTMF() 
{
	API_LOCK;
	linphone_core_stop_dtmf(this->lc);
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate, int channels) 
{
	API_LOCK;
	const char* type = Linphone::Core::Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, channels);
	delete type;
	return ref new Linphone::Core::PayloadType(pt);
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate) 
{
	API_LOCK;
	const char* type = Linphone::Core::Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, LINPHONE_FIND_PAYLOAD_IGNORE_CHANNELS);
	delete type;
	return ref new Linphone::Core::PayloadType(pt);
}

bool Linphone::Core::LinphoneCore::PayloadTypeEnabled(PayloadType^ pt)
{
	API_LOCK;
	::PayloadType *payload = pt->payload;
	return (linphone_core_payload_type_enabled(this->lc, payload) == TRUE);
}

void Linphone::Core::LinphoneCore::EnablePayloadType(PayloadType^ pt, Platform::Boolean enable) 
{
	API_LOCK;
	::PayloadType *payload = pt->payload;
	linphone_core_enable_payload_type(this->lc, payload, enable);
}

static void AddCodecToVector(void *vCodec, void *vector)
{
	::PayloadType *pt = (PayloadType *)vCodec;
	Linphone::Core::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Core::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ codecs = (list) ? list->Ref() : nullptr;

	Linphone::Core::PayloadType^ codec = (Linphone::Core::PayloadType^)Linphone::Core::Utils::CreatePayloadType(pt);
	codecs->Append(codec);
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetAudioCodecs()
{
	API_LOCK;
	IVector<Object^>^ codecs = ref new Vector<Object^>();
	const MSList *codecslist = linphone_core_get_audio_codecs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *codecsPtr = new RefToPtrProxy<IVector<Object^>^>(codecs);
	ms_list_for_each2(codecslist, AddCodecToVector, codecsPtr);
	return codecs;
}

void Linphone::Core::LinphoneCore::EnableEchoCancellation(Platform::Boolean enable) 
{
	API_LOCK;
	linphone_core_enable_echo_cancellation(this->lc, enable);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsEchoCancellationEnabled() 
{
	API_LOCK;
	return (linphone_core_echo_cancellation_enabled(this->lc) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsEchoLimiterEnabled() 
{
	API_LOCK;
	return (linphone_core_echo_limiter_enabled(this->lc) == TRUE);
}

static void EchoCalibrationCallback(LinphoneCore *lc, LinphoneEcCalibratorStatus status, int delay_ms, void *data)
{
	Linphone::Core::Utils::EchoCalibrationCallback(lc, status, delay_ms, data);
}

static void EchoCalibrationAudioInit(void *data)
{
	Linphone::Core::EchoCalibrationData *ecData = static_cast<Linphone::Core::EchoCalibrationData *>(data);
	if (ecData != nullptr) {
		ecData->endpoint = AudioRoutingManager::GetDefault()->GetAudioEndpoint();
		// Need to create a dummy VoipPhoneCall to be able to capture audio!
		VoipCallCoordinator::GetDefault()->RequestNewOutgoingCall(
			"ECCalibrator",
			"ECCalibrator",
			"ECCalibrator",
			VoipCallMedia::Audio,
			&ecData->call);
		ecData->call->NotifyCallActive();
	}
	AudioRoutingManager::GetDefault()->SetAudioEndpoint(AudioRoutingEndpoint::Speakerphone);
}

static void EchoCalibrationAudioUninit(void *data)
{
	Linphone::Core::EchoCalibrationData *ecData = static_cast<Linphone::Core::EchoCalibrationData *>(data);
	if (ecData != nullptr) {
		ecData->call->NotifyCallEnded();
		AudioRoutingManager::GetDefault()->SetAudioEndpoint(AudioRoutingEndpoint::Default);
	}
}

void Linphone::Core::LinphoneCore::StartEchoCalibration() 
{
	API_LOCK;
	Linphone::Core::EchoCalibrationData *data = new Linphone::Core::EchoCalibrationData();
	linphone_core_start_echo_calibration(this->lc, EchoCalibrationCallback, EchoCalibrationAudioInit, EchoCalibrationAudioUninit, data);
}

void Linphone::Core::LinphoneCore::EnableEchoLimiter(Platform::Boolean enable) 
{
	API_LOCK;
	linphone_core_enable_echo_limiter(this->lc, enable);
}

void Linphone::Core::LinphoneCore::SetSignalingTransportsPorts(Transports^ t) 
{
	API_LOCK;
	::LCSipTransports transports;
	memset(&transports, 0, sizeof(LCSipTransports));
	transports.udp_port = t->UDP;
	transports.tcp_port = t->TCP;
	transports.tls_port = t->TLS;
	linphone_core_set_sip_transports(this->lc, &transports);
}

Linphone::Core::Transports^ Linphone::Core::LinphoneCore::GetSignalingTransportsPorts()
{
	API_LOCK;
	::LCSipTransports transports;
	linphone_core_get_sip_transports(this->lc, &transports);
	return ref new Linphone::Core::Transports(transports.udp_port, transports.tcp_port, transports.tls_port);
}

void Linphone::Core::LinphoneCore::EnableIPv6(Platform::Boolean enable) 
{
	API_LOCK;
	linphone_core_enable_ipv6(this->lc, enable);
}

void Linphone::Core::LinphoneCore::SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status) 
{
	API_LOCK;
	const char* ac = Linphone::Core::Utils::pstoccs(alternativeContact);
	linphone_core_set_presence_info(this->lc, minuteAway, ac, (LinphoneOnlineStatus) status);
	delete(ac);
}

void Linphone::Core::LinphoneCore::SetStunServer(Platform::String^ stun) 
{
	API_LOCK;
	const char* stunserver = Linphone::Core::Utils::pstoccs(stun);
	linphone_core_set_stun_server(this->lc, stunserver);
	delete(stunserver);
}

Platform::String^ Linphone::Core::LinphoneCore::GetStunServer() 
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_core_get_stun_server(this->lc));
}

void Linphone::Core::LinphoneCore::SetFirewallPolicy(FirewallPolicy policy) 
{
	API_LOCK;
	linphone_core_set_firewall_policy(this->lc, (LinphoneFirewallPolicy) policy);
}

Linphone::Core::FirewallPolicy Linphone::Core::LinphoneCore::GetFirewallPolicy() 
{
	API_LOCK;
	return (Linphone::Core::FirewallPolicy) linphone_core_get_firewall_policy(this->lc);
}

void Linphone::Core::LinphoneCore::SetRootCA(Platform::String^ path) 
{
	API_LOCK;
	const char *ccPath = Utils::pstoccs(path);
	linphone_core_set_root_ca(this->lc, ccPath);
	delete ccPath;
}

int Linphone::Core::LinphoneCore::UploadBandwidth::get()
{
	API_LOCK;
	return linphone_core_get_upload_bandwidth(this->lc);
}

void Linphone::Core::LinphoneCore::UploadBandwidth::set(int value)
{
	API_LOCK;
	linphone_core_set_upload_bandwidth(this->lc, value);
}

int Linphone::Core::LinphoneCore::DownloadBandwidth::get()
{
	API_LOCK;
	return linphone_core_get_download_bandwidth(this->lc);
}

void Linphone::Core::LinphoneCore::DownloadBandwidth::set(int value)
{
	API_LOCK;
	linphone_core_set_download_bandwidth(this->lc, value);
}

void Linphone::Core::LinphoneCore::SetDownloadPTime(int ptime) 
{
	API_LOCK;
	linphone_core_set_download_ptime(this->lc, ptime);
}

void Linphone::Core::LinphoneCore::SetUploadPTime(int ptime) 
{
	API_LOCK;
	linphone_core_set_upload_ptime(this->lc, ptime);
}

void Linphone::Core::LinphoneCore::EnableKeepAlive(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_keep_alive(this->lc, enable);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsKeepAliveEnabled() 
{
	API_LOCK;
	return (linphone_core_keep_alive_enabled(this->lc) == TRUE);
}

void Linphone::Core::LinphoneCore::SetPlayFile(Platform::String^ path) 
{
	API_LOCK;
	const char* file = Linphone::Core::Utils::pstoccs(path);
	linphone_core_set_play_file(this->lc, file);
	delete(file);
}

Platform::Boolean Linphone::Core::LinphoneCore::PauseCall(LinphoneCall^ call) 
{
	API_LOCK;
	return (linphone_core_pause_call(this->lc, call->call) == 0);
}

Platform::Boolean Linphone::Core::LinphoneCore::ResumeCall(LinphoneCall^ call) 
{
	API_LOCK;
	return (linphone_core_resume_call(this->lc, call->call) == 0);
}

Platform::Boolean Linphone::Core::LinphoneCore::PauseAllCalls() 
{
	API_LOCK;
	return (linphone_core_pause_all_calls(this->lc) == 0);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsInConference() 
{
	API_LOCK;
	return (linphone_core_is_in_conference(this->lc) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCore::EnterConference() 
{
	API_LOCK;
	return (linphone_core_enter_conference(this->lc) == 0);
}

void Linphone::Core::LinphoneCore::LeaveConference() 
{
	API_LOCK;
	linphone_core_leave_conference(this->lc);
}

void Linphone::Core::LinphoneCore::AddToConference(LinphoneCall^ call) 
{
	API_LOCK;
	linphone_core_add_to_conference(this->lc, call->call);
}

void Linphone::Core::LinphoneCore::AddAllToConference() 
{
	API_LOCK;
	linphone_core_add_all_to_conference(this->lc);
}

void Linphone::Core::LinphoneCore::RemoveFromConference(LinphoneCall^ call) 
{
	API_LOCK;
	linphone_core_remove_from_conference(this->lc, call->call);
}

void Linphone::Core::LinphoneCore::TerminateConference() 
{
	API_LOCK;
	linphone_core_terminate_conference(this->lc);
}

int Linphone::Core::LinphoneCore::GetConferenceSize() 
{
	API_LOCK;
	return linphone_core_get_conference_size(this->lc);
}

void Linphone::Core::LinphoneCore::TerminateAllCalls() 
{
	API_LOCK;
	linphone_core_terminate_all_calls(this->lc);
}

static void AddCallToVector(void *vCall, void *vector)
{
	::LinphoneCall* c = (::LinphoneCall *)vCall;
	Linphone::Core::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Core::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ calls = (list) ? list->Ref() : nullptr;

	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(c));
	Linphone::Core::LinphoneCall^ call = (proxy) ? proxy->Ref() : nullptr;
	calls->Append(call);
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetCalls() 
{
	API_LOCK;
	Vector<Object^>^ calls = ref new Vector<Object^>();
	const MSList *callsList = linphone_core_get_calls(this->lc);
	RefToPtrProxy<IVector<Object^>^> *callsPtr = new RefToPtrProxy<IVector<Object^>^>(calls);
	ms_list_for_each2(callsList, AddCallToVector, callsPtr);
	return calls;
}

int Linphone::Core::LinphoneCore::GetCallsNb() 
{
	API_LOCK;
	return linphone_core_get_calls_nb(this->lc);
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::FindCallFromUri(Platform::String^ uri) 
{
	API_LOCK;
	const char *curi = Utils::pstoccs(uri);
	::LinphoneCall *call = const_cast<::LinphoneCall *>(linphone_core_find_call_from_uri(this->lc, curi));
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
	Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	delete curi;
	return lCall;
}

int Linphone::Core::LinphoneCore::GetMaxCalls() 
{
	API_LOCK;
	return linphone_core_get_max_calls(this->lc);
}

void Linphone::Core::LinphoneCore::SetMaxCalls(int max) 
{
	API_LOCK;
	linphone_core_set_max_calls(this->lc, max);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMyself(Platform::String^ uri) 
{
	API_LOCK;
	Platform::Boolean myself = false;
	LinphoneProxyConfig^ lpc = GetDefaultProxyConfig();
	if (lpc != nullptr) {
		myself = uri->Equals(lpc->GetIdentity());
	}
	return myself;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsSoundResourcesLocked() 
{
	API_LOCK;
	return (linphone_core_sound_resources_locked(this->lc) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMediaEncryptionSupported(MediaEncryption menc) 
{
	API_LOCK;
	return (linphone_core_media_encryption_supported(this->lc, (LinphoneMediaEncryption) menc) == TRUE);
}

void Linphone::Core::LinphoneCore::SetMediaEncryption(MediaEncryption menc) 
{
	API_LOCK;
	linphone_core_set_media_encryption(this->lc, (LinphoneMediaEncryption) menc);
}

Linphone::Core::MediaEncryption Linphone::Core::LinphoneCore::GetMediaEncryption() 
{
	API_LOCK;
	return (Linphone::Core::MediaEncryption) linphone_core_get_media_encryption(this->lc);
}

void Linphone::Core::LinphoneCore::SetMediaEncryptionMandatory(Platform::Boolean yesno) 
{
	API_LOCK;
	linphone_core_set_media_encryption_mandatory(this->lc, yesno);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMediaEncryptionMandatory() 
{
	API_LOCK;
	return (linphone_core_is_media_encryption_mandatory(this->lc) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsTunnelAvailable() 
{
	API_LOCK;
	return (linphone_core_tunnel_available() == TRUE);
}

Linphone::Core::Tunnel^ Linphone::Core::LinphoneCore::GetTunnel()
{
	API_LOCK;
	Linphone::Core::Tunnel^ tunnel = nullptr;
	LinphoneTunnel *lt = linphone_core_get_tunnel(this->lc);
	if (lt != nullptr) {
		tunnel = ref new Linphone::Core::Tunnel(lt);
	}
	return tunnel;
}

void Linphone::Core::LinphoneCore::SetUserAgent(Platform::String^ name, Platform::String^ version) 
{
	API_LOCK;
	const char* ua = Linphone::Core::Utils::pstoccs(name);
	const char* v = Linphone::Core::Utils::pstoccs(version);
	linphone_core_set_user_agent(this->lc, ua, v);
	delete(v);
	delete(ua);
}

void Linphone::Core::LinphoneCore::SetCPUCount(int count) 
{
	API_LOCK;
	ms_set_cpu_count(count);
}

int Linphone::Core::LinphoneCore::GetMissedCallsCount() 
{
	API_LOCK;
	return linphone_core_get_missed_calls_count(this->lc);
}

void Linphone::Core::LinphoneCore::ResetMissedCallsCount() 
{
	API_LOCK;
	linphone_core_reset_missed_calls_count(this->lc);
}

void Linphone::Core::LinphoneCore::RefreshRegisters() 
{
	API_LOCK;
	linphone_core_refresh_registers(this->lc);
}

Platform::String^ Linphone::Core::LinphoneCore::GetVersion() 
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_core_get_version());
}

void Linphone::Core::LinphoneCore::SetAudioPort(int port) 
{
	API_LOCK;
	linphone_core_set_audio_port(this->lc, port);
}

void Linphone::Core::LinphoneCore::SetAudioPortRange(int minP, int maxP) 
{
	API_LOCK;
	linphone_core_set_audio_port_range(this->lc, minP, maxP);
}

void Linphone::Core::LinphoneCore::SetIncomingTimeout(int timeout) 
{
	API_LOCK;
	linphone_core_set_inc_timeout(this->lc, timeout);
}

void Linphone::Core::LinphoneCore::SetInCallTimeout(int timeout) 
{
	API_LOCK;
	linphone_core_set_in_call_timeout(this->lc, timeout);
}

void Linphone::Core::LinphoneCore::SetPrimaryContact(Platform::String^ displayName, Platform::String^ userName) 
{
	API_LOCK;
	const char* dn = Linphone::Core::Utils::pstoccs(displayName);
	const char* un = Linphone::Core::Utils::pstoccs(userName);
	::LinphoneAddress* addr = linphone_core_get_primary_contact_parsed(this->lc);
	if (addr != nullptr) {
		linphone_address_set_display_name(addr, dn);
		linphone_address_set_username(addr, un);
		char* contact = linphone_address_as_string(addr);
		linphone_core_set_primary_contact(this->lc, contact);
	}
	delete(un);
	delete(dn);
}

Platform::Boolean Linphone::Core::LinphoneCore::GetUseSipInfoForDTMFs() 
{
	API_LOCK;
	return (linphone_core_get_use_info_for_dtmf(this->lc) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCore::GetUseRFC2833ForDTMFs() 
{
	API_LOCK;
	return (linphone_core_get_use_rfc2833_for_dtmf(this->lc) == TRUE);
}

void Linphone::Core::LinphoneCore::SetUseSipInfoForDTMFs(Platform::Boolean use) 
{
	API_LOCK;
	linphone_core_set_use_info_for_dtmf(this->lc, use);
}

void Linphone::Core::LinphoneCore::SetUseRFC2833ForDTMFs(Platform::Boolean use) 
{
	API_LOCK;
	linphone_core_set_use_rfc2833_for_dtmf(this->lc, use);
}

Linphone::Core::LpConfig^ Linphone::Core::LinphoneCore::GetConfig()
{
	API_LOCK;
	::LpConfig *config = linphone_core_get_config(this->lc);
	return (Linphone::Core::LpConfig^)Linphone::Core::Utils::CreateLpConfig(config);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsVideoSupported()
{
	API_LOCK;
	return (linphone_core_video_supported(this->lc) == TRUE);
}

Linphone::Core::VideoPolicy^ Linphone::Core::LinphoneCore::GetVideoPolicy()
{
	API_LOCK;
	const ::LinphoneVideoPolicy *lvp = linphone_core_get_video_policy(this->lc);
	return ref new Linphone::Core::VideoPolicy((lvp->automatically_initiate == TRUE), (lvp->automatically_accept == TRUE));
}

void Linphone::Core::LinphoneCore::SetVideoPolicy(Linphone::Core::VideoPolicy^ policy)
{
	API_LOCK;
	::LinphoneVideoPolicy lvp;
	lvp.automatically_initiate = policy->AutomaticallyInitiate;
	lvp.automatically_accept = policy->AutomaticallyAccept;
	linphone_core_set_video_policy(this->lc, &lvp);
}

Windows::Foundation::Collections::IVector<Platform::Object^>^ Linphone::Core::LinphoneCore::GetSupportedVideoSizes()
{
	API_LOCK;
	Vector<Object^>^ sizes = ref new Vector<Object^>();
	const MSVideoSizeDef *sizesList = linphone_core_get_supported_video_sizes(this->lc);
	while (sizesList->name != NULL) {
		Platform::String^ sizeName = Utils::cctops(sizesList->name);
		Linphone::Core::VideoSize^ size = ref new Linphone::Core::VideoSize(sizesList->vsize.width, sizesList->vsize.height, sizeName);
		sizes->Append(size);
		sizesList++;
	}
	return sizes;
}

Linphone::Core::VideoSize^ Linphone::Core::LinphoneCore::GetPreferredVideoSize()
{
	Linphone::Core::VideoSize^ size = nullptr;
	API_LOCK;
	const MSVideoSizeDef *sizesList = linphone_core_get_supported_video_sizes(this->lc);
	MSVideoSize vsize = linphone_core_get_preferred_video_size(this->lc);
	while (sizesList->name != NULL) {
		if ((sizesList->vsize.width == vsize.width) && (sizesList->vsize.height == vsize.height)) {
			Platform::String^ sizeName = Utils::cctops(sizesList->name);
			size = ref new Linphone::Core::VideoSize(vsize.width, vsize.height, sizeName);
			break;
		}
		sizesList++;
	}
	if (size == nullptr) {
		size = ref new Linphone::Core::VideoSize(vsize.width, vsize.height);
	}
	return size;
}

Platform::String^ Linphone::Core::LinphoneCore::GetPreferredVideoSizeName()
{
	API_LOCK;
	char *cSizeName = linphone_core_get_preferred_video_size_name(this->lc);
	Platform::String^ sizeName = Utils::cctops(cSizeName);
	ms_free(cSizeName);
	return sizeName;
}

void Linphone::Core::LinphoneCore::SetPreferredVideoSize(Linphone::Core::VideoSize^ size)
{
	if (size->Name != nullptr) {
		const char *ccname = Utils::pstoccs(size->Name);
		API_LOCK;
		linphone_core_set_preferred_video_size_by_name(this->lc, ccname);
		delete ccname;
	} else {
		SetPreferredVideoSize(size->Width, size->Height);
	}
}

void Linphone::Core::LinphoneCore::SetPreferredVideoSize(int width, int height)
{
	MSVideoSize vsize;
	vsize.width = width;
	vsize.height = height;
	API_LOCK;
	linphone_core_set_preferred_video_size(this->lc, vsize);
}

void Linphone::Core::LinphoneCore::SetPreferredVideoSizeByName(Platform::String^ sizeName)
{
	API_LOCK;
	linphone_core_set_preferred_video_size_by_name(this->lc, Utils::pstoccs(sizeName));
}

Windows::Foundation::Collections::IVector<Platform::Object^>^ Linphone::Core::LinphoneCore::GetVideoDevices()
{
	API_LOCK;
	Vector<Object^>^ devices = ref new Vector<Object^>();
	const char **lvds = linphone_core_get_video_devices(this->lc);
	while (*lvds != NULL) {
		Platform::String^ device = Utils::cctops(*lvds);
		devices->Append(device);
		lvds++;
	}
	return devices;
}

Platform::String^ Linphone::Core::LinphoneCore::GetVideoDevice()
{
	Platform::String^ device = nullptr;
	API_LOCK;
	const char *ccname = linphone_core_get_video_device(this->lc);
	if (ccname != NULL) {
		device = Utils::cctops(ccname);
	}
	return device;
}

void Linphone::Core::LinphoneCore::SetVideoDevice(Platform::String^ device)
{
	const char *ccname = Utils::pstoccs(device);
	API_LOCK;
	linphone_core_set_video_device(this->lc, ccname);
	delete ccname;
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetVideoCodecs()
{
	API_LOCK;
	IVector<Object^>^ codecs = ref new Vector<Object^>();
	const MSList *codecslist = linphone_core_get_video_codecs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *codecsPtr = new RefToPtrProxy<IVector<Object^>^>(codecs);
	ms_list_for_each2(codecslist, AddCodecToVector, codecsPtr);
	return codecs;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsVideoEnabled()
{
	API_LOCK;
	return (linphone_core_video_enabled(this->lc) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsVideoCaptureEnabled()
{
	API_LOCK;
	return (linphone_core_video_capture_enabled(this->lc) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsVideoDisplayEnabled()
{
	API_LOCK;
	return (linphone_core_video_display_enabled(this->lc) == TRUE);
}

void Linphone::Core::LinphoneCore::EnableVideo(Platform::Boolean enableCapture, Platform::Boolean enableDisplay)
{
	API_LOCK;
	linphone_core_enable_video(this->lc, enableCapture, enableDisplay);
}

void Linphone::Core::LinphoneCore::EnableVideoCapture(Platform::Boolean enable) 
{
	API_LOCK;
	linphone_core_enable_video_capture(this->lc, enable);
}

void Linphone::Core::LinphoneCore::EnableVideoDisplay(Platform::Boolean enable) 
{
	API_LOCK;
	linphone_core_enable_video_display(this->lc, enable);
}

int Linphone::Core::LinphoneCore::GetNativeVideoWindowId()
{
	API_LOCK;
	return Globals::Instance->VideoRenderer->GetNativeWindowId();
}

int Linphone::Core::LinphoneCore::GetCameraSensorRotation()
{
	API_LOCK;
	return linphone_core_get_camera_sensor_rotation(this->lc);
}

Platform::Boolean Linphone::Core::LinphoneCore::IsSelfViewEnabled()
{
	API_LOCK;
	return (linphone_core_self_view_enabled(this->lc) == TRUE);
}

void Linphone::Core::LinphoneCore::EnableSelfView(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_self_view(this->lc, enable);
}

Linphone::Core::LinphoneChatRoom^ Linphone::Core::LinphoneCore::GetChatRoom(Linphone::Core::LinphoneAddress^ address)
{
	API_LOCK;
	::LinphoneChatRoom * chatRoom = linphone_core_get_chat_room(this->lc, address->address);
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatRoom^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatRoom^> *>(linphone_chat_room_get_user_data(chatRoom));
	Linphone::Core::LinphoneChatRoom^ lChatRoom = (proxy) ? proxy->Ref() : nullptr;
	if (lChatRoom == nullptr) {
		lChatRoom = (Linphone::Core::LinphoneChatRoom^) Linphone::Core::Utils::CreateLinphoneChatRoom(chatRoom);
	}
	return lChatRoom;
}

Linphone::Core::LinphoneChatRoom^ Linphone::Core::LinphoneCore::GetChatRoomFromUri(Platform::String^ to)
{
	API_LOCK;
	::LinphoneChatRoom * chatRoom = linphone_core_get_chat_room_from_uri(this->lc, Linphone::Core::Utils::pstoccs(to));
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatRoom^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatRoom^> *>(linphone_chat_room_get_user_data(chatRoom));
	Linphone::Core::LinphoneChatRoom^ lChatRoom = (proxy) ? proxy->Ref() : nullptr;
	if (lChatRoom == nullptr) {
		lChatRoom = (Linphone::Core::LinphoneChatRoom^) Linphone::Core::Utils::CreateLinphoneChatRoom(chatRoom);
	}
	return lChatRoom;
}

void Linphone::Core::LinphoneCore::SetLogCollectionUploadServerUrl(Platform::String^ url)
{
	API_LOCK;
	const char *curl = Linphone::Core::Utils::pstoccs(url);
	linphone_core_set_log_collection_upload_server_url(this->lc, curl);
	delete(curl);
}

void Linphone::Core::LinphoneCore::UploadLogCollection()
{
	API_LOCK;
	linphone_core_upload_log_collection(this->lc);
}

void Linphone::Core::LinphoneCore::SetDeviceRotation(int rotation)
{
	API_LOCK;
	linphone_core_set_device_rotation(this->lc, rotation);
}

void Linphone::Core::LinphoneCore::NotifyMute(bool isMuted)
{
	API_LOCK;
	Globals::Instance->CallController->NotifyMute(isMuted);
	MuteMic(isMuted);
}

void Linphone::Core::LinphoneCore::SetChatDatabasePath(Platform::String^ chatDatabasePath)
{
	API_LOCK;
	linphone_core_set_chat_database_path(this->lc, Linphone::Core::Utils::pstoccs(chatDatabasePath));
}

static void AddChatRoomListToVector(void *vRoom, void *vector)
{
	::LinphoneChatRoom *chatRoom = (LinphoneChatRoom*) vRoom;
	Linphone::Core::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Core::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ rooms = (list) ? list->Ref() : nullptr;
	Linphone::Core::LinphoneChatRoom^ room = (Linphone::Core::LinphoneChatRoom^) Linphone::Core::Utils::CreateLinphoneChatRoom(chatRoom);
	rooms->Append(room);
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetChatRooms()
{
	API_LOCK;
	IVector<Object^>^ rooms = ref new Vector<Object^>();
	MSList* roomList = linphone_core_get_chat_rooms(this->lc);
	RefToPtrProxy<IVector<Object^>^> *roomsPtr = new RefToPtrProxy<IVector<Object^>^>(rooms);
	ms_list_for_each2(roomList, AddChatRoomListToVector, roomsPtr);
	return rooms;
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
	Linphone::Core::LinphoneCallState state = (Linphone::Core::LinphoneCallState) cstate;
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
	Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
	if (lCall == nullptr) {
		call = linphone_call_ref(call);
		lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
	}
		
	Linphone::Core::CallController^ callController = Linphone::Core::Globals::Instance->CallController;
	if (state == Linphone::Core::LinphoneCallState::IncomingReceived) {
		Platform::String^ name = lCall->RemoteAddress->DisplayName;
		if (name == nullptr || name->Length() <= 0) 
		{
			name = lCall->RemoteAddress->UserName;
		}
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = callController->OnIncomingCallReceived(lCall, name, lCall->RemoteAddress->AsStringUriOnly(), callController->IncomingCallViewDismissed);
		lCall->CallContext = platformCall;
	} 
	else if (state == Linphone::Core::LinphoneCallState::OutgoingProgress) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = callController->NewOutgoingCall(lCall->RemoteAddress->AsStringUriOnly());
		lCall->CallContext = platformCall;
	}
	else if (state == Linphone::Core::LinphoneCallState::CallEnd || state == Linphone::Core::LinphoneCallState::Error) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = (Windows::Phone::Networking::Voip::VoipPhoneCall^) lCall->CallContext;
		if (platformCall != nullptr)
			platformCall->NotifyCallEnded();

		if (callController->IncomingCallViewDismissed != nullptr) {
			// When we receive a call with PN, call the callback to kill the agent process in case the caller stops the call before user accepts/denies it
			callController->IncomingCallViewDismissed();
		}
	}
	else if (state == Linphone::Core::LinphoneCallState::Paused || state == Linphone::Core::LinphoneCallState::PausedByRemote) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = (Windows::Phone::Networking::Voip::VoipPhoneCall^) lCall->CallContext;
		if (platformCall != nullptr)
			platformCall->NotifyCallHeld();
	}
	else if (state == Linphone::Core::LinphoneCallState::StreamsRunning) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = (Windows::Phone::Networking::Voip::VoipPhoneCall^) lCall->CallContext;
		if (platformCall == nullptr) {
			// If CallContext is null here, it is because we have an incoming call using the custom incoming call view so create the VoipPhoneCall now
			platformCall = callController->NewIncomingCallForCustomIncomingCallView(lCall->RemoteAddress->DisplayName);
			lCall->CallContext = platformCall;
		}
		if (lCall->CameraEnabled) {
			platformCall->CallMedia = VoipCallMedia::Audio | VoipCallMedia::Video;
		} else {
			platformCall->CallMedia = VoipCallMedia::Audio;
		}
		platformCall->NotifyCallActive();
	}
	
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr) {
		listener->CallState(lCall, state, Linphone::Core::Utils::cctops(msg));
	}
}

void registration_state_changed(::LinphoneCore *lc, ::LinphoneProxyConfig *cfg, ::LinphoneRegistrationState cstate, const char *msg)
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr) {
		Linphone::Core::RegistrationState state = (Linphone::Core::RegistrationState) cstate;
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneProxyConfig^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneProxyConfig^> *>(linphone_proxy_config_get_user_data(cfg));
		Linphone::Core::LinphoneProxyConfig^ config = (proxy) ? proxy->Ref() : nullptr;
		listener->RegistrationState(config, state, Linphone::Core::Utils::cctops(msg));
	}
}

void global_state_changed(::LinphoneCore *lc, ::LinphoneGlobalState gstate, const char *msg)
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr) {
		Linphone::Core::GlobalState state = (Linphone::Core::GlobalState) gstate;
		listener->GlobalState(state, Linphone::Core::Utils::cctops(msg));
	}
}

void auth_info_requested(LinphoneCore *lc, const char *realm, const char *username, const char *domain) 
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr) {
		listener->AuthInfoRequested(Linphone::Core::Utils::cctops(realm), Linphone::Core::Utils::cctops(username), Linphone::Core::Utils::cctops(domain));
	}
}

void dtmf_received(LinphoneCore *lc, LinphoneCall *call, int dtmf)
{
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
	Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	if (lCall != nullptr) {
		Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
		if (listener != nullptr) {
			char cdtmf = (char)dtmf;
			char16 wdtmf;
			mbtowc(&wdtmf, &cdtmf, 1);
			listener->DTMFReceived(lCall, wdtmf);
		}
	}
}

void call_encryption_changed(LinphoneCore *lc, LinphoneCall *call, bool_t on, const char *authentication_token)
{
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
	Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	if (lCall != nullptr) {
		Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
		if (listener != nullptr) {
			listener->CallEncryptionChanged(lCall, (on == TRUE), Linphone::Core::Utils::cctops(authentication_token));
		}
	}
}

void call_stats_updated(LinphoneCore *lc, LinphoneCall *call, const LinphoneCallStats *stats)
{
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
	Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	Linphone::Core::LinphoneCallStats^ lStats = (Linphone::Core::LinphoneCallStats^)Linphone::Core::Utils::CreateLinphoneCallStats((void *)stats);
	if ((lCall != nullptr) && (lStats != nullptr)) {
		Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
		if (listener != nullptr) {
			listener->CallStatsUpdated(lCall, lStats);
		}
	}
}

void message_received(LinphoneCore *lc, LinphoneChatRoom* chat_room, LinphoneChatMessage* message) 
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessage^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessage^> *>(linphone_chat_message_get_user_data(message));
		Linphone::Core::LinphoneChatMessage^ lMessage = (proxy) ? proxy->Ref() : nullptr;
		if (lMessage == nullptr) {
			lMessage = (Linphone::Core::LinphoneChatMessage^)Linphone::Core::Utils::CreateLinphoneChatMessage(message);
		}

		listener->MessageReceived(lMessage);
	}
}

void composing_received(LinphoneCore *lc, LinphoneChatRoom *room) 
{
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatRoom^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatRoom^> *>(linphone_chat_room_get_user_data(room));
		Linphone::Core::LinphoneChatRoom^ lRoom = (proxy) ? proxy->Ref() : nullptr;
		if (lRoom == nullptr) {
			lRoom = (Linphone::Core::LinphoneChatRoom^)Linphone::Core::Utils::CreateLinphoneChatRoom(room);
		}
		listener->ComposingReceived(lRoom);
	}
}

void file_transfer_progress_indication(LinphoneCore *lc, LinphoneChatMessage *message, const LinphoneContent *content, size_t offset, size_t total) {
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessage^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessage^> *>(linphone_chat_message_get_user_data(message));
		Linphone::Core::LinphoneChatMessage^ lMessage = (proxy) ? proxy->Ref() : nullptr;
		if (lMessage == nullptr) {
			lMessage = (Linphone::Core::LinphoneChatMessage^)Linphone::Core::Utils::CreateLinphoneChatMessage(message);
		}
		listener->FileTransferProgressIndication(lMessage, (int)offset, (int)total);
	}
}

void log_collection_upload_progress_indication(LinphoneCore *lc, size_t offset, size_t total) {
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		listener->LogUploadProgressIndication((int)offset, (int)total);
	}
}

void log_collection_upload_state_changed(LinphoneCore *lc, ::LinphoneCoreLogCollectionUploadState state, const char *info) {
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		listener->LogUploadStatusChanged((Linphone::Core::LinphoneCoreLogCollectionUploadState)state, info ? Linphone::Core::Utils::cctops(info) : nullptr);
	}
}

Linphone::Core::LinphoneCore::LinphoneCore(LinphoneCoreListener^ coreListener) :
	lc(nullptr),
	listener(coreListener)
{

}

Linphone::Core::LinphoneCore::LinphoneCore(LinphoneCoreListener^ coreListener, LpConfig^ config) :
	lc(nullptr),
	listener(coreListener),
	config(config),
	isIterateEnabled(false)
{
}

void Linphone::Core::LinphoneCore::IterateEnabled::set(Platform::Boolean value)
{
	API_LOCK;
	if (isIterateEnabled && !value && IterateTimer)
	{
		// Disable the iterate
		IterateTimer->Cancel();
	}
	else if (!isIterateEnabled && value) 
	{
		// Enable the iterate
		TimeSpan period;
		period.Duration = 20 * 10000;
		IterateTimer = ThreadPoolTimer::CreatePeriodicTimer(
			ref new TimerElapsedHandler([this](ThreadPoolTimer^ source)
		{
			if (source == IterateTimer) {
				if (GlobalApiLock::Instance()->TryLock()) {
					linphone_core_iterate(this->lc);
					GlobalApiLock::Instance()->Unlock();
				}
			}
		}), period);
	}
	isIterateEnabled = value;
}

Platform::Boolean Linphone::Core::LinphoneCore::IterateEnabled::get()
{
	return isIterateEnabled;
}

void Linphone::Core::LinphoneCore::Init()
{
	LinphoneCoreVTable *vtable = (LinphoneCoreVTable*) malloc(sizeof(LinphoneCoreVTable));
	memset (vtable, 0, sizeof(LinphoneCoreVTable));
	vtable->global_state_changed = global_state_changed;
	vtable->registration_state_changed = registration_state_changed;
	vtable->call_state_changed = call_state_changed;
	vtable->auth_info_requested = auth_info_requested;
	vtable->dtmf_received = dtmf_received;
	vtable->call_encryption_changed = call_encryption_changed;
	vtable->call_stats_updated = call_stats_updated;
	vtable->message_received = message_received;
	vtable->is_composing_received = composing_received;
	vtable->file_transfer_progress_indication = file_transfer_progress_indication;
	vtable->log_collection_upload_progress_indication = log_collection_upload_progress_indication;
	vtable->log_collection_upload_state_changed = log_collection_upload_state_changed;

	this->lc = linphone_core_new_with_config(vtable, config ? config->config : NULL, NULL);
	RefToPtrProxy<LinphoneCore^> *proxy = new RefToPtrProxy<LinphoneCore^>(this);
	linphone_core_set_user_data(this->lc, proxy);

	linphone_core_set_ring(this->lc, nullptr);
	RefToPtrProxy<Mediastreamer2::WP8Video::IVideoRenderer^> *renderer = new RefToPtrProxy<Mediastreamer2::WP8Video::IVideoRenderer^>(Globals::Instance->VideoRenderer);
	linphone_core_set_native_video_window_id(this->lc, (unsigned long)renderer);
}

Linphone::Core::LinphoneCore::~LinphoneCore()
{
	API_LOCK;
	RefToPtrProxy<LinphoneCore^> *proxy = reinterpret_cast< RefToPtrProxy<LinphoneCore^> *>(linphone_core_get_user_data(this->lc));
	delete proxy;
}
