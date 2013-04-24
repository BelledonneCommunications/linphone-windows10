#pragma once

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
			void EnterListener();
			void LeaveListener();

		private:
			ApiLockPrivate *d;
		};

		// The global API lock
		extern ApiLock gApiLock;
    }
}
