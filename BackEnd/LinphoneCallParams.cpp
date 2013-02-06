#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "PayloadType.h"
#include "Server.h"

using namespace Linphone::BackEnd;

void LinphoneCallParams::SetAudioBandwidth()
{

}

MediaEncryption^ LinphoneCallParams::GetMediaEncryption()
{
	return nullptr;
}

void LinphoneCallParams::SetMediaEncryption(MediaEncryption^ menc)
{

}

PayloadType^ LinphoneCallParams::GetUsedAudioCodec()
{
	return nullptr;
}

void LinphoneCallParams::EnableLowBandwidth(Platform::Boolean enable)
{

}

Platform::Boolean LinphoneCallParams::IsLowBandwidthEnabled()
{
	return false;
}
