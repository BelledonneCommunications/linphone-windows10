#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneCoreFactory;

		/// <summary>
		/// Object that represents a SIP address.
		/// It's an opaque object that represents a SIP address, i.e. the content of SIP's "from" and "to" headers.
		/// A SIP address is made of a display name, username, domain, port and various URI headers(such as tags).
		/// It looks like "Alice &lt;sip:alice@example.net&gt;". The LinphoneAddress has methods to extract and manipulate all parts of the address.
		/// When some part of the address (for example the username) is empty, the accessor method return null.
		/// </summary>
		/// <example>
		/// Can be instancied using
		/// <code>
		/// LinphoneAddress addr = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress("sip:alice@example.net");
		/// </code>
		/// or
		/// <code>
		/// LinphoneAddress addr = LinphoneManager.Instance.LinphoneCoreFactory.CreateLinphoneAddress("alice", "example.net", "Alice B.");
		/// </code>
		/// </example>
		public ref class LinphoneAddress sealed
		{
		public:
			/// <summary>
			/// Gets the human display name of the address.
			/// For example for the "Alice &lt;sip:alice@example.net&gt;" URI, it will return "Alice".
			/// </summary>
			/// <returns>The human display name or an empty string if not set</returns>
			Platform::String^ GetDisplayName();

			/// <summary>
			/// Gets the username part of the address.
			/// For example for the "Alice &lt;sip:alice@example.net&gt;" URI, it will return "alice".
			/// </summary>
			/// <returns>The username part of the address or an empty string if not set</returns>
			Platform::String^ GetUserName();

			/// <summary>
			/// Gets the domain part of the address.
			/// For example for the "Alice &lt;sip:alice@example.net&gt;" URI, it will return "example.net".
			/// </summary>
			/// <returns>The domain part of the address or an empty string if not set</returns>
			Platform::String^ GetDomain();

			/// <summary>
			/// Gets the port part of the address.
			/// </summary>
			/// <returns>The port part of the address or 0 if not set</returns>
			int GetPort();

			/// <summary>
			/// Gets the transport of the address.
			/// </summary>
			/// <returns>The transport if specified. If not set, it will return UDP.</returns>
			LinphoneTransport GetTransport();

			/// <summary>
			/// Sets the human display name of the address.
			/// </summary>
			/// <param name="name">The human display name to set to the address</param>
			void SetDisplayName(Platform::String^ name);

			/// <summary>
			/// Sets the username part of the address.
			/// </summary>
			/// <param name="username">The username to set to the address</param>
			void SetUserName(Platform::String^ username);

			/// <summary>
			/// Sets the domain part of the address.
			/// </summary>
			/// <param name="domain">The domain to set to the address</param>
			void SetDomain(Platform::String^ domain);

			/// <summary>
			/// Sets the port part of the address.
			/// </summary>
			/// <param name="port">The port to set to the address</param>
			void SetPort(int port);

			/// <summary>
			/// Sets the transport in the address.
			/// </summary>
			/// <param name="transport">The transport to set.</param>
			void SetTransport(LinphoneTransport transport);

			void Clean();

			/// <summary>
			/// Gets the string representation of the address.
			/// </summary>
			/// <returns>The address as a string</returns>
			Platform::String^ AsString();

			/// <summary>
			/// Gets the string representation of the URI part of the address (without the display name).
			/// </summary>
			/// <returns>The address without display name as a string</returns>
			Platform::String^ AsStringUriOnly();

			/// <summary>
			/// Same as AsString.
			/// </summary>
			/// <seealso cref="AsString()" />
			Platform::String^ ToString();

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCoreFactory;
			friend ref class Linphone::Core::LinphoneCore;
			
			LinphoneAddress(::LinphoneAddress *addr);
			LinphoneAddress(const char *uri);
			~LinphoneAddress();

			::LinphoneAddress *address;
		};
	}
}