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

using namespace BelledonneCommunications::Linphone::Native;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
//using namespace Windows::Phone::Media::Devices;
//using namespace Windows::Phone::Networking::Voip;



OutputTraceLevel Core::logLevel = OutputTraceLevel::Error;



void Core::CPUCount::set(int count)
{
	API_LOCK;
	ms_set_cpu_count(count);
}

int Core::CPUCount::get()
{
	API_LOCK;
	return ms_get_cpu_count();
}

LogCollectionState Core::LogCollectionEnabled::get()
{
	API_LOCK;
	return (LogCollectionState) linphone_core_log_collection_enabled();
}

void Core::LogCollectionEnabled::set(LogCollectionState value)
{
	API_LOCK;
	linphone_core_enable_log_collection((LinphoneLogCollectionState)value);
}

Platform::String^ Core::LogCollectionPath::get()
{
	API_LOCK;
	return Utils::cctops(linphone_core_get_log_collection_path());
}

void Core::LogCollectionPath::set(Platform::String^ value)
{
	API_LOCK;
	const char *cvalue = Utils::pstoccs(value);
	linphone_core_set_log_collection_path(cvalue);
	delete(cvalue);
}

OutputTraceLevel Core::LogLevel::get()
{
	return Core::logLevel;
}

void Core::LogLevel::set(OutputTraceLevel logLevel)
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

Platform::Boolean Core::TunnelAvailable::get()
{
	API_LOCK;
	return (linphone_core_tunnel_available() == TRUE);
}

Platform::String^ Core::Version::get()
{
	API_LOCK;
	return Utils::cctops(linphone_core_get_version());
}




void Core::ResetLogCollection()
{
	API_LOCK;
	linphone_core_reset_log_collection();
}




static void AddPayloadTypeToVector(void *vCodec, void *vector)
{
	::PayloadType *pt = (::PayloadType *)vCodec;
	RefToPtrProxy<IVector<BelledonneCommunications::Linphone::Native::PayloadType^>^> *list = reinterpret_cast<RefToPtrProxy<IVector<BelledonneCommunications::Linphone::Native::PayloadType^>^> *>(vector);
	IVector<BelledonneCommunications::Linphone::Native::PayloadType^>^ codecs = (list) ? list->Ref() : nullptr;
	BelledonneCommunications::Linphone::Native::PayloadType^ codec = (BelledonneCommunications::Linphone::Native::PayloadType^)Utils::CreatePayloadType(pt);
	codecs->Append(codec);
}

IVector<BelledonneCommunications::Linphone::Native::PayloadType^>^ Core::AudioCodecs::get()
{
	API_LOCK;
	IVector<PayloadType^>^ codecs = ref new Vector<PayloadType^>();
	const MSList *codecslist = linphone_core_get_audio_codecs(this->lc);
	RefToPtrProxy<IVector<PayloadType^>^> *codecsPtr = new RefToPtrProxy<IVector<PayloadType^>^>(codecs);
	ms_list_for_each2(codecslist, AddPayloadTypeToVector, codecsPtr);
	return codecs;
}

int Core::AudioPort::get()
{
	API_LOCK;
	return linphone_core_get_audio_port(this->lc);
}

void Core::AudioPort::set(int port)
{
	API_LOCK;
	linphone_core_set_audio_port(this->lc, port);
}

static void AddAuthInfoToVector(void *vAuthInfo, void *vector)
{
	::LinphoneAuthInfo *ai = (::LinphoneAuthInfo *)vAuthInfo;
	RefToPtrProxy<IVector<AuthInfo^>^> *list = reinterpret_cast< RefToPtrProxy<IVector<AuthInfo^>^> *>(vector);
	IVector<AuthInfo^>^ authInfos = (list) ? list->Ref() : nullptr;

	AuthInfo^ authInfo = (AuthInfo^)Utils::CreateAuthInfo(ai);
	authInfos->Append(authInfo);
}

IVector<AuthInfo^>^ Core::AuthInfoList::get()
{
	API_LOCK;
	IVector<AuthInfo^>^ authInfos = ref new Vector<AuthInfo^>();
	const MSList *authlist = linphone_core_get_auth_info_list(this->lc);
	RefToPtrProxy<IVector<AuthInfo^>^> *authInfosPtr = new RefToPtrProxy<IVector<AuthInfo^>^>(authInfos);
	ms_list_for_each2(authlist, AddAuthInfoToVector, authInfosPtr);
	return authInfos;
}

static void AddCallLogToVector(void* nLog, void* vector)
{
	::LinphoneCallLog *cl = (::LinphoneCallLog*)nLog;
	RefToPtrProxy<IVector<CallLog^>^> *list = reinterpret_cast<RefToPtrProxy<IVector<CallLog^>^> *>(vector);
	IVector<CallLog^>^ logs = (list) ? list->Ref() : nullptr;
	CallLog^ log = (CallLog^)Utils::GetCallLog(cl);
	logs->Append(log);
}

IVector<CallLog^>^ Core::CallLogs::get()
{
	API_LOCK;
	IVector<CallLog^>^ logs = ref new Vector<CallLog^>();
	const MSList* logslist = linphone_core_get_call_logs(this->lc);
	RefToPtrProxy<IVector<CallLog^>^> *logsptr = new RefToPtrProxy<IVector<CallLog^>^>(logs);
	ms_list_for_each2(logslist, AddCallLogToVector, logsptr);
	return logs;
}

static void AddCallToVector(void *vCall, void *vector)
{
	::LinphoneCall* c = (::LinphoneCall *)vCall;
	RefToPtrProxy<IVector<Call^>^> *list = reinterpret_cast< RefToPtrProxy<IVector<Call^>^> *>(vector);
	IVector<Call^>^ calls = (list) ? list->Ref() : nullptr;
	Call^ lCall = (Call^)Utils::GetCall(vCall);
	calls->Append(lCall);
}

