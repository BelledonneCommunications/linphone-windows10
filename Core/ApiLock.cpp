#include "ApiLock.h"

namespace Linphone
{
    namespace Core
    {
        // The global API lock
		ApiLock gApiLock;

		ApiLock::ApiLock()
		{
		}

		ApiLock::~ApiLock()
		{
		}

		void ApiLock::Lock()
		{
			mut.lock();
		}

		void ApiLock::Unlock()
		{
			mut.unlock();
		}
    }
}
