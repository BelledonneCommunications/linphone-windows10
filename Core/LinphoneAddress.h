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
			/// The human display name of the address.
			/// For example for the "Alice &lt;sip:alice@example.net&gt;" URI, it will return "Alice".
			/// </summary>
			property Platform::String^ DisplayName
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The username part of the address.
			/// For example for the "Alice &lt;sip:alice@example.net&gt;" URI, it will return "alice".
			/// </summary>
			property Platform::String^ UserName
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The domain part of the address.
			/// For example for the "Alice &lt;sip:alice@example.net&gt;" URI, it will return "example.net".
			/// </summary>
			property Platform::String^ Domain
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The port part of the address.
			/// </summary>
			property int Port
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// The transport of the address.
			/// </summary>
			property LinphoneTransport Transport
			{
				LinphoneTransport get();
				void set(LinphoneTransport value);
			}

			/// <summary>
			/// The address scheme, normally "sip".
			/// </summary>
			property Platform::String^ Scheme
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Removes address's tags and uri headers so that it is displayable to the user.
			/// </summary>
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