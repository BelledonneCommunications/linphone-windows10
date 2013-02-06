#pragma once

namespace Linphone
{
	namespace BackEnd
	{
		ref class LinphoneAddress;
		ref class CallDirection;

		public ref class LinphoneCallStatus sealed
		{

		};

		public ref class LinphoneCallLog sealed
		{
		public:
			LinphoneAddress^ GetFrom();
			LinphoneAddress^ GetTo();
			CallDirection^ GetDirection();
			LinphoneCallStatus^ GetStatus();
			Platform::String^ GetStartDate();
			int64 GetTimestamp();
			int GetCallDuration();
			int GetCallId();
		};
	}
}