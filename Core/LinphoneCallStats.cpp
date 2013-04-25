#include "ApiLock.h"
#include "LinphoneCallStats.h"
#include "LinphoneCall.h"
#include "Server.h"
#include "coreapi\private.h"

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
	gApiLock.Lock();
	const ::LinphoneCallStats *stats = nullptr;
	if (mediaType == MediaType::Audio) {
		stats = linphone_call_get_audio_stats(this->call);
	} else {
		stats = linphone_call_get_video_stats(this->call);
	}
	this->mediaType = (Linphone::Core::MediaType) stats->type;
	this->iceState = (Linphone::Core::IceState) stats->ice_state;
	this->downloadBandwidth = stats->download_bandwidth;
	this->uploadBandwidth = stats->upload_bandwidth;
	this->senderLossRate = UpdateSenderLossRate(stats);
	this->receiverLossRate = UpdateReceiverLossRate(stats);
	this->senderInterarrivalJitter = UpdateSenderInterarrivalJitter(stats);
	this->receiverInterarrivalJitter = UpdateReceiverInterarrivalJitter(stats);
	this->roundTripDelay = stats->round_trip_delay;
	this->cumulativeLatePackets = UpdateLatePacketsCumulativeNumber(stats);
	this->jitterBufferSize = stats->jitter_stats.jitter_buffer_size_ms;
	this->localLossRate = stats->local_loss_rate;
	this->localLateRate = stats->local_late_rate;
	gApiLock.Unlock();
}

Linphone::Core::LinphoneCallStats::~LinphoneCallStats()
{
	
}



float Linphone::Core::LinphoneCallStats::UpdateSenderLossRate(const ::LinphoneCallStats *stats)
{
	const report_block_t *srb = NULL;

	if (!stats || !stats->sent_rtcp)
		return 0.0;

	/* Perform msgpullup() to prevent crashes in rtcp_is_SR() or rtcp_is_RR() if the RTCP packet is composed of several mblk_t structure */
	if (stats->sent_rtcp->b_cont != NULL)
		msgpullup(stats->sent_rtcp, -1);

	if (rtcp_is_SR(stats->sent_rtcp))
		srb = rtcp_SR_get_report_block(stats->sent_rtcp, 0);
	else if (rtcp_is_RR(stats->sent_rtcp))
		srb = rtcp_RR_get_report_block(stats->sent_rtcp, 0);

	if (!srb)
		return 0.0;

	return (100.0f * report_block_get_fraction_lost(srb) / 256.0f);
}

float Linphone::Core::LinphoneCallStats::UpdateReceiverLossRate(const ::LinphoneCallStats *stats)
{
	const report_block_t* rrb = NULL;

	if (!stats || !stats->received_rtcp)
		return 0.0;

	/* Perform msgpullup() to prevent crashes in rtcp_is_SR() or rtcp_is_RR() if the RTCP packet is composed of several mblk_t structure */
	if (stats->received_rtcp->b_cont != NULL)
		msgpullup(stats->received_rtcp, -1);

	if (rtcp_is_RR(stats->received_rtcp))
		rrb = rtcp_RR_get_report_block(stats->received_rtcp, 0);
	else if (rtcp_is_SR(stats->received_rtcp))
		rrb = rtcp_SR_get_report_block(stats->received_rtcp, 0);

	if (!rrb)
		return 0.0;

	return (100.0f * report_block_get_fraction_lost(rrb) / 256.0f);
}

float Linphone::Core::LinphoneCallStats::UpdateSenderInterarrivalJitter(const ::LinphoneCallStats *stats)
{
	const ::LinphoneCallParams* params;
	const ::PayloadType* pt;
	const report_block_t* srb = NULL;

	if (!stats || !this->call || !stats->sent_rtcp)
		return 0.0;

	params = linphone_call_get_current_params(this->call);
	if (!params)
		return 0.0;

	/* Perform msgpullup() to prevent crashes in rtcp_is_SR() or rtcp_is_RR() if the RTCP packet is composed of several mblk_t structure */
	if (stats->sent_rtcp->b_cont != NULL)
		msgpullup(stats->sent_rtcp, -1);

	if (rtcp_is_SR(stats->sent_rtcp))
		srb = rtcp_SR_get_report_block(stats->sent_rtcp, 0);
	else if (rtcp_is_RR(stats->sent_rtcp))
		srb = rtcp_RR_get_report_block(stats->sent_rtcp, 0);

	if (!srb)
		return 0.0;

	if (stats->type == LINPHONE_CALL_STATS_AUDIO)
		pt = linphone_call_params_get_used_audio_codec(params);
	else
		pt = linphone_call_params_get_used_video_codec(params);

	if (!pt || (pt->clock_rate == 0))
		return 0.0;

	return ((float)report_block_get_interarrival_jitter(srb) / (float)pt->clock_rate);
}

float Linphone::Core::LinphoneCallStats::UpdateReceiverInterarrivalJitter(const ::LinphoneCallStats *stats)
{
	const ::LinphoneCallParams* params;
	const ::PayloadType* pt;
	const report_block_t* rrb = NULL;

	if (!stats || !this->call || !stats->received_rtcp)
		return 0.0;

	params = linphone_call_get_current_params(this->call);
	if (!params)
		return 0.0;

	/* Perform msgpullup() to prevent crashes in rtcp_is_SR() or rtcp_is_RR() if the RTCP packet is composed of several mblk_t structure */
	if (stats->received_rtcp->b_cont != NULL)
		msgpullup(stats->received_rtcp, -1);

	if (rtcp_is_SR(stats->received_rtcp))
		rrb = rtcp_SR_get_report_block(stats->received_rtcp, 0);
	else if (rtcp_is_RR(stats->received_rtcp))
		rrb = rtcp_RR_get_report_block(stats->received_rtcp, 0);

	if (!rrb)
		return 0.0;

	if (stats->type == LINPHONE_CALL_STATS_AUDIO)
		pt = linphone_call_params_get_used_audio_codec(params);
	else
		pt = linphone_call_params_get_used_video_codec(params);

	if (!pt || (pt->clock_rate == 0))
		return 0.0;

	return ((float)report_block_get_interarrival_jitter(rrb) / (float)pt->clock_rate);
}

int64 Linphone::Core::LinphoneCallStats::UpdateLatePacketsCumulativeNumber(const ::LinphoneCallStats *stats)
{
	rtp_stats_t rtp_stats;

	if (!stats || !this->call)
		return 0;

	memset(&rtp_stats, 0, sizeof(rtp_stats));
	if (stats->type == LINPHONE_CALL_STATS_AUDIO)
		audio_stream_get_local_rtp_stats(this->call->audiostream, &rtp_stats);
#ifdef VIDEO_ENABLED
	else
		video_stream_get_local_rtp_stats(this->call->videostream, &rtp_stats);
#endif

	return rtp_stats.outoftime;
}