IVector<Call^>^ Core::Calls::get()
{
	API_LOCK;
	Vector<Call^>^ calls = ref new Vector<Call^>();
	const MSList *callsList = linphone_core_get_calls(this->lc);
	RefToPtrProxy<IVector<Call^>^> *callsPtr = new RefToPtrProxy<IVector<Call^>^>(calls);
	ms_list_for_each2(callsList, AddCallToVector, callsPtr);
	return calls;
}

int Core::CallsNb::get()
{
	API_LOCK;
	return linphone_core_get_calls_nb(this->lc);
}

int Core::CameraSensorRotation::get()
{
	API_LOCK;
	return linphone_core_get_camera_sensor_rotation(this->lc);
}

Platform::String^ Core::ChatDatabasePath::get()
{
	// TODO
	throw ref new NotImplementedException();
	return nullptr;
}

void Core::ChatDatabasePath::set(Platform::String^ chatDatabasePath)
{
	API_LOCK;
	linphone_core_set_chat_database_path(this->lc, Utils::pstoccs(chatDatabasePath));
}

static void AddChatRoomListToVector(void *vRoom, void *vector)
{
	::LinphoneChatRoom *chatRoom = (LinphoneChatRoom*)vRoom;
	RefToPtrProxy<IVector<ChatRoom^>^> *list = reinterpret_cast<RefToPtrProxy<IVector<ChatRoom^>^> *>(vector);
	IVector<ChatRoom^>^ rooms = (list) ? list->Ref() : nullptr;
	ChatRoom^ room = (ChatRoom^) Utils::GetChatRoom(chatRoom);
	rooms->Append(room);
}

IVector<ChatRoom^>^ Core::ChatRooms::get()
{
	API_LOCK;
	IVector<ChatRoom^>^ rooms = ref new Vector<ChatRoom^>();
	const MSList* roomList = linphone_core_get_chat_rooms(this->lc);
	RefToPtrProxy<IVector<ChatRoom^>^> *roomsPtr = new RefToPtrProxy<IVector<ChatRoom^>^>(rooms);
	ms_list_for_each2(roomList, AddChatRoomListToVector, roomsPtr);
	return rooms;
}

int Core::ConferenceSize::get()
{
	API_LOCK;
	return linphone_core_get_conference_size(this->lc);
}

BelledonneCommunications::Linphone::Native::LpConfig^ Core::Config::get()
{
	API_LOCK;
	::LpConfig *config = linphone_core_get_config(this->lc);
	return (LpConfig^)Utils::CreateLpConfig(config);
}

CoreListener^ Core::CoreListener::get()
{
	return this->listener;
}

void Core::CoreListener::set(BelledonneCommunications::Linphone::Native::CoreListener^ listener)
{
	API_LOCK;
	this->listener = listener;
}

Call^ Core::CurrentCall::get()
{
	API_LOCK;
	::LinphoneCall *lCall = linphone_core_get_current_call(this->lc);
	return (Call^)Utils::GetCall(lCall);
}

ProxyConfig^ Core::DefaultProxyConfig::get()
{
	API_LOCK;
	ProxyConfig^ defaultProxy = nullptr;
	::LinphoneProxyConfig *proxy = linphone_core_get_default_proxy_config(this->lc);
	if (proxy != nullptr) {
		defaultProxy = ref new ProxyConfig(proxy);
	}
	return defaultProxy;
}

void Core::DefaultProxyConfig::set(ProxyConfig^ proxyCfg)
{
	API_LOCK;
	linphone_core_set_default_proxy_config(this->lc, proxyCfg->proxyConfig);
}

int Core::DeviceRotation::get()
{
	API_LOCK;
	return linphone_core_get_device_rotation(this->lc);
}

void Core::DeviceRotation::set(int rotation)
{
	API_LOCK;
	linphone_core_set_device_rotation(this->lc, rotation);
}

int Core::DownloadBandwidth::get()
{
	API_LOCK;
	return linphone_core_get_download_bandwidth(this->lc);
}

void Core::DownloadBandwidth::set(int value)
{
	API_LOCK;
	linphone_core_set_download_bandwidth(this->lc, value);
}

int Core::DownloadPtime::get()
{
	API_LOCK;
	return linphone_core_get_download_ptime(this->lc);
}

void Core::DownloadPtime::set(int ptime)
{
	API_LOCK;
	linphone_core_set_download_ptime(this->lc, ptime);
}

FirewallPolicy Core::FirewallPolicy::get()
{
	API_LOCK;
	return (BelledonneCommunications::Linphone::Native::FirewallPolicy) linphone_core_get_firewall_policy(this->lc);
}

void Core::FirewallPolicy::set(BelledonneCommunications::Linphone::Native::FirewallPolicy policy)
{
	API_LOCK;
	linphone_core_set_firewall_policy(this->lc, (LinphoneFirewallPolicy)policy);
}

int Core::InCallTimeout::get()
{
	API_LOCK;
	return linphone_core_get_in_call_timeout(this->lc);
}

void Core::InCallTimeout::set(int timeout)
{
	API_LOCK;
	linphone_core_set_in_call_timeout(this->lc, timeout);
}

int Core::IncTimeout::get()
{
	API_LOCK;
	return linphone_core_get_inc_timeout(this->lc);
}

void Core::IncTimeout::set(int timeout)
{
	API_LOCK;
	linphone_core_set_inc_timeout(this->lc, timeout);
}

Platform::Boolean Core::IsEchoCancellationEnabled::get()
{
	API_LOCK;
	return (linphone_core_echo_cancellation_enabled(this->lc) == TRUE);
}

