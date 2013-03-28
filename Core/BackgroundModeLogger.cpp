#include "BackgroundModeLogger.h"
#include "Utils.h"

#include <ppltasks.h>

using namespace concurrency;
using namespace Windows::Storage;
using namespace Linphone::Core;


Linphone::Core::BackgroundModeLogger::BackgroundModeLogger() :
	stream(nullptr)
{
}

Linphone::Core::BackgroundModeLogger::~BackgroundModeLogger()
{
	if (stream) {
		stream->close();
		delete stream;
		stream = nullptr;
	}
}

void Linphone::Core::BackgroundModeLogger::OutputTrace(OutputTraceLevel level, Platform::String^ msg)
{
	std::lock_guard<std::recursive_mutex> lock(lock);
	if (!stream) {
		StorageFolder^ localFolder = ApplicationData::Current->LocalFolder;
		task<StorageFile^>(localFolder->CreateFileAsync("Linphone.log", CreationCollisionOption::ReplaceExisting)).then([this](StorageFile^ file) {
			stream = new std::ofstream(file->Path->Data());
		}).wait();
	}
	if (stream) {
		*stream << Utils::pstoccs(msg);
		stream->flush();
	}
}
