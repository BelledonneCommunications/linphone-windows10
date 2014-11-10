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
	API_LOCK;
	const char* body = Linphone::Core::Utils::pstoccs(url);
	linphone_chat_message_set_external_body_url(this->message, body);
	delete(body);
}

int64 Linphone::Core::LinphoneChatMessage::GetTime()
{
	return linphone_chat_message_get_time(this->message);
}

Linphone::Core::LinphoneChatMessageState  Linphone::Core::LinphoneChatMessage::GetState()
{
	return (Linphone::Core::LinphoneChatMessageState) linphone_chat_message_get_state(this->message);
}

Platform::Boolean Linphone::Core::LinphoneChatMessage::IsOutgoing() 
{
	API_LOCK;
	return (linphone_chat_message_is_outgoing(this->message) == TRUE);
}

Platform::Boolean Linphone::Core::LinphoneChatMessage::IsRead()
{
	API_LOCK;
	return (linphone_chat_message_is_read(this->message) == TRUE);
}

Platform::String^ Linphone::Core::LinphoneChatMessage::GetFileTransferName() 
{
	API_LOCK;
	Platform::String^ fileName;
	const LinphoneContent *content = linphone_chat_message_get_file_transfer_information(this->message);
	if (content) 
	{
		fileName = Linphone::Core::Utils::cctops(content->name);
	}
	return fileName;
}

Platform::String^ Linphone::Core::LinphoneChatMessage::GetFileTransferFilePath()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_chat_message_get_file_transfer_filepath(this->message));
}

Platform::String^ Linphone::Core::LinphoneChatMessage::GetAppData()
{
	API_LOCK;
	return Linphone::Core::Utils::cctops(linphone_chat_message_get_appdata(this->message));
}

void Linphone::Core::LinphoneChatMessage::SetAppData(Platform::String^ appData)
{
	API_LOCK;
	linphone_chat_message_set_appdata(this->message, Linphone::Core::Utils::pstoccs(appData));
}

static void status_cb(::LinphoneChatMessage* msg, ::LinphoneChatMessageState state, void* ud)
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

void Linphone::Core::LinphoneChatMessage::StartFileDownload(Linphone::Core::LinphoneChatMessageListener^ listener, Platform::String^ filepath)
{
	API_LOCK;
	RefToPtrProxy<LinphoneChatMessageListener^> *listenerPtr = new RefToPtrProxy<LinphoneChatMessageListener^>(listener);
	const char *cfilepath = Utils::pstoccs(filepath);
	linphone_chat_message_set_file_transfer_filepath(this->message, cfilepath);
	linphone_chat_message_start_file_download(this->message, status_cb, listenerPtr);
	delete(cfilepath);
}

Linphone::Core::LinphoneChatMessage::LinphoneChatMessage(::LinphoneChatMessage *cm) :
	message(cm)
{
	message = linphone_chat_message_ref(message);
	RefToPtrProxy<LinphoneChatMessage^> *chat_message = new RefToPtrProxy<LinphoneChatMessage^>(this);
	linphone_chat_message_set_user_data(this->message, chat_message);
}

Linphone::Core::LinphoneChatMessage::~LinphoneChatMessage()
{
	linphone_chat_message_unref(message);
	RefToPtrProxy<LinphoneChatMessage^> *chat_message = reinterpret_cast< RefToPtrProxy<LinphoneChatMessage^> *>(linphone_chat_message_get_user_data(this->message));
	delete chat_message;
}