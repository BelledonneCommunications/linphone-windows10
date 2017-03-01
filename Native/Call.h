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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#pragma once

#include "Enums.h"
#include "Core.h"

namespace BelledonneCommunications
{
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
			[Windows::Foundation::Metadata::WebHostHidden]
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
				property CallLog^ CallLog
				{
					BelledonneCommunications::Linphone::Native::CallLog^ get();
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
				/// Gets the current local call parameters.
				/// Do not change this params directly, make a copy with CallParams::Copy to do that.
				/// </summary>
				/// <returns>The current local call parameters</returns>
				property CallParams^ CurrentParams
				{
					CallParams^ get();
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
				/// Returns true if this calls has received a transfer that has not been executed yet.
				/// Pending transfers are executed when this call is being paused or closed, locally or by remote endpoint.
				/// If the call is already paused while receiving the transfer request, the transfer immediately occurs.
				/// </summary>
				property Platform::Boolean HasTransferPending
				{
					Platform::Boolean get();
				}

				/// <summary>
				/// Tells whether the call is in conference or not.
				/// </summary>
				property Platform::Boolean IsInConference
				{
					Platform::Boolean get();
				}

				/// <summary>
				/// Tells whether an operation is in progress at the media side.
				/// </summary>
				property Platform::Boolean MediaInProgress
				{
					Platform::Boolean get();
				}

				/// <summary>
				/// Sets the native video window id (a Windows::UI::Xaml::Controls::MediaElement as a Platform::Object).
				/// </summary>
				property Platform::Object^ NativeVideoWindowId
				{
					Platform::Object^ get();
					void set(Platform::Object^ value);
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
				property Reason Reason
				{
					BelledonneCommunications::Linphone::Native::Reason get();
				}

				/// <summary>
				/// Gets the refer-to uri (if the call was transfered)
				/// </summary>
				property Platform::String^ ReferTo
				{
					Platform::String^ get();
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
				/// Do not change this params directly, make a copy with CallParams::Copy to do that.
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
				/// Returns the call object this call is replacing, if any.
				/// Call replacement can occur during call transfers.
				/// By default, the core automatically terminates the replaced call and accept the new one.
				/// This function allows the application to know whether a new incoming call is a one that replaces another one.
				/// </summary>
				property Call^ ReplacedCall
				{
					Call^ get();
				}

				/// <summary>
				/// Gets the CallState of the call (StreamRunning, IncomingReceived, OutgoingProgress, ...).
				/// </summary>
				property CallState State
				{
					CallState get();
				}

				/// <summary>
				/// Gets the transferer if this call was started automatically as a result of an incoming transfer request.
				/// The call in which the transfer request was received is returned in this case.
				/// </summary>
				property Call^ TransfererCall
				{
					Call^ get();
				}

				/// <summary>
				/// Returns the current transfer state, if a transfer has been initiated from this call.
				/// </summary>
				property CallState TransferState
				{
					CallState get();
				}

				/// <summary>
				/// When this call has received a transfer request, returns the new call that was automatically created as a result of the transfer.
				/// </summary>
				property Call^ TransferTargetCall
				{
					Call^ get();
				}

				/// <summary>
				/// Gets the video statistics associated with this call.
				/// </summary>
				property CallStats^ VideoStats
				{
					CallStats^ get();
				}

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
#endif

				/// <summary>
				/// Gets the CallContext object (native VoipPhoneCall)
				/// </summary>
				property Windows::ApplicationModel::Calls::VoipPhoneCall^ CallContext
				{
					Windows::ApplicationModel::Calls::VoipPhoneCall^ get();
					void set(Windows::ApplicationModel::Calls::VoipPhoneCall^ cc);
				}

			private:
				friend class Utils;
				friend ref class CallStats;
				friend ref class Core;

				Call(::LinphoneCall *call);
				~Call();

				Windows::ApplicationModel::Calls::VoipPhoneCall^ callContext;
				::LinphoneCall *call;
			};
		}
	}
}