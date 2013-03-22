#include "Utils.h"
#include "LinphoneCall.h"
#include "LinphoneAddress.h"

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

Platform::Object^ Linphone::Core::Utils::CreateLinphoneCall(void* call)
{
	return ref new Linphone::Core::LinphoneCall((::LinphoneCall*)call);
}

Platform::Object^ Linphone::Core::Utils::CreateLinphoneAddress(void* address)
{
	return ref new Linphone::Core::LinphoneAddress((::LinphoneAddress*)address);
}

Platform::Object^ Linphone::Core::Utils::LinphoneCallFromCallPtr(void *ptr)
{
	::LinphoneCall *callptr = reinterpret_cast<::LinphoneCall *>(ptr);
	Linphone::Core::LinphoneCallPtrStub *ptrstub = (Linphone::Core::LinphoneCallPtrStub *) linphone_call_get_user_pointer(callptr);
	if (ptrstub) return ptrstub->call;
	return nullptr;
}

Platform::Object^ Linphone::Core::Utils::LinphoneProxyConfigFromProxyConfigPtr(void *ptr)
{
	::LinphoneProxyConfig *proxyconfigptr = reinterpret_cast<::LinphoneProxyConfig *>(ptr);
	Linphone::Core::LinphoneProxyConfigPtrStub *ptrstub = (Linphone::Core::LinphoneProxyConfigPtrStub *) linphone_proxy_config_get_user_data(proxyconfigptr);
	if (ptrstub) return ptrstub->proxyConfig;
	return nullptr;
}