void Core::IsEchoCancellationEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_echo_cancellation(this->lc, enable);
}

Platform::Boolean Core::IsEchoLimiterEnabled::get()
{
	API_LOCK;
	return (linphone_core_echo_limiter_enabled(this->lc) == TRUE);
}

void Core::IsEchoLimiterEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_echo_limiter(this->lc, enable);
}

Platform::Boolean Core::IsInCall::get()
{
	API_LOCK;
	return (linphone_core_in_call(this->lc) == TRUE);
}

Platform::Boolean Core::IsIncomingInvitePending::get()
{
	API_LOCK;
	return (linphone_core_is_incoming_invite_pending(this->lc) == TRUE);
}

Platform::Boolean Core::IsInConference::get()
{
	API_LOCK;
	return (linphone_core_is_in_conference(this->lc) == TRUE);
}

Platform::Boolean Core::IsIpv6Enabled::get()
{
	API_LOCK;
	return (linphone_core_ipv6_enabled(this->lc) == TRUE);
}

void Core::IsIpv6Enabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_ipv6(this->lc, enable);
}

Platform::Boolean Core::IsIterateEnabled::get()
{
	return isIterateEnabled;
}

void Core::IsIterateEnabled::set(Platform::Boolean value)
{
	API_LOCK;
	if (isIterateEnabled && !value && IterateWorkItem)
	{
		IterateWorkItem->Cancel();
		IterateWorkItem = nullptr;
	}
	else if (!isIterateEnabled && value)
	{
		IAsyncAction^ IterateWorkItem = Windows::System::Threading::ThreadPool::RunAsync(ref new Windows::System::Threading::WorkItemHandler([this](IAsyncAction^ action)
		{
			while (true) {
				GlobalApiLock::Instance()->Lock(__FUNCTION__);
				linphone_core_iterate(this->lc);
				GlobalApiLock::Instance()->Unlock(__FUNCTION__);
				ms_usleep(20000);
			}
		}), Windows::System::Threading::WorkItemPriority::Low);
	}
	isIterateEnabled = value;
}

Platform::Boolean Core::IsKeepAliveEnabled::get()
{
	API_LOCK;
	return (linphone_core_keep_alive_enabled(this->lc) == TRUE);
}

void Core::IsKeepAliveEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_keep_alive(this->lc, enable);
}

Platform::Boolean Core::IsMediaEncryptionMandatory::get()
{
	API_LOCK;
	return (linphone_core_is_media_encryption_mandatory(this->lc) == TRUE);
}

void Core::IsMediaEncryptionMandatory::set(Platform::Boolean yesno)
{
	API_LOCK;
	linphone_core_set_media_encryption_mandatory(this->lc, yesno);
}

Platform::Boolean Core::IsMicEnabled::get()
{
	API_LOCK;
	return (linphone_core_mic_enabled(this->lc) == TRUE);
}

void Core::IsMicEnabled::set(Platform::Boolean isMuted)
{
	API_LOCK;
	linphone_core_enable_mic(this->lc, isMuted);
}

Platform::Boolean Core::IsNetworkReachable::get()
{
	API_LOCK;
	return (linphone_core_is_network_reachable(this->lc) == TRUE);
}

void Core::IsNetworkReachable::set(Platform::Boolean isReachable)
{
	API_LOCK;
	linphone_core_set_network_reachable(this->lc, isReachable);
}

Platform::Boolean Core::IsSelfViewEnabled::get()
{
	API_LOCK;
	return (linphone_core_self_view_enabled(this->lc) == TRUE);
}

void Core::IsSelfViewEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_self_view(this->lc, enable);
}

Platform::Boolean Core::IsSoundResourcesLocked::get()
{
	API_LOCK;
	return (linphone_core_sound_resources_locked(this->lc) == TRUE);
}

Platform::Boolean Core::IsVideoCaptureEnabled::get()
{
	API_LOCK;
	return (linphone_core_video_capture_enabled(this->lc) == TRUE);
}

void Core::IsVideoCaptureEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_video_capture(this->lc, enable);
}

Platform::Boolean Core::IsVideoDisplayEnabled::get()
{
	API_LOCK;
	return (linphone_core_video_display_enabled(this->lc) == TRUE);
}

void Core::IsVideoDisplayEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_core_enable_video_display(this->lc, enable);
}

Platform::Boolean Core::IsVideoSupported::get()
{
	API_LOCK;
	return (linphone_core_video_supported(this->lc) == TRUE);
}

Platform::String^ Core::LogCollectionUploadServerUrl::get()
{
	// TODO
	throw ref new NotImplementedException();
	return nullptr;
}

void Core::LogCollectionUploadServerUrl::set(Platform::String^ url)
{
	API_LOCK;
	const char *curl = Utils::pstoccs(url);
	linphone_core_set_log_collection_upload_server_url(this->lc, curl);
	delete(curl);
}

int Core::MaxCalls::get()
{
	API_LOCK;
	return linphone_core_get_max_calls(this->lc);
}

void Core::MaxCalls::set(int max)
{
	API_LOCK;
	linphone_core_set_max_calls(this->lc, max);
}

MediaEncryption Core::MediaEncryption::get()
{
	API_LOCK;
	return (BelledonneCommunications::Linphone::Native::MediaEncryption) linphone_core_get_media_encryption(this->lc);
}

void Core::MediaEncryption::set(BelledonneCommunications::Linphone::Native::MediaEncryption menc)
{
	API_LOCK;
	linphone_core_set_media_encryption(this->lc, (LinphoneMediaEncryption)menc);
}

float Core::MicGainDb::get()
{
	API_LOCK;
	return linphone_core_get_mic_gain_db(this->lc);
}

