/*
Utils.h
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

#pragma once

#include <string>
#include "Enums.h"
#include <inspectable.h>

namespace Linphone
{
    namespace Native
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
			static Platform::String^ cctops(const char* cc);



			/// <summary>
			/// Creates a C++/CX Address object using an URI.
			/// </summary>
			/// <param name="uri">The URI from which to create a Linphone::Native::Address</param>
			/// <returns>The created Linphone::Native::Address as a Platform::Object</returns>
			static Platform::Object^ CreateAddress(const char *address);

			/// <summary>
			/// Creates a C++/CX Address object using pointer to C structure.
			/// </summary>
			/// <param name="addr">The ::LinphoneAddress* to create the Linphone::Native::Address from</param>
			/// <returns>The created Linphone::Native::Address as a Platform::Object</returns>
			static Platform::Object^ CreateAddress(void *addr);

			/// <summary>
			/// Creates a C++/CX AuthInfo object using pointer to C structure.
			/// </summary>
			/// <param name="auth_info">The ::LinphoneAuthInfo* to create the Linphone::Native::AuthInfo from</param>
			/// <returns>The created Linphone::Native::AuthInfo as a Platform::Object</returns>
			static Platform::Object^ CreateAuthInfo(void* auth_info);

			/// <summary>
			/// Creates a C++/CX LpConfig object using pointer to C structure.
			/// </summary>
			/// <param name="config">The ::LpConfig* to create the Linphone::Native::LpConfig from</param>
			/// <returns>The created Linphone::Native::LpConfig as a Platform::Object</returns>
			static Platform::Object^ CreateLpConfig(void* config);

			/// <summary>
			/// Creates a C++/CX LpConfig object using the path to linphonerc files.
			/// </summary>
			/// <param name="configPath">The path to the user configuration file that must be readable and writable</param>
			/// <param name="factoryConfigPath">The path to the factory configuration file that only needs to be readable</param>
			/// <returns>The created Linphone::Native::LpConfig as a Platform::Object</returns>
			static Platform::Object^ CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath);

			/// <summary>
			/// Creates a C++/CX PayloadType object using pointer to C structure.
			/// </summary>
			/// <param name="pt">The ::PayloadType* to create the Linphone::Native::PayloadType from</param>
			/// <returns>The created Linphone::Native::PayloadType as a Platform::Object</returns>
			static Platform::Object^ CreatePayloadType(void* pt);

			/// <summary>
			/// Creates a C++/CX ProxyConfig object using pointer to C structure.
			/// </summary>
			/// <param name="proxy_config">The ::LinphoneProxyConfig* to create the Linphone::Native::ProxyConfig from</param>
			/// <returns>The created Linphone::Native::ProxyConfig as a Platform::Object</returns>
			static Platform::Object^ CreateProxyConfig(void* proxy_config);

			/// <summary>
			/// Define the log level.
			/// The loglevel parameter is a bitmask parameter. Therefore to enable only warning and error
			/// messages, use ORTP_WARNING | ORTP_ERROR. To disable logs, simply set loglevel to 0.
			/// </summary>
			/// <param name="loglevel">A bitmask of the log levels to set.</param>
			static void SetLogLevel(int loglevel);




			/// <summary>
			/// Creates a C++/CX LinphoneCall object using pointer to C structure.
			/// </summary>
			/// <param name="call">The ::LinphoneCall* to create the Linphone::Native::LinphoneCall from</param>
			/// <returns>The created Linphone::Native::LinphoneCall as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCall(void* call);

			/// <summary>
			/// Creates a C++/CX CallLog object using pointer to C structure.
			/// </summary>
			/// <param name="calllog">The ::LinphoneCallLog* to create the Linphone::Native::CallLog from</param>
			/// <returns>The created Linphone::Native::CallLog as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCallLog(void* calllog);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallParams object using pointer to C structure.
			/// </summary>
			/// <param name="callParams">The ::LinphoneCallParams* to create the Linphone::Native::LinphoneCallParams from</param>
			/// <returns>The created Linphone::Native::LinphoneCallParams as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCallParams(void* callParams);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallStats object using pointer to call C structure and media type.
			/// </summary>
			/// <param name="call">The ::LinphoneCall* from which to get the call statistics</param>
			/// <param name="mediaType">The media type for which we want the statistics</param>
			/// <returns>The created Linphone::Native::LinphoneCallStats as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCallStats(void* call, int mediaType);
			
			/// <summary>
			/// Creates a C++/CX LinphoneCallStats object using pointer to C structure.
			/// </summary>
			/// <param name="callstats">The ::LinphoneCallStats* to create the Linphone::Native::LinphoneCallStats from</param>
			/// <returns>The created Linphone::Native::LinphoneCallStats as a Platform::Object</returns>
			static Platform::Object^ CreateLinphoneCallStats(void* callStats);

			/// <summary>
			/// A callback called when the echo canceller calibration finishes.
			/// </summary>
			/// <param name="lc">The linphone core pointer</param>
			/// <param name="status">The status of the echo canceller calibration</param>
			/// <param name="delay_ms">The echo delay if the status is "done", 0 otherwise</param>
			/// <param name="data">Some user data given when starting the echo canceller calibration process</param>
			static void EchoCalibrationCallback(void *lc, int status, int delay_ms, void *data);
			
			/// <summary>
			/// Creates a C++/CX LinphoneChatMessage object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneChatMessage(void* message);
			
			/// <summary>
			/// Creates a C++/CX LinphoneChatRoom object using pointer to C structure.
			/// </summary>
			static Platform::Object^ CreateLinphoneChatRoom(void* room);

		private:
			static std::wstring UTF8ToUTF16(const char *utf8);
			static std::string UTF16ToUTF8(const wchar_t *utf16);
		};

#if 0
		struct EchoCalibrationData {
			Windows::Phone::Media::Devices::AudioRoutingEndpoint endpoint;
			Windows::Phone::Networking::Voip::VoipPhoneCall^ call;
		};
#endif

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