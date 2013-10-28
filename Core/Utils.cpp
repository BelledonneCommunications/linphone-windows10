#include "Utils.h"
#include "LinphoneCall.h"
#include "LinphoneAddress.h"
#include "LinphoneAuthInfo.h"
#include "LinphoneProxyConfig.h"
#include "LinphoneCallLog.h"
#include "LinphoneCallParams.h"
#include "LinphoneCallStats.h"
#include "LinphoneChatMessage.h"
#include "LinphoneChatRoom.h"
#include "LpConfig.h"
#include "PayloadType.h"
#include <Stringapiset.h>

std::wstring Linphone::Core::Utils::UTF8ToUTF16(const char *utf8)
{
	if ((utf8 == nullptr) || (*utf8 == '\0'))
		return std::wstring();

	int utf8len = static_cast<int>(strlen(utf8));

	// Get the size to alloc for utf-16 string
	int utf16len = MultiByteToWideChar(CP_UTF8, 0, utf8, utf8len, nullptr, 0);
	if (utf16len == 0) {
		DWORD error = GetLastError();
		ms_error("Invalid UTF-8 character, can't convert to UTF-16: %d", error);
		return std::wstring();
	}

	// Do the conversion
	std::wstring utf16;
	utf16.resize(utf16len);
	if (MultiByteToWideChar(CP_UTF8, 0, utf8, utf8len, &utf16[0], utf16.length()) == 0) {
		DWORD error = GetLastError();
		ms_error("Error during string conversion from UTF-8 to UTF-16: %d", error);
		return std::wstring();
	}
	return utf16;
}

std::string Linphone::Core::Utils::UTF16ToUTF8(const wchar_t *utf16)
{
	if ((utf16 == nullptr) || (*utf16 == L'\0'))
		return std::string();

	// Get the size to alloc for utf-8 string
	int utf16len = static_cast<int>(wcslen(utf16));
	int utf8len = WideCharToMultiByte(CP_UTF8, 0, utf16, utf16len, NULL, 0, NULL, NULL);
	if (utf8len == 0) {
		DWORD error = GetLastError();
		ms_error("Invalid UTF-16 character, can't convert to UTF-8: %d", error);
		return std::string();
	}

	// Do the conversion
	std::string utf8;
	utf8.resize(utf8len);
	if (WideCharToMultiByte(CP_UTF8, 0, utf16, utf16len, &utf8[0], utf8.length(), NULL, NULL) == 0) {
		DWORD error = GetLastError();
		ms_error("Error during string conversion from UTF-16 to UTF-8: %d", error);
		return std::string();
	}
	return utf8;
}

const char* Linphone::Core::Utils::pstoccs(Platform::String^ ps)
{
	if (ps == nullptr || ps->Length() == 0)
		return NULL;

	std::string s = Linphone::Core::Utils::UTF16ToUTF8(ps->Data());
	char* cc = (char*) malloc(s.length()+1);
	memcpy(cc, s.c_str(), s.length());
	cc[s.length()] = '\0';
	return cc;
}

Platform::String^ Linphone::Core::Utils::cctops(const char* cc)
{
	if (cc == NULL)
		return nullptr;

	std::wstring wid_str = Linphone::Core::Utils::UTF8ToUTF16(cc);
	const wchar_t* w_char = wid_str.c_str();
	return ref new Platform::String(w_char);
}

void Linphone::Core::Utils::LinphoneCoreSetLogHandler(void* logfunc)
{
	gApiLock.Lock();
	linphone_core_set_log_handler(static_cast<OrtpLogFunc>(logfunc));
	gApiLock.Unlock();
}

void Linphone::Core::Utils::LinphoneCoreSetLogLevel(int loglevel)
{
	gApiLock.Lock();
	linphone_core_set_log_level(static_cast<OrtpLogLevel>(loglevel));
	gApiLock.Unlock();
}

Platform::Object^ Linphone::Core::Utils::CreateLpConfig(void* config)
{
	return ref new Linphone::Core::LpConfig((::LpConfig *)config);
}

Platform::Object^ Linphone::Core::Utils::CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath)
{
	return ref new Linphone::Core::LpConfig(configPath, factoryConfigPath);
}

