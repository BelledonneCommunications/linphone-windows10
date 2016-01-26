/*
ApiLock.cpp
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

#include "ApiLock.h"
#include "mediastreamer2/mscommon.h"

using namespace BelledonneCommunications::Linphone::Native;

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
