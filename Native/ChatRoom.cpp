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

using namespace BelledonneCommunications::Linphone::Native;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;


static void AddChatMessageToVector(void *vMessage, void *vector)
{
	::LinphoneChatMessage *chatMessage = (::LinphoneChatMessage*)vMessage;
	RefToPtrProxy<IVector<ChatMessage^>^> *list = reinterpret_cast< RefToPtrProxy<IVector<ChatMessage^>^> *>(vector);
	IVector<ChatMessage^>^ messages = (list) ? list->Ref() : nullptr;
	ChatMessage^ message = (ChatMessage^) Utils::GetChatMessage(chatMessage);
	messages->Append(message);
}

IVector<ChatMessage^>^ ChatRoom::History::get()
{
	API_LOCK;
	IVector<ChatMessage^>^ history = ref new Vector<ChatMessage^>();
	MSList* messages = linphone_chat_room_get_history(this->room, 0);
	RefToPtrProxy<IVector<ChatMessage^>^> *historyPtr = new RefToPtrProxy<IVector<ChatMessage^>^>(history);
	ms_list_for_each2(messages, AddChatMessageToVector, historyPtr);
	return history;
}

int ChatRoom::HistorySize::get()
{
	API_LOCK;
	return linphone_chat_room_get_history_size(this->room);
}

Platform::Boolean ChatRoom::IsRemoteComposing::get()
{
	API_LOCK;
	return (linphone_chat_room_is_remote_composing(this->room) == TRUE);
}

Address^ ChatRoom::PeerAddress::get()
{
	API_LOCK;
	return (Address^) Utils::CreateAddress((void*)linphone_chat_room_get_peer_address(this->room));
}
	
int ChatRoom::UnreadMessageCount::get()
{
	API_LOCK;
	return linphone_chat_room_get_unread_messages_count(this->room);
}

void ChatRoom::Compose()
{
	API_LOCK;
	linphone_chat_room_compose(this->room);
}

ChatMessage^ ChatRoom::CreateFileTransferMessage(Platform::String^ type, Platform::String^ subtype, Platform::String^ name, int size, Platform::String^ filepath)
{
	API_LOCK;
	const char *ctype = Utils::pstoccs(type);
	const char *csubtype = Utils::pstoccs(subtype);
	const char *cname = Utils::pstoccs(name);
	const char *cfilepath = Utils::pstoccs(filepath);
	LinphoneContent *content = linphone_core_create_content(linphone_chat_room_get_core(this->room));
	::LinphoneChatMessage *msg;
	linphone_content_set_type(content, ctype);
	linphone_content_set_subtype(content, csubtype);
	linphone_content_set_size(content, size);
	linphone_content_set_name(content, cname);
	msg = linphone_chat_room_create_file_transfer_message(this->room, content);
	linphone_chat_message_set_file_transfer_filepath(msg, cfilepath);
	ChatMessage^ chatMessage = (ChatMessage^) Utils::GetChatMessage(msg);
	delete(ctype);
	delete(csubtype);
	delete(cname);
	delete(cfilepath);
	return chatMessage;
}

ChatMessage^ ChatRoom::CreateMessage(Platform::String^ message)
{
	API_LOCK;
	const char* msg = Utils::pstoccs(message);
	::LinphoneChatMessage *cm = linphone_chat_room_create_message(this->room, msg);
	ChatMessage^ chatMessage = (ChatMessage^) Utils::GetChatMessage(cm);
	delete(msg);
	return chatMessage;
}

void ChatRoom::DeleteHistory()
{
	API_LOCK;
	linphone_chat_room_delete_history(this->room);
}

void ChatRoom::DeleteMessage(ChatMessage^ message)
{
	API_LOCK;
	linphone_chat_room_delete_message(this->room, message->message);
}

void ChatRoom::MarkAsRead()
{
	API_LOCK;
	linphone_chat_room_mark_as_read(this->room);
}

static void chat_room_callback(::LinphoneChatMessage* msg, ::LinphoneChatMessageState state, void* ud)
{
	RefToPtrProxy<ChatMessageListener^> *proxy = reinterpret_cast< RefToPtrProxy<ChatMessageListener^> *>(ud);
	ChatMessageListener^ listener = (proxy) ? proxy->Ref() : nullptr;

	if (listener != nullptr) {
		ChatMessage^ lChatMessage = (ChatMessage^)Utils::GetChatMessage(msg);
		listener->MessageStateChanged(lChatMessage, (ChatMessageState) state);
	}
}

void ChatRoom::SendMessage(ChatMessage^ message, ChatMessageListener^ listener)
{
	API_LOCK;
	RefToPtrProxy<ChatMessageListener^> *listenerPtr = new RefToPtrProxy<ChatMessageListener^>(listener);
	linphone_chat_room_send_message2(this->room, message->message, chat_room_callback, listenerPtr);
}

ChatRoom::ChatRoom(::LinphoneChatRoom *cr)
	: room(cr)
{
	API_LOCK;
	RefToPtrProxy<ChatRoom^> *chat_room = new RefToPtrProxy<ChatRoom^>(this);
	linphone_chat_room_ref(this->room);
	linphone_chat_room_set_user_data(this->room, chat_room);
}

ChatRoom::~ChatRoom()
{
	API_LOCK;
	if (this->room != nullptr) {
		linphone_chat_room_unref(this->room);
	}
	RefToPtrProxy<ChatRoom^> *chat_room = reinterpret_cast< RefToPtrProxy<ChatRoom^> *>(linphone_chat_room_get_user_data(this->room));
	delete chat_room;
}
