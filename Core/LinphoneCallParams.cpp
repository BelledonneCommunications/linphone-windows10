#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "PayloadType.h"
#include "Server.h"

void Linphone::Core::LinphoneCallParams::SetAudioBandwidth(int value)
{
	gApiLock.Lock();
	linphone_call_params_set_audio_bandwidth_limit(this->params, value);
	gApiLock.Unlock();
}

Linphone::Core::MediaEncryption Linphone::Core::LinphoneCallParams::GetMediaEncryption()
{
	gApiLock.Lock();
	Linphone::Core::MediaEncryption enc = (Linphone::Core::MediaEncryption) linphone_call_params_get_media_encryption(this->params);
	gApiLock.Unlock();
	return enc;
}

void Linphone::Core::LinphoneCallParams::SetMediaEncryption(Linphone::Core::MediaEncryption menc)
{
	gApiLock.Lock();
	linphone_call_params_set_media_encryption(this->params, (LinphoneMediaEncryption) menc);
	gApiLock.Unlock();
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCallParams::GetUsedAudioCodec()
{
	gApiLock.Lock();
	Linphone::Core::PayloadType^ payloadType = nullptr;
	const ::PayloadType *pt = linphone_call_params_get_used_audio_codec(this->params);
	if (pt != nullptr) {
		payloadType = (Linphone::Core::PayloadType^) Linphone::Core::Utils::CreatePayloadType((void*)pt);
	}
	gApiLock.Unlock();
	return payloadType;
}

void Linphone::Core::LinphoneCallParams::EnableLowBandwidth(Platform::Boolean enable)
{
	gApiLock.Lock();
	linphone_call_params_enable_low_bandwidth(this->params, enable);
	gApiLock.Unlock();
}

Platform::Boolean Linphone::Core::LinphoneCallParams::IsLowBandwidthEnabled()
{
	gApiLock.Lock();
	Platform::Boolean enabled = (linphone_call_params_low_bandwidth_enabled(this->params) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

Platform::Boolean Linphone::Core::LinphoneCallParams::IsVideoEnabled()
{
	gApiLock.Lock();
	Platform::Boolean enabled = (linphone_call_params_video_enabled(this->params) == TRUE);
	gApiLock.Unlock();
	return enabled;
}

void Linphone::Core::LinphoneCallParams::EnableVideo(Platform::Boolean enable)
{
	gApiLock.Lock();
	linphone_call_params_enable_video(this->params, enable);
	gApiLock.Unlock();
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCallParams::GetUsedVideoCodec()
{
	gApiLock.Lock();
	Linphone::Core::PayloadType^ payloadType = nullptr;
	const ::PayloadType *pt = linphone_call_params_get_used_video_codec(this->params);
	if (pt != nullptr) {
		payloadType = (Linphone::Core::PayloadType^) Linphone::Core::Utils::CreatePayloadType((void*)pt);
	}
	gApiLock.Unlock();
	return payloadType;
}

Linphone::Core::LinphoneCallParams::LinphoneCallParams(::LinphoneCallParams *call_params) :
	params(call_params)
{

}

Linphone::Core::LinphoneCallParams::~LinphoneCallParams()
{
	
}
