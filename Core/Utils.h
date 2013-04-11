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

			/// <summary>
			/// Define a log handler.
			/// <param name="logfunc">The function pointer of the log handler.</param>
			/// </summary>
			static void LinphoneCoreSetLogHandler(void *logfunc);

			/// <summary>
			/// Define the log level.
			/// The loglevel parameter is a bitmask parameter. Therefore to enable only warning and error
			/// messages, use ORTP_WARNING | ORTP_ERROR. To disable logs, simply set loglevel to 0.
			/// <param name="loglevel">A bitmask of the log levels to set.</param>
			/// </summary>
			static void LinphoneCoreSetLogLevel(int loglevel);

			static Platform::Object^ CreateLpConfig(void *config);

			static Platform::Object^ CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath);

			static Platform::Object^ CreatePayloadType(void *pt);

			static Platform::Object^ CreateLinphoneCall(void* call);

			static Platform::Object^ CreateLinphoneAddress(void* addr);

			static Platform::Object^ CreateLinphoneAddressFromUri(const char *uri);

			static Platform::Object^ CreateLinphoneAuthInfo(void *auth_info);

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