void Core::MicGainDb::set(float gain)
{
	API_LOCK;
	linphone_core_set_mic_gain_db(this->lc, gain);
}

int Core::MissedCallsCount::get()
{
	API_LOCK;
	return linphone_core_get_missed_calls_count(this->lc);
}

Platform::Object^ Core::NativePreviewWindowId::get()
{
	API_LOCK;
	void *id = linphone_core_get_native_preview_window_id(this->lc);
	if (id == NULL) return nullptr;
	RefToPtrProxy<Platform::Object^> *proxy = reinterpret_cast<RefToPtrProxy<Platform::Object^>*>(id);
	Platform::Object^ nativeWindowId = (proxy) ? proxy->Ref() : nullptr;
	return nativeWindowId;
}

void Core::NativePreviewWindowId::set(Platform::Object^ value)
{
	API_LOCK;
	RefToPtrProxy<Platform::Object^> *nativeWindowId = new RefToPtrProxy<Platform::Object^>(value);
	linphone_core_set_native_preview_window_id(this->lc, nativeWindowId);
}

Platform::Object^ Core::NativeVideoWindowId::get()
{
	API_LOCK;
	void *id = linphone_core_get_native_video_window_id(this->lc);
	if (id == NULL) return nullptr;
	RefToPtrProxy<Platform::Object^> *proxy = reinterpret_cast<RefToPtrProxy<Platform::Object^>*>(id);
	Platform::Object^ nativeWindowId = (proxy) ? proxy->Ref() : nullptr;
	return nativeWindowId;
}

void Core::NativeVideoWindowId::set(Platform::Object^ value)
{
	API_LOCK;
	RefToPtrProxy<Platform::Object^> *nativeWindowId = new RefToPtrProxy<Platform::Object^>(value);
	linphone_core_set_native_video_window_id(this->lc, nativeWindowId);
}

float Core::PlaybackGainDb::get()
{
	API_LOCK;
	return linphone_core_get_playback_gain_db(this->lc);
}

void Core::PlaybackGainDb::set(float gain)
{
	API_LOCK;
	linphone_core_set_playback_gain_db(this->lc, gain);
}

Platform::String^ Core::PlayFile::get()
{
	API_LOCK;
	return Utils::cctops(linphone_core_get_play_file(this->lc));
}

void Core::PlayFile::set(Platform::String^ path)
{
	API_LOCK;
	const char* file = Utils::pstoccs(path);
	linphone_core_set_play_file(this->lc, file);
	delete(file);
}

int Core::PlayLevel::get()
{
	API_LOCK;
	return linphone_core_get_play_level(this->lc);
}

void Core::PlayLevel::set(int level)
{
	API_LOCK;
	linphone_core_set_play_level(this->lc, level);
}

VideoSize^ Core::PreferredVideoSize::get()
{
	API_LOCK;
	VideoSize^ size = nullptr;
	const MSVideoSizeDef *sizesList = linphone_core_get_supported_video_sizes(this->lc);
	MSVideoSize vsize = linphone_core_get_preferred_video_size(this->lc);
	while (sizesList->name != NULL) {
		if ((sizesList->vsize.width == vsize.width) && (sizesList->vsize.height == vsize.height)) {
			Platform::String^ sizeName = Utils::cctops(sizesList->name);
			size = ref new VideoSize(vsize.width, vsize.height, sizeName);
			break;
		}
		sizesList++;
	}
	if (size == nullptr) {
		size = ref new VideoSize(vsize.width, vsize.height);
	}
	return size;
}

void Core::PreferredVideoSize::set(VideoSize^ size)
{
	API_LOCK;
	if (size->Name != nullptr) {
		const char *ccname = Utils::pstoccs(size->Name);
		linphone_core_set_preferred_video_size_by_name(this->lc, ccname);
		delete ccname;
	}
	else {
		MSVideoSize vs = { 0 };
		vs.width = size->Width;
		vs.height = size->Height;
		linphone_core_set_preferred_video_size(this->lc, vs);
	}
}

Platform::String^ Core::PreferredVideoSizeName::get()
{
	API_LOCK;
	char *cSizeName = linphone_core_get_preferred_video_size_name(this->lc);
	Platform::String^ sizeName = Utils::cctops(cSizeName);
	ms_free(cSizeName);
	return sizeName;
}

static void AddProxyConfigToVector(void *vProxyConfig, void *vector)
{
	::LinphoneProxyConfig *lProxyConfig = (::LinphoneProxyConfig *)vProxyConfig;
	RefToPtrProxy<IVector<ProxyConfig^>^> *list = reinterpret_cast< RefToPtrProxy<IVector<ProxyConfig^>^> *>(vector);
	IVector<ProxyConfig^>^ proxyconfigs = (list) ? list->Ref() : nullptr;
	ProxyConfig^ proxyConfig = (ProxyConfig^)Utils::GetProxyConfig(lProxyConfig);
	proxyconfigs->Append(proxyConfig);
}

IVector<ProxyConfig^>^ Core::ProxyConfigList::get()
{
	API_LOCK;
	IVector<ProxyConfig^>^ proxyconfigs = ref new Vector<ProxyConfig^>();
	const MSList *configList = linphone_core_get_proxy_config_list(this->lc);
	RefToPtrProxy<IVector<ProxyConfig^>^> *proxyConfigPtr = new RefToPtrProxy<IVector<ProxyConfig^>^>(proxyconfigs);
	ms_list_for_each2(configList, AddProxyConfigToVector, proxyConfigPtr);
	return proxyconfigs;
}

Platform::String^ Core::RootCa::get()
{
	API_LOCK;
	return Utils::cctops(linphone_core_get_root_ca(this->lc));
}

void Core::RootCa::set(Platform::String^ path)
{
	API_LOCK;
	const char *ccPath = Utils::pstoccs(path);
	linphone_core_set_root_ca(this->lc, ccPath);
	delete ccPath;
}

