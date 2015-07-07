/*
Call.h
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

#include "Enums.h"
#include "Core.h"

namespace Linphone
{
	namespace Native
	{
		ref class Address;
		ref class CallLog;
		ref class CallParams;
		ref class CallStats;
		ref class Core;

		/// <summary>
		/// Object representing a call.
		/// Calls are create using Core::Invite or passed to the application by the listener CoreListener::CallState.
		/// </summary>
		public ref class Call sealed
		{
		public:
			/// <summary>
			/// Gets the audio statistics associated with this call.
			/// </summary>
			property CallStats^ AudioStats
			{
				CallStats^ get();
			}

			/// <summary>
			/// Used by ZRTP encryption mechanism.
			/// </summary>
			/// <returns>SAS associated to the main stream [voice]</returns>
			property Platform::String^ AuthenticationToken
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Used by ZRTP mechanism.
			/// SAS can verified manually by the user or automatically using a previously shared secret.
			/// </summary>
			property Platform::Boolean AuthenticationTokenVerified
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Returns call quality averaged over all the duration of the call.
			/// See GetCurrentQuality for more details about quality mesurement.
			/// </summary>
			property float AverageQuality
			{
				float get();
			}

			/// <summary>
			/// Gets the CallLog associated with this call.
			/// </summary>
			property Linphone::Native::CallLog^ CallLog
			{
				Linphone::Native::CallLog^ get();
			}

			/// <summary>
			/// Tells whether video captured from the camera is sent to the remote party.
			/// </summary>
			property Platform::Boolean CameraEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Obtain real time quality rating of the call.
			/// Based on local RTP statistics and RTCP feedback, a quality rating is computed and updated during all the duration of the call.
			/// This function returns its value at the time of the function call.
			/// It is expected that the rating is updated at least every 5 seconds or so.
			/// The rating is a floating point number comprised between 0 and 5.
			/// 4-5 = good quality
			/// 3-4 = average quality
			/// 2-3 = poor quality
			/// 1-2 = very poor quality
			/// 0-1 = can't be worse, mostly unusable
			/// </summary>
			/// <returns>
			/// -1 if no quality mesurement is available, for example if no active audio stream exists. Otherwise returns the quality rating.
			/// </returns>
			property float CurrentQuality
			{
				float get();
			}

			/// <summary>
			/// Returns the CallDirection (Outgoing or incoming).
			/// </summary>
			property CallDirection Direction
			{
				CallDirection get();
			}

			/// <summary>
			/// Gets the current duration of the call in seconds.
			/// </summary>
			property int Duration
			{
				int get();
			}

			/// <summary>
			/// Enable or disable the echo cancellation.
			/// </summary>
			property Platform::Boolean EchoCancellationEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Enable or disable the echo limiter.
			/// </summary>
			property Platform::Boolean EchoLimiterEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Tells whether the call is in conference or not.
			/// </summary>
			property Platform::Boolean InConference
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Gets the measured sound volume played locally (received from remote).
			/// It is expressed in dbm0.
			/// </summary>
			property float PlayVolume
			{
				float get();
			}

			/// <summary>
			/// Gets the reason for a call termination (either error or normal termination)
			/// </summary>
			property Linphone::Native::Reason Reason
			{
				Linphone::Native::Reason get();
			}

			/// <summary>
			/// Gets the remote Address.
			/// </summary>
			property Address^ RemoteAddress
			{
				Address^ get();
			}

			/// <summary>
			/// Gets the far end's sip contact as a string, if available.
			/// </summary>
			property Platform::String^ RemoteContact
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Gets the call parameters given by the remote peer.
			/// This is useful for example to know if far end supports video or encryption.
			/// </summary>
			property CallParams^ RemoteParams
			{
				CallParams^ get();
			}

			/// <summary>
			/// Gets the far end's user agent description string, if available.
			/// </summary>
			property Platform::String^ RemoteUserAgent
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Gets the CallState of the call (StreamRunning, IncomingReceived, OutgoingProgress, ...).
			/// </summary>
			property CallState State
			{
				CallState get();
			}

			/// <summary>
			/// Gets the video statistics associated with this call.
			/// </summary>
			property CallStats^ VideoStats
			{
				CallStats^ get();
			}

			/// <summary>
			/// Gets a copy of the current local call parameters.
			/// </summary>
			/// <returns>A copy of the current local call parameters</returns>
			CallParams^ GetCurrentParamsCopy();

			/// <summary>
			/// Requests remote side to send us a Video Fast Update.
			/// </summary>
			void SendVFURequest();

#if 0
			/// <summary>
			/// Uses the CallContext object (native VoipPhoneCall) to get the DateTimeOffset at which the call started
			/// </summary>
			property Platform::Object^ CallStartTimeFromContext
			{
				Platform::Object^ get();
			}

			/// <summary>
			/// Gets the CallContext object (native VoipPhoneCall)
			/// </summary>
			property Windows::Phone::Networking::Voip::VoipPhoneCall^ CallContext
            {
				Windows::Phone::Networking::Voip::VoipPhoneCall^ get();
				void set(Windows::Phone::Networking::Voip::VoipPhoneCall^ cc);
            }
#endif

		private:
			friend class Linphone::Native::Utils;
			friend ref class Linphone::Native::CallStats;
			friend ref class Linphone::Native::Core;

			Call(::LinphoneCall *call);
			~Call();
			
			//Windows::Phone::Networking::Voip::VoipPhoneCall^ callContext;
			::LinphoneCall *call;
		};
	}
}