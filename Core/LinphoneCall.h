#pragma once

#include "CallDirection.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneAddress;
		ref class LinphoneCallLog;
		ref class LinphoneCallStats;
		ref class LinphoneCallParams;

		public enum class LinphoneCallState : int
		{
			Idle = 0,
			IncomingReceived = 1,
			OutgoingInit = 2,
			OutgoingProgress = 3,
			OutgoingRinging = 4,
			OutgoingEarlyMedia = 5,
			Connected = 6,
			StreamsRunning = 7,
			Pausing = 8,
			Paused = 9,
			Resuming = 10,
			Refered = 11,
			Error = 12,
			CallEnd = 13,
			PausedByRemote = 14,
			UpdatedByRemote = 15,
			IncomingEarlyMedia = 16,
			Udating = 17,
			Released = 18
		};

		public ref class LinphoneCall sealed
		{
		public:
			LinphoneCallState GetState();
			LinphoneAddress^ GetRemoteAddress();
			CallDirection GetDirection();
			LinphoneCallLog^ GetCallLog();
			LinphoneCallStats^ GetAudioStats();
			LinphoneCallParams^ GetRemoteParams();
			LinphoneCallParams^ GetCurrentParamsCopy();

			void EnableEchoCancellation(Platform::Boolean enable);
			Platform::Boolean IsEchoCancellationEnabled();
			void EnableEchoLimiter(Platform::Boolean enable);
			Platform::Boolean IsEchoLimiterEnabled();
			int GetDuraction();
			float GetCurrentQuality();
			float GetAverageQuality();

			Platform::String^ GetAuthenticationToken();
			Platform::Boolean IsAuthenticationTokenVerified();
			void SetAuthenticationTokenVerified(Platform::Boolean verified);

			Platform::Boolean IsInConference();
			float GetPlayVolume();
			Platform::String^ GetRemoteUserAgent();
			Platform::String^ GetRemoteContact();

			property Platform::Object^ CallContext
            {
                Platform::Object^ get();
				void set(Platform::Object^ cc);
            }

		private:
			Platform::Object^ callContext;
		};
	}
}