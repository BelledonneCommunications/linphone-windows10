#include "ApiLock.h"
#include "LinphoneCallStats.h"
#include "LinphoneCall.h"
#include "Server.h"

Linphone::Core::MediaType Linphone::Core::LinphoneCallStats::GetMediaType()
{
	return this->mediaType;
}

Linphone::Core::IceState Linphone::Core::LinphoneCallStats::GetIceState()
{
	return this->iceState;
}

float Linphone::Core::LinphoneCallStats::GetDownloadBandwidth()
{
	return this->downloadBandwidth;
}

float Linphone::Core::LinphoneCallStats::GetUploadBandwidth()
{
	return this->uploadBandwidth;
}

float Linphone::Core::LinphoneCallStats::GetSenderLossRate()
{
	return this->senderLossRate;
}

float Linphone::Core::LinphoneCallStats::GetReceiverLossRate()
{
	return this->receiverLossRate;
}

float Linphone::Core::LinphoneCallStats::GetSenderInterarrivalJitter()
{
	return this->senderInterarrivalJitter;
}

float Linphone::Core::LinphoneCallStats::GetReceiverInterarrivalJitter()
{
	return this->receiverInterarrivalJitter;
}

float Linphone::Core::LinphoneCallStats::GetRoundTripDelay()
{
	return this->roundTripDelay;
}

int64 Linphone::Core::LinphoneCallStats::GetLatePacketsCumulativeNumber()
{
	return this->cumulativeLatePackets;
}

float Linphone::Core::LinphoneCallStats::GetJitterBufferSize()
{
	return this->jitterBufferSize;
}

float Linphone::Core::LinphoneCallStats::GetLocalLossRate()
{
	return this->localLossRate;
}

float Linphone::Core::LinphoneCallStats::GetLocalLateRate()
{
	return this->localLateRate;
}

Linphone::Core::LinphoneCallStats::LinphoneCallStats(::LinphoneCall *call, Linphone::Core::MediaType mediaType) :
	call(call)
{
	TRACE; gApiLock.Lock();
	const ::LinphoneCallStats *stats = nullptr;
	if (mediaType == MediaType::Audio) {
		stats = linphone_call_get_audio_stats(this->call);
	} else {
		stats = linphone_call_get_video_stats(this->call);
	}
	FillStats(stats);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneCallStats::LinphoneCallStats(::LinphoneCallStats *callStats)
{
	TRACE; gApiLock.Lock();
	FillStats(callStats);
	gApiLock.Unlock();
}

Linphone::Core::LinphoneCallStats::~LinphoneCallStats()
{
	
}

float Linphone::Core::LinphoneCallStats::GetSenderLossRate(const ::LinphoneCallStats *stats)
{
	TRACE; gApiLock.Lock();
	float value = linphone_call_stats_get_sender_loss_rate(stats);
	gApiLock.Unlock();
	return value;
}

float Linphone::Core::LinphoneCallStats::GetReceiverLossRate(const ::LinphoneCallStats *stats)
{
	TRACE; gApiLock.Lock();
	float value = linphone_call_stats_get_receiver_loss_rate(stats);
	gApiLock.Unlock();
	return value;
}

float Linphone::Core::LinphoneCallStats::GetSenderInterarrivalJitter(const ::LinphoneCallStats *stats)
{
	TRACE; gApiLock.Lock();
	float value = linphone_call_stats_get_sender_interarrival_jitter(stats, this->call);
	gApiLock.Unlock();
	return value;
}

float Linphone::Core::LinphoneCallStats::GetReceiverInterarrivalJitter(const ::LinphoneCallStats *stats)
{
	TRACE; gApiLock.Lock();
	float value = linphone_call_stats_get_receiver_interarrival_jitter(stats, this->call);
	gApiLock.Unlock();
	return value;
}

int64 Linphone::Core::LinphoneCallStats::GetLatePacketsCumulativeNumber(const ::LinphoneCallStats *stats)
{
	TRACE; gApiLock.Lock();
	int64 value = linphone_call_stats_get_late_packets_cumulative_number(stats, this->call);
	gApiLock.Unlock();
	return value;
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