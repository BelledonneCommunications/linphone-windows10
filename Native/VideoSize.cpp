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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#include "ApiLock.h"
#include "VideoSize.h"

using namespace BelledonneCommunications::Linphone::Native;

VideoSize::VideoSize(int width, int height)
	: width(width), height(height), name("")
{
}

VideoSize::VideoSize(int width, int height, Platform::String^ name)
	: width(width), height(height), name(name)
{
}

int VideoSize::Width::get()
{
	return width;
}

void VideoSize::Width::set(int value)
{
	API_LOCK;
	width = value;
}

int VideoSize::Height::get()
{
	return height;
}

void VideoSize::Height::set(int value)
{
	API_LOCK;
	height = value;
}

Platform::String^ VideoSize::Name::get()
{
	return name;
}

void VideoSize::Name::set(Platform::String^ value)
{
	API_LOCK;
	name = value;
}