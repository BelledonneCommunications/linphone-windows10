#pragma once
#include <mutex>

namespace Linphone
{
    namespace Core
    {
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

		private:
			std::recursive_mutex mut;
		};

		// The global API lock
		extern ApiLock gApiLock;
    }
}
