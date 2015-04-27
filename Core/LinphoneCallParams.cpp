/*
LinphoneCallParams.cpp
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

#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "PayloadType.h"
#include "Server.h"

void Linphone::Core::LinphoneCallParams::AudioBandwidthLimit::set(int value)
{
	API_LOCK;
	linphone_call_params_set_audio_bandwidth_limit(this->params, value);
}

Linphone::Core::MediaEncryption Linphone::Core::LinphoneCallParams::MediaEncryption::get()
{
	API_LOCK;
	return (Linphone::Core::MediaEncryption) linphone_call_params_get_media_encryption(this->params);
}

void Linphone::Core::LinphoneCallParams::MediaEncryption::set(Linphone::Core::MediaEncryption menc)
{
	API_LOCK;
	linphone_call_params_set_media_encryption(this->params, (LinphoneMediaEncryption) menc);
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCallParams::UsedAudioCodec::get()
{
	API_LOCK;
	Linphone::Core::PayloadType^ payloadType = nullptr;
	const ::PayloadType *pt = linphone_call_params_get_used_audio_codec(this->params);
	if (pt != nullptr) {
		payloadType = (Linphone::Core::PayloadType^) Linphone::Core::Utils::CreatePayloadType((void*)pt);
	}
	return payloadType;
}

void Linphone::Core::LinphoneCallParams::LowBandwidthEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_params_enable_low_bandwidth(this->params, enable);
}

Platform::Boolean Linphone::Core::LinphoneCallParams::LowBandwidthEnabled::get()
{
	API_LOCK;
	return (linphone_call_params_low_bandwidth_enabled(this->params) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneCallParams::VideoEnabled::get()
{
	API_LOCK;
	return (linphone_call_params_video_enabled(this->params) == TRUE);
}

void Linphone::Core::LinphoneCallParams::VideoEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_params_enable_video(this->params, enable);
}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCallParams::UsedVideoCodec::get()
{
	API_LOCK;
	Linphone::Core::PayloadType^ payloadType = nullptr;
	const ::PayloadType *pt = linphone_call_params_get_used_video_codec(this->params);
	if (pt != nullptr) {
		payloadType = (Linphone::Core::PayloadType^) Linphone::Core::Utils::CreatePayloadType((void*)pt);
	}
	return payloadType;
}

Windows::Foundation::Size Linphone::Core::LinphoneCallParams::SentVideoSize::get()
{
	API_LOCK;
	MSVideoSize vs = linphone_call_params_get_sent_video_size(this->params);
	Windows::Foundation::Size size;
	size.Width = (float)vs.width;
	size.Height = (float)vs.height;
	return size;
}

Windows::Foundation::Size Linphone::Core::LinphoneCallParams::ReceivedVideoSize::get()
{
	API_LOCK;
	MSVideoSize vs = linphone_call_params_get_received_video_size(this->params);
	Windows::Foundation::Size size;
	size.Width = (float)vs.width;
	size.Height = (float)vs.height;
	return size;
}

Linphone::Core::MediaDirection Linphone::Core::LinphoneCallParams::AudioDirection::get()
{
	API_LOCK;
	return (Linphone::Core::MediaDirection) linphone_call_params_get_audio_direction(this->params);
}

void Linphone::Core::LinphoneCallParams::AudioDirection::set(Linphone::Core::MediaDirection value)
{
	API_LOCK;
	linphone_call_params_set_audio_direction(this->params, (LinphoneMediaDirection)value);
}

Linphone::Core::MediaDirection Linphone::Core::LinphoneCallParams::VideoDirection::get()
{
	API_LOCK;
	return (Linphone::Core::MediaDirection) linphone_call_params_get_video_direction(this->params);
}

void Linphone::Core::LinphoneCallParams::VideoDirection::set(Linphone::Core::MediaDirection value)
{
	API_LOCK;
	linphone_call_params_set_video_direction(this->params, (LinphoneMediaDirection)value);
}

Linphone::Core::LinphoneCallParams::LinphoneCallParams(::LinphoneCallParams *call_params) :
	params(call_params)
{
}

Linphone::Core::LinphoneCallParams::~LinphoneCallParams()
{
}
