#pragma once

#include <string>
#include <inspectable.h>

namespace Linphone
{
    namespace Core
    {
		/// <summary>
		/// Class to hold static methods for some interfaces between C code and C++/CX code.
		/// </summary>
		class Utils
		{
		public:
			/// <summary>
			/// Converts a Platform::String into a C char*.
			/// Required to send strings from C# to C.
			/// WARNING: The return C char* is dynamically allocated and must be freed by calling delete on it.
			/// </summary>
			/// <param name="ps">The Platform::String to convert into a C char*</param>
			/// <returns>The C char* string created from the given Platform::String</returns>
			static const char* pstoccs(Platform::String^ ps);

			/// <summary>
			/// Converts a C char* to a Platform::String.
			/// Required to send strings from C to C#.
			/// </summary>
			/// <param name="cc">The C char* string to convert into a Platform::String</param>
			/// <returns>The Platform::String created from the given C char* string</returns>
			static Platform::String^ Linphone::Core::Utils::cctops(const char* cc);

			/// <summary>
			/// Define a log handler.
			/// </summary>
			/// <param name="logfunc">The function pointer of the log handler.</param>
			static void LinphoneCoreSetLogHandler(void* logfunc);

			/// <summary>
			/// Define the log level.
			/// The loglevel parameter is a bitmask parameter. Therefore to enable only warning and error
			/// messages, use ORTP_WARNING | ORTP_ERROR. To disable logs, simply set loglevel to 0.
			/// </summary>
			/// <param name="loglevel">A bitmask of the log levels to set.</param>
			static void LinphoneCoreSetLogLevel(int loglevel);

			/// <summary>
			/// Creates a C++/CX LpConfig object using pointer to C structure.
			/// </summary>
			/// <param name="config">The ::LpConfig* to create the Linphone::Core::LpConfig from</param>
			/// <returns>The created Linphone::Core::LpConfig as a Platform::Object</returns>
			static Platform::Object^ CreateLpConfig(void* config);

			/// <summary>
			/// Creates a C++/CX LpConfig object using the path to linphonerc files.
			/// </summary>
			/// <param name="configPath">The path to the user configuration file that must be readable and writable</param>
			/// <param name="factoryConfigPath">The path to the factory configuration file that only needs to be readable</param>
			/// <returns>The created Linphone::Core::LpConfig as a Platform::Object</returns>
			static Platform::Object^ CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath);
			
			/// <summary>
			/// Creates a C++/CX PayloadType object using pointer to C structure.
			/// </summary>
			/// <param name="pt">The ::PayloadType* to create the Linphone::Core::PayloadType from</param>
			/// <returns>The created Linphone::Core::PayloadType as a Platform::Object</returns>
			static Platform::Object^ CreatePayloadType(void* pt);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCall object using pointer to C structure.
			/// </summary>
			/// <param name="call">The ::LinphoneCall* to create the Linphone::Core::LinphoneCall from</param>
			/// <returns>The created Linphone::Core::LinphoneCall as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCall(void* call);
			
			/// <summary>
			/// Creates a C++/CX LinphoneAddress object using pointer to C structure.
			/// </summary>
			/// <param name="addr">The ::LinphoneAddress* to create the Linphone::Core::LinphoneAddress from</param>
			/// <returns>The created Linphone::Core::LinphoneAddress as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneAddress(void* addr);
			
			/// <summary>
			/// Creates a C++/CX LinphoneAddress object using an URI.
			/// </summary>
			/// <param name="uri">The URI from which to create a Linphone::Core::LinphoneAddress</param>
			/// <returns>The created Linphone::Core::LinphoneAddress as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneAddressFromUri(const char* uri);
			
			/// <summary>
			/// Creates a C++/CX LinphoneAuthInfo object using pointer to C structure.
			/// </summary>
			/// <param name="auth_info">The ::LinphoneAuthInfo* to create the Linphone::Core::LinphoneAuthInfo from</param>
			/// <returns>The created Linphone::Core::LinphoneAuthInfo as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneAuthInfo(void* auth_info);

			/// <summary>
			/// Creates a C++/CX LinphoneAuthInfo object.
			/// </summary>
			/// <param name="username">The authentication username</param>
			/// <param name="userid">The authentication userid</param>
			/// <param name="password">The authentication password</param>
			/// <param name="ha1">The authentication ha1</param>
			/// <param name="realm">The authentication realm</param>
			/// <returns>The created Linphone::Core::LinphoneAuthInfo as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm);
			
			/// <summary>
			/// Creates a C++/CX LinphoneProxyConfig object using pointer to C structure.
			/// </summary>
			/// <param name="proxy_config">The ::LinphoneProxyConfig* to create the Linphone::Core::LinphoneProxyConfig from</param>
			/// <returns>The created Linphone::Core::LinphoneProxyConfig as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneProxyConfig(void* proxy_config);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallLog object using pointer to C structure.
			/// </summary>
			/// <param name="calllog">The ::LinphoneCallLog* to create the Linphone::Core::LinphoneCallLog from</param>
			/// <returns>The created Linphone::Core::LinphoneCallLog as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCallLog(void* calllog);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallParams object using pointer to C structure.
			/// </summary>
			/// <param name="callParams">The ::LinphoneCallParams* to create the Linphone::Core::LinphoneCallParams from</param>
			/// <returns>The created Linphone::Core::LinphoneCallParams as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCallParams(void* callParams);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallStats object using pointer to call C structure and media type.
			/// </summary>
			/// <param name="call">The ::LinphoneCall* from which to get the call statistics</param>
			/// <param name="mediaType">The media type for which we want the statistics</param>
			/// <returns>The created Linphone::Core::LinphoneCallStats as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCallStats(void* call, int mediaType);

			/// <summary>
			/// Creates a C++/CX LinphoneCallStats object using pointer to C structure.
			/// </summary>
			/// <param name="callstats">The ::LinphoneCallStats* to create the Linphone::Core::LinphoneCallStats from</param>
			/// <returns>The created Linphone::Core::LinphoneCallStats as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCallStats(void* callstats);

			/// <summary>
			/// A callback called when the echo canceller calibration finishes.
			/// </summary>
			/// <param name="lc">The linphone core pointer</param>
			/// <param name="status">The status of the echo canceller calibration</param>
			/// <param name="delay_ms">The echo delay if the status is "done", 0 otherwise</param>
			/// <param name="data">Some user data given when starting the echo canceller calibration process</param>
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