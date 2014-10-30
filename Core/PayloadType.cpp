#include "ApiLock.h"
#include "PayloadType.h"
#include "Server.h"

Platform::String^ Linphone::Core::PayloadType::GetMimeType()
{
	API_LOCK;
	Platform::String^ mimeType = Utils::cctops(this->payload->mime_type);
	API_UNLOCK;
	return mimeType;
}

int Linphone::Core::PayloadType::GetClockRate()
{
	API_LOCK;
	int rate = this->payload->clock_rate;
	API_UNLOCK;
	return rate;
}

Linphone::Core::PayloadType::PayloadType(::PayloadType *payload) :
	payload(payload)
{
}

Linphone::Core::PayloadType::~PayloadType()
{
}