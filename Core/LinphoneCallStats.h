#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		/// <summary>
		/// Media type of the statistics (audio or video).
		/// </summary>
		public enum class MediaType : int
		{
			Audio = 0,
			Video = 1
		};

		/// <summary>
		/// State of the ICE processing.
		/// </summary>
		public enum class IceState : int
		{
			NotActivated = 0,
			Failed = 1,
			InProgress = 2,
			HostConnection = 3,
			ReflexiveConnection = 4,
			RelayConnection = 5
		};

		/// <summary>
		/// Object representing the statistics of a call.
		/// To get the statistics of a call use the LinphoneCall::GetAudioStats() method. It gives the call statistics at the specific time it is asked for.
		/// So to have updated statistics you need to get the statistics from the call again.
		/// </summary>
		public ref class LinphoneCallStats sealed
		{
		public:
			/// <summary>
			/// Gets the media type (audio or video).
			/// </summary>
			MediaType GetMediaType();

			/// <summary>
			/// Gets the state of the ICE process.
			/// </summary>
			IceState GetIceState();

			/// <summary>
			/// Gets the download bandwidth in kbits/s.
			/// </summary>
			float GetDownloadBandwidth();

			/// <summary>
			/// Gets the upload bandwidth in kbits/s.
			/// </summary>
			float GetUploadBandwidth();

			/// <summary>
			/// Gets the local loss rate since last emitted RTCP report.
			/// </summary>
			float GetSenderLossRate();

			/// <summary>
			/// Gets the remote loss rate from the last received RTCP report.
			/// </summary>
			float GetReceiverLossRate();

			/// <summary>
			/// Gets the local interarrival jitter.
			/// </summary>
			float GetSenderInterarrivalJitter();

			/// <summary>
			/// Gets the remote reported interarrival jitter.
			/// </summary>
			float GetReceiverInterarrivalJitter();

			/// <summary>
			/// Gets the round trip delay in seconds.
			/// </summary>
			/// <returns>
			/// -1 if the information is not available.
			/// </returns>
			float GetRoundTripDelay();

			/// <summary>
			/// Gets the cumulative number of late packets.
			/// </summary>
			int64 GetLatePacketsCumulativeNumber();

			/// <summary>
			/// Gets the jitter buffer size in milliseconds.
			/// </summary>
			float GetJitterBufferSize();

			/// <summary>
			/// Get the local loss rate. Unlike GetSenderLossRate() that returns this loss rate "since last emitted RTCP report", the value returned here is updated every second.
			/// </summary>
			float GetLocalLossRate();

			/// <summary>
			/// Get the local late packets rate. The value returned here is updated every second.
			/// </summary>
			float GetLocalLateRate();

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneCallStats(::LinphoneCall *call, Linphone::Core::MediaType mediaType);
			LinphoneCallStats(::LinphoneCallStats *callStats);
			~LinphoneCallStats();

			float Linphone::Core::LinphoneCallStats::UpdateSenderLossRate(const ::LinphoneCallStats *stats);
			float Linphone::Core::LinphoneCallStats::UpdateReceiverLossRate(const ::LinphoneCallStats *stats);
			float Linphone::Core::LinphoneCallStats::UpdateSenderInterarrivalJitter(const ::LinphoneCallStats *stats);
			float Linphone::Core::LinphoneCallStats::UpdateReceiverInterarrivalJitter(const ::LinphoneCallStats *stats);
			int64 Linphone::Core::LinphoneCallStats::UpdateLatePacketsCumulativeNumber(const ::LinphoneCallStats *stats);
			void Linphone::Core::LinphoneCallStats::FillStats(const ::LinphoneCallStats *stats);

			::LinphoneCall *call;
			MediaType mediaType;
			IceState iceState;
			float downloadBandwidth;
			float uploadBandwidth;
			float senderLossRate;
			float receiverLossRate;
			float senderInterarrivalJitter;
			float receiverInterarrivalJitter;
			float roundTripDelay;
			int64 cumulativeLatePackets;
			float jitterBufferSize;
			float localLossRate;
			float localLateRate;
		};
	}
}