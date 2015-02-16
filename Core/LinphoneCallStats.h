/*
LinphoneCallStats.h
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
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
			property Linphone::Core::MediaType MediaType
			{
				Linphone::Core::MediaType get();
			}

			/// <summary>
			/// Gets the state of the ICE process.
			/// </summary>
			property Linphone::Core::IceState IceState
			{
				Linphone::Core::IceState get();
			}

			/// <summary>
			/// Gets the download bandwidth in kbits/s.
			/// </summary>
			property float DownloadBandwidth
			{
				float get();
			}

			/// <summary>
			/// Gets the upload bandwidth in kbits/s.
			/// </summary>
			property float UploadBandwidth
			{
				float get();
			}

			/// <summary>
			/// Gets the local loss rate since last emitted RTCP report.
			/// </summary>
			property float SenderLossRate
			{
				float get();
			}

			/// <summary>
			/// Gets the remote loss rate from the last received RTCP report.
			/// </summary>
			property float ReceiverLossRate
			{
				float get();
			}

			/// <summary>
			/// Gets the local interarrival jitter.
			/// </summary>
			property float SenderInterarrivalJitter
			{
				float get();
			}

			/// <summary>
			/// Gets the remote reported interarrival jitter.
			/// </summary>
			property float ReceiverInterarrivalJitter
			{
				float get();
			}

			/// <summary>
			/// Gets the round trip delay in seconds. -1 if the information is not available.
			/// </summary>
			property float RoundTripDelay
			{
				float get();
			}

			/// <summary>
			/// Gets the cumulative number of late packets.
			/// </summary>
			property int64 LatePacketsCumulativeNumber
			{
				int64 get();
			}

			/// <summary>
			/// Gets the jitter buffer size in milliseconds.
			/// </summary>
			property float JitterBufferSize
			{
				float get();
			}

			/// <summary>
			/// Get the local loss rate. Unlike GetSenderLossRate() that returns this loss rate "since last emitted RTCP report", the value returned here is updated every second.
			/// </summary>
			property float LocalLossRate
			{
				float get();
			}

			/// <summary>
			/// Get the local late packets rate. The value returned here is updated every second.
			/// </summary>
			property float LocalLateRate
			{
				float get();
			}

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneCallStats(::LinphoneCall *call, Linphone::Core::MediaType mediaType);
			LinphoneCallStats(::LinphoneCallStats *callStats);
			~LinphoneCallStats();

			float GetSenderLossRate(const ::LinphoneCallStats *stats);
			float GetReceiverLossRate(const ::LinphoneCallStats *stats);
			float GetSenderInterarrivalJitter(const ::LinphoneCallStats *stats);
			float GetReceiverInterarrivalJitter(const ::LinphoneCallStats *stats);
			int64 GetLatePacketsCumulativeNumber(const ::LinphoneCallStats *stats);
			void FillStats(const ::LinphoneCallStats *stats);

			::LinphoneCall *call;
			Linphone::Core::MediaType mediaType;
			Linphone::Core::IceState iceState;
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