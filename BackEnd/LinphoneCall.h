#pragma once

namespace Linphone
{
	namespace BackEnd
	{
		ref class LinphoneAddress;
		ref class CallDirection;
		ref class LinphoneCallLog;
		ref class LinphoneCallStats;
		ref class LinphoneCallParams;

		public ref class LinphoneCallState sealed
		{
		};

		public ref class LinphoneCall sealed
		{
		public:
			LinphoneCallState^ GetState();
			LinphoneAddress^ GetRemoteAddress();
			CallDirection^ GetDirection();
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
		};
	}
}