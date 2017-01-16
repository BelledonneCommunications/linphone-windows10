############################################################################
# CMakeLists.txt
# Copyright (C) 2016  Belledonne Communications, Grenoble France
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
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
#
############################################################################

cmake_minimum_required(VERSION 3.0)
project(NATIVE CXX)


find_package(Linphone REQUIRED)


include_directories(
	${LINPHONE_INCLUDE_DIRS}
)

set(SOURCE_FILES
	Address.cpp
	Address.h
	ApiLock.cpp
	ApiLock.h
	AuthInfo.cpp
	AuthInfo.h
	Call.cpp
	Call.h
	CallLog.cpp
	CallLog.h
	CallParams.cpp
	CallParams.h
	CallStats.cpp
	CallStats.h
	ChatMessage.cpp
	ChatMessage.h
	ChatMessageListener.h
	ChatRoom.cpp
	ChatRoom.h
	Config.cpp
	Config.h
	Core.cpp
	Core.h
	CoreListener.h
	Enums.h
	PayloadType.cpp
	PayloadType.h
	ProxyConfig.cpp
	ProxyConfig.h
	Transports.cpp
	Transports.h
	Tunnel.cpp
	Tunnel.h
	Utils.cpp
	Utils.h
	VideoPolicy.cpp
	VideoPolicy.h
	VideoSize.cpp
	VideoSize.h
	VoipCallController.cpp
	VoipCallController.h
)
set(LIBS ${LINPHONE_LIBRARIES})

add_library(linphone_native MODULE ${SOURCE_FILES})
set_target_properties(linphone_native PROPERTIES VERSION 0)
target_compile_options(linphone_native PRIVATE "/wd4996") # Ignore deprecated warnings
target_compile_options(linphone_native PRIVATE "/doc")
set_target_properties(linphone_native PROPERTIES LIBRARY_OUTPUT_NAME "BelledonneCommunications.Linphone.Native")
set_target_properties(linphone_native PROPERTIES LINK_FLAGS "/WINMDFILE:BelledonneCommunications.Linphone.Native.winmd")
set_target_properties(linphone_native PROPERTIES VS_WINRT_COMPONENT TRUE)
set_target_properties(linphone_native PROPERTIES VS_MOBILE_EXTENSIONS_VERSION ${CMAKE_VS_WINDOWS_TARGET_PLATFORM_VERSION}) # Include mobile extensions
target_link_libraries(linphone_native ${LIBS})

install(TARGETS linphone_native
	RUNTIME DESTINATION bin
	LIBRARY DESTINATION lib
	ARCHIVE DESTINATION lib
	PERMISSIONS OWNER_READ OWNER_WRITE OWNER_EXECUTE GROUP_READ GROUP_EXECUTE WORLD_READ WORLD_EXECUTE
)
install(FILES "${CMAKE_CURRENT_BINARY_DIR}/BelledonneCommunications.Linphone.Native.winmd" DESTINATION lib)
install(FILES "${CMAKE_CURRENT_BINARY_DIR}/${CMAKE_BUILD_TYPE}/BelledonneCommunications.Linphone.Native.xml" DESTINATION lib)
if(CMAKE_BUILD_TYPE STREQUAL "Debug")
	install(FILES "${CMAKE_CURRENT_BINARY_DIR}/Debug/BelledonneCommunications.Linphone.Native.pdb"
		DESTINATION lib
		PERMISSIONS OWNER_READ OWNER_WRITE OWNER_EXECUTE GROUP_READ GROUP_EXECUTE WORLD_READ WORLD_EXECUTE
	)
endif()
