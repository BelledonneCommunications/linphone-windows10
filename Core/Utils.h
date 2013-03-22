#pragma once

#include <string>
#include <inspectable.h>

namespace Linphone
{
    namespace Core
    {
		class Utils
		{
		public:
			static const char* pstoccs(Platform::String^ ps);

			static Platform::String^ Linphone::Core::Utils::cctops(const char*);

			static void* GetRawPointer(Platform::Object^ object);

			static Platform::Object^ CreateLinphoneCall(void* call);

			static Platform::Object^ CreateLinphoneAddress(void* addr);

			static Platform::Object^ LinphoneCallfromCallPtr(void *ptr);

		private:
			static std::string wstos(std::wstring ws);

			static std::string pstos(Platform::String^ ps);
		};

		typedef struct {
			Platform::Object^ call;
		} LinphoneCallPtrStub;
	}
}