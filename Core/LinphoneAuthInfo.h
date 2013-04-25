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
			/// Gets the authentication username.
			/// </summary>
			/// <returns>The authentication username</returns>
			Platform::String^ GetUsername();

			/// <summary>
			/// Sets the authentication username.
			/// </summary>
			/// <param name="username">The authentication username</param>
			void SetUsername(Platform::String^ username);

			/// <summary>
			/// Gets the authentication userid.
			/// </summary>
			/// <returns>The authentication userid</returns>
			Platform::String^ GetUserId();

			/// <summary>
			/// Sets the authentication userid.
			/// </summary>
			/// <param name="userid">The authentication userid</param>
			void SetUserId(Platform::String^ userid);

			/// <summary>
			/// Gets the authentication password.
			/// </summary>
			/// <returns>The authentication password</returns>
			Platform::String^ GetPassword();

			/// <summary>
			/// Sets the authentication password.
			/// </summary>
			/// <param name="password">The authentication password</param>
			void SetPassword(Platform::String^ password);

			/// <summary>
			/// Gets the authentication realm.
			/// </summary>
			/// <returns>The authentication realm</returns>
			Platform::String^ GetRealm();

			/// <summary>
			/// Sets the authentication realm.
			/// </summary>
			/// <param name="realm">The authentication realm</param>
			void SetRealm(Platform::String^ realm);

			/// <summary>
			/// Gets the authentication ha1.
			/// </summary>
			/// <returns>The authentication ha1</returns>
			Platform::String^ GetHa1();

			/// <summary>
			/// Sets the authentication ha1.
			/// </summary>
			/// <param name="ha1">The authentication ha1</param>
			void SetHa1(Platform::String^ ha1);

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm);
			LinphoneAuthInfo(::LinphoneAuthInfo *auth_info);
			~LinphoneAuthInfo();

			::LinphoneAuthInfo *auth_info;
		};
	}
}