Platform::Object^ Linphone::Core::Utils::CreatePayloadType(void* pt)
{
	return ref new Linphone::Core::PayloadType((::PayloadType *)pt);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneCall(void* call)
{
	return ref new Linphone::Core::LinphoneCall((::LinphoneCall *)call);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneAddress(void* address)
{
	return ref new Linphone::Core::LinphoneAddress((::LinphoneAddress *)address);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneAddressFromUri(const char* uri)
{
	LinphoneAddress^ addr = ref new Linphone::Core::LinphoneAddress(uri);
	if (addr->address != nullptr)
		return addr;
	return nullptr;
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneAuthInfo(void* auth_info)
{
	return ref new Linphone::Core::LinphoneAuthInfo((::LinphoneAuthInfo *)auth_info);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm)
{
	return ref new Linphone::Core::LinphoneAuthInfo(username, userid, password, ha1, realm);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneProxyConfig(void* proxy_config)
{
	return ref new Linphone::Core::LinphoneProxyConfig((::LinphoneProxyConfig *)proxy_config);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneCallLog(void* callLog)
{
	return ref new Linphone::Core::LinphoneCallLog((::LinphoneCallLog *)callLog);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneCallParams(void* callParams)
{
	return ref new Linphone::Core::LinphoneCallParams((::LinphoneCallParams *)callParams);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneCallStats(void* call, int mediaType)
{
	return ref new Linphone::Core::LinphoneCallStats((::LinphoneCall *)call, (Linphone::Core::MediaType)mediaType);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneCallStats(void *callStats)
{
	return ref new Linphone::Core::LinphoneCallStats((::LinphoneCallStats *)callStats);
}

Platform::Object^ Linphone::Core::Utils::CreateTransports()
{
	return ref new Linphone::Core::Transports();
}

Platform::Object^ Linphone::Core::Utils::CreateTransports(int udp_port, int tcp_port, int tls_port)
{
	return ref new Linphone::Core::Transports(udp_port, tcp_port, tls_port);
}

Platform::Object^ Linphone::Core::Utils::CreateTransports(Platform::Object^ t)
{
	return ref new Linphone::Core::Transports(dynamic_cast<Linphone::Core::Transports^>(t));
}

Platform::Object^ Linphone::Core::Utils::CreateVideoPolicy()
{
	return ref new Linphone::Core::VideoPolicy();
}

Platform::Object^ Linphone::Core::Utils::CreateVideoPolicy(Platform::Boolean automaticallyInitiate, Platform::Boolean automaticallyAccept)
{
	return ref new Linphone::Core::VideoPolicy(automaticallyInitiate, automaticallyAccept);
}

Platform::Object^ Linphone::Core::Utils::CreateVideoSize(int width, int height)
{
	return ref new Linphone::Core::VideoSize(width, height);
}

Platform::Object^ Linphone::Core::Utils::CreateVideoSize(int width, int height, Platform::String^ name)
{
	return ref new Linphone::Core::VideoSize(width, height, name);
}

void Linphone::Core::Utils::EchoCalibrationCallback(void *lc, int status, int delay_ms, void *data)
{
	gApiLock.Lock();
	EchoCalibrationData *ecData = static_cast<EchoCalibrationData *>(data);
	if (ecData != nullptr) {
		delete ecData;
	}
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCore^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneCore^> *>(linphone_core_get_user_data(static_cast<::LinphoneCore *>(lc)));
	Linphone::Core::LinphoneCore^ lCore = (proxy) ? proxy->Ref() : nullptr;
	Linphone::Core::EcCalibratorStatus ecStatus = (Linphone::Core::EcCalibratorStatus) status;
	lCore->listener->EcCalibrationStatus(ecStatus, delay_ms);
	gApiLock.Unlock();
}

 Platform::Object^ Linphone::Core::Utils::CreateLinphoneChatMessage(void* message)
 {
	 return ref new Linphone::Core::LinphoneChatMessage((::LinphoneChatMessage *)message);
 }

 Platform::Object^ Linphone::Core::Utils::CreateLinphoneChatRoom(void* room)
 {
	 return ref new Linphone::Core::LinphoneChatRoom((::LinphoneChatRoom *)room);
 }
