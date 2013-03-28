#include "BackgroundModeLogger.h"

#include <ppltasks.h>

using namespace concurrency;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Linphone::Core;


Linphone::Core::BackgroundModeLogger::BackgroundModeLogger()
{
}

Linphone::Core::BackgroundModeLogger::~BackgroundModeLogger()
{
}

void Linphone::Core::BackgroundModeLogger::OutputTrace(int level, Platform::String^ msg)
{
	if (!dataWriter) {
		StorageFolder^ localFolder = ApplicationData::Current->LocalFolder;
		task<StorageFile^>(localFolder->CreateFileAsync("Linphone.log", CreationCollisionOption::OpenIfExists)).then([this](StorageFile^ file) {
			storageFile = file;
			task<IRandomAccessStream^>(storageFile->OpenAsync(FileAccessMode::ReadWrite)).then([this](IRandomAccessStream^ stream) {
				dataWriter = ref new DataWriter(stream);
			});
		}).wait();
	}
	if (dataWriter) {
		dataWriter->WriteString(msg);
		task<unsigned int>(dataWriter->StoreAsync()).wait();
	}
}
