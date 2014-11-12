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
			property Linphone::Core::LinphoneAddress^ PeerAddress
			{
				Linphone::Core::LinphoneAddress^ get();
			}
			
			/// <summary>
			/// Sends a LinphoneChatMessage using the current ChatRoom, and sets the listener to be called when the massge state changes.
			/// </summary>
			void SendMessage(Linphone::Core::LinphoneChatMessage^ message, Linphone::Core::LinphoneChatMessageListener^ listener);

			/// <summary>
			/// Tells whether the remote is currently composing a message.
			/// </summary>
			property Platform::Boolean IsRemoteComposing
			{
				Platform::Boolean get();
			}

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
			property int HistorySize
			{
				int get();
			}

			/// <summary>
			/// Deletes all the messages associated with the peer of this chat room
			/// </summary>
			void DeleteHistory();

			/// <summary>
			/// Returns the amount of unread messages associated with the peer of this chatRoom.
			/// </summary>
			property int UnreadMessageCount
			{
				int get();
			}

			/// <summary>
			/// Marks all the messages in this conversation as read
			/// </summary>
			void MarkAsRead();

			/// <summary>
			/// Gets the list of the messages in the history of this chatroom
			/// </summary>
			property Windows::Foundation::Collections::IVector<Platform::Object^>^ History
			{
				Windows::Foundation::Collections::IVector<Platform::Object^>^ get();
			}

			/// <summary>
			/// Deletes a message from the history of the chatroom
			/// </summary>
			void DeleteMessageFromHistory(Linphone::Core::LinphoneChatMessage^ message);

			/// <summary>
			/// Creates a LinphoneChatMessage to transfer a file.
			/// </summary>
			/// <param name="type">MIME type of the file to transfer</param>
			/// <param name="subtype">MIME subtype of the file to transfer</param>
			/// <param name="name">Name of the file to transfer</param>
			/// <param name="size">Size in bytes of the file to transfer</param>
			/// <param name="filepath">Path to the file to transfer</param>
			/// <returns>A new LinphoneChatMessage</returns>
			Linphone::Core::LinphoneChatMessage^ CreateFileTransferMessage(Platform::String^ type, Platform::String^ subtype, Platform::String^ name, int size, Platform::String^ filepath);

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneChatRoom(::LinphoneChatRoom *cr);
			~LinphoneChatRoom();

			::LinphoneChatRoom *room;
		};
	}
}