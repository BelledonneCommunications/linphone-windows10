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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

#include "ApiLock.h"
#include "VideoPolicy.h"

Linphone::Core::VideoPolicy::VideoPolicy() :
automaticallyInitiate(true), automaticallyAccept(true)
{
}

Linphone::Core::VideoPolicy::VideoPolicy(bool automaticallyInitiate, bool automaticallyAccept) :
automaticallyInitiate(automaticallyInitiate), automaticallyAccept(automaticallyAccept)
{
}

bool Linphone::Core::VideoPolicy::AutomaticallyInitiate::get()
{
	return automaticallyInitiate;
}

void Linphone::Core::VideoPolicy::AutomaticallyInitiate::set(bool value)
{
	API_LOCK;
	automaticallyInitiate = value;
}

bool Linphone::Core::VideoPolicy::AutomaticallyAccept::get()
{
	return automaticallyAccept;
}

void Linphone::Core::VideoPolicy::AutomaticallyAccept::set(bool value)
{
	API_LOCK;
	automaticallyAccept = value;
}