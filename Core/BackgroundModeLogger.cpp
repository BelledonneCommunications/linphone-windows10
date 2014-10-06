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
