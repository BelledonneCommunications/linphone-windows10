#pragma once

namespace Linphone
{
	namespace BackEnd
	{
		public ref class LpConfig sealed
		{
		public:
			void SetInt(Platform::String^ section, Platform::String^ key, int value);
		};
	}
}