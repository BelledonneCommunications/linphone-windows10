#pragma once

#include "Enums.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneAddress;

		/// <summary>
		/// Represents a call status
		/// </summary>
		public enum class LinphoneCallStatus : int
		{
			Success = 0,
			Aborted = 1,
			Missed = 2,
			Declined = 3
		};

		/// <summary>
		/// Call data records object
		/// </summary>
		public ref class LinphoneCallLog sealed
		{
		public:
			LinphoneAddress^ GetFrom();
			LinphoneAddress^ GetTo();
			CallDirection GetDirection();
			LinphoneCallStatus GetStatus();

			/// <summary>
			/// Returns a human readable string with the start date/time of the call.
			/// </summary>
			Platform::String^ GetStartDate();

			/// <summary>
			/// Returns a timestamp of the start date/time of the call in milliseconds elapsed since Januart 1st 1970.
			/// </summary>
			int64 GetTimestamp();

			/// <summary>
			/// Returns the call duration in seconds.
			/// </summary>
			int GetCallDuration();

			/// <summary>
			/// Returns the call id from signaling.
			/// </summary>
			int GetCallId();
		};
	}
}