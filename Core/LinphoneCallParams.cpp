#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "PayloadType.h"
#include "Server.h"

void Linphone::Core::LinphoneCallParams::SetAudioBandwidth(int value)
{
	API_LOCK;
	linphone_call_params_set_audio_bandwidth_limit(this->params, value);
	API_UNLOCK;
}

Linphone::Core::MediaEncryption Linphone::Core::LinphoneCallParams::GetMediaEncryption()
{
	API_LOCK;
	Linphone::Core::MediaEncryption enc = (Linphone::Core::MediaEncryption) linphone_call_params_get_media_encryption(this->params);
	API_UNLOCK;
	return enc;
}

void Linphone::Core::LinphoneCallParams::SetMediaEncryption(Linphone::Core::MediaEncryption menc)
{
	API_LOCK;
	linphone_call_params_set_media_encryption(this->params, (LinphoneMediaEncryption) menc);
	API_UNLOCK;
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCallParams::GetUsedAudioCodec()
{
	API_LOCK;
	Linphone::Core::PayloadType^ payloadType = nullptr;
	const ::PayloadType *pt = linphone_call_params_get_used_audio_codec(this->params);
	if (pt != nullptr) {
		payloadType = (Linphone::Core::PayloadType^) Linphone::Core::Utils::CreatePayloadType((void*)pt);
	}
	API_UNLOCK;
	return payloadType;
}

void Linphone::Core::LinphoneCallParams::EnableLowBandwidth(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_params_enable_low_bandwidth(this->params, enable);
	API_UNLOCK;
}

Platform::Boolean Linphone::Core::LinphoneCallParams::IsLowBandwidthEnabled()
{
	API_LOCK;
	Platform::Boolean enabled = (linphone_call_params_low_bandwidth_enabled(this->params) == TRUE);
	API_UNLOCK;
	return enabled;
}

Platform::Boolean Linphone::Core::LinphoneCallParams::IsVideoEnabled()
{
	API_LOCK;
	Platform::Boolean enabled = (linphone_call_params_video_enabled(this->params) == TRUE);
	API_UNLOCK;
	return enabled;
}

void Linphone::Core::LinphoneCallParams::EnableVideo(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_params_enable_video(this->params, enable);
	API_UNLOCK;
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCallParams::GetUsedVideoCodec()
{
	API_LOCK;
	Linphone::Core::PayloadType^ payloadType = nullptr;
	const ::PayloadType *pt = linphone_call_params_get_used_video_codec(this->params);
	if (pt != nullptr) {
		payloadType = (Linphone::Core::PayloadType^) Linphone::Core::Utils::CreatePayloadType((void*)pt);
	}
	API_UNLOCK;
	return payloadType;
}

Windows::Foundation::Size Linphone::Core::LinphoneCallParams::GetSentVideoSize()
{
	API_LOCK;
	MSVideoSize vs = linphone_call_params_get_sent_video_size(this->params);
	Windows::Foundation::Size size;
	size.Width = vs.width;
	size.Height = vs.height;
	API_UNLOCK;
	return size;
}

Windows::Foundation::Size Linphone::Core::LinphoneCallParams::GetReceivedVideoSize()
{
	API_LOCK;
	MSVideoSize vs = linphone_call_params_get_received_video_size(this->params);
	Windows::Foundation::Size size;
	size.Width = vs.width;
	size.Height = vs.height;
	API_UNLOCK;
	return size;
}

Linphone::Core::LinphoneCallParams::LinphoneCallParams(::LinphoneCallParams *call_params) :
	params(call_params)
{

}

Linphone::Core::LinphoneCallParams::~LinphoneCallParams()
{
	
}
