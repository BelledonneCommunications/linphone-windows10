#include "LinphoneCallStats.h"
#include "Server.h"

using namespace Linphone::BackEnd;

MediaType LinphoneCallStats::GetMediaType()
{
	return MediaType::Audio;
}

IceState LinphoneCallStats::GetIceState()
{
	return IceState::Failed;
}

float LinphoneCallStats::GetDownloadBandwidth()
{
	return -1;
}

float LinphoneCallStats::GetUploadBandwidth()
{
	return -1;
}

float LinphoneCallStats::GetSenderLossRate()
{
	return -1;
}

float LinphoneCallStats::GetReceiverLossRate()
{
	return -1;
}

float LinphoneCallStats::GetSenderInterarrivalJitter()
{
	return -1;
}

float LinphoneCallStats::GetReceiverInterarrivalJitter()
{
	return -1;
}

float LinphoneCallStats::GetRoundTripDelay()
{
	return -1;
}

int64 LinphoneCallStats::GetLatePacketsCumulativeNumber()
{
	return -1;
}

float LinphoneCallStats::GetJitterBufferSize()
{
	return -1;
}
