#include "BackgroundModeLogger.h"
#include "Utils.h"

#include <ppltasks.h>

using namespace concurrency;
using namespace Windows::Storage;
using namespace Linphone::Core;


Linphone::Core::BackgroundModeLogger::BackgroundModeLogger() :
	stream(nullptr), enabled(false), dest(OutputTraceDest::File), filename(nullptr)
{
}

Linphone::Core::BackgroundModeLogger::~BackgroundModeLogger()
{
}

void Linphone::Core::BackgroundModeLogger::Configure(bool enable, OutputTraceDest dest, Platform::String^ filename)
{
	std::lock_guard<std::recursive_mutex> lock(this->lock);
	this->enabled = enable;
	this->dest = dest;
	if ((dest == OutputTraceDest::File) && (filename != this->filename) && (this->stream != nullptr)) {
		stream->close();
		delete stream;
		stream = nullptr;
	}
	this->filename = filename;
}

void Linphone::Core::BackgroundModeLogger::OutputTrace(OutputTraceLevel level, Platform::String^ msg)
{
	if (this->enabled) {
		std::lock_guard<std::recursive_mutex> lock(this->lock);
		if (this->dest == OutputTraceDest::Debugger) {
			OutputDebugString(msg->Data());
		}
		else if (this->dest == OutputTraceDest::File) {
			if (this->stream == nullptr) {
				StorageFolder^ localFolder = ApplicationData::Current->LocalFolder;
				task<StorageFile^>(localFolder->CreateFileAsync(this->filename, CreationCollisionOption::ReplaceExisting)).then([this](StorageFile^ file) {
					this->stream = new std::ofstream(file->Path->Data());
				}).wait();
			}
			if (this->stream != nullptr) {
				*(this->stream) << Utils::pstoccs(msg);
				this->stream->flush();
			}
		}
	}
}
