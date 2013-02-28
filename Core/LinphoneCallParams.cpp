#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "PayloadType.h"
#include "Server.h"

void Linphone::Core::LinphoneCallParams::SetAudioBandwidth(int value)
{

}

Linphone::Core::MediaEncryption Linphone::Core::LinphoneCallParams::GetMediaEncryption()
{
	return Linphone::Core::MediaEncryption::None;
}

void Linphone::Core::LinphoneCallParams::SetMediaEncryption(Linphone::Core::MediaEncryption menc)
{

}

Linphone::Core::PayloadType^ Linphone::Core::LinphoneCallParams::GetUsedAudioCodec()
{
	return nullptr;
}

void Linphone::Core::LinphoneCallParams::EnableLowBandwidth(Platform::Boolean enable)
{

}

Platform::Boolean Linphone::Core::LinphoneCallParams::IsLowBandwidthEnabled()
{
	return false;
}
