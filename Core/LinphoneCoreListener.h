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
		ref class LinphoneChatRoom;

		/// <summary>
		/// Definition of the LinphoneCoreListener interface.
		/// </summary>
		public interface class LinphoneCoreListener
		{
		public:
			/// <summary>
			/// Callback method called when authentication information are requested.
			/// </summary>
			/// <param name="realm">The realm for which authentication information are requested</param>
			/// <param name="username">The username for which authentication information are requested</param>
			void AuthInfoRequested(Platform::String^ realm, Platform::String^ username, Platform::String^ domain);

			/// <summary>
			/// Callback method called when the application state has changed.
			/// </summary>
			/// <param name="state">The new state of the application</param>
			/// <param name="message">A message describing the new state of the application</param>
			void GlobalState(GlobalState state, Platform::String^ message);

			/// <summary>
			/// Callback method called when the state of a call has changed.
			/// </summary>
			/// <param name="call">The call whose state has changed</param>
			/// <param name="state">The new state of the call</param>
			void CallState(LinphoneCall^ call, LinphoneCallState state);

			/// <summary>
			/// Callback method called when the state of the registration of a proxy config has changed.
			/// </summary>
			/// <param name="config">The proxy config for which the registration state has changed</param>
			/// <param name="state">The new registration state for the proxy config</param>
			/// <param name="message">A message describing the new registration state</param>
			void RegistrationState(LinphoneProxyConfig^ config, RegistrationState state, Platform::String^ message);

			/// <summary>
			/// Callback method called when a DTMF is received.
			/// </summary>
			/// <param name="call">The call on which a DTMF has been received</param>
			/// <param name="dtmf">The DTMF that has been received</param>
			void DTMFReceived(LinphoneCall^ call, char16 dtmf);

			/// <summary>
			/// Callback method called when the echo canceller calibration finishes.
			/// </summary>
			/// <param name="status">The status of the echo canceller calibration</param>
			/// <param name="delayMs">The echo delay in milliseconds if the status is EcCalibratorStatus::Done</param>
			void EcCalibrationStatus(EcCalibratorStatus status, int delayMs); 

			/// <summary>
			/// Callback method called when the encryption of a call has changed.
			/// </summary>
			/// <param name="call">The call for which the encryption has changed</param>
			/// <param name="encrypted">A boolean value telling whether the call is encrypted</param>
			/// <param name="authenticationToken">The authentication token for the call if it is encrypted</param>
			void CallEncryptionChanged(LinphoneCall^ call, Platform::Boolean encrypted, Platform::String^ authenticationToken);

			/// <summary>
			/// Callback method called when the statistics of a call have been updated.
			/// </summary>
			/// <param name="call">The call for which the statistics have been updated</param>
			/// <param name="stats">The updated statistics for the call</param>
			void CallStatsUpdated(LinphoneCall^ call, LinphoneCallStats^ stats);

			void MessageReceived(LinphoneChatMessage^ message);

			/// <summary>
			/// Callback method called when the composing status for this room has been updated.
			/// </summary>
			/// <param name="room">The room for which the composing status has been updated</param>
			void ComposingReceived(LinphoneChatRoom^ room);

			void LogUploadStatusChanged(Platform::Boolean uploadComplete, Platform::String^ info);
		};
	}
}