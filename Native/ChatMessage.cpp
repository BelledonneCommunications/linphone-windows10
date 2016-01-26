/*
ChatMessage.cpp
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
#include "ChatMessage.h"

using namespace BelledonneCommunications::Linphone::Native;

Platform::String^ ChatMessage::AppData::get()
{
	API_LOCK;
	return Utils::cctops(linphone_chat_message_get_appdata(this->message));
}

void ChatMessage::AppData::set(Platform::String^ appData)
{
	API_LOCK;
	linphone_chat_message_set_appdata(this->message, Utils::pstoccs(appData));
}

Platform::String^ ChatMessage::ExternalBodyUrl::get()
{
	API_LOCK;
	return Utils::cctops(linphone_chat_message_get_external_body_url(this->message));
}

void ChatMessage::ExternalBodyUrl::set(Platform::String^ url)
{
	API_LOCK;
	const char* body = Utils::pstoccs(url);
	linphone_chat_message_set_external_body_url(this->message, body);
	delete(body);
}

Platform::String^ ChatMessage::FileTransferFilepath::get()
{
	API_LOCK;
	return Utils::cctops(linphone_chat_message_get_file_transfer_filepath(this->message));
}

Platform::String^ ChatMessage::FileTransferName::get()
{
	API_LOCK;
	Platform::String^ fileName;
	const LinphoneContent *content = linphone_chat_message_get_file_transfer_information(this->message);
	if (content)
	{
		fileName = Utils::cctops(linphone_content_get_name(content));
	}
	return fileName;
}

Address^ ChatMessage::FromAddress::get()
{
	API_LOCK;
	return (Address^) Utils::CreateAddress((void*)linphone_chat_message_get_from_address(this->message));
}

Platform::Boolean ChatMessage::IsOutgoing::get()
{
	API_LOCK;
	return (linphone_chat_message_is_outgoing(this->message) == TRUE);
}

Platform::Boolean ChatMessage::IsRead::get()
{
	API_LOCK;
	return (linphone_chat_message_is_read(this->message) == TRUE);
}

Address^ ChatMessage::PeerAddress::get()
{
	API_LOCK;
	return (Address^) Utils::CreateAddress((void*)linphone_chat_message_get_peer_address(this->message));
}

ChatMessageState ChatMessage::State::get()
{
	API_LOCK;
	return (ChatMessageState) linphone_chat_message_get_state(this->message);
}

Platform::String^ ChatMessage::Text::get()
{
	API_LOCK;
	return Utils::cctops(linphone_chat_message_get_text(this->message));
}

int64 ChatMessage::Time::get()
{
	API_LOCK;
	return linphone_chat_message_get_time(this->message);
}

static void status_cb(::LinphoneChatMessage* msg, ::LinphoneChatMessageState state, void* ud)
{
	RefToPtrProxy<ChatMessageListener^> *proxy = reinterpret_cast< RefToPtrProxy<ChatMessageListener^> *>(ud);
	ChatMessageListener^ listener = (proxy) ? proxy->Ref() : nullptr;

	if (listener != nullptr) {
		ChatMessage^ lChatMessage = (ChatMessage^)Utils::GetChatMessage(msg);
		listener->MessageStateChanged(lChatMessage, (ChatMessageState) state);
	}
}

void ChatMessage::StartFileDownload(ChatMessageListener^ listener, Platform::String^ filepath)
{
	API_LOCK;
	RefToPtrProxy<ChatMessageListener^> *listenerPtr = new RefToPtrProxy<ChatMessageListener^>(listener);
	const char *cfilepath = Utils::pstoccs(filepath);
	linphone_chat_message_set_file_transfer_filepath(this->message, cfilepath);
	linphone_chat_message_start_file_download(this->message, status_cb, listenerPtr);
	delete(cfilepath);
}

ChatMessage::ChatMessage(::LinphoneChatMessage *cm)
	: message(cm)
{
	API_LOCK;
	RefToPtrProxy<ChatMessage^> *chat_message = new RefToPtrProxy<ChatMessage^>(this);
	linphone_chat_message_ref(message);
	linphone_chat_message_set_user_data(this->message, chat_message);
}

ChatMessage::~ChatMessage()
{
	API_LOCK;
	linphone_chat_message_unref(message);
	RefToPtrProxy<ChatMessage^> *chat_message = reinterpret_cast< RefToPtrProxy<ChatMessage^> *>(linphone_chat_message_get_user_data(this->message));
	delete chat_message;
}
