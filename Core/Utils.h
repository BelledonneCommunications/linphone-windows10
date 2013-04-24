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
			/// <summary>
			/// Converts a Platform::String into a C char*.
			/// Required to send strings from C# to C.
			/// </summary>
			static const char* pstoccs(Platform::String^ ps);

			/// <summary>
			/// Converts a C char* to a Platform::String.
			/// Required to send strings from C to C#.
			/// </summary>
			static Platform::String^ Linphone::Core::Utils::cctops(const char*);

			/// <summary>
			/// Define a log handler.
			/// <param name="logfunc">The function pointer of the log handler.</param>
			/// </summary>
			static void LinphoneCoreSetLogHandler(void* logfunc);

			/// <summary>
			/// Define the log level.
			/// The loglevel parameter is a bitmask parameter. Therefore to enable only warning and error
			/// messages, use ORTP_WARNING | ORTP_ERROR. To disable logs, simply set loglevel to 0.
			/// <param name="loglevel">A bitmask of the log levels to set.</param>
			/// </summary>
			static void LinphoneCoreSetLogLevel(int loglevel);

			/// <summary>
			/// Creates a C++/CX LpConfig object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLpConfig(void* config);

			/// <summary>
			/// Creates a C++/CX LpConfig object using the path to linphonerc files.
			/// </summary>
			static Platform::Object^ CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath);
			
			/// <summary>
			/// Creates a C++/CX PayloadType object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreatePayloadType(void* pt);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCall object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneCall(void* call);
			
			/// <summary>
			/// Creates a C++/CX LinphoneAddress object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneAddress(void* addr);
			
			/// <summary>
			/// Creates a C++/CX LinphoneAddress object using an URI.
			/// </summary>
			static Platform::Object^ CreateLinphoneAddressFromUri(const char* uri);
			
			/// <summary>
			/// Creates a C++/CX LinphoneAuthInfo object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneAuthInfo(void* auth_info);

			/// <summary>
			/// Creates a C++/CX LinphoneAuthInfo object.
			/// </summary>
			static Platform::Object^ CreateLinphoneAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm);
			
			/// <summary>
			/// Creates a C++/CX LinphoneProxyConfig object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneProxyConfig(void* proxy_config);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallLog object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneCallLog(void* calllog);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallParams object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneCallParams(void* callParams);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallStats object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneCallStats(void* callStats, void* call);

			static void EchoCalibrationCallback(void *lc, int status, int delay_ms, void *data);

		private:
			static std::string wstos(std::wstring ws);

			static std::string pstos(Platform::String^ ps);
		};

		struct EchoCalibrationData {
			Windows::Phone::Media::Devices::AudioRoutingEndpoint endpoint;
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