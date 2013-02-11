#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		ref class PayloadType;

		public ref class LinphoneCallParams sealed
		{
		public:
			void SetAudioBandwidth();
			MediaEncryption GetMediaEncryption();
			void SetMediaEncryption(MediaEncryption menc);
			PayloadType^ GetUsedAudioCodec();
			void EnableLowBandwidth(Platform::Boolean enable);
			Platform::Boolean IsLowBandwidthEnabled();
		};
	}
}