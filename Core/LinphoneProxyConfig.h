#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		/// <summary>
		/// This object represents a proxy configuration to be used by the LinphoneCore object. 
		/// Its fields mustn't be used directly in favour of the accessors methods.
		/// Once created and filled properly, the LinphoneProxyConfig can be given to LinphoneCore using LinphoneCore::AddProxyConfig.
		/// This will automatically triggers the registration if enabled.
		/// The proxy configuration are persistent to restarts because they are saved in the configuration file.
		/// As a consequence, after LinphoneCoreFactory::CreateLinphoneCore there might already be a default proxy that can be examined with LinphoneCore::GetDefaultProxyConfig.
		/// </summary>
		public ref class LinphoneProxyConfig sealed
		{
		public:
			/// <summary>
			/// Starts editing a proxy config.
			/// Because proxy config must be consistent, app MUST call Edit before doing any attempts to modify proxy config (such as identity, address and so on).
			/// Once modifications are done, then the app MUST call Done to commit the changes.
			/// </summary>
			void Edit();

			/// <summary>
			/// Commits changes made to the proxy config.
			/// </summary>
			void Done();

			/// <param name="identity">
			/// identity is normally formed with display name, username and domain, such as: Alice &lt;sip:alice@example.net&gt;.
			/// The REGISTER messages will have from and to set to this identity.
			/// </param>
			void SetIdentity(Platform::String^ identity);
			Platform::String^ GetIdentity();

			/// <summary>
			/// Sets the proxy address.
			/// <example>
			/// Examples of valid sip proxy are:
			/// <list type="bullet">
			/// <item>
			/// <term>sip:87.98.157.38</term>
			/// <description>IP address</description>
			/// </item>
			/// <item>
			/// <term>sip:87.98.157.38:5062</term>
			/// <description>IP address with port</description>
			/// </item>
			/// <item>
			/// <term>sip:sip.example.net</term>
			/// <description>Hostname</description>
			/// </item>
			/// </list>
			/// </example>
			/// </summary>
			void SetProxy(Platform::String^ proxyUri);

			/// <summary>
			/// Enables register for this proxy config.
			/// Register message is issued after call to Done.
			/// </summary>
			void EnableRegister(Platform::Boolean enable);
			Platform::Boolean IsRegisterEnabled();

			/// <summary>
			/// Normalizes a human readable phone number into a basic string.
			/// <example>
			/// 888-444-222 becomes 888444222
			/// </example>
			/// </summary>
			Platform::String^ NormalizePhoneNumber(Platform::String^ phoneNumber);

			/// <summary>
			/// Automatically add international prefix to e164 phone numbers
			/// </summary>
			void SetDialPrefix(Platform::String^ prefix);

			/// <summary>
			/// Sets whether Linphone should replace "+" by "00" in dialed numbers passed to LinphoneCore::Invite.
			/// </summary>
			void SetDialEscapePlus(Platform::Boolean value);
			Platform::String^ GetDomain();
			Platform::Boolean IsRegistered();

			/// <summary>
			/// Sets a SIP route.
			/// When a route is set, all outgoing calls will go the the route's destination if this proxy is the default one (see LinphoneCore::GetDefaultProxyConfig).
			/// </summary>
			void SetRoute(Platform::String^ routeUri);
			Platform::String^ GetRoute();

			/// <summary>
			/// Indicates either or not PUBLISH must be issued for this LinphoneProxyConfig.
			/// </summary>
			void EnablePublish(Platform::Boolean enable);
			Platform::Boolean IsPublishEnabled();
			RegistrationState GetState();

			/// <summary>
			/// Sets the registration expiration time in seconds.
			/// </summary>
			void SetExpires(int delay);
			void SetContactParameters(Platform::String^ params);

			/// <summary>
			/// Returns the international prefix for the given country.
			/// </summary>
			int LookupCCCFromIso(Platform::String^ iso);

			/// <summary>
			/// Returns the international prefix for the given country.
			/// </summary>
			int LookupCCCFromE164(Platform::String^ e164);

		private:
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneProxyConfig(::LinphoneCore *lc);
			~LinphoneProxyConfig();

			::LinphoneProxyConfig *proxy_config;
		};
	}
}