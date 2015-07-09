/*
AuthInfo.h
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

namespace Linphone
{
	namespace Native
	{
		/// <summary>
		/// Object holding authentication information.
		/// In most cases, authentication information consists in an username and a password.
		/// Sometimes, a userid is required by the proxy, and realm can be useful to discriminate different SIP domains.
		/// This object is instanciated using Core::CreateAuthInfo.
		/// Once created and filled, a AuthInfo must be added to the Core in order to become known and used automatically when needed.
		/// The Core object can take the initiative to request authentication information when needed in the application through the CoreListener::AuthInfoRequested listener.
		/// The application can respond to this information request later using Core::AddAuthInfo.
		/// This will unblock all pending authentication transactions and retry them with authentication headers.
		/// </summary>
		public ref class AuthInfo sealed
		{
		public:
			/// <summary>
			/// The authentication ha1.
			/// </summary>
			property Platform::String^ Ha1
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The authentication password.
			/// </summary>
			property Platform::String^ Passwd
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
			/// The authentication userid.
			/// </summary>
			property Platform::String^ Userid
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// The authentication username.
			/// </summary>
			property Platform::String^ Username
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

		private:
			friend class Linphone::Native::Utils;
			friend ref class Linphone::Native::Core;

			AuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain);
			AuthInfo(::LinphoneAuthInfo *auth_info);
			~AuthInfo();

			::LinphoneAuthInfo *auth_info;
		};
	}
}