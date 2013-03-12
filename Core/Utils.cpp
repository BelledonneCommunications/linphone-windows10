#include "Utils.h"

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
	std::string s = pstos(ps);
	char* cc = (char*) malloc(s.length()+1);
	memcpy(cc, s.c_str(), s.length());
	cc[s.length()] = '\0';
	return cc;
}

Platform::String^ Linphone::Core::Utils::cctops(const char* cc)
{
	std::string s_str = std::string(cc);
	std::wstring wid_str = std::wstring(s_str.begin(), s_str.end());
	const wchar_t* w_char = wid_str.c_str();
	return ref new Platform::String(w_char);
}