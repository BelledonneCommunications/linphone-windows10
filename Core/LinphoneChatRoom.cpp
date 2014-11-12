#include "LinphoneChatRoom.h"
#include "LinphoneAddress.h"
#include "ApiLock.h"
#include <collection.h>

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneChatRoom::PeerAddress::get()
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
	API_LOCK;
	RefToPtrProxy<LinphoneChatMessageListener^> *listenerPtr = new RefToPtrProxy<LinphoneChatMessageListener^>(listener);
	linphone_chat_room_send_message2(this->room, message->message, chat_room_callback, listenerPtr);
}

Linphone::Core::LinphoneChatMessage^ Linphone::Core::LinphoneChatRoom::CreateLinphoneChatMessage(Platform::String^ message)
{
	API_LOCK;
	const char* msg = Linphone::Core::Utils::pstoccs(message);
	Linphone::Core::LinphoneChatMessage^ chatMessage = (Linphone::Core::LinphoneChatMessage^) Linphone::Core::Utils::CreateLinphoneChatMessage(linphone_chat_room_create_message(this->room, msg));
	delete(msg);
	return chatMessage;
}

Linphone::Core::LinphoneChatMessage^ Linphone::Core::LinphoneChatRoom::CreateFileTransferMessage(Platform::String^ type, Platform::String^ subtype, Platform::String^ name, int size, Platform::String^ filepath)
{
	API_LOCK;
	const char *ctype = Linphone::Core::Utils::pstoccs(type);
	const char *csubtype = Linphone::Core::Utils::pstoccs(subtype);
	const char *cname = Linphone::Core::Utils::pstoccs(name);
	const char *cfilepath = Linphone::Core::Utils::pstoccs(filepath);
	LinphoneContent content;
	::LinphoneChatMessage *msg;
	memset(&content, 0, sizeof(content));
	content.type = (char *)ctype;
	content.subtype = (char *)csubtype;
	content.size = size;
	content.name = (char *)cname;
	msg = linphone_chat_room_create_file_transfer_message(this->room, &content);
	linphone_chat_message_set_file_transfer_filepath(msg, cfilepath);
	Linphone::Core::LinphoneChatMessage^ chatMessage = (Linphone::Core::LinphoneChatMessage^) Linphone::Core::Utils::CreateLinphoneChatMessage(msg);
	delete(ctype);
	delete(csubtype);
	delete(cname);
	delete(cfilepath);
	return chatMessage;
}

Platform::Boolean Linphone::Core::LinphoneChatRoom::IsRemoteComposing::get()
{
	API_LOCK;
	return (linphone_chat_room_is_remote_composing(this->room) == TRUE);
}

void Linphone::Core::LinphoneChatRoom::Compose()
{
	API_LOCK;
	linphone_chat_room_compose(this->room);
}

int Linphone::Core::LinphoneChatRoom::HistorySize::get()
{
	API_LOCK;
	return linphone_chat_room_get_history_size(this->room);
}

void Linphone::Core::LinphoneChatRoom::DeleteHistory()
{
	API_LOCK;
	linphone_chat_room_delete_history(this->room);
}

int Linphone::Core::LinphoneChatRoom::UnreadMessageCount::get()
{
	API_LOCK;
	return linphone_chat_room_get_unread_messages_count(this->room);
}

void Linphone::Core::LinphoneChatRoom::MarkAsRead()
{
	API_LOCK;
	linphone_chat_room_mark_as_read(this->room);
}

static void AddChatMessageToVector(void *vMessage, void *vector)
{
	::LinphoneChatMessage *chatMessage = (LinphoneChatMessage*)vMessage;
	Linphone::Core::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Core::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ messages = (list) ? list->Ref() : nullptr;
	Linphone::Core::LinphoneChatMessage^ message = (Linphone::Core::LinphoneChatMessage^) Linphone::Core::Utils::CreateLinphoneChatMessage(chatMessage);
	messages->Append(message);
}

IVector<Object^>^ Linphone::Core::LinphoneChatRoom::History::get()
{
	API_LOCK;
	IVector<Object^>^ history = ref new Vector<Object^>();
	MSList* messages = linphone_chat_room_get_history(this->room, 0);
	RefToPtrProxy<IVector<Object^>^> *historyPtr = new RefToPtrProxy<IVector<Object^>^>(history);
	ms_list_for_each2(messages, AddChatMessageToVector, historyPtr);
	return history;
}

void Linphone::Core::LinphoneChatRoom::DeleteMessageFromHistory(Linphone::Core::LinphoneChatMessage^ message)
{
	API_LOCK;
	linphone_chat_room_delete_message(this->room, message->message);
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