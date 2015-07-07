/*
ApiLock.h
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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


#define API_LOCK Linphone::Native::ApiLock apiLock(__FUNCTION__)


namespace Linphone
{
    namespace Native
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
