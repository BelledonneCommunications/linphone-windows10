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

			static Platform::Object^ CreateLinphoneCall(void* call);

			static Platform::Object^ CreateLinphoneAddress(void* addr);

			static Platform::Object^ CreateLinphoneCallLog(void* calllog);

		private:
			static std::string wstos(std::wstring ws);

			static std::string pstos(Platform::String^ ps);
		};

		template <class T>
		class RefToPtrProxy
		{
		public:
			RefToPtrProxy(T obj) : mObj(obj) {}
			~RefToPtrProxy() { mObj = nullptr; }
			T Ref() { return mObj; }
		private:
			T mObj;
		};
	}
}