#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "PayloadType.h"
#include "Server.h"

using namespace Linphone::Core;

void LinphoneCallParams::SetAudioBandwidth(int value)
{

}

MediaEncryption LinphoneCallParams::GetMediaEncryption()
{
	return MediaEncryption::None;
}

void LinphoneCallParams::SetMediaEncryption(MediaEncryption menc)
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
