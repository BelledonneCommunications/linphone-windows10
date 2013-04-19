#include "LinphoneChatRoom.h"
#include "LinphoneAddress.h"

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneChatRoom::GetPeerAddress()
{
	return (Linphone::Core::LinphoneAddress^) Linphone::Core::Utils::CreateLinphoneAddress((void*)linphone_chat_room_get_peer_address(this->room));
}
	
static void chat_room_callback(::LinphoneChatMessage* msg, ::LinphoneChatMessageState state, void* ud)
{
	Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessageListener^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessageListener^> *>(ud);
	Linphone::Core::LinphoneChatMessageListener^ listener = (proxy) ? proxy->Ref() : nullptr;

	if (listener != nullptr) {
		Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessage^> *proxy = reinterpret_cast< Linphone::Core::RefToPtrProxy<Linphone::Core::LinphoneChatMessage^> *>(linphone_chat_message_get_user_data(msg));
		Linphone::Core::LinphoneChatMessage^ lChatMessage = (proxy) ? proxy->Ref() : nullptr;
		if (lChatMessage == nullptr) {
			lChatMessage = (Linphone::Core::LinphoneChatMessage^)Linphone::Core::Utils::CreateLinphoneChatMessage(msg);
		}

		listener->MessageStateChanged(lChatMessage, (Linphone::Core::LinphoneChatMessageState) state);
	}
}

void Linphone::Core::LinphoneChatRoom::SendMessage(Linphone::Core::LinphoneChatMessage^ message, Linphone::Core::LinphoneChatMessageListener^ listener)
{
	RefToPtrProxy<LinphoneChatMessageListener^> *listenerPtr = new RefToPtrProxy<LinphoneChatMessageListener^>(listener);
	linphone_chat_room_send_message2(this->room, message->message, chat_room_callback, listenerPtr);
}

Linphone::Core::LinphoneChatMessage^ Linphone::Core::LinphoneChatRoom::CreateLinphoneChatMessage(Platform::String^ message)
{
	const char* msg = Linphone::Core::Utils::pstoccs(message);
	Linphone::Core::LinphoneChatMessage^ chatMessage = (Linphone::Core::LinphoneChatMessage^) Linphone::Core::Utils::CreateLinphoneChatMessage(linphone_chat_room_create_message(this->room, msg));
	delete(msg);
	return chatMessage;
}

Linphone::Core::LinphoneChatRoom::LinphoneChatRoom(::LinphoneChatRoom *cr) :
	room(cr)
{
	RefToPtrProxy<LinphoneChatRoom^> *chat_room = new RefToPtrProxy<LinphoneChatRoom^>(this);
	linphone_chat_room_set_user_data(this->room, chat_room);
}

Linphone::Core::LinphoneChatRoom::~LinphoneChatRoom()
{
	RefToPtrProxy<LinphoneChatRoom^> *chat_room = reinterpret_cast< RefToPtrProxy<LinphoneChatRoom^> *>(linphone_chat_room_get_user_data(this->room));
	delete chat_room;
}