/*
CallStats.cpp
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#include "ApiLock.h"
#include "CallStats.h"

using namespace BelledonneCommunications::Linphone::Native;

float CallStats::DownloadBandwidth::get()
{
	return linphone_call_stats_get_download_bandwidth(this->stats);
}

IceState CallStats::IceState::get()
{
	return (BelledonneCommunications::Linphone::Native::IceState)linphone_call_stats_get_ice_state(this->stats);
}

float CallStats::JitterBufferSize::get()
{
	return linphone_call_stats_get_jitter_buffer_size_ms(this->stats);
}

int64 CallStats::LatePacketsCumulativeNumber::get()
{
	return linphone_call_stats_get_late_packets_cumulative_number(this->stats);
}

float CallStats::LocalLateRate::get()
{
	return linphone_call_stats_get_local_late_rate(this->stats);
}

float CallStats::LocalLossRate::get()
{
	return linphone_call_stats_get_local_loss_rate(this->stats);
}

MediaType CallStats::MediaType::get()
{
	return (BelledonneCommunications::Linphone::Native::MediaType)linphone_call_stats_get_type(this->stats);
}

float CallStats::ReceiverInterarrivalJitter::get()
{
	return linphone_call_stats_get_receiver_interarrival_jitter(this->stats);
}

float CallStats::ReceiverLossRate::get()
{
	return linphone_call_stats_get_receiver_loss_rate(this->stats);
}

float CallStats::RoundTripDelay::get()
{
	return linphone_call_stats_get_round_trip_delay(this->stats);
}

float CallStats::SenderInterarrivalJitter::get()
{
	return linphone_call_stats_get_sender_interarrival_jitter(this->stats);
}

float CallStats::SenderLossRate::get()
{
	return linphone_call_stats_get_sender_loss_rate(this->stats);
}

float CallStats::UploadBandwidth::get()
{
	return linphone_call_stats_get_upload_bandwidth(this->stats);
}

CallStats::CallStats(::LinphoneCall *call, BelledonneCommunications::Linphone::Native::MediaType mediaType)
{
	API_LOCK;
	RefToPtrProxy<CallStats^> *proxy = new RefToPtrProxy<CallStats^>(this);
	const ::LinphoneCallStats *stats = nullptr;
	if (mediaType == BelledonneCommunications::Linphone::Native::MediaType::Audio) {
		stats = linphone_call_get_audio_stats(call);
	} else {
		stats = linphone_call_get_video_stats(call);
	}
	linphone_call_stats_ref(this->stats);
	linphone_call_stats_set_user_data(this->stats, proxy);
}

CallStats::CallStats(::LinphoneCallStats *callStats)
{
	API_LOCK;
	RefToPtrProxy<CallStats^> *proxy = new RefToPtrProxy<CallStats^>(this);
	this->stats = callStats;
	linphone_call_stats_ref(this->stats);
	linphone_call_stats_set_user_data(this->stats, proxy);
}

CallStats::~CallStats()
{
	linphone_call_stats_unref(this->stats);
	RefToPtrProxy<CallStats^> *proxy = reinterpret_cast< RefToPtrProxy<CallStats^> *>(linphone_call_stats_get_user_data(this->stats));
	delete proxy;
}
