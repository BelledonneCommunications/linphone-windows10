/*
VideoPolicy.cpp
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#include "ApiLock.h"
#include "VideoPolicy.h"

using namespace BelledonneCommunications::Linphone::Native;

VideoPolicy::VideoPolicy()
	: automaticallyInitiate(true), automaticallyAccept(true)
{
}

VideoPolicy::VideoPolicy(bool automaticallyInitiate, bool automaticallyAccept)
	: automaticallyInitiate(automaticallyInitiate), automaticallyAccept(automaticallyAccept)
{
}

bool VideoPolicy::AutomaticallyInitiate::get()
{
	return automaticallyInitiate;
}

void VideoPolicy::AutomaticallyInitiate::set(bool value)
{
	API_LOCK;
	automaticallyInitiate = value;
}

bool VideoPolicy::AutomaticallyAccept::get()
{
	return automaticallyAccept;
}

void VideoPolicy::AutomaticallyAccept::set(bool value)
{
	API_LOCK;
	automaticallyAccept = value;
}