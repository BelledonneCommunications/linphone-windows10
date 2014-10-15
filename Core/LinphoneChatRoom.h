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
			/// Tells whether the remote is currently composing a message.
			/// </summary>
			Platform::Boolean IsRemoteComposing();

			/// <summary>
			/// Notify the destination of the chat message being composed that the user is typing a new message.
			/// </summary>
			void Compose();
			
			/// <summary>
			/// Creates a LinphoneChatMessage from a String.
			/// </summary>
			Linphone::Core::LinphoneChatMessage^ CreateLinphoneChatMessage(Platform::String^ message);

			/// <summary>
			/// Returns the amount of messages associated with the peer of this chatRoom.
			/// </summary>
			int GetHistorySize();

			/// <summary>
			/// Deletes all the messages associated with the peer of this chat room
			/// </summary>
			void DeleteHistory();

			/// <summary>
			/// Returns the amount of unread messages associated with the peer of this chatRoom.
			/// </summary>
			int GetUnreadMessageCount();

			/// <summary>
			/// Marks all the messages in this conversation as read
			/// </summary>
			void MarkAsRead();

			/// <summary>
			/// Gets the list of the messages in the history of this chatroom
			/// </summary>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetHistory();

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneChatRoom(::LinphoneChatRoom *cr);
			~LinphoneChatRoom();

			::LinphoneChatRoom *room;
		};
	}
}