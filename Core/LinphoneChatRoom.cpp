#include "LinphoneChatRoom.h"
#include "LinphoneAddress.h"

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneChatRoom::GetPeerAddress()
{
	//TODO
	return nullptr;
}
			
void Linphone::Core::LinphoneChatRoom::SendMessage(Linphone::Core::LinphoneChatMessage^ message, Linphone::Core::LinphoneChatMessageListener^ listener)
{
	//TODO
}

Linphone::Core::LinphoneChatMessage^ Linphone::Core::LinphoneChatRoom::CreateLinphoneChatMessage(Platform::String^ message)
{
	//TODO
	return nullptr;
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