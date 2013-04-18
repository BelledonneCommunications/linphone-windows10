#pragma once

#include "LinphoneCore.h"

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

			/// <summary>
			/// Gets the download bandwidth in kbits/s.
			/// </summary>
			float GetDownloadBandwidth();

			/// <summary>
			/// Gets the upload bandwidth in kbits/s.
			/// </summary>
			float GetUploadBandwidth();
			float GetSenderLossRate();
			float GetReceiverLossRate();
			float GetSenderInterarrivalJitter();
			float GetReceiverInterarrivalJitter();

			/// <summary>
			/// Gets the round trip delay in seconds.
			/// </summary>
			/// <returns>
			/// -1 if the information is not available.
			/// </returns>
			float GetRoundTripDelay();
			int64 GetLatePacketsCumulativeNumber();

			/// <summary>
			/// Gets the jitter buffer size in milliseconds.
			/// </summary>
			float GetJitterBufferSize();

			float GetLocalLossRate();
			float GetLocalLateRate();

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneCallStats(::LinphoneCallStats* stats, ::LinphoneCall *call);
			~LinphoneCallStats();

			::LinphoneCallStats *stats;
			::LinphoneCall *call;
		};
	}
}