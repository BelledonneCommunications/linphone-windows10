#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		/// <summary>
		/// Object representing a media payload type.
		/// </summary>
		public ref class PayloadType sealed
		{
		public:
			/// <summary>
			/// Gets the MIME type of the payload type.
			/// </summary>
			/// <returns>The MIME type as a string</returns>
			Platform::String^ GetMimeType();

			/// <summary>
			/// Gets the clock rate of the payload type.
			/// </summary>
			/// <returns>The clock rate</returns>
			int GetClockRate();

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			PayloadType(::PayloadType *payload);
			~PayloadType();

			::PayloadType *payload;
		};
	}
}