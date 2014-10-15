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

void Linphone::Core::LinphoneCore::ClearProxyConfigs()
{
	TRACE; gApiLock.Lock();
	linphone_core_clear_proxy_config(this->lc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::AddProxyConfig(Linphone::Core::LinphoneProxyConfig^ proxyCfg)
{
	TRACE; gApiLock.Lock();
	linphone_core_add_proxy_config(this->lc, proxyCfg->proxy_config);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetDefaultProxyConfig(Linphone::Core::LinphoneProxyConfig^ proxyCfg)
{
	TRACE; gApiLock.Lock();
	linphone_core_set_default_proxy(this->lc, proxyCfg->proxy_config);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneProxyConfig^ Linphone::Core::LinphoneCore::GetDefaultProxyConfig()
{
	TRACE; gApiLock.Lock();
	LinphoneProxyConfig^ defaultProxy = nullptr;
	::LinphoneProxyConfig *proxy=NULL;
	linphone_core_get_default_proxy(this->lc,&proxy);
	if (proxy != nullptr) {
		defaultProxy = ref new Linphone::Core::LinphoneProxyConfig(proxy);
	}
	gApiLock.Unlock();
	return defaultProxy;
}

Linphone::Core::LinphoneProxyConfig^ Linphone::Core::LinphoneCore::CreateEmptyProxyConfig()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneProxyConfig^ proxyConfig = ref new Linphone::Core::LinphoneProxyConfig();
	gApiLock.Unlock();
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
	TRACE; gApiLock.Lock();
	IVector<Object^>^ proxyconfigs = ref new Vector<Object^>();
	const MSList *configList = linphone_core_get_proxy_config_list(this->lc);
	RefToPtrProxy<IVector<Object^>^> *proxyConfigPtr = new RefToPtrProxy<IVector<Object^>^>(proxyconfigs);
	ms_list_for_each2(configList, AddProxyConfigToVector, proxyConfigPtr);
	gApiLock.Unlock();
	return proxyconfigs;
}

void Linphone::Core::LinphoneCore::ClearAuthInfos() 
{
	TRACE; gApiLock.Lock();
	linphone_core_clear_all_auth_info(this->lc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::AddAuthInfo(Linphone::Core::LinphoneAuthInfo^ info) 
{
	TRACE; gApiLock.Lock();
	linphone_core_add_auth_info(this->lc, info->auth_info);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneAuthInfo^ Linphone::Core::LinphoneCore::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneAuthInfo^ authInfo = ref new Linphone::Core::LinphoneAuthInfo(username, userid, password, ha1, realm, domain);
	gApiLock.Unlock();
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
	TRACE; gApiLock.Lock();
	IVector<Object^>^ authInfos = ref new Vector<Object^>();
	const MSList *authlist = linphone_core_get_auth_info_list(this->lc);
	RefToPtrProxy<IVector<Object^>^> *authInfosPtr = new RefToPtrProxy<IVector<Object^>^>(authInfos);
	ms_list_for_each2(authlist, AddAuthInfoToVector, authInfosPtr);
	gApiLock.Unlock();
	return authInfos;
}

void Linphone::Core::LinphoneCore::Destroy() 
{
	TRACE; gApiLock.Lock();
	linphone_core_destroy(this->lc);
	IterateEnabled = false;
	gApiLock.Unlock();
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneCore::InterpretURL(Platform::String^ destination) 
{
	TRACE; gApiLock.Lock();
	const char* url = Linphone::Core::Utils::pstoccs(destination);
	Linphone::Core::LinphoneAddress^ addr = (Linphone::Core::LinphoneAddress^) Linphone::Core::Utils::CreateLinphoneAddress(linphone_core_interpret_url(this->lc, url));
	delete(url);
	gApiLock.Unlock();
	return addr;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::Invite(Platform::String^ destination) 
{
	TRACE; gApiLock.Lock();
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
	gApiLock.Unlock();
	return lCall;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::InviteAddress(Linphone::Core::LinphoneAddress^ destination) 
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCall^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_invite_address(this->lc, destination->address);
	call = linphone_call_ref(call);
	if (call != NULL) {
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_data(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Core::LinphoneCall^) Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	gApiLock.Unlock();
	return lCall;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::InviteAddressWithParams(Linphone::Core::LinphoneAddress^ destination, LinphoneCallParams^ params) 
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCall^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_invite_address_with_params(this->lc, destination->address, params->params);
	call = linphone_call_ref(call);
	if (call != NULL) {
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Core::LinphoneCall^) Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	gApiLock.Unlock();
	return lCall;
}

void Linphone::Core::LinphoneCore::TerminateCall(Linphone::Core::LinphoneCall^ call) 
{
	TRACE; gApiLock.Lock();
	linphone_core_terminate_call(this->lc, call->call);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::DeclineCall(Linphone::Core::LinphoneCall^ call, DeclineReason reason)
{
	TRACE; gApiLock.Lock();
	linphone_core_decline_call(this->lc, call->call, (LinphoneReason)reason);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::GetCurrentCall() 
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCall^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_get_current_call(this->lc);
	if (call != nullptr) {
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
	}
	gApiLock.Unlock();
	return lCall;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsInCall() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean inCall = (linphone_core_in_call(this->lc) == TRUE);
	gApiLock.Unlock();
	return inCall;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsIncomingInvitePending() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean invitePending = (linphone_core_inc_invite_pending(this->lc) == TRUE);
	gApiLock.Unlock();
	return invitePending;
}

void Linphone::Core::LinphoneCore::AcceptCall(Linphone::Core::LinphoneCall^ call) 
{
	TRACE; gApiLock.Lock();
	linphone_core_accept_call(this->lc, call->call);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::AcceptCallWithParams(Linphone::Core::LinphoneCall^ call, Linphone::Core::LinphoneCallParams^ params) 
{
	TRACE; gApiLock.Lock();
	linphone_core_accept_call_with_params(this->lc, call->call, params->params);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::AcceptCallUpdate(Linphone::Core::LinphoneCall^ call, Linphone::Core::LinphoneCallParams^ params) 
{
	TRACE; gApiLock.Lock();
	linphone_core_accept_call_update(this->lc, call->call, params->params);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::DeferCallUpdate(Linphone::Core::LinphoneCall^ call) 
{
	TRACE; gApiLock.Lock();
	linphone_core_defer_call_update(this->lc, call->call);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::UpdateCall(Linphone::Core::LinphoneCall^ call, Linphone::Core::LinphoneCallParams^ params) 
{
	TRACE; gApiLock.Lock();
	if (params != nullptr) {
		linphone_core_update_call(this->lc, call->call, params->params);
	} else {
		linphone_core_update_call(this->lc, call->call, nullptr);
	}
	gApiLock.Unlock();
}

Linphone::Core::LinphoneCallParams^ Linphone::Core::LinphoneCore::CreateDefaultCallParameters() 
{
	TRACE; gApiLock.Lock();
	Linphone::Core::LinphoneCallParams^ params = (Linphone::Core::LinphoneCallParams^) Linphone::Core::Utils::CreateLinphoneCallParams(linphone_core_create_default_call_parameters(this->lc));
	gApiLock.Unlock();
	return params;
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
	TRACE; gApiLock.Lock();
	IVector<Object^>^ logs = ref new Vector<Object^>();
	const MSList* logslist = linphone_core_get_call_logs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *logsptr = new RefToPtrProxy<IVector<Object^>^>(logs);
	ms_list_for_each2(logslist, AddLogToVector, logsptr);
	gApiLock.Unlock();
	return logs;
}

void Linphone::Core::LinphoneCore::ClearCallLogs() 
{
	TRACE; gApiLock.Lock();
	linphone_core_clear_call_logs(this->lc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::RemoveCallLog(Linphone::Core::LinphoneCallLog^ log) 
{
	TRACE; gApiLock.Lock();
	linphone_core_remove_call_log(this->lc, log->callLog);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetNetworkReachable(Platform::Boolean isReachable) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_network_reachable(this->lc, isReachable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCore::IsNetworkReachable() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean networkReachable = (linphone_core_is_network_reachable(this->lc) == TRUE);
	gApiLock.Unlock();
	return networkReachable;
}

void Linphone::Core::LinphoneCore::SetMicrophoneGain(float gain) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_mic_gain_db(this->lc, gain);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetPlaybackGain(float gain) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_playback_gain_db(this->lc, gain);
	gApiLock.Unlock();
}

float Linphone::Core::LinphoneCore::GetPlaybackGain() 
{
	TRACE; gApiLock.Lock();
	float gain = linphone_core_get_playback_gain_db(this->lc);
	gApiLock.Unlock();
	return gain;
}

void Linphone::Core::LinphoneCore::SetPlayLevel(int level) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_play_level(this->lc, level);
	gApiLock.Unlock();
}

int Linphone::Core::LinphoneCore::GetPlayLevel() 
{
	TRACE; gApiLock.Lock();
	int level = linphone_core_get_play_level(this->lc);
	gApiLock.Unlock();
	return level;
}

void Linphone::Core::LinphoneCore::MuteMic(Platform::Boolean isMuted) 
{
	TRACE; gApiLock.Lock();
	linphone_core_mute_mic(this->lc, isMuted);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMicMuted() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean muted = (linphone_core_is_mic_muted(this->lc) == TRUE);
	gApiLock.Unlock();
	return muted;
}

void Linphone::Core::LinphoneCore::SendDTMF(char16 number) 
{
	TRACE; gApiLock.Lock();
	char conv[4];
	if (wctomb(conv, number) == 1) {
		linphone_core_send_dtmf(this->lc, conv[0]);
	}
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::PlayDTMF(char16 number, int duration) 
{
	TRACE; gApiLock.Lock();
	char conv[4];
	if (wctomb(conv, number) == 1) {
		linphone_core_play_dtmf(this->lc, conv[0], duration);
	}
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::StopDTMF() 
{
	TRACE; gApiLock.Lock();
	linphone_core_stop_dtmf(this->lc);
	gApiLock.Unlock();
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate, int channels) 
{
	TRACE; gApiLock.Lock();
	const char* type = Linphone::Core::Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, channels);
	delete type;
	Linphone::Core::PayloadType^ payloadType = ref new Linphone::Core::PayloadType(pt);
	gApiLock.Unlock();
	return payloadType;
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCore::FindPayloadType(Platform::String^ mime, int clockRate) 
{
	TRACE; gApiLock.Lock();
	const char* type = Linphone::Core::Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, LINPHONE_FIND_PAYLOAD_IGNORE_CHANNELS);
	delete type;
	Linphone::Core::PayloadType^ payloadType = ref new Linphone::Core::PayloadType(pt);
	gApiLock.Unlock();
	return payloadType;
}

bool Linphone::Core::LinphoneCore::PayloadTypeEnabled(PayloadType^ pt)
{
	TRACE; gApiLock.Lock();
	::PayloadType *payload = pt->payload;
	bool enabled = (linphone_core_payload_type_enabled(this->lc, payload) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

void Linphone::Core::LinphoneCore::EnablePayloadType(PayloadType^ pt, Platform::Boolean enable) 
{
	TRACE; gApiLock.Lock();
	::PayloadType *payload = pt->payload;
	linphone_core_enable_payload_type(this->lc, payload, enable);
	gApiLock.Unlock();
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
	TRACE; gApiLock.Lock();
	IVector<Object^>^ codecs = ref new Vector<Object^>();
	const MSList *codecslist = linphone_core_get_audio_codecs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *codecsPtr = new RefToPtrProxy<IVector<Object^>^>(codecs);
	ms_list_for_each2(codecslist, AddCodecToVector, codecsPtr);
	gApiLock.Unlock();
	return codecs;
}

void Linphone::Core::LinphoneCore::EnableEchoCancellation(Platform::Boolean enable) 
{
	TRACE; gApiLock.Lock();
	linphone_core_enable_echo_cancellation(this->lc, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCore::IsEchoCancellationEnabled() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_core_echo_cancellation_enabled(this->lc) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsEchoLimiterEnabled() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_core_echo_limiter_enabled(this->lc) == TRUE);
	gApiLock.Unlock();
	return enabled;
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
	TRACE; gApiLock.Lock();
	Linphone::Core::EchoCalibrationData *data = new Linphone::Core::EchoCalibrationData();
	linphone_core_start_echo_calibration(this->lc, EchoCalibrationCallback, EchoCalibrationAudioInit, EchoCalibrationAudioUninit, data);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::EnableEchoLimiter(Platform::Boolean enable) 
{
	TRACE; gApiLock.Lock();
	linphone_core_enable_echo_limiter(this->lc, enable);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetSignalingTransportsPorts(Transports^ t) 
{
	TRACE; gApiLock.Lock();
	::LCSipTransports transports;
	memset(&transports, 0, sizeof(LCSipTransports));
	transports.udp_port = t->UDP;
	transports.tcp_port = t->TCP;
	transports.tls_port = t->TLS;
	linphone_core_set_sip_transports(this->lc, &transports);
	gApiLock.Unlock();
}

Linphone::Core::Transports^ Linphone::Core::LinphoneCore::GetSignalingTransportsPorts()
{
	TRACE; gApiLock.Lock();
	::LCSipTransports transports;
	linphone_core_get_sip_transports(this->lc, &transports);
	Linphone::Core::Transports^ t = ref new Linphone::Core::Transports(transports.udp_port, transports.tcp_port, transports.tls_port);
	gApiLock.Unlock();
	return t;
}

void Linphone::Core::LinphoneCore::EnableIPv6(Platform::Boolean enable) 
{
	TRACE; gApiLock.Lock();
	linphone_core_enable_ipv6(this->lc, enable);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status) 
{
	TRACE; gApiLock.Lock();
	const char* ac = Linphone::Core::Utils::pstoccs(alternativeContact);
	linphone_core_set_presence_info(this->lc, minuteAway, ac, (LinphoneOnlineStatus) status);
	delete(ac);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetStunServer(Platform::String^ stun) 
{
	TRACE; gApiLock.Lock();
	const char* stunserver = Linphone::Core::Utils::pstoccs(stun);
	linphone_core_set_stun_server(this->lc, stunserver);
	delete(stunserver);
	gApiLock.Unlock();
}

Platform::String^ Linphone::Core::LinphoneCore::GetStunServer() 
{
	TRACE; gApiLock.Lock();
	Platform::String^ server = Linphone::Core::Utils::cctops(linphone_core_get_stun_server(this->lc));
	gApiLock.Unlock();
	return server;
}

void Linphone::Core::LinphoneCore::SetFirewallPolicy(FirewallPolicy policy) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_firewall_policy(this->lc, (LinphoneFirewallPolicy) policy);
	gApiLock.Unlock();
}

Linphone::Core::FirewallPolicy Linphone::Core::LinphoneCore::GetFirewallPolicy() 
{
	TRACE; gApiLock.Lock();
	Linphone::Core::FirewallPolicy policy = (Linphone::Core::FirewallPolicy) linphone_core_get_firewall_policy(this->lc);
	gApiLock.Unlock();
	return policy;
}

void Linphone::Core::LinphoneCore::SetRootCA(Platform::String^ path) 
{
	TRACE; gApiLock.Lock();
	const char *ccPath = Utils::pstoccs(path);
	linphone_core_set_root_ca(this->lc, ccPath);
	delete ccPath;
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetUploadBandwidth(int bw) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_upload_bandwidth(this->lc, bw);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetDownloadBandwidth(int bw) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_download_bandwidth(this->lc, bw);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetDownloadPTime(int ptime) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_download_ptime(this->lc, ptime);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetUploadPTime(int ptime) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_upload_ptime(this->lc, ptime);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::EnableKeepAlive(Platform::Boolean enable)
{
	TRACE; gApiLock.Lock();
	linphone_core_enable_keep_alive(this->lc, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCore::IsKeepAliveEnabled() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_core_keep_alive_enabled(this->lc) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

void Linphone::Core::LinphoneCore::SetPlayFile(Platform::String^ path) 
{
	TRACE; gApiLock.Lock();
	const char* file = Linphone::Core::Utils::pstoccs(path);
	linphone_core_set_play_file(this->lc, file);
	delete(file);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCore::PauseCall(LinphoneCall^ call) 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean ok = (linphone_core_pause_call(this->lc, call->call) == 0);
	gApiLock.Unlock();
	return ok;
}

Platform::Boolean Linphone::Core::LinphoneCore::ResumeCall(LinphoneCall^ call) 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean ok = (linphone_core_resume_call(this->lc, call->call) == 0);
	gApiLock.Unlock();
	return ok;
}

Platform::Boolean Linphone::Core::LinphoneCore::PauseAllCalls() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean ok = (linphone_core_pause_all_calls(this->lc) == 0);
	gApiLock.Unlock();
	return ok;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsInConference() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean inConference = (linphone_core_is_in_conference(this->lc) == TRUE);
	gApiLock.Unlock();
	return inConference;
}

Platform::Boolean Linphone::Core::LinphoneCore::EnterConference() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean ok = (linphone_core_enter_conference(this->lc) == 0);
	gApiLock.Unlock();
	return ok;
}

void Linphone::Core::LinphoneCore::LeaveConference() 
{
	TRACE; gApiLock.Lock();
	linphone_core_leave_conference(this->lc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::AddToConference(LinphoneCall^ call) 
{
	TRACE; gApiLock.Lock();
	linphone_core_add_to_conference(this->lc, call->call);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::AddAllToConference() 
{
	TRACE; gApiLock.Lock();
	linphone_core_add_all_to_conference(this->lc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::RemoveFromConference(LinphoneCall^ call) 
{
	TRACE; gApiLock.Lock();
	linphone_core_remove_from_conference(this->lc, call->call);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::TerminateConference() 
{
	TRACE; gApiLock.Lock();
	linphone_core_terminate_conference(this->lc);
	gApiLock.Unlock();
}

int Linphone::Core::LinphoneCore::GetConferenceSize() 
{
	TRACE; gApiLock.Lock();
	int size = linphone_core_get_conference_size(this->lc);
	gApiLock.Unlock();
	return size;
}

void Linphone::Core::LinphoneCore::TerminateAllCalls() 
{
	TRACE; gApiLock.Lock();
	linphone_core_terminate_all_calls(this->lc);
	gApiLock.Unlock();
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
	TRACE; gApiLock.Lock();
	Vector<Object^>^ calls = ref new Vector<Object^>();
	const MSList *callsList = linphone_core_get_calls(this->lc);
	RefToPtrProxy<IVector<Object^>^> *callsPtr = new RefToPtrProxy<IVector<Object^>^>(calls);
	ms_list_for_each2(callsList, AddCallToVector, callsPtr);
	gApiLock.Unlock();
	return calls;
}

int Linphone::Core::LinphoneCore::GetCallsNb() 
{
	TRACE; gApiLock.Lock();
	int nb = linphone_core_get_calls_nb(this->lc);
	gApiLock.Unlock();
	return nb;
}

Linphone::Core::LinphoneCall^ Linphone::Core::LinphoneCore::FindCallFromUri(Platform::String^ uri) 
{
	TRACE; gApiLock.Lock();
	const char *curi = Utils::pstoccs(uri);
	::LinphoneCall *call = const_cast<::LinphoneCall *>(linphone_core_find_call_from_uri(this->lc, curi));
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
	Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
	}
	delete curi;
	gApiLock.Unlock();
	return lCall;
}

int Linphone::Core::LinphoneCore::GetMaxCalls() 
{
	TRACE; gApiLock.Lock();
	int max = linphone_core_get_max_calls(this->lc);
	gApiLock.Unlock();
	return max;
}

void Linphone::Core::LinphoneCore::SetMaxCalls(int max) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_max_calls(this->lc, max);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMyself(Platform::String^ uri) 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean myself = false;
	LinphoneProxyConfig^ lpc = GetDefaultProxyConfig();
	if (lpc != nullptr) {
		myself = uri->Equals(lpc->GetIdentity());
	}
	gApiLock.Unlock();
	return myself;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsSoundResourcesLocked() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean locked = (linphone_core_sound_resources_locked(this->lc) == TRUE);
	gApiLock.Unlock();
	return locked;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMediaEncryptionSupported(MediaEncryption menc) 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean supported = (linphone_core_media_encryption_supported(this->lc, (LinphoneMediaEncryption) menc) == TRUE);
	gApiLock.Unlock();
	return supported;
}

void Linphone::Core::LinphoneCore::SetMediaEncryption(MediaEncryption menc) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_media_encryption(this->lc, (LinphoneMediaEncryption) menc);
	gApiLock.Unlock();
}

Linphone::Core::MediaEncryption Linphone::Core::LinphoneCore::GetMediaEncryption() 
{
	TRACE; gApiLock.Lock();
	Linphone::Core::MediaEncryption enc = (Linphone::Core::MediaEncryption) linphone_core_get_media_encryption(this->lc);
	gApiLock.Unlock();
	return enc;
}

void Linphone::Core::LinphoneCore::SetMediaEncryptionMandatory(Platform::Boolean yesno) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_media_encryption_mandatory(this->lc, yesno);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCore::IsMediaEncryptionMandatory() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean mandatory = (linphone_core_is_media_encryption_mandatory(this->lc) == TRUE);
	gApiLock.Unlock();
	return mandatory;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsTunnelAvailable() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean available = (linphone_core_tunnel_available() == TRUE);
	gApiLock.Unlock();
	return available;
}

Linphone::Core::Tunnel^ Linphone::Core::LinphoneCore::GetTunnel()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::Tunnel^ tunnel = nullptr;
	LinphoneTunnel *lt = linphone_core_get_tunnel(this->lc);
	if (lt != nullptr) {
		tunnel = ref new Linphone::Core::Tunnel(lt);
	}
	gApiLock.Unlock();
	return tunnel;
}

void Linphone::Core::LinphoneCore::SetUserAgent(Platform::String^ name, Platform::String^ version) 
{
	TRACE; gApiLock.Lock();
	const char* ua = Linphone::Core::Utils::pstoccs(name);
	const char* v = Linphone::Core::Utils::pstoccs(version);
	linphone_core_set_user_agent(this->lc, ua, v);
	delete(v);
	delete(ua);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetCPUCount(int count) 
{
	TRACE; gApiLock.Lock();
	ms_set_cpu_count(count);
	gApiLock.Unlock();
}

int Linphone::Core::LinphoneCore::GetMissedCallsCount() 
{
	TRACE; gApiLock.Lock();
	int count = linphone_core_get_missed_calls_count(this->lc);
	gApiLock.Unlock();
	return count;
}

void Linphone::Core::LinphoneCore::ResetMissedCallsCount() 
{
	TRACE; gApiLock.Lock();
	linphone_core_reset_missed_calls_count(this->lc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::RefreshRegisters() 
{
	TRACE; gApiLock.Lock();
	linphone_core_refresh_registers(this->lc);
	gApiLock.Unlock();
}

Platform::String^ Linphone::Core::LinphoneCore::GetVersion() 
{
	TRACE; gApiLock.Lock();
	Platform::String^ version = Linphone::Core::Utils::cctops(linphone_core_get_version());
	gApiLock.Unlock();
	return version;
}

void Linphone::Core::LinphoneCore::SetAudioPort(int port) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_audio_port(this->lc, port);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetAudioPortRange(int minP, int maxP) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_audio_port_range(this->lc, minP, maxP);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetIncomingTimeout(int timeout) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_inc_timeout(this->lc, timeout);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetInCallTimeout(int timeout) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_in_call_timeout(this->lc, timeout);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetPrimaryContact(Platform::String^ displayName, Platform::String^ userName) 
{
	TRACE; gApiLock.Lock();
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
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCore::GetUseSipInfoForDTMFs() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean use = (linphone_core_get_use_info_for_dtmf(this->lc) == TRUE);
	gApiLock.Unlock();
	return use;
}

Platform::Boolean Linphone::Core::LinphoneCore::GetUseRFC2833ForDTMFs() 
{
	TRACE; gApiLock.Lock();
	Platform::Boolean use = (linphone_core_get_use_rfc2833_for_dtmf(this->lc) == TRUE);
	gApiLock.Unlock();
	return use;
}

void Linphone::Core::LinphoneCore::SetUseSipInfoForDTMFs(Platform::Boolean use) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_use_info_for_dtmf(this->lc, use);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetUseRFC2833ForDTMFs(Platform::Boolean use) 
{
	TRACE; gApiLock.Lock();
	linphone_core_set_use_rfc2833_for_dtmf(this->lc, use);
	gApiLock.Unlock();
}

Linphone::Core::LpConfig^ Linphone::Core::LinphoneCore::GetConfig()
{
	TRACE; gApiLock.Lock();
	::LpConfig *config = linphone_core_get_config(this->lc);
	Linphone::Core::LpConfig^ lpConfig = (Linphone::Core::LpConfig^)Linphone::Core::Utils::CreateLpConfig(config);
	gApiLock.Unlock();
	return lpConfig;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsVideoSupported()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean supported = (linphone_core_video_supported(this->lc) == TRUE);
	gApiLock.Unlock();
	return supported;
}

Linphone::Core::VideoPolicy^ Linphone::Core::LinphoneCore::GetVideoPolicy()
{
	TRACE; gApiLock.Lock();
	const ::LinphoneVideoPolicy *lvp = linphone_core_get_video_policy(this->lc);
	Linphone::Core::VideoPolicy^ policy = ref new Linphone::Core::VideoPolicy((lvp->automatically_initiate == TRUE), (lvp->automatically_accept == TRUE));
	gApiLock.Unlock();
	return policy;
}

void Linphone::Core::LinphoneCore::SetVideoPolicy(Linphone::Core::VideoPolicy^ policy)
{
	TRACE; gApiLock.Lock();
	::LinphoneVideoPolicy lvp;
	lvp.automatically_initiate = policy->AutomaticallyInitiate;
	lvp.automatically_accept = policy->AutomaticallyAccept;
	linphone_core_set_video_policy(this->lc, &lvp);
	gApiLock.Unlock();
}

Windows::Foundation::Collections::IVector<Platform::Object^>^ Linphone::Core::LinphoneCore::GetSupportedVideoSizes()
{
	TRACE; gApiLock.Lock();
	Vector<Object^>^ sizes = ref new Vector<Object^>();
	const MSVideoSizeDef *sizesList = linphone_core_get_supported_video_sizes(this->lc);
	while (sizesList->name != NULL) {
		Platform::String^ sizeName = Utils::cctops(sizesList->name);
		Linphone::Core::VideoSize^ size = ref new Linphone::Core::VideoSize(sizesList->vsize.width, sizesList->vsize.height, sizeName);
		sizes->Append(size);
		sizesList++;
	}
	gApiLock.Unlock();
	return sizes;
}

Linphone::Core::VideoSize^ Linphone::Core::LinphoneCore::GetPreferredVideoSize()
{
	Linphone::Core::VideoSize^ size = nullptr;
	TRACE; gApiLock.Lock();
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
	gApiLock.Unlock();
	return size;
}

void Linphone::Core::LinphoneCore::SetPreferredVideoSize(Linphone::Core::VideoSize^ size)
{
	if (size->Name != nullptr) {
		const char *ccname = Utils::pstoccs(size->Name);
		TRACE; gApiLock.Lock();
		linphone_core_set_preferred_video_size_by_name(this->lc, ccname);
		gApiLock.Unlock();
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
	TRACE; gApiLock.Lock();
	linphone_core_set_preferred_video_size(this->lc, vsize);
	gApiLock.Unlock();
}

Windows::Foundation::Collections::IVector<Platform::Object^>^ Linphone::Core::LinphoneCore::GetVideoDevices()
{
	TRACE; gApiLock.Lock();
	Vector<Object^>^ devices = ref new Vector<Object^>();
	const char **lvds = linphone_core_get_video_devices(this->lc);
	while (*lvds != NULL) {
		Platform::String^ device = Utils::cctops(*lvds);
		devices->Append(device);
		lvds++;
	}
	gApiLock.Unlock();
	return devices;
}

Platform::String^ Linphone::Core::LinphoneCore::GetVideoDevice()
{
	Platform::String^ device = nullptr;
	TRACE; gApiLock.Lock();
	const char *ccname = linphone_core_get_video_device(this->lc);
	if (ccname != NULL) {
		device = Utils::cctops(ccname);
	}
	gApiLock.Unlock();
	return device;
}

void Linphone::Core::LinphoneCore::SetVideoDevice(Platform::String^ device)
{
	const char *ccname = Utils::pstoccs(device);
	TRACE; gApiLock.Lock();
	linphone_core_set_video_device(this->lc, ccname);
	gApiLock.Unlock();
	delete ccname;
}

IVector<Object^>^ Linphone::Core::LinphoneCore::GetVideoCodecs()
{
	TRACE; gApiLock.Lock();
	IVector<Object^>^ codecs = ref new Vector<Object^>();
	const MSList *codecslist = linphone_core_get_video_codecs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *codecsPtr = new RefToPtrProxy<IVector<Object^>^>(codecs);
	ms_list_for_each2(codecslist, AddCodecToVector, codecsPtr);
	gApiLock.Unlock();
	return codecs;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsVideoEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_core_video_enabled(this->lc) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsVideoCaptureEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_core_video_capture_enabled(this->lc) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsVideoDisplayEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_core_video_display_enabled(this->lc) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

void Linphone::Core::LinphoneCore::EnableVideo(Platform::Boolean enableCapture, Platform::Boolean enableDisplay)
{
	TRACE; gApiLock.Lock();
	linphone_core_enable_video(this->lc, enableCapture, enableDisplay);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::EnableVideoCapture(Platform::Boolean enable) 
{
	TRACE; gApiLock.Lock();
	linphone_core_enable_video_capture(this->lc, enable);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::EnableVideoDisplay(Platform::Boolean enable) 
{
	TRACE; gApiLock.Lock();
	linphone_core_enable_video_display(this->lc, enable);
	gApiLock.Unlock();
}

int Linphone::Core::LinphoneCore::GetNativeVideoWindowId()
{
	TRACE; gApiLock.Lock();
	int id = Globals::Instance->VideoRenderer->GetNativeWindowId();
	gApiLock.Unlock();
	return id;
}

int Linphone::Core::LinphoneCore::GetCameraSensorRotation()
{
	TRACE; gApiLock.Lock();
	int rotation = linphone_core_get_camera_sensor_rotation(this->lc);
	gApiLock.Unlock();
	return rotation;
}

Platform::Boolean Linphone::Core::LinphoneCore::IsSelfViewEnabled()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean enabled = (linphone_core_self_view_enabled(this->lc) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

void Linphone::Core::LinphoneCore::EnableSelfView(Platform::Boolean enable)
{
	TRACE; gApiLock.Lock();
	linphone_core_enable_self_view(this->lc, enable);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneChatRoom^ Linphone::Core::LinphoneCore::CreateChatRoom(Platform::String^ to)
{
	TRACE; gApiLock.Lock();
	const char* address = Linphone::Core::Utils::pstoccs(to);
	Linphone::Core::LinphoneChatRoom^ chatRoom = (Linphone::Core::LinphoneChatRoom^) Linphone::Core::Utils::CreateLinphoneChatRoom(linphone_core_get_or_create_chat_room(this->lc, address));
	delete(address);
	gApiLock.Unlock();

	return chatRoom;
}

void Linphone::Core::LinphoneCore::SetLogCollectionUploadServerUrl(Platform::String^ url)
{
	TRACE; gApiLock.Lock();
	const char *curl = Linphone::Core::Utils::pstoccs(url);
	linphone_core_set_log_collection_upload_server_url(this->lc, curl);
	delete(curl);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::UploadLogCollection()
{
	TRACE; gApiLock.Lock();
	linphone_core_upload_log_collection(this->lc);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetDeviceRotation(int rotation)
{
	TRACE; gApiLock.Lock();
	linphone_core_set_device_rotation(this->lc, rotation);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::NotifyMute(bool isMuted)
{
	TRACE; gApiLock.Lock();
	Globals::Instance->CallController->NotifyMute(isMuted);
	MuteMic(isMuted);
	gApiLock.Unlock();
}

void Linphone::Core::LinphoneCore::SetChatDatabasePath(Platform::String^ chatDatabasePath)
{
	TRACE; gApiLock.Lock();
	linphone_core_set_chat_database_path(this->lc, Linphone::Core::Utils::pstoccs(chatDatabasePath));
	gApiLock.Unlock();
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
	Linphone::Core::gApiLock.EnterListener();
	Linphone::Core::LinphoneCallState state = (Linphone::Core::LinphoneCallState) cstate;
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCall^> *>(linphone_call_get_user_pointer(call));
	Linphone::Core::LinphoneCall^ lCall = (proxy) ? proxy->Ref() : nullptr;
	if (lCall == nullptr) {
		call = linphone_call_ref(call);
		lCall = (Linphone::Core::LinphoneCall^)Linphone::Core::Utils::CreateLinphoneCall(call);
	}
		
	Linphone::Core::CallController^ callController = Linphone::Core::Globals::Instance->CallController;
	if (state == Linphone::Core::LinphoneCallState::IncomingReceived) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = callController->OnIncomingCallReceived(lCall, lCall->GetRemoteAddress()->GetDisplayName(), lCall->GetRemoteAddress()->AsStringUriOnly(), callController->IncomingCallViewDismissed);
		lCall->CallContext = platformCall;
	} 
	else if (state == Linphone::Core::LinphoneCallState::OutgoingProgress) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = callController->NewOutgoingCall(lCall->GetRemoteAddress()->AsStringUriOnly());
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
			platformCall = callController->NewIncomingCallForCustomIncomingCallView(lCall->GetRemoteAddress()->GetDisplayName());
			lCall->CallContext = platformCall;
		}
		if (lCall->IsCameraEnabled()) {
			platformCall->CallMedia = VoipCallMedia::Audio | VoipCallMedia::Video;
		} else {
			platformCall->CallMedia = VoipCallMedia::Audio;
		}
		platformCall->NotifyCallActive();
	}
	
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr) {
		listener->CallState(lCall, state);
	}
	Linphone::Core::gApiLock.LeaveListener();
}

void registration_state_changed(::LinphoneCore *lc, ::LinphoneProxyConfig *cfg, ::LinphoneRegistrationState cstate, const char *msg)
{
	Linphone::Core::gApiLock.EnterListener();
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr) {
		Linphone::Core::RegistrationState state = (Linphone::Core::RegistrationState) cstate;
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneProxyConfig^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneProxyConfig^> *>(linphone_proxy_config_get_user_data(cfg));
		Linphone::Core::LinphoneProxyConfig^ config = (proxy) ? proxy->Ref() : nullptr;
		listener->RegistrationState(config, state, Linphone::Core::Utils::cctops(msg));
	}
	Linphone::Core::gApiLock.LeaveListener();
}

void global_state_changed(::LinphoneCore *lc, ::LinphoneGlobalState gstate, const char *msg)
{
	Linphone::Core::gApiLock.EnterListener();
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr) {
		Linphone::Core::GlobalState state = (Linphone::Core::GlobalState) gstate;
		listener->GlobalState(state, Linphone::Core::Utils::cctops(msg));
	}
	Linphone::Core::gApiLock.LeaveListener();
}

void auth_info_requested(LinphoneCore *lc, const char *realm, const char *username, const char *domain) 
{
	Linphone::Core::gApiLock.EnterListener();
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr) {
		listener->AuthInfoRequested(Linphone::Core::Utils::cctops(realm), Linphone::Core::Utils::cctops(username), Linphone::Core::Utils::cctops(domain));
	}
	Linphone::Core::gApiLock.LeaveListener();
}

void dtmf_received(LinphoneCore *lc, LinphoneCall *call, int dtmf)
{
	Linphone::Core::gApiLock.EnterListener();
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
	Linphone::Core::gApiLock.LeaveListener();
}

void call_encryption_changed(LinphoneCore *lc, LinphoneCall *call, bool_t on, const char *authentication_token)
{
	Linphone::Core::gApiLock.EnterListener();
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
	Linphone::Core::gApiLock.LeaveListener();
}

void call_stats_updated(LinphoneCore *lc, LinphoneCall *call, const LinphoneCallStats *stats)
{
	Linphone::Core::gApiLock.EnterListener();
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
	Linphone::Core::gApiLock.LeaveListener();
}

void message_received(LinphoneCore *lc, LinphoneChatRoom* chat_room, LinphoneChatMessage* message) 
{
	Linphone::Core::gApiLock.EnterListener();
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessage^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessage^> *>(linphone_chat_message_get_user_data(message));
		Linphone::Core::LinphoneChatMessage^ lMessage = (proxy) ? proxy->Ref() : nullptr;
		if (lMessage == nullptr) {
			message = linphone_chat_message_ref(message);
			lMessage = (Linphone::Core::LinphoneChatMessage^)Linphone::Core::Utils::CreateLinphoneChatMessage(message);
		}

		listener->MessageReceived(lMessage);
	}
	Linphone::Core::gApiLock.LeaveListener();
}

void composing_received(LinphoneCore *lc, LinphoneChatRoom *room) 
{
	Linphone::Core::gApiLock.EnterListener();
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
	Linphone::Core::gApiLock.LeaveListener();
}

void log_collection_upload_progress_indication(LinphoneCore *lc, size_t progress) {
	Linphone::Core::gApiLock.EnterListener();
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		listener->LogUploadProgressChanged((int)progress);
	}
	Linphone::Core::gApiLock.LeaveListener();
}

void log_collection_upload_state_changed(LinphoneCore *lc, ::LinphoneCoreLogCollectionUploadState state, const char *info) {
	Linphone::Core::gApiLock.EnterListener();
	Linphone::Core::LinphoneCoreListener^ listener = Linphone::Core::Globals::Instance->LinphoneCore->CoreListener;
	if (listener != nullptr)
	{
		listener->LogUploadStatusChanged((Linphone::Core::LinphoneCoreLogCollectionUploadState)state, info ? Linphone::Core::Utils::cctops(info) : nullptr);
	}
	Linphone::Core::gApiLock.LeaveListener();
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
	TRACE; gApiLock.Lock();
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
				if (gApiLock.TryLock()) {
					linphone_core_iterate(this->lc);
					gApiLock.Unlock();
				}
			}
		}), period);
	}
	isIterateEnabled = value;
	gApiLock.Unlock();
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
	TRACE; gApiLock.Lock();
	RefToPtrProxy<LinphoneCore^> *proxy = reinterpret_cast< RefToPtrProxy<LinphoneCore^> *>(linphone_core_get_user_data(this->lc));
	delete proxy;
	gApiLock.Unlock();
}
