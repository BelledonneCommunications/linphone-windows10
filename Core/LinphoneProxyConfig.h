#pragma once

#include "LinphoneCore.h"
#include "ApiLock.h"

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

			/// <summary>
			/// Sets the identity for this proxy config.
			/// </summary>
			/// <param name="displayname">The display name of the identity address</param>
			/// <param name="username">The username part of the identity address</param>
			/// <param name="domain">The domain part of the identity address</param>
			void SetIdentity(Platform::String^ displayname, Platform::String^ username, Platform::String^ domain);

			/// <summary>
			/// Gets the identity associated with this proxy config.
			/// </summary>
			/// <returns>The identity address associated with the proxy config as a string</returns>
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
			/// <param name="proxyUri">The URI to use as proxy address</param>
			void SetProxy(Platform::String^ proxyUri);

			/// <summary>
			/// Get the reason why registration failed when the proxy config state is LinphoneRegistrationFailed.
			/// <returns>The reason why registration failed for this proxy config.</returns>
			/// </summary>
			property Reason Error
			{
				Reason get();
			}

			/// <summary>
			/// Enables register for this proxy config.
			/// Register message is issued after call to Done.
			/// </summary>
			/// <param name="enable">A boolean value telling whether register is enabled for the proxy config</param>
			void EnableRegister(Platform::Boolean enable);

			/// <summary>
			/// Returns true if the register is enabled for this proxy config, else returns false.
			/// </summary>
			/// <returns>A boolean value telling whether register is enabled for the proxy config</returns>
			Platform::Boolean IsRegisterEnabled();

			/// <summary>
			/// Normalizes a human readable phone number into a basic string.
			/// <example>
			/// 888-444-222 becomes 888444222
			/// </example>
			/// </summary>
			/// <param name="phoneNumber">The phone number to be normalized</param>
			/// <returns>The normalized phone number</returns>
			Platform::String^ NormalizePhoneNumber(Platform::String^ phoneNumber);

			/// <summary>
			/// Automatically add international prefix to e164 phone numbers
			/// </summary>
			/// <param name="prefix">The dial prefix to be automatically added to phone numbers</param>
			void SetDialPrefix(Platform::String^ prefix);

			/// <summary>
			/// Sets whether Linphone should replace "+" by "00" in dialed numbers passed to LinphoneCore::Invite.
			/// </summary>
			/// <param name="value">A boolean value telling whether to replace "+" by "00" in dialed numbers</param>
			void SetDialEscapePlus(Platform::Boolean value);

			/// <summary>
			/// Gets the address.
			/// </summary>
			/// <returns>The address of the proxy config as a string</returns>
			Platform::String^ GetAddr();

			/// <summary>
			/// Gets the domain of the address.
			/// </summary>
			/// <returns>The domain part of the address of the proxy config</returns>
			Platform::String^ GetDomain();

			/// <summary>
			/// Returns true if this proxy config is currently registered, else returns false.
			/// </summary>
			/// <returns>A boolean value telling whether the proxy config is registered</returns>
			Platform::Boolean IsRegistered();

			/// <summary>
			/// Sets a SIP route.
			/// When a route is set, all outgoing calls will go the the route's destination if this proxy is the default one (see LinphoneCore::GetDefaultProxyConfig).
			/// </summary>
			/// <param name="routeUri">The SIP route to set</param>
			void SetRoute(Platform::String^ routeUri);

			/// <summary>
			/// Returns the SIP route is any.
			/// </summary>
			/// <returns>The SIP route</returns>
			Platform::String^ GetRoute();

			/// <summary>
			/// Indicates either or not PUBLISH must be issued for this LinphoneProxyConfig.
			/// </summary>
			/// <param name="enable">A boolean value telling whether publish is enabled for the proxy config</param>
			void EnablePublish(Platform::Boolean enable);

			/// <summary>
			/// Returns true if PUBLISH must be issued, else returns false.
			/// </summary>
			/// <returns>A boolean value telling whether publish is enabled for the proxy config</returns>
			Platform::Boolean IsPublishEnabled();

			/// <summary>
			/// Returns the current RegistrationState for this proxy config.
			/// </summary>
			/// <returns>The current registration state of the proxy config</returns>
			RegistrationState GetState();

			/// <summary>
			/// Sets the registration expiration time in seconds.
			/// </summary>
			/// <param name="delay">The registration expiration time in seconds</param>
			void SetExpires(int delay);

			/// <summary>
			/// Sets the contact params to be sent along with the REGISTERs.
			/// </summary>
			/// <param name="params">The contact parameters to be sent along with the REGISTERs</param>
			void SetContactParameters(Platform::String^ params);

			/// <summary>
			/// Returns the international prefix for the given country.
			/// </summary>
			/// <param name="iso">The country as ISO 3166-1 alpha-2 code</param>
			/// <returns>The international prefix or -1 if not found</returns>
			int LookupCCCFromIso(Platform::String^ iso);

			/// <summary>
			/// Returns the international prefix for the given e164 number.
			/// </summary>
			/// <param name="e164">The e164 number</param>
			/// <returns>The international prefix or -1 if not found</returns>
			int LookupCCCFromE164(Platform::String^ e164);

			/// <summary>
			/// Set optional contact parameters that will be added to the contact information sent in the registration, inside the URI.
			/// The main use case for this function is provide the proxy additional information regarding the user agent, like for example unique identifier or apple push id.
			/// As an example, the contact address in the SIP register sent will look like <sip:joe@15.128.128.93:50421;apple-push-id=43143-DFE23F-2323-FA2232>.
			/// </summary>
			/// <param name="params">a string contaning the additional parameters in text form, like "myparam=something;myparam2=something_else"</param>
			void SetContactUriParameters(Platform::String^ params);

		private:
			friend ref class Linphone::Core::LinphoneCore;
			friend class Linphone::Core::Utils;

			LinphoneProxyConfig();
			LinphoneProxyConfig(::LinphoneProxyConfig* proxy_config);
			~LinphoneProxyConfig();

			::LinphoneProxyConfig *proxy_config;
		};
	}
}