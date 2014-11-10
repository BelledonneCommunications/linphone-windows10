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
			property LinphoneAddress^ From
			{
				LinphoneAddress^ get();
			}

			/// <summary>
			/// Gets the LinphoneAddress of the callee.
			/// </summary>
			property LinphoneAddress^ To
			{
				LinphoneAddress^ get();
			}

			/// <summary>
			/// Gets the CallDirection of the call (Incoming or Outgoing).
			/// </summary>
			property CallDirection Direction
			{
				CallDirection get();
			}

			/// <summary>
			/// Gets the LinphoneCallStatus of the call (Success, Aborted, Missed or Declined).
			/// </summary>
			property LinphoneCallStatus Status
			{
				LinphoneCallStatus get();
			}

			/// <summary>
			/// Returns the start date/time of the call in seconds elpsed since January first 1970.
			/// </summary>
			property int64 StartDate
			{
				int64 get();
			}

			/// <summary>
			/// Returns the call duration in seconds.
			/// </summary>
			property int Duration
			{
				int get();
			}

			/// <summary>
			/// Returns the call id from signaling.
			/// </summary>
			property Platform::String^ CallId
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Tells whether video was enabled at the end of the call.
			/// </summary>
			property Platform::Boolean VideoEnabled
			{
				Platform::Boolean get();
			}

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneCallLog(::LinphoneCallLog *cl);
			~LinphoneCallLog();

			::LinphoneCallLog *callLog;
		};
	}
}