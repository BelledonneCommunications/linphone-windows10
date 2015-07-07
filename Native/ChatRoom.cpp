/*
ChatRoom.cpp
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

#include "Address.h"
#include "ApiLock.h"
#include "ChatRoom.h"

#include <collection.h>

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;


static void AddChatMessageToVector(void *vMessage, void *vector)
{
	::LinphoneChatMessage *chatMessage = (::LinphoneChatMessage*)vMessage;
	Linphone::Native::RefToPtrProxy<IVector<Linphone::Native::ChatMessage^>^> *list = reinterpret_cast< Linphone::Native::RefToPtrProxy<IVector<Linphone::Native::ChatMessage^>^> *>(vector);
	IVector<Linphone::Native::ChatMessage^>^ messages = (list) ? list->Ref() : nullptr;
	Linphone::Native::ChatMessage^ message = (Linphone::Native::ChatMessage^) Linphone::Native::Utils::CreateLinphoneChatMessage(chatMessage);
	messages->Append(message);
}

IVector<Linphone::Native::ChatMessage^>^ Linphone::Native::ChatRoom::History::get()
{
	API_LOCK;
	IVector<Linphone::Native::ChatMessage^>^ history = ref new Vector<Linphone::Native::ChatMessage^>();
	MSList* messages = linphone_chat_room_get_history(this->room, 0);
	RefToPtrProxy<IVector<Linphone::Native::ChatMessage^>^> *historyPtr = new RefToPtrProxy<IVector<Linphone::Native::ChatMessage^>^>(history);
	ms_list_for_each2(messages, AddChatMessageToVector, historyPtr);
	return history;
}

int Linphone::Native::ChatRoom::HistorySize::get()
{
	API_LOCK;
	return linphone_chat_room_get_history_size(this->room);
}

Platform::Boolean Linphone::Native::ChatRoom::IsRemoteComposing::get()
{
	API_LOCK;
	return (linphone_chat_room_is_remote_composing(this->room) == TRUE);
}

Linphone::Native::Address^ Linphone::Native::ChatRoom::PeerAddress::get()
{
	API_LOCK;
	return (Linphone::Native::Address^) Linphone::Native::Utils::CreateAddress((void*)linphone_chat_room_get_peer_address(this->room));
}
	
int Linphone::Native::ChatRoom::UnreadMessageCount::get()
{
	API_LOCK;
	return linphone_chat_room_get_unread_messages_count(this->room);
}

void Linphone::Native::ChatRoom::Compose()
{
	API_LOCK;
	linphone_chat_room_compose(this->room);
}

Linphone::Native::ChatMessage^ Linphone::Native::ChatRoom::CreateFileTransferMessage(Platform::String^ type, Platform::String^ subtype, Platform::String^ name, int size, Platform::String^ filepath)
{
	API_LOCK;
	const char *ctype = Linphone::Native::Utils::pstoccs(type);
	const char *csubtype = Linphone::Native::Utils::pstoccs(subtype);
	const char *cname = Linphone::Native::Utils::pstoccs(name);
	const char *cfilepath = Linphone::Native::Utils::pstoccs(filepath);
	LinphoneContent *content = linphone_core_create_content(linphone_chat_room_get_core(this->room));
	::LinphoneChatMessage *msg;
	linphone_content_set_type(content, ctype);
	linphone_content_set_subtype(content, csubtype);
	linphone_content_set_size(content, size);
	linphone_content_set_name(content, cname);
	msg = linphone_chat_room_create_file_transfer_message(this->room, content);
	linphone_chat_message_set_file_transfer_filepath(msg, cfilepath);
	Linphone::Native::ChatMessage^ chatMessage = (Linphone::Native::ChatMessage^) Linphone::Native::Utils::CreateLinphoneChatMessage(msg);
	delete(ctype);
	delete(csubtype);
	delete(cname);
	delete(cfilepath);
	return chatMessage;
}

Linphone::Native::ChatMessage^ Linphone::Native::ChatRoom::CreateLinphoneChatMessage(Platform::String^ message)
{
	API_LOCK;
	const char* msg = Linphone::Native::Utils::pstoccs(message);
	Linphone::Native::ChatMessage^ chatMessage = (Linphone::Native::ChatMessage^) Linphone::Native::Utils::CreateLinphoneChatMessage(linphone_chat_room_create_message(this->room, msg));
	delete(msg);
	return chatMessage;
}

void Linphone::Native::ChatRoom::DeleteHistory()
{
	API_LOCK;
	linphone_chat_room_delete_history(this->room);
}

void Linphone::Native::ChatRoom::DeleteMessageFromHistory(Linphone::Native::ChatMessage^ message)
{
	API_LOCK;
	linphone_chat_room_delete_message(this->room, message->message);
}

void Linphone::Native::ChatRoom::MarkAsRead()
{
	API_LOCK;
	linphone_chat_room_mark_as_read(this->room);
}

static void chat_room_callback(::LinphoneChatMessage* msg, ::LinphoneChatMessageState state, void* ud)
{
	Linphone::Native::RefToPtrProxy<Linphone::Native::ChatMessageListener^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::ChatMessageListener^> *>(ud);
	Linphone::Native::ChatMessageListener^ listener = (proxy) ? proxy->Ref() : nullptr;

	if (listener != nullptr) {
		Linphone::Native::RefToPtrProxy<Linphone::Native::ChatMessage^> *proxy = reinterpret_cast< Linphone::Native::RefToPtrProxy<Linphone::Native::ChatMessage^> *>(linphone_chat_message_get_user_data(msg));
		Linphone::Native::ChatMessage^ lChatMessage = (proxy) ? proxy->Ref() : nullptr;
		if (lChatMessage == nullptr) {
			lChatMessage = (Linphone::Native::ChatMessage^)Linphone::Native::Utils::CreateLinphoneChatMessage(msg);
		}

		listener->MessageStateChanged(lChatMessage, (Linphone::Native::ChatMessageState) state);
	}
}

void Linphone::Native::ChatRoom::SendMessage(Linphone::Native::ChatMessage^ message, Linphone::Native::ChatMessageListener^ listener)
{
	API_LOCK;
	RefToPtrProxy<ChatMessageListener^> *listenerPtr = new RefToPtrProxy<ChatMessageListener^>(listener);
	linphone_chat_room_send_message2(this->room, message->message, chat_room_callback, listenerPtr);
}

Linphone::Native::ChatRoom::ChatRoom(::LinphoneChatRoom *cr)
	: room(cr)
{
	API_LOCK;
	RefToPtrProxy<ChatRoom^> *chat_room = new RefToPtrProxy<ChatRoom^>(this);
	linphone_chat_room_set_user_data(this->room, chat_room);
}

Linphone::Native::ChatRoom::~ChatRoom()
{
	API_LOCK;
	RefToPtrProxy<ChatRoom^> *chat_room = reinterpret_cast< RefToPtrProxy<ChatRoom^> *>(linphone_chat_room_get_user_data(this->room));
	delete chat_room;
}