Transports^ Core::SipTransports::get()
{
	API_LOCK;
	::LCSipTransports transports;
	linphone_core_get_sip_transports(this->lc, &transports);
	return ref new Transports(transports.udp_port, transports.tcp_port, transports.tls_port);
}

void Core::SipTransports::set(Transports^ t)
{
	API_LOCK;
	::LCSipTransports transports;
	memset(&transports, 0, sizeof(LCSipTransports));
	transports.udp_port = t->UDP;
	transports.tcp_port = t->TCP;
	transports.tls_port = t->TLS;
	linphone_core_set_sip_transports(this->lc, &transports);
}

Platform::String^ Core::StunServer::get()
{
	API_LOCK;
	return Utils::cctops(linphone_core_get_stun_server(this->lc));
}

void Core::StunServer::set(Platform::String^ stun)
{
	API_LOCK;
	const char* stunserver = Utils::pstoccs(stun);
	linphone_core_set_stun_server(this->lc, stunserver);
	delete(stunserver);
}

Windows::Foundation::Collections::IVector<VideoSize^>^ Core::SupportedVideoSizes::get()
{
	API_LOCK;
	Vector<VideoSize^>^ sizes = ref new Vector<VideoSize^>();
	const MSVideoSizeDef *sizesList = linphone_core_get_supported_video_sizes(this->lc);
	while (sizesList->name != NULL) {
		Platform::String^ sizeName = Utils::cctops(sizesList->name);
		VideoSize^ size = ref new VideoSize(sizesList->vsize.width, sizesList->vsize.height, sizeName);
		sizes->Append(size);
		sizesList++;
	}
	return sizes;
}

Tunnel^ Core::Tunnel::get()
{
	API_LOCK;
	BelledonneCommunications::Linphone::Native::Tunnel^ tunnel = nullptr;
	LinphoneTunnel *lt = linphone_core_get_tunnel(this->lc);
	if (lt != nullptr) {
		tunnel = ref new BelledonneCommunications::Linphone::Native::Tunnel(lt);
	}
	return tunnel;
}

int Core::UploadBandwidth::get()
{
	API_LOCK;
	return linphone_core_get_upload_bandwidth(this->lc);
}

void Core::UploadBandwidth::set(int value)
{
	API_LOCK;
	linphone_core_set_upload_bandwidth(this->lc, value);
}

int Core::UploadPtime::get()
{
	API_LOCK;
	return linphone_core_get_upload_ptime(this->lc);
}

void Core::UploadPtime::set(int ptime)
{
	API_LOCK;
	linphone_core_set_upload_ptime(this->lc, ptime);
}

Platform::Boolean Core::UseInfoForDtmf::get()
{
	API_LOCK;
	return (linphone_core_get_use_info_for_dtmf(this->lc) == TRUE);
}

void Core::UseInfoForDtmf::set(Platform::Boolean use)
{
	API_LOCK;
	linphone_core_set_use_info_for_dtmf(this->lc, use);
}

Platform::Boolean Core::UseRfc2833ForDtmf::get()
{
	API_LOCK;
	return (linphone_core_get_use_rfc2833_for_dtmf(this->lc) == TRUE);
}

void Core::UseRfc2833ForDtmf::set(Platform::Boolean use)
{
	API_LOCK;
	linphone_core_set_use_rfc2833_for_dtmf(this->lc, use);
}

IVector<BelledonneCommunications::Linphone::Native::PayloadType^>^ Core::VideoCodecs::get()
{
	API_LOCK;
	IVector<PayloadType^>^ codecs = ref new Vector<PayloadType^>();
	const MSList *codecslist = linphone_core_get_video_codecs(this->lc);
	RefToPtrProxy<IVector<PayloadType^>^> *codecsPtr = new RefToPtrProxy<IVector<PayloadType^>^>(codecs);
	ms_list_for_each2(codecslist, AddPayloadTypeToVector, codecsPtr);
	return codecs;
}

Platform::String^ Core::VideoDevice::get()
{
	API_LOCK;
	Platform::String^ device = nullptr;
	const char *ccname = linphone_core_get_video_device(this->lc);
	if (ccname != NULL) {
		device = Utils::cctops(ccname);
	}
	return device;
}

void Core::VideoDevice::set(Platform::String^ device)
{
	API_LOCK;
	const char *ccname = Utils::pstoccs(device);
	linphone_core_set_video_device(this->lc, ccname);
	delete ccname;
}

Windows::Foundation::Collections::IVector<Platform::String^>^ Core::VideoDevices::get()
{
	API_LOCK;
	Vector<Platform::String^>^ devices = ref new Vector<Platform::String^>();
	const char **lvds = linphone_core_get_video_devices(this->lc);
	while (*lvds != NULL) {
		Platform::String^ device = Utils::cctops(*lvds);
		devices->Append(device);
		lvds++;
	}
	return devices;
}

VideoPolicy^ Core::VideoPolicy::get()
{
	API_LOCK;
	const ::LinphoneVideoPolicy *lvp = linphone_core_get_video_policy(this->lc);
	return ref new BelledonneCommunications::Linphone::Native::VideoPolicy((lvp->automatically_initiate == TRUE), (lvp->automatically_accept == TRUE));
}

void Core::VideoPolicy::set(BelledonneCommunications::Linphone::Native::VideoPolicy^ policy)
{
	API_LOCK;
	::LinphoneVideoPolicy lvp;
	lvp.automatically_initiate = policy->AutomaticallyInitiate;
	lvp.automatically_accept = policy->AutomaticallyAccept;
	linphone_core_set_video_policy(this->lc, &lvp);
}





