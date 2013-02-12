#pragma once

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
			Platform::String^ GetUsername();

			void SetUsername(Platform::String^ username);

			Platform::String^ GetPassword();

			void SetPassword(Platform::String^ password);

			Platform::String^ GetRealm();

			void SetRealm(Platform::String^ realm);
		};
	}
}