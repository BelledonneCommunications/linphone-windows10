#include "Utils.h"
#include "LinphoneCall.h"
#include "LinphoneAddress.h"
#include "LinphoneAuthInfo.h"
#include "LinphoneCallLog.h"
#include "LpConfig.h"
#include "PayloadType.h"

std::string Linphone::Core::Utils::wstos(std::wstring ws)
{
	std::string s;
	s.assign(ws.begin(), ws.end());
	return s;
}

std::string Linphone::Core::Utils::pstos(Platform::String^ ps)
{
	return wstos(std::wstring(ps->Data()));
}

const char* Linphone::Core::Utils::pstoccs(Platform::String^ ps)
{
	if (ps == nullptr || ps->Length() == 0)
		return NULL;

	std::string s = pstos(ps);
	char* cc = (char*) malloc(s.length()+1);
	memcpy(cc, s.c_str(), s.length());
	cc[s.length()] = '\0';
	return cc;
}

Platform::String^ Linphone::Core::Utils::cctops(const char* cc)
{
	if (cc == NULL)
		return nullptr;

	std::string s_str = std::string(cc);
	std::wstring wid_str = std::wstring(s_str.begin(), s_str.end());
	const wchar_t* w_char = wid_str.c_str();
	return ref new Platform::String(w_char);
}

void Linphone::Core::Utils::LinphoneCoreSetLogHandler(void *logfunc)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_set_log_handler(static_cast<OrtpLogFunc>(logfunc));
}

void Linphone::Core::Utils::LinphoneCoreSetLogLevel(int loglevel)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_core_set_log_level(static_cast<OrtpLogLevel>(loglevel));
}

Platform::Object^ Linphone::Core::Utils::CreateLpConfig(void *config)
{
	return ref new Linphone::Core::LpConfig((::LpConfig *)config);
}

Platform::Object^ Linphone::Core::Utils::CreatePayloadType(void *pt)
{
	return ref new Linphone::Core::PayloadType((::PayloadType *)pt);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneCall(void* call)
{
	return ref new Linphone::Core::LinphoneCall((::LinphoneCall*)call);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneAddress(void* address)
{
	return ref new Linphone::Core::LinphoneAddress((::LinphoneAddress*)address);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneAddressFromUri(const char *uri)
{
	return ref new Linphone::Core::LinphoneAddress(uri);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneAuthInfo(void *auth_info)
{
	return ref new Linphone::Core::LinphoneAuthInfo((::LinphoneAuthInfo *)auth_info);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneCallLog(void* callLog)
{
	return ref new Linphone::Core::LinphoneCallLog((::LinphoneCallLog*)callLog);
}