void Core::AcceptCall(Call^ call)
{
	API_LOCK;
	linphone_core_accept_call(this->lc, call->call);
}

void Core::AcceptCallUpdate(Call^ call, CallParams^ params)
{
	API_LOCK;
	linphone_core_accept_call_update(this->lc, call->call, params->params);
}

void Core::AcceptCallWithParams(Call^ call, CallParams^ params)
{
	API_LOCK;
	linphone_core_accept_call_with_params(this->lc, call->call, params->params);
}

void Core::AddAllToConference()
{
	API_LOCK;
	linphone_core_add_all_to_conference(this->lc);
}

void Core::AddAuthInfo(AuthInfo^ info)
{
	API_LOCK;
	linphone_core_add_auth_info(this->lc, info->auth_info);
}

void Core::AddProxyConfig(ProxyConfig^ proxyCfg)
{
	API_LOCK;
	linphone_core_add_proxy_config(this->lc, proxyCfg->proxyConfig);
}

void Core::AddToConference(Call^ call)
{
	API_LOCK;
	linphone_core_add_to_conference(this->lc, call->call);
}

void Core::ClearAllAuthInfo()
{
	API_LOCK;
	linphone_core_clear_all_auth_info(this->lc);
}

void Core::ClearCallLogs()
{
	API_LOCK;
	linphone_core_clear_call_logs(this->lc);
}

void Core::ClearProxyConfig()
{
	API_LOCK;
	linphone_core_clear_proxy_config(this->lc);
}

Address^ Core::CreateAddress(Platform::String^ address)
{
	API_LOCK;
	const char *caddress = Utils::pstoccs(address);
	Address^ addr = dynamic_cast<Address^>(Utils::CreateAddress(caddress));
	delete(caddress);
	return addr;
}

AuthInfo^ Core::CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain)
{
	API_LOCK;
	AuthInfo^ authInfo = ref new AuthInfo(username, userid, password, ha1, realm, domain);
	return authInfo;
}

CallParams^ Core::CreateCallParams(Call^ call)
{
	API_LOCK;
	return (CallParams^) Utils::GetCallParams(linphone_core_create_call_params(this->lc, call->call));
}

ProxyConfig^ Core::CreateProxyConfig()
{
	API_LOCK;
	ProxyConfig^ proxyConfig = nullptr;
	::LinphoneProxyConfig *proxy = linphone_core_create_proxy_config(this->lc);
	if (proxy != nullptr) {
		proxyConfig = ref new ProxyConfig(proxy);
	}
	return proxyConfig;
}

void Core::DeclineCall(Call^ call, Reason reason)
{
	API_LOCK;
	linphone_core_decline_call(this->lc, call->call, (LinphoneReason)reason);
}

void Core::DeferCallUpdate(Call^ call)
{
	API_LOCK;
	linphone_core_defer_call_update(this->lc, call->call);
}

void Core::EnablePayloadType(BelledonneCommunications::Linphone::Native::PayloadType^ pt, Platform::Boolean enable)
{
	API_LOCK;
	::PayloadType *payload = pt->payload;
	linphone_core_enable_payload_type(this->lc, payload, enable);
}

Platform::Boolean Core::EnterConference()
{
	API_LOCK;
	return (linphone_core_enter_conference(this->lc) == 0);
}

Call^ Core::FindCallFromUri(Platform::String^ uri)
{
	API_LOCK;
	const char *curi = Utils::pstoccs(uri);
	::LinphoneCall *call = const_cast<::LinphoneCall *>(linphone_core_find_call_from_uri(this->lc, curi));
	delete curi;
	return (Call^)Utils::GetCall(call);
}

BelledonneCommunications::Linphone::Native::PayloadType^ Core::FindPayloadType(Platform::String^ mime, int clockRate, int channels)
{
	API_LOCK;
	const char* type = Utils::pstoccs(mime);
	::PayloadType* pt = linphone_core_find_payload_type(this->lc, type, clockRate, channels);
	delete type;
	return (BelledonneCommunications::Linphone::Native::PayloadType^)Utils::CreatePayloadType(pt);
}

ChatRoom^ Core::GetChatRoom(Address^ address)
{
	API_LOCK;
	::LinphoneChatRoom *lChatRoom = linphone_core_get_chat_room(this->lc, address->address);
	return (ChatRoom^)Utils::GetChatRoom(lChatRoom);
}

ChatRoom^ Core::GetChatRoomFromUri(Platform::String^ to)
{
	API_LOCK;
	::LinphoneChatRoom *lChatRoom = linphone_core_get_chat_room_from_uri(this->lc, Utils::pstoccs(to));
	return (ChatRoom^)Utils::GetChatRoom(lChatRoom);
}

Address^ Core::InterpretURL(Platform::String^ destination)
{
	API_LOCK;
	const char* url = Utils::pstoccs(destination);
	Address^ addr = (Address^) Utils::CreateAddress(linphone_core_interpret_url(this->lc, url));
	delete(url);
	return addr;
}

Call^ Core::Invite(Platform::String^ destination)
{
	API_LOCK;
	const char *cc = Utils::pstoccs(destination);
	::LinphoneCall *call = linphone_core_invite(this->lc, cc);
	delete(cc);
	return (Call^)Utils::GetCall(call);
}

Call^ Core::InviteAddress(Address^ destination)
{
	API_LOCK;
	::LinphoneCall *call = linphone_core_invite_address(this->lc, destination->address);
	return (Call^)Utils::GetCall(call);
}

Call^ Core::InviteAddressWithParams(Address^ destination, CallParams^ params)
{
	API_LOCK;
	::LinphoneCall *call = linphone_core_invite_address_with_params(this->lc, destination->address, params->params);
	return (Call^)Utils::GetCall(call);
}

