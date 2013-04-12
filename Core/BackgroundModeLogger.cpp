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


namespace Linphone
{
	namespace Core
	{
		private class BackgroundModeLoggerPrivate
		{
		private:
			friend ref class BackgroundModeLogger;

			BackgroundModeLoggerPrivate() : stream(nullptr) {}
			~BackgroundModeLoggerPrivate() {}

			std::ofstream *stream;
			StreamSocket^ streamSocket;
			DataWriter^ dataWriter;
			Platform::String^ remoteHost;
			Platform::String^ remotePort;
		};
	}
}



Linphone::Core::BackgroundModeLogger::BackgroundModeLogger() :
	enabled(false), dest(OutputTraceDest::File), filename(nullptr), d(new BackgroundModeLoggerPrivate())
{
}

Linphone::Core::BackgroundModeLogger::~BackgroundModeLogger()
{
	delete d;
}

void Linphone::Core::BackgroundModeLogger::Configure(bool enable, OutputTraceDest dest, Platform::String^ option)
{
	std::lock_guard<std::recursive_mutex> lock(this->lock);
	if ((this->dest == OutputTraceDest::File) && (this->d->stream != nullptr)) {
		this->d->stream->close();
		delete this->d->stream;
		this->d->stream = nullptr;
	}
	if ((this->dest == OutputTraceDest::TCPRemote) && (this->d->dataWriter != nullptr)) {
		this->d->dataWriter = nullptr;
		this->d->streamSocket = nullptr;
	}
	this->enabled = enable;
	this->dest = dest;
	if (dest == OutputTraceDest::File) {
		this->filename = option;
	}
	else if (dest == OutputTraceDest::TCPRemote) {
		char buffer[256];
		const char *cOption = Utils::pstoccs(option);
		strncpy(buffer, cOption, sizeof(buffer));
		char *portPtr = strrchr(buffer, ':');
		if (portPtr != nullptr) {
			*portPtr = '\0';
			portPtr += 1;
			this->d->remoteHost = Utils::cctops(buffer);
			this->d->remotePort = Utils::cctops(portPtr);
		}
		delete cOption;
	}
}

void Linphone::Core::BackgroundModeLogger::OutputTrace(OutputTraceLevel level, Platform::String^ msg)
{
	if (this->enabled) {
		std::lock_guard<std::recursive_mutex> lock(this->lock);
		if (this->dest == OutputTraceDest::Debugger) {
			OutputDebugString(msg->Data());
		}
		else if (this->dest == OutputTraceDest::File) {
			if (this->d->stream == nullptr) {
				StorageFolder^ localFolder = ApplicationData::Current->LocalFolder;
				task<StorageFile^>(localFolder->CreateFileAsync(this->filename, CreationCollisionOption::OpenIfExists)).then([this](StorageFile^ file) {
					this->d->stream = new std::ofstream(file->Path->Data(), std::ios_base::out | std::ios_base::ate | std::ios_base::app);
				}).wait();
			}
			if (this->d->stream != nullptr) {
				const char *cMsg = Utils::pstoccs(msg);
				*(this->d->stream) << cMsg;
				this->d->stream->flush();
				delete cMsg;
			}
		}
		else if (this->dest == OutputTraceDest::TCPRemote) {
			if (this->d->streamSocket == nullptr) {
				this->d->streamSocket = ref new StreamSocket();
				HostName^ remoteHost = ref new HostName(this->d->remoteHost);
				EndpointPair^ ep = ref new EndpointPair(nullptr, "", remoteHost, this->d->remotePort);
				task<void>(this->d->streamSocket->ConnectAsync(ep)).then([this]() {
					this->d->dataWriter = ref new DataWriter(this->d->streamSocket->OutputStream);
					this->d->dataWriter->ByteOrder = ByteOrder::BigEndian;
				}).wait();
				
			}
			if (this->d->dataWriter != nullptr) {
				this->d->dataWriter->WriteByte(static_cast<unsigned char>(level));
				this->d->dataWriter->WriteUInt16(msg->Length());
				this->d->dataWriter->WriteString(msg);
				DataWriterStoreOperation^ op = this->d->dataWriter->StoreAsync();
				create_task(op).then([this](task<unsigned int> written) {
					return this->d->dataWriter->FlushAsync();
				}).wait();
			}
		}
	}
}
