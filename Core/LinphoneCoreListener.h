#pragma once

#include "Enums.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneCall;
		ref class LinphoneProxyConfig;
		ref class LinphoneCallStats;
		ref class LinphoneChatMessage;

		public interface class LinphoneCoreListener
		{
		public:
			void AuthInfoRequested(Platform::String^ realm, Platform::String^ username);

			void GlobalState(GlobalState state, Platform::String^ message);

			void CallState(LinphoneCall^ call, LinphoneCallState state);

			void RegistrationState(LinphoneProxyConfig^ config, RegistrationState state, Platform::String^ message);

			void DTMFReceived(LinphoneCall^ call, int dtmf);

			void EcCalibrationStatus(EcCalibratorStatus status, int delay_ms, Platform::Object^ data); 

			void CallEncryptionChanged(LinphoneCall^ call, Platform::Boolean encrypted, Platform::String^ authenticationToken);

			void CallStatsUpdated(LinphoneCall^ call, LinphoneCallStats^ stats);

			void MessageReceived(LinphoneChatMessage^ message);
		};
	}
}