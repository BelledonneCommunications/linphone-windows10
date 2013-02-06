#pragma once

namespace Linphone
{
	namespace BackEnd
	{
		public ref class PayloadType sealed
		{
		public:
			Platform::String^ GetMime();
			int GetRate();
		};
	}
}