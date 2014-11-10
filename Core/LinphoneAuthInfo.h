#pragma once

#include "LinphoneCore.h"
#include "ApiLock.h"

namespace Linphone
{
	namespace Core
	{
		/// <summary>
		/// Object holding authentication information.
		/// In most cases, authentication information consists in an username and a password.
		/// Sometimes, a userid is required by the proxy, and realm can be useful to discriminate different SIP domains.
		/// This object is instanciated using LinphoneCoreFactory::CreateAuthInfo.
		/// Once created and filled, a LinphoneAuthInfo must be added to the LinphoneCore in order to become known and used automatically when needed.
		/// The LinphoneCore object can take the initiative to request authentication information when needed in the application through the LinphoneCoreListener::AuthInfoRequested listener.
		/// The application can respond to this information request later using LinphoneCore::AddAuthInfo.
		/// This will unblock all pending authentication transactions and retry them with authentication headers.
		/// </summary>
		public ref class LinphoneAuthInfo sealed
		{
		public:
			/// <summary>
			/// The authentication username.
			/// </summary>
			property Platform::String^ Username
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The authentication userid.
			/// </summary>
			property Platform::String^ UserId
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The authentication password.
			/// </summary>
			property Platform::String^ Password
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The authentication realm.
			/// </summary>
			property Platform::String^ Realm
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The authentication ha1.
			/// </summary>
			property Platform::String^ Ha1
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain);
			LinphoneAuthInfo(::LinphoneAuthInfo *auth_info);
			~LinphoneAuthInfo();

			::LinphoneAuthInfo *auth_info;
		};
	}
}