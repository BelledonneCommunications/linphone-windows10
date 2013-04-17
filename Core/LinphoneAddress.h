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
			/// Human display name
			/// </summary>
			/// <returns>
			/// an empty string if not set
			/// </returns>
			Platform::String^ GetDisplayName();

			/// <returns>
			/// an empty string if not set
			/// </returns>
			Platform::String^ GetUserName();

			/// <returns>
			/// an empty string if not set
			/// </returns>
			Platform::String^ GetDomain();

			int GetPort();

			void SetDisplayName(Platform::String^ name);
			void SetUserName(Platform::String^ username);
			void SetDomain(Platform::String^ domain);
			void SetPort(int port);

			void Clean();

			/// <returns>
			/// the address as a string
			/// </returns>
			Platform::String^ AsString();

			/// <returns>
			/// the address without display name as a string
			/// </returns>
			Platform::String^ AsStringUriOnly();

			/// <summary>
			/// Same as AsString.
			/// </summary>
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