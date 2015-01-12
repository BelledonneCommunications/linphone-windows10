#include "ApiLock.h"
#include "mediastreamer2/mscommon.h"

namespace Linphone
{
    namespace Core
    {
		GlobalApiLock *GlobalApiLock::instance = nullptr;
		std::mutex GlobalApiLock::instance_mutex;

		GlobalApiLock::GlobalApiLock() : count(0), pool(nullptr)
		{}

		GlobalApiLock::~GlobalApiLock()
		{}

		GlobalApiLock * GlobalApiLock::Instance()
		{
			if (instance == nullptr) {
				instance_mutex.lock();
				if (instance == nullptr) {
					instance = new GlobalApiLock();
				}
				instance_mutex.unlock();
			}
			return instance;
		}

		void GlobalApiLock::Lock()
		{
			mutex.lock();
			if (count == 0) {
				pool = belle_sip_object_pool_push();
			}
			count++;
		}

		void GlobalApiLock::Unlock()
		{
			count--;
			if ((count == 0) && (pool != nullptr)) {
				belle_sip_object_unref(pool);
				pool = nullptr;
			}
			mutex.unlock();
		}

//#define TRACE_LOCKS
		ApiLock::ApiLock(const char *function)
		{
#ifdef TRACE_LOCKS
			if (function != NULL) {
				this->function = ms_strdup(function);
			}
			ms_error("### Locking in %s [%ul]", this->function, WIN_thread_self());
#endif
			GlobalApiLock::Instance()->Lock();
#ifdef TRACE_LOCKS
			ms_error("### Locked in %s [%ul]", this->function, WIN_thread_self());
#endif
		}

		ApiLock::~ApiLock()
		{
#ifdef TRACE_LOCKS
			ms_error("### Unlocking in %s [%ul]", this->function, WIN_thread_self());
			if (this->function != NULL) {
				ms_free(this->function);
			}
#endif
			GlobalApiLock::Instance()->Unlock();
		}
    }
}
