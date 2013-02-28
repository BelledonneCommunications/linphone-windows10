#include "LinphoneCallStats.h"
#include "Server.h"

Linphone::Core::MediaType Linphone::Core::LinphoneCallStats::GetMediaType()
{
	return Linphone::Core::MediaType::Audio;
}

Linphone::Core::IceState Linphone::Core::LinphoneCallStats::GetIceState()
{
	return Linphone::Core::IceState::Failed;
}

float Linphone::Core::LinphoneCallStats::GetDownloadBandwidth()
{
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetUploadBandwidth()
{
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetSenderLossRate()
{
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetReceiverLossRate()
{
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetSenderInterarrivalJitter()
{
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetReceiverInterarrivalJitter()
{
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetRoundTripDelay()
{
	return -1;
}

int64 Linphone::Core::LinphoneCallStats::GetLatePacketsCumulativeNumber()
{
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetJitterBufferSize()
{
	return -1;
}
