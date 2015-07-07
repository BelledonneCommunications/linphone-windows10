/*
ChatMessage.h
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

#include "Core.h"
#include "ChatMessageListener.h"
#include "ChatRoom.h"
#include "Enums.h"


namespace Linphone
{
	namespace Native
	{
		ref class ChatRoom;
		ref class Core;

		/// <summary>
		/// Represents a chat message shared between two users.
		/// </summary>
		public ref class ChatMessage sealed
		{
		public:
			/// <summary>
			/// Returns the content of the appData
			/// </summary>
			property Platform::String^ AppData
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Linphone message can carry external body as defined by rfc2017.
			/// </summary>
			property Platform::String^ ExternalBodyUrl
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Returns the name of the file used in the file transfer if it exists
			/// </summary>
			property Platform::String^ FileTransferName
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Gets the path to the file to read from or write to during the file transfer.
			/// </summary>
			property Platform::String^ FileTransferFilePath
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Get from address as Address associated to this message.
			/// </summary>
			property Linphone::Native::Address^ From
			{
				Linphone::Native::Address^ get();
			}

			/// <summary>
			/// Returns true if the message was outgoing, otherwise return false.
			/// </summary>
			property Platform::Boolean IsOutgoing
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Returns true if the message has been read, otherwise return false.
			/// </summary>
			property Platform::Boolean IsRead
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Get peer address as Address associated to this message.
			/// </summary>
			property Linphone::Native::Address^ PeerAddress
			{
				Linphone::Native::Address^ get();
			}

			/// <summary>
			/// Gets the state of the message (Idle, InProgress, Delivered or NotDelivered).
			/// </summary>
			property Linphone::Native::ChatMessageState State
			{
				Linphone::Native::ChatMessageState get();
			}

			/// <summary>
			/// Returns the text associated to this message.
			/// </summary>
			property Platform::String^ Text
			{
				Platform::String^ get();
			}
			
			/// <summary>
			/// Gets the time at which the message was sent (in seconds since 01/01/1970).
			/// </summary>
			property int64 Time
			{
				int64 get();
			}

			/// <summary>
			/// Starts the download of the image in the message if exists
			/// </summary>
			void StartFileDownload(Linphone::Native::ChatMessageListener^ listener, Platform::String^ filepath);

		private:
			friend class Linphone::Native::Utils;
			friend ref class Linphone::Native::ChatRoom;
			friend ref class Linphone::Native::Core;

			ChatMessage(::LinphoneChatMessage *cm);
			~ChatMessage();

			::LinphoneChatMessage *message;
		};
	}
}