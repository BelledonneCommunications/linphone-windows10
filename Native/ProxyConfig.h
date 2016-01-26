/*
ProxyConfig.h
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

#include "ApiLock.h"
#include "Core.h"


namespace BelledonneCommunications
{
	namespace Linphone
	{
		namespace Native
		{
			/// <summary>
			/// This object represents a proxy configuration to be used by the Core object. 
			/// Its fields mustn't be used directly in favour of the accessors methods.
			/// Once created and filled properly, the ProxyConfig can be given to Core using Core::AddProxyConfig.
			/// This will automatically triggers the registration if enabled.
			/// The proxy configuration are persistent to restarts because they are saved in the configuration file.
			/// As a consequence, after creating a Core there might already be a default proxy that can be examined with Core::GetDefaultProxyConfig.
			/// </summary>
			public ref class ProxyConfig sealed
			{
			public:
				/// <summary>
				/// Sets the contact params to be sent along with the REGISTERs.
				/// </summary>
				property Platform::String^ ContactParameters
				{
					Platform::String^ get();
					void set(Platform::String^ value);
				}

				/// <summary>
				/// Set optional contact parameters that will be added to the contact information sent in the registration, inside the URI.
				/// The main use case for this function is provide the proxy additional information regarding the user agent, like for example unique identifier or apple push id.
				/// As an example, the contact address in the SIP register sent will look like &lt;sip:joe@15.128.128.93:50421;apple-push-id=43143-DFE23F-2323-FA2232&gt;.
				/// </summary>
				property Platform::String^ ContactUriParameters
				{
					Platform::String^ get();
					void set(Platform::String^ value);
				}

				/// <summary>
				/// Sets whether Linphone should replace "+" by "00" in dialed numbers passed to Core::Invite.
				/// </summary>
				property Platform::Boolean DialEscapePlus
				{
					Platform::Boolean get();
					void set(Platform::Boolean value);
				}

				/// <summary>
				/// Automatically add international prefix to e164 phone numbers
				/// </summary>
				property Platform::String^ DialPrefix
				{
					Platform::String^ get();
					void set(Platform::String^ value);
				}

				/// <summary>
				/// Gets the domain of the address.
				/// </summary>
				property Platform::String^ Domain
				{
					Platform::String^ get();
				}

				/// <summary>
				/// Get the reason why registration failed when the proxy config state is LinphoneRegistrationFailed.
				/// <returns>The reason why registration failed for this proxy config.</returns>
				/// </summary>
				property Reason Error
				{
					Reason get();
				}

				/// <summary>
				/// Sets the registration expiration time in seconds.
				/// </summary>
				property int Expires
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Sets the identity for this proxy config.
				/// </summary>
				property Platform::String^ Identity
				{
					Platform::String^ get();
					void set(Platform::String^ value);
				}

				/// <summary>
				/// Indicates whether AVPF/SAVPF is being used for calls using this proxy config.
				/// </summary>
				property Platform::Boolean IsAvpfEnabled
				{
					Platform::Boolean get();
					void set(Platform::Boolean value);
				}

				/// <summary>
				/// Indicates either or not PUBLISH must be issued for this ProxyConfig.
				/// </summary>
				property Platform::Boolean IsPublishEnabled
				{
					Platform::Boolean get();
					void set(Platform::Boolean value);
				}

				/// <summary>
				/// Returns true if this proxy config is currently registered, else returns false.
				/// </summary>
				property Platform::Boolean IsRegistered
				{
					Platform::Boolean get();
				}

				/// <summary>
				/// Enables register for this proxy config.
				/// Register message is issued after call to Done.
				/// </summary>
				property Platform::Boolean IsRegisterEnabled
				{
					Platform::Boolean get();
					void set(Platform::Boolean value);
				}

				/// <summary>
				/// Sets a SIP route.
				/// When a route is set, all outgoing calls will go the the route's destination if this proxy is the default one (see Core::GetDefaultProxyConfig).
				/// </summary>
				property Platform::String^ Route
				{
					Platform::String^ get();
					void set(Platform::String^ value);
				}

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
				property Platform::String^ ServerAddr
				{
					Platform::String^ get();
					void set(Platform::String^ value);
				}

				/// <summary>
				/// Returns the current RegistrationState for this proxy config.
				/// </summary>
				property RegistrationState State
				{
					RegistrationState get();
				}

				/// <summary>
				/// Commits changes made to the proxy config.
				/// </summary>
				void Done();

				/// <summary>
				/// Starts editing a proxy config.
				/// Because proxy config must be consistent, app MUST call Edit before doing any attempts to modify proxy config (such as identity, address and so on).
				/// Once modifications are done, then the app MUST call Done to commit the changes.
				/// </summary>
				void Edit();

				/// <summary>
				/// Returns the international prefix for the given e164 number.
				/// </summary>
				/// <param name="e164">The e164 number</param>
				/// <returns>The international prefix or -1 if not found</returns>
				int LookupCccFromE164(Platform::String^ e164);

				/// <summary>
				/// Returns the international prefix for the given country.
				/// </summary>
				/// <param name="iso">The country as ISO 3166-1 alpha-2 code</param>
				/// <returns>The international prefix or -1 if not found</returns>
				int LookupCccFromIso(Platform::String^ iso);

				/// <summary>
				/// Normalizes a human readable phone number into a basic string.
				/// <example>
				/// 888-444-222 becomes 888444222
				/// </example>
				/// </summary>
				/// <param name="phoneNumber">The phone number to be normalized</param>
				/// <returns>The normalized phone number</returns>
				Platform::String^ NormalizeNumber(Platform::String^ phoneNumber);

			private:
				friend ref class Core;
				friend class Utils;

				ProxyConfig(::LinphoneProxyConfig* proxyConfig);
				~ProxyConfig();

				::LinphoneProxyConfig *proxyConfig;
			};
		}
	}
}