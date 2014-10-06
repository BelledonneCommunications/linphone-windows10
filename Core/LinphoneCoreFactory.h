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
		ref class Transports;
		ref class VideoPolicy;
		ref class VideoSize;

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
			/// <param name="domain">The authentication domain</param>
			/// <returns>The LinphoneAuthInfo that has been created</returns>
			LinphoneAuthInfo^ CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain);

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
			/// <returns>The LinphoneAddress that has been created, or null if the URI couldn't be parsed</returns>
			LinphoneAddress^ CreateLinphoneAddress(Platform::String^ uri);

			/// <summary>
			/// Creates a default Transports object (using the UDP 5060 port).
			/// </summary>
			/// <returns>The Transports that has been created</returns>
			Transports^ CreateTransports();

			/// <summary>
			/// Creates a Transports object specifying the ports to use.
			/// </summary>
			/// <param name="udp_port">The UDP port to use (0 to disable)</param>
			/// <param name="tcp_port">The TCP port to use (0 to disable)</param>
			/// <param name="tls_port">The TLS port to use (0 to disable)</param>
			/// <returns>The Transports that has been created</returns>
			Transports^ CreateTransports(int udp_port, int tcp_port, int tls_port);

			/// <summary>
			/// Duplicates a Transports object.
			/// </summary>
			/// <param name="t">The Transports object to duplicate</param>
			/// <returns>The duplicated Transports</returns>
			Transports^ CreateTransports(Transports^ t);

			/// <summary>
			/// Creates a default VideoPolicy object (automatically initiate and accept video).
			/// </summary>
			/// <returns>The VideoPolicy that has been created</returns>
			VideoPolicy^ CreateVideoPolicy();

			/// <summary>
			/// Creates a VideoPolicy object specifying the behaviour for video calls.
			/// </summary>
			/// <param name="automaticallyInitiate">Whether video shall be automatically proposed for outgoing calls</param>
			/// <param name="automaticallyAccept">Whether video shall be automatically accepted for incoming calls</param>
			/// <returns>The VideoPolicy that has been created</returns>
			VideoPolicy^ CreateVideoPolicy(Platform::Boolean automaticallyInitiate, Platform::Boolean automaticallyAccept);

			/// <summary>
			/// Creates an unnamed VideoSize object.
			/// </summary>
			/// <param name="width">The video width</param>
			/// <param name="height">The video height</param>
			/// <returns>The VideoSize that has been created</returns>
			VideoSize^ CreateVideoSize(int width, int height);

			/// <summary>
			/// Creates a named VideoSize object.
			/// </summary>
			/// <param name="width">The video width</param>
			/// <param name="height">The video height</param>
			/// <param name="name">The video size name</param>
			/// <returns>The VideoSize that has been created</returns>
			VideoSize^ CreateVideoSize(int width, int height, Platform::String^ name);

			/// <summary>
			/// Sets the global log level of the application.
			/// </summary>
			/// <param name="logLevel">The log level to use</param>
			void SetLogLevel(OutputTraceLevel logLevel);

			/// <summary>
			/// Destroys the LinphoneCore attached to this factory
			/// </summary>
			void Destroy();

		private:
			friend ref class Linphone::Core::Globals;

			Linphone::Core::LinphoneCore^ linphoneCore;

			LinphoneCoreFactory();
			~LinphoneCoreFactory();
		};
	}
}