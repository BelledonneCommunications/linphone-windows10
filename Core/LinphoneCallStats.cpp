#include "LinphoneCallStats.h"
#include "LinphoneCall.h"
#include "Server.h"
#include "coreapi\private.h"

Linphone::Core::MediaType Linphone::Core::LinphoneCallStats::GetMediaType()
{
	return (Linphone::Core::MediaType) this->stats->type;
}

Linphone::Core::IceState Linphone::Core::LinphoneCallStats::GetIceState()
{
	return (Linphone::Core::IceState) this->stats->ice_state;
}

float Linphone::Core::LinphoneCallStats::GetDownloadBandwidth()
{
	return this->stats->download_bandwidth;
}

float Linphone::Core::LinphoneCallStats::GetUploadBandwidth()
{
	return this->stats->upload_bandwidth;
}

float Linphone::Core::LinphoneCallStats::GetSenderLossRate()
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

	return (100.0 * report_block_get_fraction_lost(srb) / 256.0);
}

float Linphone::Core::LinphoneCallStats::GetReceiverLossRate()
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

	return (100.0 * report_block_get_fraction_lost(rrb) / 256.0);
}

float Linphone::Core::LinphoneCallStats::GetSenderInterarrivalJitter()
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

float Linphone::Core::LinphoneCallStats::GetReceiverInterarrivalJitter()
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

float Linphone::Core::LinphoneCallStats::GetRoundTripDelay()
{
	return this->stats->round_trip_delay;
}

int64 Linphone::Core::LinphoneCallStats::GetLatePacketsCumulativeNumber()
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

float Linphone::Core::LinphoneCallStats::GetJitterBufferSize()
{
	return this->stats->jitter_stats.jitter_buffer_size_ms;
}

float Linphone::Core::LinphoneCallStats::GetLocalLossRate()
{
	return this->stats->local_loss_rate;
}

float Linphone::Core::LinphoneCallStats::GetLocalLateRate()
{
	return this->stats->local_late_rate;
}

Linphone::Core::LinphoneCallStats::LinphoneCallStats(::LinphoneCallStats *call_stats, ::LinphoneCall *call) :
	stats(call_stats),
	call(call)
{

}

Linphone::Core::LinphoneCallStats::~LinphoneCallStats()
{
	
}

