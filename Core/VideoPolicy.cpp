#include "ApiLock.h"
#include "VideoPolicy.h"

Linphone::Core::VideoPolicy::VideoPolicy() :
automaticallyInitiate(true), automaticallyAccept(true)
{
}

Linphone::Core::VideoPolicy::VideoPolicy(bool automaticallyInitiate, bool automaticallyAccept) :
automaticallyInitiate(automaticallyInitiate), automaticallyAccept(automaticallyAccept)
{
}

bool Linphone::Core::VideoPolicy::AutomaticallyInitiate::get()
{
	return automaticallyInitiate;
}

void Linphone::Core::VideoPolicy::AutomaticallyInitiate::set(bool value)
{
	API_LOCK;
	automaticallyInitiate = value;
}

bool Linphone::Core::VideoPolicy::AutomaticallyAccept::get()
{
	return automaticallyAccept;
}

void Linphone::Core::VideoPolicy::AutomaticallyAccept::set(bool value)
{
	API_LOCK;
	automaticallyAccept = value;
}