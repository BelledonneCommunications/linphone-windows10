#pragma once

namespace Linphone
{
	namespace Core
	{
		public ref class PayloadType sealed
		{
		public:
			Platform::String^ GetMime();
			int GetRate();
		};
	}
}