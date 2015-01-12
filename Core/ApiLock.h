#pragma once

// Do not treat doxygen documentation as XML
#pragma warning(push)
#pragma warning(disable : 4635)
#include "belle-sip/object.h"
#pragma warning(pop)

#include <list>
#include <mutex>

#include "Utils.h"


//#define API_LOCK_DEBUG


#define API_LOCK Linphone::Core::ApiLock apiLock(__FUNCTION__)


namespace Linphone
{
    namespace Core
    {
#ifdef API_LOCK_DEBUG
		struct LockInfo
		{
			unsigned long thread;
			std::string func;
			bool locked;
		};
#endif

		class GlobalApiLock
		{
		public:
			static GlobalApiLock * Instance();
#ifdef API_LOCK_DEBUG
			static std::list<LockInfo> lock_info_list;
#endif
			void Lock(std::string func);
			void Unlock(std::string func);

		private:
			GlobalApiLock();
			~GlobalApiLock();
#ifdef API_LOCK_DEBUG
			void RegisterLocking(std::string func);
			void RegisterLocked(std::string func);
			void UnregisterLocked(std::string func);
#endif

			static GlobalApiLock *instance;
			static std::mutex instance_mutex;
			std::recursive_mutex mutex;
			int count;
			belle_sip_object_pool_t *pool;
		};

		/// <summary>
		/// A class that implements a mutex mechanism to protect objects accessible from the API surface exposed by this DLL
		/// </summary>
		class ApiLock
		{
		public:
			ApiLock(std::string func);
			~ApiLock();

		private:
			std::string func;
		};
    }
}
