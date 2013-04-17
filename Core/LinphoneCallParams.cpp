#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "PayloadType.h"
#include "Server.h"

void Linphone::Core::LinphoneCallParams::SetAudioBandwidth(int value)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_call_params_set_audio_bandwidth_limit(this->params, value);
}

Linphone::Core::MediaEncryption Linphone::Core::LinphoneCallParams::GetMediaEncryption()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	return (Linphone::Core::MediaEncryption) linphone_call_params_get_media_encryption(this->params);
}

void Linphone::Core::LinphoneCallParams::SetMediaEncryption(Linphone::Core::MediaEncryption menc)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_call_params_set_media_encryption(this->params, (LinphoneMediaEncryption) menc);
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCallParams::GetUsedAudioCodec()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	return (Linphone::Core::PayloadType^) Linphone::Core::Utils::CreatePayloadType((void*) linphone_call_params_get_used_audio_codec(this->params));
}

void Linphone::Core::LinphoneCallParams::EnableLowBandwidth(Platform::Boolean enable)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	linphone_call_params_enable_low_bandwidth(this->params, enable);
}

Platform::Boolean Linphone::Core::LinphoneCallParams::IsLowBandwidthEnabled()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	return linphone_call_params_low_bandwidth_enabled(this->params);
}

Linphone::Core::LinphoneCallParams::LinphoneCallParams(::LinphoneCallParams *call_params) :
	params(call_params)
{

}

Linphone::Core::LinphoneCallParams::~LinphoneCallParams()
{
	
}
