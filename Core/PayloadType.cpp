#include "ApiLock.h"
#include "PayloadType.h"
#include "Server.h"

Platform::String^ Linphone::Core::PayloadType::GetMimeType()
{
	gApiLock.Lock();
	Platform::String^ mimeType = Utils::cctops(this->payload->mime_type);
	gApiLock.Unlock();
	return mimeType;
}

int Linphone::Core::PayloadType::GetClockRate()
{
	gApiLock.Lock();
	int rate = this->payload->clock_rate;
	gApiLock.Unlock();
	return rate;
}

Linphone::Core::PayloadType::PayloadType(::PayloadType *payload) :
	payload(payload)
{
}

Linphone::Core::PayloadType::~PayloadType()
{
}