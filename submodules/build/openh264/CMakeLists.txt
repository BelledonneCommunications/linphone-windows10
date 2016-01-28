############################################################################
# CMakeLists.txt
# Copyright (C) 2015  Belledonne Communications, Grenoble France
#
############################################################################
#
# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
#
############################################################################

cmake_minimum_required(VERSION 2.8)
project(OPENH264 C)

set(HEADER_FILES
	include/wels/codec_api.h
	include/wels/codec_app_def.h
	include/wels/codec_def.h
	include/wels/codec_ver.h
)

install(FILES ${HEADER_FILES}
	DESTINATION include/wels
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)

set(arch "x86")
if(CMAKE_GENERATOR MATCHES "^.* ARM$")
	set(arch "ARM")
elseif(CMAKE_GENERATOR MATCHES "^.* Win64$")
	set(arch "x64")
endif()

install(FILES ${arch}/openh264.dll
	DESTINATION bin
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)
install(FILES ${arch}/openh264_dll.lib
	DESTINATION lib
	PERMISSIONS OWNER_READ OWNER_WRITE GROUP_READ WORLD_READ
)
