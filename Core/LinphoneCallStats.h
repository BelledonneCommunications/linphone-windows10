#pragma once

namespace Linphone
{
	namespace Core
	{
		public enum class MediaType : int
		{
			Audio = 0,
			Video = 1
		};

		public enum class IceState : int
		{
			NotActivated = 0,
			Failed = 1,
			InProgress = 2,
			HostConnection = 3,
			ReflexiveConnection = 4,
			RelayConnection = 5
		};

		public ref class LinphoneCallStats sealed
		{
		public:
			MediaType GetMediaType();
			IceState GetIceState();
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