#pragma once

#include <string>

namespace Linphone
{
    namespace Core
    {
		class Utils
		{
		public:
			static const char* pstoccs(Platform::String^ ps);

			static Platform::String^ Linphone::Core::Utils::cctops(const char*);

		private:
			static std::string wstos(std::wstring ws);

			static std::string pstos(Platform::String^ ps);
		};
	}
}