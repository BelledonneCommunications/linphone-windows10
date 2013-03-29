#include "PayloadType.h"
#include "Server.h"

Platform::String^ Linphone::Core::PayloadType::GetMime()
{
	return nullptr;
}

int Linphone::Core::PayloadType::GetRate()
{
	return -1;
}

Linphone::Core::PayloadType::PayloadType(::PayloadType *payload) :
	payload(payload)
{
}

Linphone::Core::PayloadType::~PayloadType()
{
}