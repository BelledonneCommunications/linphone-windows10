#pragma once

#include "CallDirection.h"

namespace Linphone
{
	namespace BackEnd
	{
		ref class LinphoneAddress;

		public enum class LinphoneCallStatus : int
		{
			Success = 0,
			Aborted = 1,
			Missed = 2,
			Declined = 3
		};

		public ref class LinphoneCallLog sealed
		{
		public:
			LinphoneAddress^ GetFrom();
			LinphoneAddress^ GetTo();
			CallDirection GetDirection();
			LinphoneCallStatus GetStatus();
			Platform::String^ GetStartDate();
			int64 GetTimestamp();
			int GetCallDuration();
			int GetCallId();
		};
	}
}