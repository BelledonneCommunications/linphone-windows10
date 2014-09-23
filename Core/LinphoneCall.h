#pragma once

#include "Enums.h"
#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneAddress;
		ref class LinphoneCallLog;
		ref class LinphoneCallStats;
		ref class LinphoneCallParams;
		ref class LinphoneCore;

		/// <summary>
		/// Object representing a call.
		/// Calls are create using LinphoneCore::Invite or passed to the application by the listener LinphoneCoreListener::CallState.
		/// </summary>
		public ref class LinphoneCall sealed
		{
		public:
			/// <summary>
			/// Gets the LinphoneCallState of the call (StreamRunning, IncomingReceived, OutgoingProgress, ...).
			/// </summary>
			/// <returns>The LinphoneCallState of the call</returns>
			LinphoneCallState GetState();

			/// <summary>
			/// Gets the remote LinphoneAddress.
			/// </summary>
			/// <returns>The remote address of the call</returns>
			LinphoneAddress^ GetRemoteAddress();

			/// <summary>
			/// Returns the CallDirection (Outgoing or incoming).
			/// </summary>
			/// <returns>The direction of the call</returns>
			CallDirection GetDirection();

			/// <summary>
			/// Gets the LinphoneCallLog associated with this call.
			/// </summary>
			/// <returns>The LinphoneCallLog associated with the call</returns>
			LinphoneCallLog^ GetCallLog();

			/// <summary>
			/// Gets the audio statistics associated with this call.
			/// </summary>
			/// <returns>The audio statistics associated with the call</returns>
			LinphoneCallStats^ GetAudioStats();

			/// <summary>
			/// Gets the call parameters given by the remote peer.
			/// This is useful for example to know if far end supports video or encryption.
			/// </summary>
			/// <returns>The call parameters given by the remote peer</returns>
			LinphoneCallParams^ GetRemoteParams();

			/// <summary>
			/// Gets a copy of the current local call parameters.
			/// </summary>
			/// <returns>A copy of the current local call parameters</returns>
			LinphoneCallParams^ GetCurrentParamsCopy();

			/// <summary>
			/// Enable or disable the echo cancellation.
			/// </summary>
			/// <param name="enable">A boolean value telling whether to enable or to disable the echo cancellation</param>
			void EnableEchoCancellation(Platform::Boolean enable);

			/// <summary>
			/// Tells whether echo cancellation is enabled or not.
			/// </summary>
			/// <returns>true if echo cancellation is enabled, false otherwise</returns>
			Platform::Boolean IsEchoCancellationEnabled();

			/// <summary>
			/// Enable or disable the echo limiter.
			/// </summary>
			/// <param name="enable">A boolean value telling whether to enable or to disable the echo limiter</param>
			void EnableEchoLimiter(Platform::Boolean enable);

			/// <summary>
			/// Tells whether echo limitation is enabled or not.
			/// </summary>
			/// <returns>true if echo limitation is enabled, false otherwise</returns>
			Platform::Boolean IsEchoLimiterEnabled();

			/// <summary>
			/// Gets the current duration of the call in seconds.
			/// </summary>
			/// <returns>The current duration of the call in seconds</returns>
			int GetDuration();
			
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
			float GetCurrentQuality();

			/// <summary>
			/// Returns call quality averaged over all the duration of the call.
			/// See GetCurrentQuality for more details about quality mesurement.
			/// </summary>
			/// <returns>Average quality over all the duration of the call</returns>
			float GetAverageQuality();

			/// <summary>
			/// Used by ZRTP encryption mechanism.
			/// </summary>
			/// <returns>SAS associated to the main stream [voice]</returns>
			Platform::String^ GetAuthenticationToken();

			/// <summary>
			/// Used by ZRTP mechanism.
			/// SAS can verified manually by the user or automatically using a previously shared secret.
			/// </summary>
			/// <returns>true if the main stream [voice] SAS was verified</returns>
			Platform::Boolean IsAuthenticationTokenVerified();

			/// <summary>
			/// Used by ZRTP mechanism.
			/// </summary>
			/// <param name="verified">true when displayed SAS is correct</param>
			void SetAuthenticationTokenVerified(Platform::Boolean verified);

			/// <summary>
			/// Tells whether the call is in conference or not.
			/// </summary>
			/// <returns>A boolean value telling whether the call is in conference</returns>
			Platform::Boolean IsInConference();

			/// <summary>
			/// Gets the measured sound volume played locally (received from remote).
			/// It is expressed in dbm0.
			/// </summary>
			/// <returns>The play volume in dbm0.</returns>
			float GetPlayVolume();

			/// <summary>
			/// Gets the far end's user agent description string, if available.
			/// </summary>
			/// <returns>The remote user agent as a string</returns>
			Platform::String^ GetRemoteUserAgent();

			/// <summary>
			/// Gets the far end's sip contact as a string, if available.
			/// </summary>
			/// <returns>The remote sip contact as a string</returns>
			Platform::String^ GetRemoteContact();

			/// <summary>
			/// Uses the CallContext object (native VoipPhoneCall) to get the DateTimeOffset at which the call started
			/// </summary>
			Platform::Object^ GetCallStartTimeFromContext();

			/// <summary>
			/// Tells whether video captured from the camera is sent to the remote party.
			/// </summary>
			/// <returns>true if video capture from the camera is sent to the remote party, false otherwise</returns>
			Platform::Boolean IsCameraEnabled();

			/// <summary>
			/// Enable or disable sending video captured from the camera to the remote party.
			/// </summary>
			/// <param name="enable">A boolean value telling whether to enable or disable sending video captured from the camera</param>
			void EnableCamera(Platform::Boolean enable);

			/// <summary>
			/// Gets the video statistics associated with this call.
			/// </summary>
			/// <returns>The video statistics associated with the call</returns>
			LinphoneCallStats^ GetVideoStats();

			/// <summary>
			/// Requests remote side to send us a Video Fast Update.
			/// </summary>
			void SendVFURequest();

			/// <summary>
			/// Gets the CallContext object (native VoipPhoneCall)
			/// </summary>
			property Windows::Phone::Networking::Voip::VoipPhoneCall^ CallContext
            {
				Windows::Phone::Networking::Voip::VoipPhoneCall^ get();
				void set(Windows::Phone::Networking::Voip::VoipPhoneCall^ cc);
            }

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;
			friend ref class Linphone::Core::LinphoneCallStats;

			LinphoneCall(::LinphoneCall *call);
			~LinphoneCall();
			
			Windows::Phone::Networking::Voip::VoipPhoneCall^ callContext;
			::LinphoneCall *call;
		};
	}
}