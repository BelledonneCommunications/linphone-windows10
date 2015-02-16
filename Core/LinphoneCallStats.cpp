/*
LinphoneCallStats.cpp
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
#include "LinphoneCallStats.h"
#include "LinphoneCall.h"
#include "Server.h"

Linphone::Core::MediaType Linphone::Core::LinphoneCallStats::MediaType::get()
{
	return this->mediaType;
}

Linphone::Core::IceState Linphone::Core::LinphoneCallStats::IceState::get()
{
	return this->iceState;
}

float Linphone::Core::LinphoneCallStats::DownloadBandwidth::get()
{
	return this->downloadBandwidth;
}

float Linphone::Core::LinphoneCallStats::UploadBandwidth::get()
{
	return this->uploadBandwidth;
}

float Linphone::Core::LinphoneCallStats::SenderLossRate::get()
{
	return this->senderLossRate;
}

float Linphone::Core::LinphoneCallStats::ReceiverLossRate::get()
{
	return this->receiverLossRate;
}

float Linphone::Core::LinphoneCallStats::SenderInterarrivalJitter::get()
{
	return this->senderInterarrivalJitter;
}

float Linphone::Core::LinphoneCallStats::ReceiverInterarrivalJitter::get()
{
	return this->receiverInterarrivalJitter;
}

float Linphone::Core::LinphoneCallStats::RoundTripDelay::get()
{
	return this->roundTripDelay;
}

int64 Linphone::Core::LinphoneCallStats::LatePacketsCumulativeNumber::get()
{
	return this->cumulativeLatePackets;
}

float Linphone::Core::LinphoneCallStats::JitterBufferSize::get()
{
	return this->jitterBufferSize;
}

float Linphone::Core::LinphoneCallStats::LocalLossRate::get()
{
	return this->localLossRate;
}

float Linphone::Core::LinphoneCallStats::LocalLateRate::get()
{
	return this->localLateRate;
}

Linphone::Core::LinphoneCallStats::LinphoneCallStats(::LinphoneCall *call, Linphone::Core::MediaType mediaType) :
	call(call)
{
	API_LOCK;
	const ::LinphoneCallStats *stats = nullptr;
	if (mediaType == Linphone::Core::MediaType::Audio) {
		stats = linphone_call_get_audio_stats(this->call);
	} else {
		stats = linphone_call_get_video_stats(this->call);
	}
	FillStats(stats);
}

Linphone::Core::LinphoneCallStats::LinphoneCallStats(::LinphoneCallStats *callStats)
{
	API_LOCK;
	FillStats(callStats);
}

Linphone::Core::LinphoneCallStats::~LinphoneCallStats()
{
	
}

float Linphone::Core::LinphoneCallStats::GetSenderLossRate(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_sender_loss_rate(stats);
}

float Linphone::Core::LinphoneCallStats::GetReceiverLossRate(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_receiver_loss_rate(stats);
}

float Linphone::Core::LinphoneCallStats::GetSenderInterarrivalJitter(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_sender_interarrival_jitter(stats, this->call);
}

float Linphone::Core::LinphoneCallStats::GetReceiverInterarrivalJitter(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_receiver_interarrival_jitter(stats, this->call);
}

int64 Linphone::Core::LinphoneCallStats::GetLatePacketsCumulativeNumber(const ::LinphoneCallStats *stats)
{
	return linphone_call_stats_get_late_packets_cumulative_number(stats, this->call);
}

void Linphone::Core::LinphoneCallStats::FillStats(const ::LinphoneCallStats *stats)
{
	this->mediaType = (Linphone::Core::MediaType) stats->type;
	this->iceState = (Linphone::Core::IceState) stats->ice_state;
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