Platform::Boolean Core::IsMediaEncryptionSupported(BelledonneCommunications::Linphone::Native::MediaEncryption menc)
{
	API_LOCK;
	return (linphone_core_media_encryption_supported(this->lc, (LinphoneMediaEncryption)menc) == TRUE);
}

void Core::LeaveConference()
{
	API_LOCK;
	linphone_core_leave_conference(this->lc);
}

Platform::Boolean Core::PauseAllCalls()
{
	API_LOCK;
	return (linphone_core_pause_all_calls(this->lc) == 0);
}

Platform::Boolean Core::PauseCall(Call^ call)
{
	API_LOCK;
	return (linphone_core_pause_call(this->lc, call->call) == 0);
}

bool Core::PayloadTypeEnabled(BelledonneCommunications::Linphone::Native::PayloadType^ pt)
{
	API_LOCK;
	::PayloadType *payload = pt->payload;
	return (linphone_core_payload_type_enabled(this->lc, payload) == TRUE);
}

void Core::PlayDtmf(char16 number, int duration)
{
	API_LOCK;
	char conv[4];
	if (wctomb(conv, number) == 1) {
		linphone_core_play_dtmf(this->lc, conv[0], duration);
	}
}

void Core::RefreshRegisters()
{
	API_LOCK;
	linphone_core_refresh_registers(this->lc);
}

void Core::RemoveCallLog(CallLog^ log)
{
	API_LOCK;
	linphone_core_remove_call_log(this->lc, log->callLog);
}

void Core::RemoveFromConference(Call^ call)
{
	API_LOCK;
	linphone_core_remove_from_conference(this->lc, call->call);
}

void Core::ResetMissedCallsCount()
{
	API_LOCK;
	linphone_core_reset_missed_calls_count(this->lc);
}

Platform::Boolean Core::ResumeCall(Call^ call)
{
	API_LOCK;
	return (linphone_core_resume_call(this->lc, call->call) == 0);
}

void Core::SendDtmf(char16 number)
{
	API_LOCK;
	char conv[4];
	if (wctomb(conv, number) == 1) {
		linphone_core_send_dtmf(this->lc, conv[0]);
	}
}

void Core::SetAudioPortRange(int minP, int maxP)
{
	API_LOCK;
	linphone_core_set_audio_port_range(this->lc, minP, maxP);
}

void Core::SetPreferredVideoSizeByName(Platform::String^ sizeName)
{
	API_LOCK;
	linphone_core_set_preferred_video_size_by_name(this->lc, Utils::pstoccs(sizeName));
}

void Core::SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status)
{
	API_LOCK;
	const char* ac = Utils::pstoccs(alternativeContact);
	linphone_core_set_presence_info(this->lc, minuteAway, ac, (LinphoneOnlineStatus)status);
	delete(ac);
}

void Core::SetPrimaryContact(Platform::String^ contact)
{
	API_LOCK;
	const char *ccontact = Utils::pstoccs(contact);
	linphone_core_set_primary_contact(this->lc, ccontact);
	delete(ccontact);
}

void Core::SetUserAgent(Platform::String^ name, Platform::String^ version)
{
	API_LOCK;
	const char* ua = Utils::pstoccs(name);
	const char* v = Utils::pstoccs(version);
	linphone_core_set_user_agent(this->lc, ua, v);
	delete(v);
	delete(ua);
}

void Core::StopDtmf()
{
	API_LOCK;
	linphone_core_stop_dtmf(this->lc);
}

void Core::TerminateAllCalls()
{
	API_LOCK;
	linphone_core_terminate_all_calls(this->lc);
}

void Core::TerminateCall(Call^ call)
{
	API_LOCK;
	linphone_core_terminate_call(this->lc, call->call);
}

void Core::TerminateConference()
{
	API_LOCK;
	linphone_core_terminate_conference(this->lc);
}

void Core::UpdateCall(Call^ call, CallParams^ params)
{
	API_LOCK;
	if (params != nullptr) {
		linphone_core_update_call(this->lc, call->call, params->params);
	}
	else {
		linphone_core_update_call(this->lc, call->call, nullptr);
	}
}

void Core::UploadLogCollection()
{
	API_LOCK;
	linphone_core_upload_log_collection(this->lc);
}

#if 0
static void EchoCalibrationCallback(LinphoneCore *lc, LinphoneEcCalibratorStatus status, int delay_ms, void *data)
{
	Utils::EchoCalibrationCallback(lc, status, delay_ms, data);
}

static void EchoCalibrationAudioInit(void *data)
{
	EchoCalibrationData *ecData = static_cast<EchoCalibrationData *>(data);
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
	EchoCalibrationData *ecData = static_cast<EchoCalibrationData *>(data);
	if (ecData != nullptr) {
		ecData->call->NotifyCallEnded();
		AudioRoutingManager::GetDefault()->SetAudioEndpoint(AudioRoutingEndpoint::Default);
	}
}

void Core::StartEchoCalibration() 
{
	API_LOCK;
	EchoCalibrationData *data = new EchoCalibrationData();
	linphone_core_start_echo_calibration(this->lc, EchoCalibrationCallback, EchoCalibrationAudioInit, EchoCalibrationAudioUninit, data);
}

int Core::NativeVideoWindowId::get()
{
	API_LOCK;
	return Globals::Instance->VideoRenderer->GetNativeWindowId();
}
#endif

void global_state_changed(::LinphoneCore *lc, ::LinphoneGlobalState gstate, const char *msg)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		GlobalState state = (GlobalState) gstate;
		listener->GlobalStateChanged(state, Utils::cctops(msg));
	}
}

