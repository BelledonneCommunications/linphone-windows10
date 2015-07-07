/*
VideoSize.cpp
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
#include "VideoSize.h"

Linphone::Native::VideoSize::VideoSize(int width, int height)
	: width(width), height(height), name("")
{
}

Linphone::Native::VideoSize::VideoSize(int width, int height, Platform::String^ name)
	: width(width), height(height), name(name)
{
}

int Linphone::Native::VideoSize::Width::get()
{
	return width;
}

void Linphone::Native::VideoSize::Width::set(int value)
{
	API_LOCK;
	width = value;
}

int Linphone::Native::VideoSize::Height::get()
{
	return height;
}

void Linphone::Native::VideoSize::Height::set(int value)
{
	API_LOCK;
	height = value;
}

Platform::String^ Linphone::Native::VideoSize::Name::get()
{
	return name;
}

void Linphone::Native::VideoSize::Name::set(Platform::String^ value)
{
	API_LOCK;
	name = value;
}