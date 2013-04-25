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
			ApiLockPrivate() : count(0), pool(nullptr), inListener(false)
			{
			}
			~ApiLockPrivate()
			{
			}

			std::recursive_mutex mut;
			int count;
			belle_sip_object_pool_t *pool;
			bool inListener;
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
			if (d->inListener == true) return;
			d->mut.lock();
			if (d->count == 0) {
				d->pool = belle_sip_object_pool_push();
			}
			d->count++;
		}

		void ApiLock::Unlock()
		{
			if (d->inListener == true) return;
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

		void ApiLock::EnterListener()
		{
			d->inListener = true;
		}

		void ApiLock::LeaveListener()
		{
			d->inListener = false;
		}
    }
}
