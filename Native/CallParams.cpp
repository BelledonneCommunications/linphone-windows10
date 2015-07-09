/*
CallParams.cpp
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

#include "CallParams.h"
#include "Core.h"
#include "PayloadType.h"
#include "VideoSize.h"

using namespace Platform;


int Linphone::Native::CallParams::AudioBandwidthLimit::get()
{
	// TODO
	throw ref new NotImplementedException();
	return 0;
}

void Linphone::Native::CallParams::AudioBandwidthLimit::set(int value)
{
	API_LOCK;
	linphone_call_params_set_audio_bandwidth_limit(this->params, value);
}

Linphone::Native::MediaDirection Linphone::Native::CallParams::AudioDirection::get()
{
	API_LOCK;
	return (Linphone::Native::MediaDirection) linphone_call_params_get_audio_direction(this->params);
}

void Linphone::Native::CallParams::AudioDirection::set(Linphone::Native::MediaDirection value)
{
	API_LOCK;
	linphone_call_params_set_audio_direction(this->params, (LinphoneMediaDirection)value);
}

Platform::Boolean Linphone::Native::CallParams::IsLowBandwidthEnabled::get()
{
	API_LOCK;
	return (linphone_call_params_low_bandwidth_enabled(this->params) == TRUE);
}

void Linphone::Native::CallParams::IsLowBandwidthEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_params_enable_low_bandwidth(this->params, enable);
}

Platform::Boolean Linphone::Native::CallParams::IsVideoEnabled::get()
{
	API_LOCK;
	return (linphone_call_params_video_enabled(this->params) == TRUE);
}

void Linphone::Native::CallParams::IsVideoEnabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_call_params_enable_video(this->params, enable);
}

Linphone::Native::MediaEncryption Linphone::Native::CallParams::MediaEncryption::get()
{
	API_LOCK;
	return (Linphone::Native::MediaEncryption) linphone_call_params_get_media_encryption(this->params);
}

void Linphone::Native::CallParams::MediaEncryption::set(Linphone::Native::MediaEncryption menc)
{
	API_LOCK;
	linphone_call_params_set_media_encryption(this->params, (LinphoneMediaEncryption) menc);
}

Linphone::Native::VideoSize^ Linphone::Native::CallParams::ReceivedVideoSize::get()
{
	API_LOCK;
	MSVideoSize vs = linphone_call_params_get_received_video_size(this->params);
	return ref new Linphone::Native::VideoSize(vs.width, vs.height);
}

Linphone::Native::VideoSize^ Linphone::Native::CallParams::SentVideoSize::get()
{
	API_LOCK;
	MSVideoSize vs = linphone_call_params_get_sent_video_size(this->params);
	return ref new Linphone::Native::VideoSize(vs.width, vs.height);
}

Linphone::Native::PayloadType^ Linphone::Native::CallParams::UsedAudioCodec::get()
{
	API_LOCK;
	Linphone::Native::PayloadType^ payloadType = nullptr;
	const ::PayloadType *pt = linphone_call_params_get_used_audio_codec(this->params);
	if (pt != nullptr) {
		payloadType = (Linphone::Native::PayloadType^) Linphone::Native::Utils::CreatePayloadType((void*)pt);
	}
	return payloadType;
}

Linphone::Native::PayloadType^ Linphone::Native::CallParams::UsedVideoCodec::get()
{
	API_LOCK;
	Linphone::Native::PayloadType^ payloadType = nullptr;
	const ::PayloadType *pt = linphone_call_params_get_used_video_codec(this->params);
	if (pt != nullptr) {
		payloadType = (Linphone::Native::PayloadType^) Linphone::Native::Utils::CreatePayloadType((void*)pt);
	}
	return payloadType;
}

Linphone::Native::MediaDirection Linphone::Native::CallParams::VideoDirection::get()
{
	API_LOCK;
	return (Linphone::Native::MediaDirection) linphone_call_params_get_video_direction(this->params);
}

void Linphone::Native::CallParams::VideoDirection::set(Linphone::Native::MediaDirection value)
{
	API_LOCK;
	linphone_call_params_set_video_direction(this->params, (LinphoneMediaDirection)value);
}

Linphone::Native::CallParams^ Linphone::Native::CallParams::Copy()
{
	::LinphoneCallParams *newParams = linphone_call_params_copy(this->params);
	return ref new Linphone::Native::CallParams(newParams);
}

Linphone::Native::CallParams::CallParams(::LinphoneCallParams *call_params)
	: params(call_params)
{
	API_LOCK;
	RefToPtrProxy<CallParams^> *proxy = new RefToPtrProxy<CallParams^>(this);
	linphone_call_params_ref(this->params);
	linphone_call_params_set_user_data(this->params, proxy);
}

Linphone::Native::CallParams::~CallParams()
{
	if (this->params != nullptr) {
		linphone_call_params_unref(this->params);
	}
	RefToPtrProxy<CallParams^> *proxy = reinterpret_cast< RefToPtrProxy<CallParams^> *>(linphone_call_params_get_user_data(this->params));
	delete proxy;
}
