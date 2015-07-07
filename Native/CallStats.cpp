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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

#include "ApiLock.h"
#include "CallStats.h"


float Linphone::Native::CallStats::DownloadBandwidth::get()
{
	return this->downloadBandwidth;
}

Linphone::Native::IceState Linphone::Native::CallStats::IceState::get()
{
	return this->iceState;
}

float Linphone::Native::CallStats::JitterBufferSize::get()
{
	return this->jitterBufferSize;
}

int64 Linphone::Native::CallStats::LatePacketsCumulativeNumber::get()
{
	return this->cumulativeLatePackets;
}

float Linphone::Native::CallStats::LocalLateRate::get()
{
	return this->localLateRate;
}

float Linphone::Native::CallStats::LocalLossRate::get()
{
	return this->localLossRate;
}

Linphone::Native::MediaType Linphone::Native::CallStats::MediaType::get()
{
	return this->mediaType;
}

float Linphone::Native::CallStats::ReceiverInterarrivalJitter::get()
{
	return this->receiverInterarrivalJitter;
}

float Linphone::Native::CallStats::ReceiverLossRate::get()
{
	return this->receiverLossRate;
}

float Linphone::Native::CallStats::RoundTripDelay::get()
{
	return this->roundTripDelay;
}

float Linphone::Native::CallStats::SenderInterarrivalJitter::get()
{
	return this->senderInterarrivalJitter;
}

float Linphone::Native::CallStats::SenderLossRate::get()
{
	return this->senderLossRate;
}

float Linphone::Native::CallStats::UploadBandwidth::get()
{
	return this->uploadBandwidth;
}

Linphone::Native::CallStats::CallStats(::LinphoneCall *call, Linphone::Native::MediaType mediaType)
	: call(call)
{
	API_LOCK;
	const ::LinphoneCallStats *stats = nullptr;
	if (mediaType == Linphone::Native::MediaType::Audio) {
		stats = linphone_call_get_audio_stats(this->call);
	} else {
		stats = linphone_call_get_video_stats(this->call);
	}
	FillStats(stats);
}

Linphone::Native::CallStats::CallStats(::LinphoneCallStats *callStats)
{
	API_LOCK;
	FillStats(callStats);
}

Linphone::Native::CallStats::~CallStats()
{
	
}

float Linphone::Native::CallStats::GetSenderLossRate(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_sender_loss_rate(stats);
}

float Linphone::Native::CallStats::GetReceiverLossRate(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_receiver_loss_rate(stats);
}

float Linphone::Native::CallStats::GetSenderInterarrivalJitter(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_sender_interarrival_jitter(stats, this->call);
}

float Linphone::Native::CallStats::GetReceiverInterarrivalJitter(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_receiver_interarrival_jitter(stats, this->call);
}

int64 Linphone::Native::CallStats::GetLatePacketsCumulativeNumber(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_late_packets_cumulative_number(stats, this->call);
}

void Linphone::Native::CallStats::FillStats(const ::LinphoneCallStats *stats)
{
	this->mediaType = (Linphone::Native::MediaType) stats->type;
	this->iceState = (Linphone::Native::IceState) stats->ice_state;
	this->downloadBandwidth = stats->download_bandwidth;
	this->uploadBandwidth = stats->upload_bandwidth;
	this->senderLossRate = GetSenderLossRate(stats);
	this->receiverLossRate = GetReceiverLossRate(stats);
	this->senderInterarrivalJitter = GetSenderInterarrivalJitter(stats);
	this->receiverInterarrivalJitter = GetReceiverInterarrivalJitter(stats);
	this->roundTripDelay = stats->round_trip_delay;
	this->cumulativeLatePackets = GetLatePacketsCumulativeNumber(stats);
	this->jitterBufferSize = stats->jitter_stats.jitter_buffer_size_ms;
	this->localLossRate = stats->local_loss_rate;
	this->localLateRate = stats->local_late_rate;
}
