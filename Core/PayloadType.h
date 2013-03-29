#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		public ref class PayloadType sealed
		{
		public:
			Platform::String^ GetMime();
			int GetRate();

		private:
			friend ref class Linphone::Core::LinphoneCore;

			PayloadType(::PayloadType *payload);
			~PayloadType();

			::PayloadType *payload;
		};
	}
}