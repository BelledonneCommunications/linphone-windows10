#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		public ref class PayloadType sealed
		{
		public:
			Platform::String^ GetMimeType();
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