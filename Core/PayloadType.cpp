#include "ApiLock.h"
#include "PayloadType.h"
#include "Server.h"

Platform::String^ Linphone::Core::PayloadType::GetMimeType()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	return Utils::cctops(this->payload->mime_type);
}

int Linphone::Core::PayloadType::GetClockRate()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);
	return this->payload->clock_rate;
}

Linphone::Core::PayloadType::PayloadType(::PayloadType *payload) :
	payload(payload)
{
}

Linphone::Core::PayloadType::~PayloadType()
{
}