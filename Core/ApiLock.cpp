#include "ApiLock.h"
#include "mediastreamer2/mscommon.h"

namespace Linphone
{
    namespace Core
    {
		GlobalApiLock *GlobalApiLock::instance = nullptr;

		GlobalApiLock::GlobalApiLock() : count(0), pool(nullptr)
		{}

		GlobalApiLock::~GlobalApiLock()
		{}

		GlobalApiLock * GlobalApiLock::Instance()
		{
			if (instance == nullptr) {
				instance = new GlobalApiLock();
			}
			return instance;
		}

		void GlobalApiLock::Lock()
		{
			mut.lock();
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
			mut.unlock();
		}

		bool GlobalApiLock::TryLock()
		{
			bool ok = mut.try_lock();
			if (ok) {
				if (count == 0) {
					pool = belle_sip_object_pool_push();
				}
				count++;
			}
			return ok;
		}


		ApiLock::ApiLock(const char *function)
		{
#ifdef TRACE_LOCKS
			if (function != NULL) {
				this->function = ms_strdup(function);
			}
			char *s = ms_strdup_printf("### Locking in %s [%ul] at %ul\r\n", this->function, WIN_thread_self(), (unsigned long)ortp_get_cur_time_ms());
			OutputDebugStringA(s);
			ms_free(s);
#endif
			GlobalApiLock::Instance()->Lock();
		}

		ApiLock::~ApiLock()
		{
#ifdef TRACE_LOCKS
			char *s = ms_strdup_printf("### Unlocking in %s [%ul] at %ul\r\n", this->function, WIN_thread_self(), (unsigned long)ortp_get_cur_time_ms());
			OutputDebugStringA(s);
			ms_free(s);
			if (this->function != NULL) {
				ms_free(this->function);
			}
#endif
			GlobalApiLock::Instance()->Unlock();
		}
    }
}
