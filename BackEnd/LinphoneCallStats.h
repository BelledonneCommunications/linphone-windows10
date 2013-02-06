#pragma once

namespace Linphone
{
	namespace BackEnd
	{
		public ref class MediaType sealed
		{
		};

		public ref class IceState sealed
		{
		};

		public ref class LinphoneCallStats sealed
		{
		public:
			MediaType^ GetMediaType();
			IceState^ GetIceState();
			float GetDownloadBandwidth();
			float GetUploadBandwidth();
			float GetSenderLossRate();
			float GetReceiverLossRate();
			float GetSenderInterarrivalJitter();
			float GetReceiverInterarrivalJitter();
			float GetRoundTripDelay();
			int64 GetLatePacketsCumulativeNumber();
			float GetJitterBufferSize();
		};
	}
}