void registration_state_changed(::LinphoneCore *lc, ::LinphoneProxyConfig *cfg, ::LinphoneRegistrationState cstate, const char *msg)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		RegistrationState state = (RegistrationState) cstate;
		RefToPtrProxy<ProxyConfig^> *proxy = reinterpret_cast< RefToPtrProxy<ProxyConfig^> *>(linphone_proxy_config_get_user_data(cfg));
		ProxyConfig^ config = (proxy) ? proxy->Ref() : nullptr;
		listener->RegistrationStateChanged(config, state, Utils::cctops(msg));
	}
}

void call_state_changed(::LinphoneCore *lc, ::LinphoneCall *call, ::LinphoneCallState cstate, const char *msg)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		Call^ lCall = (Call^)Utils::GetCall(call);
		CallState state = (CallState) cstate;
		listener->CallStateChanged(lCall, state, Utils::cctops(msg));
	}

#if 0
	CallController^ callController = Globals::Instance->CallController;
	if (state == CallState::IncomingReceived) {
		Platform::String^ name = lCall->RemoteAddress->DisplayName;
		if (name == nullptr || name->Length() <= 0) 
		{
			name = lCall->RemoteAddress->UserName;
		}
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = callController->OnIncomingCallReceived(lCall, name, lCall->RemoteAddress->AsStringUriOnly(), callController->IncomingCallViewDismissed);
		lCall->CallContext = platformCall;
	} 
	else if (state == CallState::OutgoingProgress) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = callController->NewOutgoingCall(lCall->RemoteAddress->AsStringUriOnly());
		lCall->CallContext = platformCall;
	}
	else if (state == CallState::End || state == CallState::Error) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = (Windows::Phone::Networking::Voip::VoipPhoneCall^) lCall->CallContext;
		if (platformCall != nullptr)
			platformCall->NotifyCallEnded();

		if (callController->IncomingCallViewDismissed != nullptr) {
			// When we receive a call with PN, call the callback to kill the agent process in case the caller stops the call before user accepts/denies it
			callController->IncomingCallViewDismissed();
		}
	}
	else if (state == CallState::Paused || state == CallState::PausedByRemote) {
		Windows::Phone::Networking::Voip::VoipPhoneCall^ platformCall = (Windows::Phone::Networking::Voip::VoipPhoneCall^) lCall->CallContext;
		if (platformCall != nullptr)
			platformCall->NotifyCallHeld();
	}
	else if (state == CallState::StreamsRunning) {
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

void auth_info_requested(::LinphoneCore *lc, const char *realm, const char *username, const char *domain) 
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		listener->AuthInfoRequested(Utils::cctops(realm), Utils::cctops(username), Utils::cctops(domain));
	}
}

void message_received(::LinphoneCore *lc, LinphoneChatRoom* room, LinphoneChatMessage* message)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr)
	{
		ChatRoom^ lChatRoom = (ChatRoom^)Utils::GetChatRoom(room);
		ChatMessage^ lChatMessage = (ChatMessage^)Utils::GetChatMessage(message);
		listener->MessageReceived(lChatRoom, lChatMessage);
	}
}

void is_composing_received(::LinphoneCore *lc, LinphoneChatRoom *room)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		ChatRoom^ lChatRoom = (ChatRoom^)Utils::GetChatRoom(room);
		listener->IsComposingReceived(lChatRoom);
	}
}

void dtmf_received(::LinphoneCore *lc, LinphoneCall *call, int dtmf)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		Call^ lCall = (Call^)Utils::GetCall(call);
		char cdtmf = (char)dtmf;
		char16 wdtmf;
		mbtowc(&wdtmf, &cdtmf, 1);
		listener->DtmfReceived(lCall, wdtmf);
	}
}

void call_encryption_changed(::LinphoneCore *lc, LinphoneCall *call, bool_t on, const char *authentication_token)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		Call^ lCall = (Call^)Utils::GetCall(call);
		listener->CallEncryptionChanged(lCall, (on == TRUE), Utils::cctops(authentication_token));
	}
}

void call_stats_updated(::LinphoneCore *lc, LinphoneCall *call, const LinphoneCallStats *stats)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	Call^ lCall = (Call^)Utils::GetCall(call);
	CallStats^ lStats = (CallStats^)Utils::CreateCallStats((void *)stats);
	if ((lStats != nullptr) && (listener != nullptr)) {
		listener->CallStatsUpdated(lCall, lStats);
	}
}

void log_collection_upload_state_changed(::LinphoneCore *lc, ::LinphoneCoreLogCollectionUploadState state, const char *info)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		listener->LogCollectionUploadStateChanged((LogCollectionUploadState)state, info ? Utils::cctops(info) : nullptr);
	}
}

void log_collection_upload_progress_indication(::LinphoneCore *lc, size_t offset, size_t total)
{
	Core^ lCore = (Core^)Utils::GetCore(lc);
	CoreListener^ listener = lCore->CoreListener;
	if (listener != nullptr) {
		listener->LogCollectionUploadProgressIndication((int)offset, (int)total);
	}
}

Core::Core(BelledonneCommunications::Linphone::Native::CoreListener^ coreListener)
	: lc(nullptr), listener(coreListener), config(ref new LpConfig(nullptr, nullptr)), isIterateEnabled(false)
{
	Init();
}

Core::Core(BelledonneCommunications::Linphone::Native::CoreListener^ coreListener, BelledonneCommunications::Linphone::Native::LpConfig^ config)
	: lc(nullptr), listener(coreListener), config(config), isIterateEnabled(false)
{
	Init();
}

void Core::Init()
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
}

Core::~Core()
{
	API_LOCK;
	IsIterateEnabled = false;
	RefToPtrProxy<Core^> *proxy = reinterpret_cast< RefToPtrProxy<Core^> *>(linphone_core_get_user_data(this->lc));
	delete proxy;
	linphone_core_destroy(this->lc);
}
