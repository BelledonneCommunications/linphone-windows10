#pragma once

#include "LinphoneCore.h"
#include "Enums.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneCore;
		ref class LinphoneAddress;

		/// <summary>
		/// Call data records object
		/// </summary>
		public ref class LinphoneCallLog sealed
		{
		public:
			/// <summary>
			/// Gets the LinphoneAddress of the caller.
			/// </summary>
			LinphoneAddress^ GetFrom();

			/// <summary>
			/// Gets the LinphoneAddress of the callee.
			/// </summary>
			LinphoneAddress^ GetTo();

			/// <summary>
			/// Gets the CallDirection of the call (Incoming or Outgoing).
			/// </summary>
			CallDirection GetDirection();

			/// <summary>
			/// Gets the LinphoneCallStatus of the call (Success, Aborted, Missed or Declined).
			/// </summary>
			LinphoneCallStatus GetStatus();

			/// <summary>
			/// Returns the start date/time of the call in seconds elpsed since January first 1970.
			/// </summary>
			int64 GetStartDate();

			/// <summary>
			/// Returns the call duration in seconds.
			/// </summary>
			int GetCallDuration();

			/// <summary>
			/// Returns the call id from signaling.
			/// </summary>
			Platform::String^ GetCallId();

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneCallLog(::LinphoneCallLog *cl);
			~LinphoneCallLog();

			::LinphoneCallLog *callLog;
		};
	}
}