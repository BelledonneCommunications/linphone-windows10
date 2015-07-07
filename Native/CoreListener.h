/*
CoreListener.h
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

#include "Enums.h"

namespace Linphone
{
	namespace Native
	{
		ref class Call;
		ref class CallStats;
		ref class ChatMessage;
		ref class ChatRoom;
		ref class ProxyConfig;

		/// <summary>
		/// Definition of the CoreListener interface.
		/// </summary>
		public interface class CoreListener
		{
		public:
			/// <summary>
			/// Callback method called when authentication information are requested.
			/// </summary>
			/// <param name="realm">The realm for which authentication information are requested</param>
			/// <param name="username">The username for which authentication information are requested</param>
			void AuthInfoRequested(Platform::String^ realm, Platform::String^ username, Platform::String^ domain);

			/// <summary>
			/// Callback method called when the encryption of a call has changed.
			/// </summary>
			/// <param name="call">The call for which the encryption has changed</param>
			/// <param name="encrypted">A boolean value telling whether the call is encrypted</param>
			/// <param name="authenticationToken">The authentication token for the call if it is encrypted</param>
			void CallEncryptionChanged(Call^ call, Platform::Boolean encrypted, Platform::String^ authenticationToken);

			/// <summary>
			/// Callback method called when the state of a call has changed.
			/// </summary>
			/// <param name="call">The call whose state has changed</param>
			/// <param name="state">The new state of the call</param>
			void CallStateChanged(Call^ call, CallState state, Platform::String^ message);

			/// <summary>
			/// Callback method called when the statistics of a call have been updated.
			/// </summary>
			/// <param name="call">The call for which the statistics have been updated</param>
			/// <param name="stats">The updated statistics for the call</param>
			void CallStatsUpdated(Call^ call, CallStats^ stats);

			/// <summary>
			/// Callback method called when a DTMF is received.
			/// </summary>
			/// <param name="call">The call on which a DTMF has been received</param>
			/// <param name="dtmf">The DTMF that has been received</param>
			void DtmfReceived(Call^ call, char16 dtmf);

			/// <summary>
			/// Callback method called when the application state has changed.
			/// </summary>
			/// <param name="state">The new state of the application</param>
			/// <param name="message">A message describing the new state of the application</param>
			void GlobalStateChanged(GlobalState state, Platform::String^ message);

			/// <summary>
			/// Callback method called when the composing status for this room has been updated.
			/// </summary>
			/// <param name="room">The room for which the composing status has been updated</param>
			void IsComposingReceived(ChatRoom^ room);

			/// <summary>
			/// Callback method called when the progress of the current logs upload has changed.
			/// </summary>
			/// <param name="progress"></param>
			void LogCollectionUploadProgressIndication(int offset, int total);

			/// <summary>
			/// Callback method called when the state of the current log upload changes.
			/// </summary>
			/// <param name="state">Tells the state of the upload</param>
			/// <param name="info">An error message if the upload went wrong, the url of the uploaded logs if it went well, null if upload not yet finished</param>
			void LogCollectionUploadStateChanged(LogCollectionUploadState state, Platform::String^ info);

			/// <summary>
			/// Callback method called when a chat message is received.
			/// </summary>
			/// <param name="room">The ChatRoom involved in the conversation</param>
			/// <param name="message">The incoming ChatMessage</param>
			void MessageReceived(ChatRoom^ room, ChatMessage^ message);

			/// <summary>
			/// Callback method called when the state of the registration of a proxy config has changed.
			/// </summary>
			/// <param name="config">The proxy config for which the registration state has changed</param>
			/// <param name="state">The new registration state for the proxy config</param>
			/// <param name="message">A message describing the new registration state</param>
			void RegistrationStateChanged(ProxyConfig^ config, RegistrationState state, Platform::String^ message);
		};
	}
}