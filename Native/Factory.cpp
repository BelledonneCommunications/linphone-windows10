/*
Factory.cpp
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
#include "Factory.h"

using namespace BelledonneCommunications::Linphone::Native;
using namespace Platform;



Platform::String^ Factory::TopResourcesDir::get()
{
	API_LOCK;
	return Utils::cctops(linphone_factory_get_top_resources_dir(linphone_factory_get()));
}

void Factory::TopResourcesDir::set(Platform::String^ value)
{
	API_LOCK;
	const char *cvalue = Utils::pstoccs(value);
	linphone_factory_set_top_resources_dir(linphone_factory_get(), cvalue);
	delete(cvalue);
}

Platform::String^ Factory::DataResourcesDir::get()
{
	API_LOCK;
	return Utils::cctops(linphone_factory_get_data_resources_dir(linphone_factory_get()));
}

void Factory::DataResourcesDir::set(Platform::String^ value)
{
	API_LOCK;
	const char *cvalue = Utils::pstoccs(value);
	linphone_factory_set_data_resources_dir(linphone_factory_get(), cvalue);
	delete(cvalue);
}

Platform::String^ Factory::SoundResourcesDir::get()
{
	API_LOCK;
	return Utils::cctops(linphone_factory_get_sound_resources_dir(linphone_factory_get()));
}

void Factory::SoundResourcesDir::set(Platform::String^ value)
{
	API_LOCK;
	const char *cvalue = Utils::pstoccs(value);
	linphone_factory_set_sound_resources_dir(linphone_factory_get(), cvalue);
	delete(cvalue);
}

Platform::String^ Factory::RingResourcesDir::get()
{
	API_LOCK;
	return Utils::cctops(linphone_factory_get_ring_resources_dir(linphone_factory_get()));
}

void Factory::RingResourcesDir::set(Platform::String^ value)
{
	API_LOCK;
	const char *cvalue = Utils::pstoccs(value);
	linphone_factory_set_ring_resources_dir(linphone_factory_get(), cvalue);
	delete(cvalue);
}

Platform::String^ Factory::ImageResourcesDir::get()
{
	API_LOCK;
	return Utils::cctops(linphone_factory_get_image_resources_dir(linphone_factory_get()));
}

void Factory::ImageResourcesDir::set(Platform::String^ value)
{
	API_LOCK;
	const char *cvalue = Utils::pstoccs(value);
	linphone_factory_set_image_resources_dir(linphone_factory_get(), cvalue);
	delete(cvalue);
}

Platform::String^ Factory::MspluginsDir::get()
{
	API_LOCK;
	return Utils::cctops(linphone_factory_get_msplugins_dir(linphone_factory_get()));
}

void Factory::MspluginsDir::set(Platform::String^ value)
{
	API_LOCK;
	const char *cvalue = Utils::pstoccs(value);
	linphone_factory_set_msplugins_dir(linphone_factory_get(), cvalue);
	delete(cvalue);
}
