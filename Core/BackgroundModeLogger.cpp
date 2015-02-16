/*
BackgroundModeLogger.cpp
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

#include "BackgroundModeLogger.h"
#include "Utils.h"

#include <ppltasks.h>
#include <iostream>
#include <fstream>
#include <cstring>

using namespace concurrency;
using namespace Windows::Networking;
using namespace Windows::Networking::Sockets;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Linphone::Core;


Linphone::Core::BackgroundModeLogger::BackgroundModeLogger()
{
}

Linphone::Core::BackgroundModeLogger::~BackgroundModeLogger()
{
}

void Linphone::Core::BackgroundModeLogger::OutputTrace(OutputTraceLevel level, Platform::String^ msg)
{
	const char *cMsg = Utils::pstoccs(msg);
	switch (level) {
	case OutputTraceLevel::Debug:
		ms_debug(cMsg);
		break;
	case OutputTraceLevel::Message:
		ms_message(cMsg);
		break;
	case OutputTraceLevel::Warning:
		ms_warning(cMsg);
		break;
	case OutputTraceLevel::Error:
		ms_error(cMsg);
		break;
	}
}
