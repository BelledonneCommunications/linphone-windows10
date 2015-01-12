#include "ApiLock.h"
#include "mediastreamer2/mscommon.h"

namespace Linphone
{
    namespace Core
    {
		GlobalApiLock *GlobalApiLock::instance = nullptr;
		std::mutex GlobalApiLock::instance_mutex;
#ifdef API_LOCK_DEBUG
		std::list<LockInfo> GlobalApiLock::lock_info_list;
#endif

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

		void GlobalApiLock::Lock(std::string func)
		{
#ifdef API_LOCK_DEBUG
			RegisterLocking(func);
#endif
			mutex.lock();
#ifdef API_LOCK_DEBUG
			RegisterLocked(func);
#endif
			if (count == 0) {
				pool = belle_sip_object_pool_push();
			}
			count++;
		}

		void GlobalApiLock::Unlock(std::string func)
		{
			count--;
			if ((count == 0) && (pool != nullptr)) {
				belle_sip_object_unref(pool);
				pool = nullptr;
			}
#ifdef API_LOCK_DEBUG
			UnregisterLocked(func);
#endif
			mutex.unlock();
		}

#ifdef API_LOCK_DEBUG
		void GlobalApiLock::RegisterLocking(std::string func)
		{
			instance_mutex.lock();
			LockInfo li = { WIN_thread_self(), func, false };
			lock_info_list.push_back(li);
			instance_mutex.unlock();
		}

		void GlobalApiLock::RegisterLocked(std::string func)
		{
			instance_mutex.lock();
			unsigned long current_thread = WIN_thread_self();
			for (std::list<LockInfo>::reverse_iterator it = lock_info_list.rbegin(); it != lock_info_list.rend(); it++) {
				if ((it->func == func) && (it->thread = current_thread) && !it->locked) {
					it->locked = true;
					break;
				}
			}
			instance_mutex.unlock();
		}

		void GlobalApiLock::UnregisterLocked(std::string func)
		{
			instance_mutex.lock();
			unsigned long current_thread = WIN_thread_self();
			for (std::list<LockInfo>::reverse_iterator it = lock_info_list.rbegin(); it != lock_info_list.rend(); it++) {
				if ((it->func == func) && (it->thread = current_thread) && it->locked) {
					lock_info_list.erase(--(it.base()));
					break;
				}
			}
			instance_mutex.unlock();
		}
#endif

		ApiLock::ApiLock(std::string func) : func(func)
		{
			GlobalApiLock::Instance()->Lock(func);
		}

		ApiLock::~ApiLock()
		{
			GlobalApiLock::Instance()->Unlock(func);
		}
    }
}
