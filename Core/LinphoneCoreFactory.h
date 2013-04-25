#pragma once

#include "ApiLock.h"
#include "LinphoneCoreListener.h"

namespace Linphone
{
	namespace Core
	{
		ref class Globals;
		ref class LinphoneCore;
		ref class LinphoneAuthInfo;
		ref class LinphoneAddress;
		ref class LpConfig;

		/// <summary>
		/// Definition of the OutputTraceListener interface. The classes that are to be used to ouput trace logs must implement this interface.
		/// </summary>
		public interface class OutputTraceListener
		{
		public:
			/// <summary>
			/// The method to implement to be able to output trace logs.
			/// </summary>
			/// <param name="level">The log level of the trace to output</param>
			/// <param name="msg">The trace message to ouput</param>
			void OutputTrace(OutputTraceLevel level, Platform::String^ msg);
		};

		/// <summary>
		/// Class whose role is to create instances of other classes : LinphoneCore, LpConfig, AuthInfo and LinphoneAddress.
		/// </summary>
		public ref class LinphoneCoreFactory sealed
		{
		public:
			/// <summary>
			/// Gets the LinphoneCore instance.
			/// </summary>
			property LinphoneCore^ LinphoneCore
            {
                Linphone::Core::LinphoneCore^ get();
            }

			/// <summary>
			/// The listener class to output trace logs.
			/// </summary>
			property OutputTraceListener^ OutputTraceListener
			{
				Linphone::Core::OutputTraceListener^ get();
				void set(Linphone::Core::OutputTraceListener^ listener);
			}

			/// <summary>
			/// Creates a LinphoneCore instance.
			/// To access the created LinphoneCore use the LinphoneCoreFactory::LinphoneCore property.
			/// </summary>
			/// <param name="listener">The listener class that will handle the callbacks from the native linphone core</param>
			/// <seealso cref="CreateLinphoneCore(LinphoneCoreListener^, LpConfig^)"/>
			void CreateLinphoneCore(LinphoneCoreListener^ listener);

			/// <summary>
			/// Creates a LinphoneCore instance given an existing LpConfig.
			/// To access the created LinphoneCore use the LinphoneCoreFactory::LinphoneCore property.
			/// </summary>
			/// <param name="listener">The listener class that will handle the callbacks from the native linphone core</param>
			/// <param name="config">The LpConfig to use for the configuration of the created LinphoneCore</param>
			/// <seealso cref="CreateLinphoneCore(LinphoneCoreListener^)"/>
			void CreateLinphoneCore(LinphoneCoreListener^ listener, LpConfig^ config);

			/// <summary>
			/// Creates a LpConfig.
			/// </summary>
			/// <param name="configPath">The path of the user configuration file that must be readable and writable</param>
			/// <param name="factoryConfigPath">The path of the factory configuration file that needs only to be readable</param>
			/// <returns>The LpConfig that has been created</returns>
			LpConfig^ CreateLpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath);

			/// <summary>
			/// Creates a LinphoneAuthInfo.
			/// </summary>
			/// <param name="username">The authentication username</param>
			/// <param name="userid">The authentication userid</param>
			/// <param name="password">The authentication password</param>
			/// <param name="ha1">The authentication ha1</param>
			/// <param name="realm">The authentication realm</param>
			/// <returns>The LinphoneAuthInfo that has been created</returns>
			LinphoneAuthInfo^ CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm);

			/// <summary>
			/// Creates a LinphoneAddress.
			/// </summary>
			/// <param name="username">The username part of the address</param>
			/// <param name="domain">The domain part of the address</param>
			/// <param name="displayName">The display name of the address</param>
			/// <returns>The LinphoneAddress that has been created</returns>
			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ username, Platform::String^ domain, Platform::String^ displayName);

			/// <summary>
			/// Constructs a LinphoneAddress object by parsing the user supplied address, given as a string.
			/// </summary>
			/// <param name="uri">address, should be like "sip:joe@sip.linphone.org"</param>
			/// <returns>The LinphoneAddress that has been created</returns>
			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ uri);

			/// <summary>
			/// Sets the global log level of the application.
			/// </summary>
			/// <param name="logLevel">The log level to use</param>
			void SetLogLevel(OutputTraceLevel logLevel);

		private:
			friend ref class Linphone::Core::Globals;

			Linphone::Core::LinphoneCore^ linphoneCore;
			Linphone::Core::OutputTraceListener^ outputTraceListener;

			LinphoneCoreFactory();
			~LinphoneCoreFactory();
		};
	}
}