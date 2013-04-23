#include "ApiLock.h"
#include "belle-sip/object.h"
#include <mutex>

namespace Linphone
{
    namespace Core
    {
        // The global API lock
		ApiLock gApiLock;


		class ApiLockPrivate
		{
		public:
			ApiLockPrivate() : count(0), pool(nullptr)
			{
			}
			~ApiLockPrivate()
			{
			}

			std::recursive_mutex mut;
			int count;
			belle_sip_object_pool_t *pool;
		};


		ApiLock::ApiLock() : d(new ApiLockPrivate())
		{
		}

		ApiLock::~ApiLock()
		{
			delete d;
		}

		void ApiLock::Lock()
		{
			d->mut.lock();
			if (d->count == 0) {
				d->pool = belle_sip_object_pool_push();
			}
			d->count++;
		}

		void ApiLock::Unlock()
		{
			d->count--;
			if ((d->count == 0) && (d->pool != nullptr)) {
				belle_sip_object_unref(d->pool);
				d->pool = nullptr;
			}
			d->mut.unlock();
		}
    }
}
