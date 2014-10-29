#include "ApiLock.h"

// Do not treat doxygen documentation as XML
#pragma warning(push)
#pragma warning(disable : 4635)
#include "belle-sip/object.h"
#pragma warning(pop)

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

		bool ApiLock::TryLock()
		{
			bool ok = d->mut.try_lock();
			if (ok) {
				if (d->count == 0) {
					d->pool = belle_sip_object_pool_push();
				}
				d->count++;
			}
			return ok;
		}
    }
}
