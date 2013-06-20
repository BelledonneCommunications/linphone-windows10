#pragma once

#include "LinphoneCore.h"
#include "LinphoneChatMessage.h"
#include "LinphoneChatMessageListener.h"
#include "Enums.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneCore;
		ref class LinphoneAddress;

		/// <summary>
		/// Represents a chat message shared between two users.
		/// </summary>
		public ref class LinphoneChatRoom sealed
		{
		public:
			/// <summary>
			/// Returns the LinphoneAddress associated with this ChatRoom.
			/// </summary>
			Linphone::Core::LinphoneAddress^ GetPeerAddress();
			
			/// <summary>
			/// Sends a LinphoneChatMessage using the current ChatRoom, and sets the listener to be called when the massge state changes.
			/// </summary>
			void SendMessage(Linphone::Core::LinphoneChatMessage^ message, Linphone::Core::LinphoneChatMessageListener^ listener);
			
			/// <summary>
			/// Creates a LinphoneChatMessage from a String.
			/// </summary>
			Linphone::Core::LinphoneChatMessage^ CreateLinphoneChatMessage(Platform::String^ message);

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneChatRoom(::LinphoneChatRoom *cr);
			~LinphoneChatRoom();

			::LinphoneChatRoom *room;
		};
	}
}