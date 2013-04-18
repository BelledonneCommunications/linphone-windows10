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
			LinphoneCallState GetState();

			/// <summary>
			/// Gets the remote LinphoneAddress.
			/// </summary>
			LinphoneAddress^ GetRemoteAddress();

			/// <summary>
			/// Returns the CallDirection (Outgoing or incoming).
			/// </summary>
			CallDirection GetDirection();

			/// <summary>
			/// Gets the LinphoneCallLog associated with this call.
			/// </summary>
			LinphoneCallLog^ GetCallLog();

			/// <summary>
			/// Gets the audio stats associated with this call.
			/// </summary>
			LinphoneCallStats^ GetAudioStats();

			/// <summary>
			/// This is usefull for example to know if far end supports video or encryption.
			/// </summary>
			LinphoneCallParams^ GetRemoteParams();
			LinphoneCallParams^ GetCurrentParamsCopy();

			void EnableEchoCancellation(Platform::Boolean enable);
			Platform::Boolean IsEchoCancellationEnabled();
			void EnableEchoLimiter(Platform::Boolean enable);
			Platform::Boolean IsEchoLimiterEnabled();
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
			float GetAverageQuality();

			/// <summary>
			/// Used by ZRTP encryption mechanism.
			/// </summary>
			/// <returns>
			/// SAS associated to the main stream [voice].
			/// </returns>
			Platform::String^ GetAuthenticationToken();

			/// <summary>
			/// Used by ZRTP mechanism.
			/// SAS can verified manually by the user or automatically using a previously shared secret.
			/// </summary>
			/// <returns>
			/// true if the main stream [voice] SAS was verified.
			/// </returns>
			Platform::Boolean IsAuthenticationTokenVerified();

			/// <summary>
			/// Used by ZRTP mechanism.
			/// </summary>
			/// <param name="verified">true when displayed SAS is correct</param>
			void SetAuthenticationTokenVerified(Platform::Boolean verified);

			Platform::Boolean IsInConference();
			float GetPlayVolume();
			Platform::String^ GetRemoteUserAgent();
			Platform::String^ GetRemoteContact();

			/// <summary>
			/// Uses the CallContext object (native VoipPhoneCall) to get the DateTimeOffset at which the call started
			/// </summary>
			Platform::Object^ GetCallStartTimeFromContext();

			/// <summary>
			/// Gets the CallContext object (native VoipPhoneCall)
			/// </summary>
			property Platform::Object^ CallContext
            {
                Platform::Object^ get();
				void set(Platform::Object^ cc);
            }

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;
			friend ref class Linphone::Core::LinphoneCallStats;

			LinphoneCall(::LinphoneCall *call);
			~LinphoneCall();
			
			Platform::Object^ callContext;
			::LinphoneCall *call;
		};
	}
}