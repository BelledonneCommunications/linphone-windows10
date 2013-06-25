#include "LinphoneChatMessage.h"
#include "LinphoneAddress.h"
#include "ApiLock.h"

Platform::String^ Linphone::Core::LinphoneChatMessage::GetText()
{
	return Linphone::Core::Utils::cctops(linphone_chat_message_get_text(this->message));
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneChatMessage::GetPeerAddress()
{
	return (Linphone::Core::LinphoneAddress^) Linphone::Core::Utils::CreateLinphoneAddress((void*)linphone_chat_message_get_peer_address(this->message));
}

Linphone::Core::LinphoneAddress^ Linphone::Core::LinphoneChatMessage::GetFrom()
{
	return (Linphone::Core::LinphoneAddress^) Linphone::Core::Utils::CreateLinphoneAddress((void*)linphone_chat_message_get_from(this->message));
}

Platform::String^ Linphone::Core::LinphoneChatMessage::GetExternalBodyUrl()
{
	return Linphone::Core::Utils::cctops(linphone_chat_message_get_external_body_url(this->message));
}

void Linphone::Core::LinphoneChatMessage::SetExternalBodyUrl(Platform::String^ url)
{
	gApiLock.Lock();
	const char* body = Linphone::Core::Utils::pstoccs(url);
	linphone_chat_message_set_external_body_url(this->message, body);
	delete(body);
	gApiLock.Unlock();
}

int64 Linphone::Core::LinphoneChatMessage::GetTime()
{
	return linphone_chat_message_get_time(this->message);
}

Linphone::Core::LinphoneChatMessageState  Linphone::Core::LinphoneChatMessage::GetState()
{
	return (Linphone::Core::LinphoneChatMessageState) linphone_chat_message_get_state(this->message);
}

Linphone::Core::LinphoneChatMessage::LinphoneChatMessage(::LinphoneChatMessage *cm) :
	message(cm)
{
	RefToPtrProxy<LinphoneChatMessage^> *chat_message = new RefToPtrProxy<LinphoneChatMessage^>(this);
	linphone_chat_message_set_user_data(this->message, chat_message);
}

Linphone::Core::LinphoneChatMessage::~LinphoneChatMessage()
{
	RefToPtrProxy<LinphoneChatMessage^> *chat_message = reinterpret_cast< RefToPtrProxy<LinphoneChatMessage^> *>(linphone_chat_message_get_user_data(this->message));
	delete chat_message;
}