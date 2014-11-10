#pragma once

// Do not treat doxygen documentation as XML
#pragma warning(push)
#pragma warning(disable : 4635)
#include "belle-sip/object.h"
#pragma warning(pop)

#include <mutex>

#include "Utils.h"


#define API_LOCK ApiLock apiLock(__FUNCTION__)


namespace Linphone
{
    namespace Core
    {
		class GlobalApiLock
		{
		public:
			static GlobalApiLock * Instance();
			void Lock();
			void Unlock();
			bool TryLock();

		private:
			GlobalApiLock();
			~GlobalApiLock();

			static GlobalApiLock *instance;
			std::recursive_mutex mut;
			int count;
			belle_sip_object_pool_t *pool;
		};

		/// <summary>
		/// A class that implements a mutex mechanism to protect objects accessible from the API surface exposed by this DLL
		/// </summary>
		class ApiLock
		{
		public:
			ApiLock(const char *function);
			~ApiLock();

		private:
			char *function;
		};
    }
}
