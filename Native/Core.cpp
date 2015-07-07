/*
Core.cpp
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

#include "Address.h"
#include "ApiLock.h"
#include "AuthInfo.h"
#include "Call.h"
#include "CallController.h"
#include "CallLog.h"
#include "CallParams.h"
#include "ChatRoom.h"
#include "Core.h"
#include "CoreListener.h"
#include "Enums.h"
#include "LpConfig.h"
#include "PayloadType.h"
#include "ProxyConfig.h"
#include "Transports.h"
#include "Tunnel.h"
#include "VideoPolicy.h"
#include "VideoSize.h"

#include <collection.h>

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
//using namespace Windows::Phone::Media::Devices;
//using namespace Windows::Phone::Networking::Voip;
using namespace Windows::System::Threading;



Linphone::Native::OutputTraceLevel Linphone::Native::Core::logLevel = OutputTraceLevel::Error;



void Linphone::Native::Core::CPUCount::set(int count)
{
	API_LOCK;
	ms_set_cpu_count(count);
}

int Linphone::Native::Core::CPUCount::get()
{
	API_LOCK;
	return ms_get_cpu_count();
}

Linphone::Native::LogCollectionState Linphone::Native::Core::LogCollectionEnabled::get()
{
	API_LOCK;
	return (Linphone::Native::LogCollectionState) linphone_core_log_collection_enabled();
}

void Linphone::Native::Core::LogCollectionEnabled::set(Linphone::Native::LogCollectionState value)
{
	API_LOCK;
	linphone_core_enable_log_collection((LinphoneLogCollectionState)value);
}

Platform::String^ Linphone::Native::Core::LogCollectionPath::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_core_get_log_collection_path());
}

void Linphone::Native::Core::LogCollectionPath::set(Platform::String^ value)
{
	API_LOCK;
	const char *cvalue = Linphone::Native::Utils::pstoccs(value);
	linphone_core_set_log_collection_path(cvalue);
	delete(cvalue);
}

Linphone::Native::OutputTraceLevel Linphone::Native::Core::LogLevel::get()
{
	return Linphone::Native::Core::logLevel;
}

void Linphone::Native::Core::LogLevel::set(OutputTraceLevel logLevel)
{
	API_LOCK;
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
	Utils::SetLogLevel(coreLogLevel);
}

Platform::Boolean Linphone::Native::Core::TunnelAvailable::get()
{
	API_LOCK;
	return (linphone_core_tunnel_available() == TRUE);
}

void Linphone::Native::Core::ResetLogCollection()
{
	API_LOCK;
	linphone_core_reset_log_collection();
}




static void AddAuthInfoToVector(void *vAuthInfo, void *vector)
{
	::LinphoneAuthInfo *ai = (::LinphoneAuthInfo *)vAuthInfo;
	Linphone::Native::RefToPtrProxy<IVector<Linphone::Native::AuthInfo^>^> *list = reinterpret_cast< Linphone::Native::RefToPtrProxy<IVector<Linphone::Native::AuthInfo^>^> *>(vector);
	IVector<Linphone::Native::AuthInfo^>^ authInfos = (list) ? list->Ref() : nullptr;

	Linphone::Native::AuthInfo^ authInfo = (Linphone::Native::AuthInfo^)Linphone::Native::Utils::CreateAuthInfo(ai);
	authInfos->Append(authInfo);
}

IVector<Linphone::Native::AuthInfo^>^ Linphone::Native::Core::AuthInfos::get()
{
	API_LOCK;
	IVector<Linphone::Native::AuthInfo^>^ authInfos = ref new Vector<Linphone::Native::AuthInfo^>();
	const MSList *authlist = linphone_core_get_auth_info_list(this->lc);
	RefToPtrProxy<IVector<Linphone::Native::AuthInfo^>^> *authInfosPtr = new RefToPtrProxy<IVector<Linphone::Native::AuthInfo^>^>(authInfos);
	ms_list_for_each2(authlist, AddAuthInfoToVector, authInfosPtr);
	return authInfos;
}

Linphone::Native::CoreListener^ Linphone::Native::Core::CoreListener::get()
{
	return this->listener;
}

void Linphone::Native::Core::CoreListener::set(Linphone::Native::CoreListener^ listener)
{
	API_LOCK;
	this->listener = listener;
}

Linphone::Native::ProxyConfig^ Linphone::Native::Core::DefaultProxyConfig::get()
{
	API_LOCK;
	ProxyConfig^ defaultProxy = nullptr;
	::LinphoneProxyConfig *proxy = linphone_core_get_default_proxy_config(this->lc);
	if (proxy != nullptr) {
		defaultProxy = ref new Linphone::Native::ProxyConfig(proxy);
	}
	return defaultProxy;
}

void Linphone::Native::Core::DefaultProxyConfig::set(Linphone::Native::ProxyConfig^ proxyCfg)
{
	API_LOCK;
	linphone_core_set_default_proxy_config(this->lc, proxyCfg->proxyConfig);
}

Platform::Boolean Linphone::Native::Core::IterateEnabled::get()
{
	return isIterateEnabled;
}

void Linphone::Native::Core::IterateEnabled::set(Platform::Boolean value)
{
	API_LOCK;
	if (isIterateEnabled && !value && IterateWorkItem)
	{
		IterateWorkItem->Cancel();
		IterateWorkItem = nullptr;
	}
	else if (!isIterateEnabled && value)
	{
		IAsyncAction^ IterateWorkItem = ThreadPool::RunAsync(ref new WorkItemHandler([this](IAsyncAction^ action)
		{
			while (true) {
				GlobalApiLock::Instance()->Lock(__FUNCTION__);
				linphone_core_iterate(this->lc);
				GlobalApiLock::Instance()->Unlock(__FUNCTION__);
				ms_usleep(20000);
			}
		}), WorkItemPriority::Low);
	}
	isIterateEnabled = value;
}

static void AddProxyConfigToVector(void *vProxyConfig, void *vector)
{
	::LinphoneProxyConfig *pc = (::LinphoneProxyConfig *)vProxyConfig;
	Linphone::Native::RefToPtrProxy<IVector<Linphone::Native::ProxyConfig^>^> *list = reinterpret_cast< Linphone::Native::RefToPtrProxy<IVector<Linphone::Native::ProxyConfig^>^> *>(vector);
	IVector<Linphone::Native::ProxyConfig^>^ proxyconfigs = (list) ? list->Ref() : nullptr;

	Linphone::Native::ProxyConfig^ proxyConfig = (Linphone::Native::ProxyConfig^)Linphone::Native::Utils::CreateProxyConfig(pc);
	proxyconfigs->Append(proxyConfig);
}

IVector<Linphone::Native::ProxyConfig^>^ Linphone::Native::Core::ProxyConfigList::get()
{
	API_LOCK;
	IVector<Linphone::Native::ProxyConfig^>^ proxyconfigs = ref new Vector<Linphone::Native::ProxyConfig^>();
	const MSList *configList = linphone_core_get_proxy_config_list(this->lc);
	RefToPtrProxy<IVector<Linphone::Native::ProxyConfig^>^> *proxyConfigPtr = new RefToPtrProxy<IVector<Linphone::Native::ProxyConfig^>^>(proxyconfigs);
	ms_list_for_each2(configList, AddProxyConfigToVector, proxyConfigPtr);
	return proxyconfigs;
}

Linphone::Native::Tunnel^ Linphone::Native::Core::Tunnel::get()
{
	API_LOCK;
	Linphone::Native::Tunnel^ tunnel = nullptr;
	LinphoneTunnel *lt = linphone_core_get_tunnel(this->lc);
	if (lt != nullptr) {
		tunnel = ref new Linphone::Native::Tunnel(lt);
	}
	return tunnel;
}




void Linphone::Native::Core::AddAuthInfo(Linphone::Native::AuthInfo^ info)
{
	API_LOCK;
	linphone_core_add_auth_info(this->lc, info->auth_info);
}

void Linphone::Native::Core::AddProxyConfig(Linphone::Native::ProxyConfig^ proxyCfg)
{
	API_LOCK;
	linphone_core_add_proxy_config(this->lc, proxyCfg->proxyConfig);
}

void Linphone::Native::Core::ClearAuthInfos()
{
	API_LOCK;
	linphone_core_clear_all_auth_info(this->lc);
}

void Linphone::Native::Core::ClearProxyConfigs()
{
	API_LOCK;
	linphone_core_clear_proxy_config(this->lc);
}

Linphone::Native::Address^ Linphone::Native::Core::CreateAddress(Platform::String^ address)
{
	API_LOCK;
	const char *caddress = Linphone::Native::Utils::pstoccs(address);
	Linphone::Native::Address^ addr = dynamic_cast<Linphone::Native::Address^>(Linphone::Native::Utils::CreateAddress(caddress));
	delete(caddress);
	return addr;
}

Linphone::Native::AuthInfo^ Linphone::Native::Core::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
	API_LOCK;
	Linphone::Native::AuthInfo^ authInfo = ref new Linphone::Native::AuthInfo(username, userid, password, ha1, realm, domain);
	return authInfo;
}

Linphone::Native::ProxyConfig^ Linphone::Native::Core::CreateProxyConfig()
{
	API_LOCK;
	ProxyConfig^ proxyConfig = nullptr;
	::LinphoneProxyConfig *proxy = linphone_core_create_proxy_config(this->lc);
	if (proxy != nullptr) {
		proxyConfig = ref new Linphone::Native::ProxyConfig(proxy);
	}
	return proxyConfig;
}






void Linphone::Native::Core::Destroy() 
{
	API_LOCK;
	linphone_core_destroy(this->lc);
	IterateEnabled = false;
}

Linphone::Native::Address^ Linphone::Native::Core::InterpretURL(Platform::String^ destination) 
{
	API_LOCK;
	const char* url = Linphone::Native::Utils::pstoccs(destination);
	Linphone::Native::Address^ addr = (Linphone::Native::Address^) Linphone::Native::Utils::CreateAddress(linphone_core_interpret_url(this->lc, url));
	delete(url);
	return addr;
}

Linphone::Native::Call^ Linphone::Native::Core::Invite(Platform::String^ destination) 
{
	API_LOCK;
	Linphone::Native::Call^ lCall = nullptr;
	const char *cc = Linphone::Native::Utils::pstoccs(destination);
	::LinphoneCall *call = linphone_core_invite(this->lc, cc);
	call = linphone_call_ref(call);
	delete(cc);
	if (call != NULL) {
		Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_data(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Native::Call^)Linphone::Native::Utils::CreateLinphoneCall(call);
	}
	return lCall;
}

Linphone::Native::Call^ Linphone::Native::Core::InviteAddress(Linphone::Native::Address^ destination) 
{
	API_LOCK;
	Linphone::Native::Call^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_invite_address(this->lc, destination->address);
	call = linphone_call_ref(call);
	if (call != NULL) {
		Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_data(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Native::Call^) Linphone::Native::Utils::CreateLinphoneCall(call);
	}
	return lCall;
}

Linphone::Native::Call^ Linphone::Native::Core::InviteAddressWithParams(Linphone::Native::Address^ destination, CallParams^ params) 
{
	API_LOCK;
	Linphone::Native::Call^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_invite_address_with_params(this->lc, destination->address, params->params);
	call = linphone_call_ref(call);
	if (call != NULL) {
		Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_pointer(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
		if (lCall == nullptr)
			lCall = (Linphone::Native::Call^) Linphone::Native::Utils::CreateLinphoneCall(call);
	}
	return lCall;
}

void Linphone::Native::Core::TerminateCall(Linphone::Native::Call^ call) 
{
	API_LOCK;
	linphone_core_terminate_call(this->lc, call->call);
}

void Linphone::Native::Core::DeclineCall(Linphone::Native::Call^ call, Linphone::Native::Reason reason)
{
	API_LOCK;
	linphone_core_decline_call(this->lc, call->call, (LinphoneReason)reason);
}

Linphone::Native::Call^ Linphone::Native::Core::CurrentCall::get() 
{
	API_LOCK;
	Linphone::Native::Call^ lCall = nullptr;
	::LinphoneCall *call = linphone_core_get_current_call(this->lc);
	if (call != nullptr) {
		Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_pointer(call));
		lCall = (proxy) ? proxy->Ref() : nullptr;
	}
	return lCall;
}

Platform::Boolean Linphone::Native::Core::InCall::get() 
{
	API_LOCK;
	return (linphone_core_in_call(this->lc) == TRUE);
}

Platform::Boolean Linphone::Native::Core::IncomingInvitePending::get()
{
	API_LOCK;
	return (linphone_core_is_incoming_invite_pending(this->lc) == TRUE);
}

void Linphone::Native::Core::AcceptCall(Linphone::Native::Call^ call) 
{
	API_LOCK;
	linphone_core_accept_call(this->lc, call->call);
}

void Linphone::Native::Core::AcceptCallWithParams(Linphone::Native::Call^ call, Linphone::Native::CallParams^ params) 
{
	API_LOCK;
	linphone_core_accept_call_with_params(this->lc, call->call, params->params);
}

void Linphone::Native::Core::AcceptCallUpdate(Linphone::Native::Call^ call, Linphone::Native::CallParams^ params) 
{
	API_LOCK;
	linphone_core_accept_call_update(this->lc, call->call, params->params);
}

void Linphone::Native::Core::DeferCallUpdate(Linphone::Native::Call^ call) 
{
	API_LOCK;
	linphone_core_defer_call_update(this->lc, call->call);
}

void Linphone::Native::Core::UpdateCall(Linphone::Native::Call^ call, Linphone::Native::CallParams^ params) 
{
	API_LOCK;
	if (params != nullptr) {
		linphone_core_update_call(this->lc, call->call, params->params);
	} else {
		linphone_core_update_call(this->lc, call->call, nullptr);
	}
}

Linphone::Native::CallParams^ Linphone::Native::Core::CreateDefaultCallParameters() 
{
	API_LOCK;
	return (Linphone::Native::CallParams^) Linphone::Native::Utils::CreateLinphoneCallParams(linphone_core_create_default_call_parameters(this->lc));
}

void AddLogToVector(void* nLog, void* vector)
{
	::LinphoneCallLog *cl = (::LinphoneCallLog*)nLog;
	Linphone::Native::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Native::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ logs = (list) ? list->Ref() : nullptr;

	Linphone::Native::CallLog^ log = (Linphone::Native::CallLog^)Linphone::Native::Utils::CreateLinphoneCallLog(cl);
	logs->Append(log);
}

IVector<Object^>^ Linphone::Native::Core::CallLogs::get() 
{
	API_LOCK;
	IVector<Object^>^ logs = ref new Vector<Object^>();
	const MSList* logslist = linphone_core_get_call_logs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *logsptr = new RefToPtrProxy<IVector<Object^>^>(logs);
	ms_list_for_each2(logslist, AddLogToVector, logsptr);
	return logs;
}

void Linphone::Native::Core::ClearCallLogs() 
{
	API_LOCK;
	linphone_core_clear_call_logs(this->lc);
}

void Linphone::Native::Core::RemoveCallLog(Linphone::Native::CallLog^ log) 
{
	API_LOCK;
	linphone_core_remove_call_log(this->lc, log->callLog);
}

void Linphone::Native::Core::NetworkReachable::set(Platform::Boolean isReachable) 
{
	API_LOCK;
	linphone_core_set_network_reachable(this->lc, isReachable);
}

Platform::Boolean Linphone::Native::Core::NetworkReachable::get() 
{
	API_LOCK;
	return (linphone_core_is_network_reachable(this->lc) == TRUE);
}

void Linphone::Native::Core::MicGainDb::set(float gain) 
{
	API_LOCK;
	linphone_core_set_mic_gain_db(this->lc, gain);
}

float Linphone::Native::Core::MicGainDb::get()
{
	API_LOCK;
	return linphone_core_get_mic_gain_db(this->lc);
}

void Linphone::Native::Core::PlaybackGainDb::set(float gain)
{
	API_LOCK;
	linphone_core_set_playback_gain_db(this->lc, gain);
}

float Linphone::Native::Core::PlaybackGainDb::get()
{
	API_LOCK;
	return linphone_core_get_playback_gain_db(this->lc);
}

void Linphone::Native::Core::PlayLevel::set(int level)
{
	API_LOCK;
	linphone_core_set_play_level(this->lc, level);
}

int Linphone::Native::Core::PlayLevel::get()
{
	API_LOCK;
	return linphone_core_get_play_level(this->lc);
}

void Linphone::Native::Core::MicMuted::set(Platform::Boolean isMuted) 
{
	API_LOCK;
	linphone_core_mute_mic(this->lc, isMuted);
}

Platform::Boolean Linphone::Native::Core::MicMuted::get() 
{
	API_LOCK;
	return (linphone_core_is_mic_muted(this->lc) == TRUE);
}

void Linphone::Native::Core::SendDTMF(char16 number) 
{
	API_LOCK;
	char conv[4];
	if (wctomb(conv, number) == 1) {
		linphone_core_send_dtmf(this->lc, conv[0]);
	}
}

void Linphone::Native::Core::PlayDTMF(char16 number, int duration) 
{
	API_LOCK;
	char conv[4];
	if (wctomb(conv, number) == 1) {
		linphone_core_play_dtmf(this->lc, conv[0], duration);
	}
}

void Linphone::Native::Core::StopDTMF() 
{
	API_LOCK;
	linphone_core_stop_dtmf(this->lc);
}

Linphone::Native::PayloadType^ Linphone::Native::Core::FindPayloadType(Platform::String^ mime, int clockRate, int channels) 
{
	API_LOCK;
	const char* type = Linphone::Native::Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, channels);
	delete type;
	return ref new Linphone::Native::PayloadType(pt);
}

Linphone::Native::PayloadType^ Linphone::Native::Core::FindPayloadType(Platform::String^ mime, int clockRate) 
{
	API_LOCK;
	const char* type = Linphone::Native::Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, LINPHONE_FIND_PAYLOAD_IGNORE_CHANNELS);
	delete type;
	return ref new Linphone::Native::PayloadType(pt);
}

bool Linphone::Native::Core::PayloadTypeEnabled(PayloadType^ pt)
{
	API_LOCK;
	::PayloadType *payload = pt->payload;
	return (linphone_core_payload_type_enabled(this->lc, payload) == TRUE);
}

void Linphone::Native::Core::EnablePayloadType(PayloadType^ pt, Platform::Boolean enable) 
{
	API_LOCK;
	::PayloadType *payload = pt->payload;
	linphone_core_enable_payload_type(this->lc, payload, enable);
}

static void AddCodecToVector(void *vCodec, void *vector)
{
	::PayloadType *pt = (PayloadType *)vCodec;
	Linphone::Native::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Native::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ codecs = (list) ? list->Ref() : nullptr;

	Linphone::Native::PayloadType^ codec = (Linphone::Native::PayloadType^)Linphone::Native::Utils::CreatePayloadType(pt);
	codecs->Append(codec);
}

IVector<Object^>^ Linphone::Native::Core::AudioCodecs::get()
{
	API_LOCK;
	IVector<Object^>^ codecs = ref new Vector<Object^>();
	const MSList *codecslist = linphone_core_get_audio_codecs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *codecsPtr = new RefToPtrProxy<IVector<Object^>^>(codecs);
	ms_list_for_each2(codecslist, AddCodecToVector, codecsPtr);
	return codecs;
}

void Linphone::Native::Core::EchoCancellationEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_echo_cancellation(this->lc, enable);
}

Platform::Boolean Linphone::Native::Core::EchoCancellationEnabled::get()
{
	API_LOCK;
	return (linphone_core_echo_cancellation_enabled(this->lc) == TRUE);
}

static void EchoCalibrationCallback(LinphoneCore *lc, LinphoneEcCalibratorStatus status, int delay_ms, void *data)
{
	Linphone::Native::Utils::EchoCalibrationCallback(lc, status, delay_ms, data);
}

static void EchoCalibrationAudioInit(void *data)
{
#if 0
	Linphone::Native::EchoCalibrationData *ecData = static_cast<Linphone::Native::EchoCalibrationData *>(data);
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
#endif
}

static void EchoCalibrationAudioUninit(void *data)
{
#if 0
	Linphone::Native::EchoCalibrationData *ecData = static_cast<Linphone::Native::EchoCalibrationData *>(data);
	if (ecData != nullptr) {
		ecData->call->NotifyCallEnded();
		AudioRoutingManager::GetDefault()->SetAudioEndpoint(AudioRoutingEndpoint::Default);
	}
#endif
}

void Linphone::Native::Core::StartEchoCalibration() 
{
#if 0
	API_LOCK;
	Linphone::Native::EchoCalibrationData *data = new Linphone::Native::EchoCalibrationData();
	linphone_core_start_echo_calibration(this->lc, EchoCalibrationCallback, EchoCalibrationAudioInit, EchoCalibrationAudioUninit, data);
#endif
}

void Linphone::Native::Core::EchoLimiterEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_echo_limiter(this->lc, enable);
}

Platform::Boolean Linphone::Native::Core::EchoLimiterEnabled::get()
{
	API_LOCK;
	return (linphone_core_echo_limiter_enabled(this->lc) == TRUE);
}

void Linphone::Native::Core::SipTransports::set(Transports^ t)
{
	API_LOCK;
	::LCSipTransports transports;
	memset(&transports, 0, sizeof(LCSipTransports));
	transports.udp_port = t->UDP;
	transports.tcp_port = t->TCP;
	transports.tls_port = t->TLS;
	linphone_core_set_sip_transports(this->lc, &transports);
}

Linphone::Native::Transports^ Linphone::Native::Core::SipTransports::get()
{
	API_LOCK;
	::LCSipTransports transports;
	linphone_core_get_sip_transports(this->lc, &transports);
	return ref new Linphone::Native::Transports(transports.udp_port, transports.tcp_port, transports.tls_port);
}

void Linphone::Native::Core::IPv6Enabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_ipv6(this->lc, enable);
}

Platform::Boolean Linphone::Native::Core::IPv6Enabled::get()
{
	API_LOCK;
	return (linphone_core_ipv6_enabled(this->lc) == TRUE);
}

void Linphone::Native::Core::SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status) 
{
	API_LOCK;
	const char* ac = Linphone::Native::Utils::pstoccs(alternativeContact);
	linphone_core_set_presence_info(this->lc, minuteAway, ac, (LinphoneOnlineStatus) status);
	delete(ac);
}

void Linphone::Native::Core::StunServer::set(Platform::String^ stun)
{
	API_LOCK;
	const char* stunserver = Linphone::Native::Utils::pstoccs(stun);
	linphone_core_set_stun_server(this->lc, stunserver);
	delete(stunserver);
}

Platform::String^ Linphone::Native::Core::StunServer::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_core_get_stun_server(this->lc));
}

void Linphone::Native::Core::FirewallPolicy::set(Linphone::Native::FirewallPolicy policy)
{
	API_LOCK;
	linphone_core_set_firewall_policy(this->lc, (LinphoneFirewallPolicy) policy);
}

Linphone::Native::FirewallPolicy Linphone::Native::Core::FirewallPolicy::get()
{
	API_LOCK;
	return (Linphone::Native::FirewallPolicy) linphone_core_get_firewall_policy(this->lc);
}

void Linphone::Native::Core::RootCA::set(Platform::String^ path)
{
	API_LOCK;
	const char *ccPath = Utils::pstoccs(path);
	linphone_core_set_root_ca(this->lc, ccPath);
	delete ccPath;
}

Platform::String^ Linphone::Native::Core::RootCA::get()
{
	API_LOCK;
	return Utils::cctops(linphone_core_get_root_ca(this->lc));
}

int Linphone::Native::Core::UploadBandwidth::get()
{
	API_LOCK;
	return linphone_core_get_upload_bandwidth(this->lc);
}

void Linphone::Native::Core::UploadBandwidth::set(int value)
{
	API_LOCK;
	linphone_core_set_upload_bandwidth(this->lc, value);
}

int Linphone::Native::Core::DownloadBandwidth::get()
{
	API_LOCK;
	return linphone_core_get_download_bandwidth(this->lc);
}

void Linphone::Native::Core::DownloadBandwidth::set(int value)
{
	API_LOCK;
	linphone_core_set_download_bandwidth(this->lc, value);
}

void Linphone::Native::Core::DownloadPTime::set(int ptime)
{
	API_LOCK;
	linphone_core_set_download_ptime(this->lc, ptime);
}

int Linphone::Native::Core::DownloadPTime::get()
{
	API_LOCK;
	return linphone_core_get_download_ptime(this->lc);
}

void Linphone::Native::Core::UploadPTime::set(int ptime)
{
	API_LOCK;
	linphone_core_set_upload_ptime(this->lc, ptime);
}

int Linphone::Native::Core::UploadPTime::get()
{
	API_LOCK;
	return linphone_core_get_upload_ptime(this->lc);
}

void Linphone::Native::Core::KeepAliveEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_keep_alive(this->lc, enable);
}

Platform::Boolean Linphone::Native::Core::KeepAliveEnabled::get()
{
	API_LOCK;
	return (linphone_core_keep_alive_enabled(this->lc) == TRUE);
}

Platform::String^ Linphone::Native::Core::PlayFile::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_core_get_play_file(this->lc));
}

void Linphone::Native::Core::PlayFile::set(Platform::String^ path)
{
	API_LOCK;
	const char* file = Linphone::Native::Utils::pstoccs(path);
	linphone_core_set_play_file(this->lc, file);
	delete(file);
}

Platform::Boolean Linphone::Native::Core::PauseCall(Call^ call) 
{
	API_LOCK;
	return (linphone_core_pause_call(this->lc, call->call) == 0);
}

Platform::Boolean Linphone::Native::Core::ResumeCall(Call^ call) 
{
	API_LOCK;
	return (linphone_core_resume_call(this->lc, call->call) == 0);
}

Platform::Boolean Linphone::Native::Core::PauseAllCalls() 
{
	API_LOCK;
	return (linphone_core_pause_all_calls(this->lc) == 0);
}

Platform::Boolean Linphone::Native::Core::InConference::get()
{
	API_LOCK;
	return (linphone_core_is_in_conference(this->lc) == TRUE);
}

Platform::Boolean Linphone::Native::Core::EnterConference() 
{
	API_LOCK;
	return (linphone_core_enter_conference(this->lc) == 0);
}

void Linphone::Native::Core::LeaveConference() 
{
	API_LOCK;
	linphone_core_leave_conference(this->lc);
}

void Linphone::Native::Core::AddToConference(Call^ call) 
{
	API_LOCK;
	linphone_core_add_to_conference(this->lc, call->call);
}

void Linphone::Native::Core::AddAllToConference() 
{
	API_LOCK;
	linphone_core_add_all_to_conference(this->lc);
}

void Linphone::Native::Core::RemoveFromConference(Call^ call) 
{
	API_LOCK;
	linphone_core_remove_from_conference(this->lc, call->call);
}

void Linphone::Native::Core::TerminateConference() 
{
	API_LOCK;
	linphone_core_terminate_conference(this->lc);
}

int Linphone::Native::Core::ConferenceSize::get()
{
	API_LOCK;
	return linphone_core_get_conference_size(this->lc);
}

void Linphone::Native::Core::TerminateAllCalls() 
{
	API_LOCK;
	linphone_core_terminate_all_calls(this->lc);
}

static void AddCallToVector(void *vCall, void *vector)
{
	::LinphoneCall* c = (::LinphoneCall *)vCall;
	Linphone::Native::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Native::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ calls = (list) ? list->Ref() : nullptr;

	Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_pointer(c));
	Linphone::Native::Call^ call = (proxy) ? proxy->Ref() : nullptr;
	calls->Append(call);
}

IVector<Object^>^ Linphone::Native::Core::Calls::get()
{
	API_LOCK;
	Vector<Object^>^ calls = ref new Vector<Object^>();
	const MSList *callsList = linphone_core_get_calls(this->lc);
	RefToPtrProxy<IVector<Object^>^> *callsPtr = new RefToPtrProxy<IVector<Object^>^>(calls);
	ms_list_for_each2(callsList, AddCallToVector, callsPtr);
	return calls;
}

int Linphone::Native::Core::CallsNb::get()
{
	API_LOCK;
	return linphone_core_get_calls_nb(this->lc);
}

Linphone::Native::Call^ Linphone::Native::Core::FindCallFromUri(Platform::String^ uri) 
{
	API_LOCK;
	const char *curi = Utils::pstoccs(uri);
	::LinphoneCall *call = const_cast<::LinphoneCall *>(linphone_core_find_call_from_uri(this->lc, curi));
	Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_pointer(call));
	Linphone::Native::Call^ lCall = (proxy) ? proxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Native::Call^)Linphone::Native::Utils::CreateLinphoneCall(call);
	}
	delete curi;
	return lCall;
}

int Linphone::Native::Core::MaxCalls::get() 
{
	API_LOCK;
	return linphone_core_get_max_calls(this->lc);
}

void Linphone::Native::Core::MaxCalls::set(int max)
{
	API_LOCK;
	linphone_core_set_max_calls(this->lc, max);
}

Platform::Boolean Linphone::Native::Core::IsMyself(Platform::String^ uri) 
{
	API_LOCK;
	Platform::Boolean myself = false;
	ProxyConfig^ lpc = DefaultProxyConfig;
	if (lpc != nullptr) {
		myself = uri->Equals(lpc->Identity);
	}
	return myself;
}

Platform::Boolean Linphone::Native::Core::SoundResourcesLocked::get()
{
	API_LOCK;
	return (linphone_core_sound_resources_locked(this->lc) == TRUE);
}

Platform::Boolean Linphone::Native::Core::IsMediaEncryptionSupported(Linphone::Native::MediaEncryption menc) 
{
	API_LOCK;
	return (linphone_core_media_encryption_supported(this->lc, (LinphoneMediaEncryption) menc) == TRUE);
}

void Linphone::Native::Core::MediaEncryption::set(Linphone::Native::MediaEncryption menc)
{
	API_LOCK;
	linphone_core_set_media_encryption(this->lc, (LinphoneMediaEncryption) menc);
}

Linphone::Native::MediaEncryption Linphone::Native::Core::MediaEncryption::get()
{
	API_LOCK;
	return (Linphone::Native::MediaEncryption) linphone_core_get_media_encryption(this->lc);
}

void Linphone::Native::Core::MediaEncryptionMandatory::set(Platform::Boolean yesno)
{
	API_LOCK;
	linphone_core_set_media_encryption_mandatory(this->lc, yesno);
}

Platform::Boolean Linphone::Native::Core::MediaEncryptionMandatory::get()
{
	API_LOCK;
	return (linphone_core_is_media_encryption_mandatory(this->lc) == TRUE);
}

void Linphone::Native::Core::SetUserAgent(Platform::String^ name, Platform::String^ version) 
{
	API_LOCK;
	const char* ua = Linphone::Native::Utils::pstoccs(name);
	const char* v = Linphone::Native::Utils::pstoccs(version);
	linphone_core_set_user_agent(this->lc, ua, v);
	delete(v);
	delete(ua);
}

int Linphone::Native::Core::MissedCallsCount::get()
{
	API_LOCK;
	return linphone_core_get_missed_calls_count(this->lc);
}

void Linphone::Native::Core::ResetMissedCallsCount() 
{
	API_LOCK;
	linphone_core_reset_missed_calls_count(this->lc);
}

void Linphone::Native::Core::RefreshRegisters() 
{
	API_LOCK;
	linphone_core_refresh_registers(this->lc);
}

Platform::String^ Linphone::Native::Core::Version::get()
{
	API_LOCK;
	return Linphone::Native::Utils::cctops(linphone_core_get_version());
}

void Linphone::Native::Core::AudioPort::set(int port)
{
	API_LOCK;
	linphone_core_set_audio_port(this->lc, port);
}

int Linphone::Native::Core::AudioPort::get()
{
	API_LOCK;
	return linphone_core_get_audio_port(this->lc);
}

void Linphone::Native::Core::SetAudioPortRange(int minP, int maxP) 
{
	API_LOCK;
	linphone_core_set_audio_port_range(this->lc, minP, maxP);
}

void Linphone::Native::Core::IncTimeout::set(int timeout)
{
	API_LOCK;
	linphone_core_set_inc_timeout(this->lc, timeout);
}

int Linphone::Native::Core::IncTimeout::get()
{
	API_LOCK;
	return linphone_core_get_inc_timeout(this->lc);
}

void Linphone::Native::Core::InCallTimeout::set(int timeout)
{
	API_LOCK;
	linphone_core_set_in_call_timeout(this->lc, timeout);
}

int Linphone::Native::Core::InCallTimeout::get()
{
	API_LOCK;
	return linphone_core_get_in_call_timeout(this->lc);
}

void Linphone::Native::Core::SetPrimaryContact(Platform::String^ displayName, Platform::String^ userName) 
{
	API_LOCK;
	const char* dn = Linphone::Native::Utils::pstoccs(displayName);
	const char* un = Linphone::Native::Utils::pstoccs(userName);
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

Platform::Boolean Linphone::Native::Core::UseInfoForDtmf::get()
{
	API_LOCK;
	return (linphone_core_get_use_info_for_dtmf(this->lc) == TRUE);
}

Platform::Boolean Linphone::Native::Core::UseRfc2833ForDtmf::get()
{
	API_LOCK;
	return (linphone_core_get_use_rfc2833_for_dtmf(this->lc) == TRUE);
}

void Linphone::Native::Core::UseInfoForDtmf::set(Platform::Boolean use)
{
	API_LOCK;
	linphone_core_set_use_info_for_dtmf(this->lc, use);
}

void Linphone::Native::Core::UseRfc2833ForDtmf::set(Platform::Boolean use)
{
	API_LOCK;
	linphone_core_set_use_rfc2833_for_dtmf(this->lc, use);
}

Linphone::Native::LpConfig^ Linphone::Native::Core::Config::get()
{
	API_LOCK;
	::LpConfig *config = linphone_core_get_config(this->lc);
	return (Linphone::Native::LpConfig^)Linphone::Native::Utils::CreateLpConfig(config);
}

Platform::Boolean Linphone::Native::Core::VideoSupported::get()
{
	API_LOCK;
	return (linphone_core_video_supported(this->lc) == TRUE);
}

Linphone::Native::VideoPolicy^ Linphone::Native::Core::VideoPolicy::get()
{
	API_LOCK;
	const ::LinphoneVideoPolicy *lvp = linphone_core_get_video_policy(this->lc);
	return ref new Linphone::Native::VideoPolicy((lvp->automatically_initiate == TRUE), (lvp->automatically_accept == TRUE));
}

void Linphone::Native::Core::VideoPolicy::set(Linphone::Native::VideoPolicy^ policy)
{
	API_LOCK;
	::LinphoneVideoPolicy lvp;
	lvp.automatically_initiate = policy->AutomaticallyInitiate;
	lvp.automatically_accept = policy->AutomaticallyAccept;
	linphone_core_set_video_policy(this->lc, &lvp);
}

Windows::Foundation::Collections::IVector<Platform::Object^>^ Linphone::Native::Core::SupportedVideoSizes::get()
{
	API_LOCK;
	Vector<Object^>^ sizes = ref new Vector<Object^>();
	const MSVideoSizeDef *sizesList = linphone_core_get_supported_video_sizes(this->lc);
	while (sizesList->name != NULL) {
		Platform::String^ sizeName = Utils::cctops(sizesList->name);
		Linphone::Native::VideoSize^ size = ref new Linphone::Native::VideoSize(sizesList->vsize.width, sizesList->vsize.height, sizeName);
		sizes->Append(size);
		sizesList++;
	}
	return sizes;
}

Linphone::Native::VideoSize^ Linphone::Native::Core::PreferredVideoSize::get()
{
	API_LOCK;
	Linphone::Native::VideoSize^ size = nullptr;
	const MSVideoSizeDef *sizesList = linphone_core_get_supported_video_sizes(this->lc);
	MSVideoSize vsize = linphone_core_get_preferred_video_size(this->lc);
	while (sizesList->name != NULL) {
		if ((sizesList->vsize.width == vsize.width) && (sizesList->vsize.height == vsize.height)) {
			Platform::String^ sizeName = Utils::cctops(sizesList->name);
			size = ref new Linphone::Native::VideoSize(vsize.width, vsize.height, sizeName);
			break;
		}
		sizesList++;
	}
	if (size == nullptr) {
		size = ref new Linphone::Native::VideoSize(vsize.width, vsize.height);
	}
	return size;
}

Platform::String^ Linphone::Native::Core::GetPreferredVideoSizeName()
{
	API_LOCK;
	char *cSizeName = linphone_core_get_preferred_video_size_name(this->lc);
	Platform::String^ sizeName = Utils::cctops(cSizeName);
	ms_free(cSizeName);
	return sizeName;
}

void Linphone::Native::Core::PreferredVideoSize::set(Linphone::Native::VideoSize^ size)
{
	API_LOCK;
	if (size->Name != nullptr) {
		const char *ccname = Utils::pstoccs(size->Name);
		linphone_core_set_preferred_video_size_by_name(this->lc, ccname);
		delete ccname;
	} else {
		SetPreferredVideoSize(size->Width, size->Height);
	}
}

void Linphone::Native::Core::SetPreferredVideoSize(int width, int height)
{
	API_LOCK;
	MSVideoSize vsize;
	vsize.width = width;
	vsize.height = height;
	linphone_core_set_preferred_video_size(this->lc, vsize);
}

void Linphone::Native::Core::SetPreferredVideoSizeByName(Platform::String^ sizeName)
{
	API_LOCK;
	linphone_core_set_preferred_video_size_by_name(this->lc, Utils::pstoccs(sizeName));
}

Windows::Foundation::Collections::IVector<Platform::Object^>^ Linphone::Native::Core::VideoDevices::get()
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

Platform::String^ Linphone::Native::Core::VideoDevice::get()
{
	API_LOCK;
	Platform::String^ device = nullptr;
	const char *ccname = linphone_core_get_video_device(this->lc);
	if (ccname != NULL) {
		device = Utils::cctops(ccname);
	}
	return device;
}

void Linphone::Native::Core::VideoDevice::set(Platform::String^ device)
{
	API_LOCK;
	const char *ccname = Utils::pstoccs(device);
	linphone_core_set_video_device(this->lc, ccname);
	delete ccname;
}

IVector<Object^>^ Linphone::Native::Core::VideoCodecs::get()
{
	API_LOCK;
	IVector<Object^>^ codecs = ref new Vector<Object^>();
	const MSList *codecslist = linphone_core_get_video_codecs(this->lc);
	RefToPtrProxy<IVector<Object^>^> *codecsPtr = new RefToPtrProxy<IVector<Object^>^>(codecs);
	ms_list_for_each2(codecslist, AddCodecToVector, codecsPtr);
	return codecs;
}

Platform::Boolean Linphone::Native::Core::VideoEnabled::get()
{
	API_LOCK;
	return (linphone_core_video_enabled(this->lc) == TRUE);
}

Platform::Boolean Linphone::Native::Core::VideoCaptureEnabled::get()
{
	API_LOCK;
	return (linphone_core_video_capture_enabled(this->lc) == TRUE);
}

Platform::Boolean Linphone::Native::Core::VideoDisplayEnabled::get()
{
	API_LOCK;
	return (linphone_core_video_display_enabled(this->lc) == TRUE);
}

void Linphone::Native::Core::EnableVideo(Platform::Boolean enableCapture, Platform::Boolean enableDisplay)
{
	API_LOCK;
	linphone_core_enable_video(this->lc, enableCapture, enableDisplay);
}

void Linphone::Native::Core::VideoCaptureEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_video_capture(this->lc, enable);
}

void Linphone::Native::Core::VideoDisplayEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_video_display(this->lc, enable);
}

int Linphone::Native::Core::NativeVideoWindowId::get()
{
#if 0
	API_LOCK;
	return Globals::Instance->VideoRenderer->GetNativeWindowId();
#else
	return 0;
#endif
}

int Linphone::Native::Core::CameraSensorRotation::get()
{
	API_LOCK;
	return linphone_core_get_camera_sensor_rotation(this->lc);
}

Platform::Boolean Linphone::Native::Core::SelfViewEnabled::get()
{
	API_LOCK;
	return (linphone_core_self_view_enabled(this->lc) == TRUE);
}

void Linphone::Native::Core::SelfViewEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_self_view(this->lc, enable);
}

Linphone::Native::ChatRoom^ Linphone::Native::Core::GetChatRoom(Linphone::Native::Address^ address)
{
	API_LOCK;
	::LinphoneChatRoom * chatRoom = linphone_core_get_chat_room(this->lc, address->address);
	Linphone::Native::RefToPtrProxy<Linphone::Native::ChatRoom^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::ChatRoom^> *>(linphone_chat_room_get_user_data(chatRoom));
	Linphone::Native::ChatRoom^ lChatRoom = (proxy) ? proxy->Ref() : nullptr;
	if (lChatRoom == nullptr) {
		lChatRoom = (Linphone::Native::ChatRoom^) Linphone::Native::Utils::CreateLinphoneChatRoom(chatRoom);
	}
	return lChatRoom;
}

Linphone::Native::ChatRoom^ Linphone::Native::Core::GetChatRoomFromUri(Platform::String^ to)
{
	API_LOCK;
	::LinphoneChatRoom * chatRoom = linphone_core_get_chat_room_from_uri(this->lc, Linphone::Native::Utils::pstoccs(to));
	Linphone::Native::RefToPtrProxy<Linphone::Native::ChatRoom^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::ChatRoom^> *>(linphone_chat_room_get_user_data(chatRoom));
	Linphone::Native::ChatRoom^ lChatRoom = (proxy) ? proxy->Ref() : nullptr;
	if (lChatRoom == nullptr) {
		lChatRoom = (Linphone::Native::ChatRoom^) Linphone::Native::Utils::CreateLinphoneChatRoom(chatRoom);
	}
	return lChatRoom;
}

Platform::String^ Linphone::Native::Core::LogCollectionUploadServerUrl::get()
{
	API_LOCK;
	return nullptr; // TODO
}

void Linphone::Native::Core::LogCollectionUploadServerUrl::set(Platform::String^ url)
{
	API_LOCK;
	const char *curl = Linphone::Native::Utils::pstoccs(url);
	linphone_core_set_log_collection_upload_server_url(this->lc, curl);
	delete(curl);
}

void Linphone::Native::Core::UploadLogCollection()
{
	API_LOCK;
	linphone_core_upload_log_collection(this->lc);
}

void Linphone::Native::Core::DeviceRotation::set(int rotation)
{
	API_LOCK;
	linphone_core_set_device_rotation(this->lc, rotation);
}

int Linphone::Native::Core::DeviceRotation::get()
{
	API_LOCK;
	return linphone_core_get_device_rotation(this->lc);
}

void Linphone::Native::Core::NotifyMute(bool isMuted)
{
#if 0
	API_LOCK;
	Globals::Instance->CallController->NotifyMute(isMuted);
#endif
	MicMuted = isMuted;
}

Platform::String^ Linphone::Native::Core::ChatDatabasePath::get()
{
	API_LOCK;
	return nullptr; // TODO
}

void Linphone::Native::Core::ChatDatabasePath::set(Platform::String^ chatDatabasePath)
{
	API_LOCK;
	linphone_core_set_chat_database_path(this->lc, Linphone::Native::Utils::pstoccs(chatDatabasePath));
}

static void AddChatRoomListToVector(void *vRoom, void *vector)
{
	::LinphoneChatRoom *chatRoom = (LinphoneChatRoom*) vRoom;
	Linphone::Native::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Native::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ rooms = (list) ? list->Ref() : nullptr;
	Linphone::Native::ChatRoom^ room = (Linphone::Native::ChatRoom^) Linphone::Native::Utils::CreateLinphoneChatRoom(chatRoom);
	rooms->Append(room);
}

IVector<Object^>^ Linphone::Native::Core::ChatRooms::get()
{
	API_LOCK;
	IVector<Object^>^ rooms = ref new Vector<Object^>();
	MSList* roomList = linphone_core_get_chat_rooms(this->lc);
	RefToPtrProxy<IVector<Object^>^> *roomsPtr = new RefToPtrProxy<IVector<Object^>^>(rooms);
	ms_list_for_each2(roomList, AddChatRoomListToVector, roomsPtr);
	return rooms;
}

void global_state_changed(::LinphoneCore *lc, ::LinphoneGlobalState gstate, const char *msg)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *proxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = proxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	if (listener != nullptr) {
		Linphone::Native::GlobalState state = (Linphone::Native::GlobalState) gstate;
		listener->GlobalStateChanged(state, Linphone::Native::Utils::cctops(msg));
	}
}

void registration_state_changed(::LinphoneCore *lc, ::LinphoneProxyConfig *cfg, ::LinphoneRegistrationState cstate, const char *msg)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *proxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = proxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	if (listener != nullptr) {
		Linphone::Native::RegistrationState state = (Linphone::Native::RegistrationState) cstate;
		Linphone::Native::RefToPtrProxy<Linphone::Native::ProxyConfig^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::ProxyConfig^> *>(linphone_proxy_config_get_user_data(cfg));
		Linphone::Native::ProxyConfig^ config = (proxy) ? proxy->Ref() : nullptr;
		listener->RegistrationStateChanged(config, state, Linphone::Native::Utils::cctops(msg));
	}
}

void call_state_changed(::LinphoneCore *lc, ::LinphoneCall *call, ::LinphoneCallState cstate, const char *msg)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *lcProxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = lcProxy->Ref();
	Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *callProxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_pointer(call));
	Linphone::Native::Call^ lCall = (callProxy) ? callProxy->Ref() : nullptr;
	if (lCall == nullptr) {
		call = linphone_call_ref(call);
		lCall = (Linphone::Native::Call^)Linphone::Native::Utils::CreateLinphoneCall(call);
	}
	Linphone::Native::CallState state = (Linphone::Native::CallState) cstate;
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	if (listener != nullptr) {
		listener->CallStateChanged(lCall, state, Linphone::Native::Utils::cctops(msg));
	}

#if 0
	Linphone::Native::CallController^ callController = Linphone::Native::Globals::Instance->CallController;
	if (state == Linphone::Native::CallState::IncomingReceived) {
		Platform::String^ name = lCall->RemoteAddress->DisplayName;
		if (name == nullptr || name->Length() <= 0) 
		{
			name = lCall->RemoteAddress->UserName;
		}
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = callController->OnIncomingCallReceived(lCall, name, lCall->RemoteAddress->AsStringUriOnly(), callController->IncomingCallViewDismissed);
		lCall->CallContext = platformCall;
	} 
	else if (state == Linphone::Native::CallState::OutgoingProgress) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = callController->NewOutgoingCall(lCall->RemoteAddress->AsStringUriOnly());
		lCall->CallContext = platformCall;
	}
	else if (state == Linphone::Native::CallState::End || state == Linphone::Native::CallState::Error) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = (Windows::Phone::Networking::Voip::VoipPhoneCall^) lCall->CallContext;
		if (platformCall != nullptr)
			platformCall->NotifyCallEnded();

		if (callController->IncomingCallViewDismissed != nullptr) {
			// When we receive a call with PN, call the callback to kill the agent process in case the caller stops the call before user accepts/denies it
			callController->IncomingCallViewDismissed();
		}
	}
	else if (state == Linphone::Native::CallState::Paused || state == Linphone::Native::CallState::PausedByRemote) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = (Windows::Phone::Networking::Voip::VoipPhoneCall^) lCall->CallContext;
		if (platformCall != nullptr)
			platformCall->NotifyCallHeld();
	}
	else if (state == Linphone::Native::CallState::StreamsRunning) {
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
#endif
}

void auth_info_requested(LinphoneCore *lc, const char *realm, const char *username, const char *domain) 
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *proxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = proxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	if (listener != nullptr) {
		listener->AuthInfoRequested(Linphone::Native::Utils::cctops(realm), Linphone::Native::Utils::cctops(username), Linphone::Native::Utils::cctops(domain));
	}
}

void message_received(LinphoneCore *lc, LinphoneChatRoom* room, LinphoneChatMessage* message)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *lcProxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = lcProxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	if (listener != nullptr)
	{
		Linphone::Native::RefToPtrProxy<Linphone::Native::ChatRoom^> *crProxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::ChatRoom^> *>(linphone_chat_room_get_user_data(room));
		Linphone::Native::ChatRoom^ lRoom = (crProxy) ? crProxy->Ref() : nullptr;
		if (lRoom == nullptr) {
			lRoom = (Linphone::Native::ChatRoom^)Linphone::Native::Utils::CreateLinphoneChatRoom(room);
		}
		Linphone::Native::RefToPtrProxy<Linphone::Native::ChatMessage^> *cmProxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::ChatMessage^> *>(linphone_chat_message_get_user_data(message));
		Linphone::Native::ChatMessage^ lMessage = (cmProxy) ? cmProxy->Ref() : nullptr;
		if (lMessage == nullptr) {
			lMessage = (Linphone::Native::ChatMessage^)Linphone::Native::Utils::CreateLinphoneChatMessage(message);
		}

		listener->MessageReceived(lRoom, lMessage);
	}
}

void is_composing_received(LinphoneCore *lc, LinphoneChatRoom *room)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *lcProxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = lcProxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	if (listener != nullptr) {
		Linphone::Native::RefToPtrProxy<Linphone::Native::ChatRoom^> *crProxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::ChatRoom^> *>(linphone_chat_room_get_user_data(room));
		Linphone::Native::ChatRoom^ lRoom = (crProxy) ? crProxy->Ref() : nullptr;
		if (lRoom == nullptr) {
			lRoom = (Linphone::Native::ChatRoom^)Linphone::Native::Utils::CreateLinphoneChatRoom(room);
		}
		listener->IsComposingReceived(lRoom);
	}
}

void dtmf_received(LinphoneCore *lc, LinphoneCall *call, int dtmf)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *lcProxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = lcProxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *callProxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_pointer(call));
	Linphone::Native::Call^ lCall = (callProxy) ? callProxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Native::Call^)Linphone::Native::Utils::CreateLinphoneCall(call);
	}
	if (listener != nullptr) {
		char cdtmf = (char)dtmf;
		char16 wdtmf;
		mbtowc(&wdtmf, &cdtmf, 1);
		listener->DtmfReceived(lCall, wdtmf);
	}
}

void call_encryption_changed(LinphoneCore *lc, LinphoneCall *call, bool_t on, const char *authentication_token)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *lcProxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = lcProxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *callProxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_pointer(call));
	Linphone::Native::Call^ lCall = (callProxy) ? callProxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Native::Call^)Linphone::Native::Utils::CreateLinphoneCall(call);
	}
	if ((lCall != nullptr) && (listener != nullptr)) {
		listener->CallEncryptionChanged(lCall, (on == TRUE), Linphone::Native::Utils::cctops(authentication_token));
	}
}

void call_stats_updated(LinphoneCore *lc, LinphoneCall *call, const LinphoneCallStats *stats)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *lcProxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = lcProxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *callProxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::Call^> *>(linphone_call_get_user_pointer(call));
	Linphone::Native::Call^ lCall = (callProxy) ? callProxy->Ref() : nullptr;
	if (lCall == nullptr) {
		lCall = (Linphone::Native::Call^)Linphone::Native::Utils::CreateLinphoneCall(call);
	}
	Linphone::Native::CallStats^ lStats = (Linphone::Native::CallStats^)Linphone::Native::Utils::CreateLinphoneCallStats((void *)stats);
	if ((lCall != nullptr) && (lStats != nullptr) && (listener != nullptr)) {
		listener->CallStatsUpdated(lCall, lStats);
	}
}

void log_collection_upload_state_changed(LinphoneCore *lc, ::LinphoneCoreLogCollectionUploadState state, const char *info)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *lcProxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = lcProxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	if (listener != nullptr) {
		listener->LogCollectionUploadStateChanged((Linphone::Native::LogCollectionUploadState)state, info ? Linphone::Native::Utils::cctops(info) : nullptr);
	}
}

void log_collection_upload_progress_indication(LinphoneCore *lc, size_t offset, size_t total)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *lcProxy = reinterpret_cast<Linphone::Native::RefToPtrProxy<Linphone::Native::Core^> *>(linphone_core_get_user_data(lc));
	Linphone::Native::Core^ core = lcProxy->Ref();
	Linphone::Native::CoreListener^ listener = core->CoreListener;
	if (listener != nullptr) {
		listener->LogCollectionUploadProgressIndication((int)offset, (int)total);
	}
}

Linphone::Native::Core::Core(Linphone::Native::CoreListener^ coreListener)
	: lc(nullptr), listener(coreListener), config(ref new LpConfig(nullptr, nullptr)), isIterateEnabled(false)
{
	Init();
}

Linphone::Native::Core::Core(Linphone::Native::CoreListener^ coreListener, LpConfig^ config)
	: lc(nullptr), listener(coreListener), config(config), isIterateEnabled(false)
{
	Init();
}

void Linphone::Native::Core::Init()
{
	LinphoneCoreVTable *vtable = (LinphoneCoreVTable*) malloc(sizeof(LinphoneCoreVTable));
	memset (vtable, 0, sizeof(LinphoneCoreVTable));
	vtable->global_state_changed = global_state_changed;
	vtable->registration_state_changed = registration_state_changed;
	vtable->call_state_changed = call_state_changed;
	vtable->auth_info_requested = auth_info_requested;
	vtable->message_received = message_received;
	vtable->is_composing_received = is_composing_received;
	vtable->dtmf_received = dtmf_received;
	vtable->call_encryption_changed = call_encryption_changed;
	vtable->call_stats_updated = call_stats_updated;
	vtable->log_collection_upload_state_changed = log_collection_upload_state_changed;
	vtable->log_collection_upload_progress_indication = log_collection_upload_progress_indication;

	RefToPtrProxy<Core^> *proxy = new RefToPtrProxy<Core^>(this);
	this->lc = linphone_core_new_with_config(vtable, config ? config->config : NULL, proxy);

	linphone_core_set_ring(this->lc, nullptr);
#if 0
	RefToPtrProxy<Mediastreamer2::WP8Video::IVideoRenderer^> *renderer = new RefToPtrProxy<Mediastreamer2::WP8Video::IVideoRenderer^>(Globals::Instance->VideoRenderer);
	linphone_core_set_native_video_window_id(this->lc, (unsigned long)renderer);
#endif
}

Linphone::Native::Core::~Core()
{
	API_LOCK;
	RefToPtrProxy<Core^> *proxy = reinterpret_cast< RefToPtrProxy<Core^> *>(linphone_core_get_user_data(this->lc));
	delete proxy;
	linphone_core_destroy(this->lc);
}
