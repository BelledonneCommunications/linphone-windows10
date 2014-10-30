#pragma once

#include "Utils.h"


#ifdef TRACE_LOCKS
#define API_LOCK \
	{ \
		char *s = ms_strdup_printf("### Locking in %s\r\n", __FUNCTION__); \
		OutputDebugStringA(s); \
		ms_free(s); \
	} \
	Linphone::Core::gApiLock.Lock()
#define API_UNLOCK \
	{ \
		char *s = ms_strdup_printf("### Unlocking in %s\r\n", __FUNCTION__); \
		OutputDebugStringA(s); \
		ms_free(s); \
	} \
	Linphone::Core::gApiLock.Unlock()
#else
#define API_LOCK Linphone::Core::gApiLock.Lock()
#define API_UNLOCK Linphone::Core::gApiLock.Unlock()
#endif


namespace Linphone
{
    namespace Core
    {
		class ApiLockPrivate;

		/// <summary>
		/// A class that implements a mutex mechanism to protect objects accessible from the API surface exposed by this DLL
		/// </summary>
		class ApiLock
		{
		public:
			ApiLock();
			~ApiLock();
			void Lock();
			void Unlock();
			bool TryLock();

		private:
			ApiLockPrivate *d;
		};

		// The global API lock
		extern ApiLock gApiLock;
    }
}
