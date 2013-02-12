#pragma once

namespace Linphone
{
	namespace Core
	{
		public interface class CallControllerListener
		{
			/// <summary>
			/// Called when a call is started (incoming or outgoing), after the creation of the VoipPhoneCall.
			/// </summary>
			void NewCallStarted(Platform::String^ callerNumber);

			/// <summary>
			/// Called when a call is terminated, after the VoipPhoneCall->NotifyCallEnded();
			/// </summary>
			void CallEnded();
		};
	}
}