#include "LinphoneCallStats.h"
#include "Server.h"

Linphone::Core::MediaType Linphone::Core::LinphoneCallStats::GetMediaType()
{
	//TODO
	return Linphone::Core::MediaType::Audio;
}

Linphone::Core::IceState Linphone::Core::LinphoneCallStats::GetIceState()
{
	//TODO
	return Linphone::Core::IceState::Failed;
}

float Linphone::Core::LinphoneCallStats::GetDownloadBandwidth()
{
	//TODO
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetUploadBandwidth()
{
	//TODO
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetSenderLossRate()
{
	//TODO
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetReceiverLossRate()
{
	//TODO
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetSenderInterarrivalJitter()
{
	//TODO
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetReceiverInterarrivalJitter()
{
	//TODO
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetRoundTripDelay()
{
	//TODO
	return -1;
}

int64 Linphone::Core::LinphoneCallStats::GetLatePacketsCumulativeNumber()
{
	//TODO
	return -1;
}

float Linphone::Core::LinphoneCallStats::GetJitterBufferSize()
{
	//TODO
	return -1;
}
