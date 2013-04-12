#include "connection.h"

Connection::Connection(QObject *parent) :
    QTcpSocket(parent), mState(ReadingHeader)
{
    connect(this, SIGNAL(connected()),
            this, SLOT(notifyConnectionActive()));
    connect(this, SIGNAL(readyRead()),
            this, SLOT(processReadyRead()));
}

void Connection::notifyConnectionActive()
{
}

void Connection::processReadyRead()
{
    do {
        if (!hasEnoughData())
            return;
        processData();
    } while (bytesAvailable() > 0);
}

bool Connection::hasEnoughData()
{
    if (mState == ReadingHeader) {
        return (bytesAvailable() >= HeaderSize);
    } else if (mState == ReadingMessage) {
        return (bytesAvailable() >= mMessageLength);
    }
    return false;
}

void Connection::processData()
{
    if (mState == ReadingHeader) {
        mBuffer = read(HeaderSize);
        if (mBuffer.size() != HeaderSize) {
            abort();
            return;
        }

        mLogLevel = static_cast<LogLevel>(mBuffer.at(0));
        mMessageLength = ((static_cast<unsigned int>(mBuffer[1]) & 0xFF) << 8)
                | (static_cast<unsigned int>(mBuffer[2]) & 0xFF);
        mBuffer.clear();
        mState = ReadingMessage;
    }

    if (mState == ReadingMessage) {
        if (hasEnoughData()) {
            mBuffer = read(mMessageLength);
            emit newMessage(mLogLevel, QString::fromUtf8(mBuffer));
            mBuffer.clear();
            mMessageLength = 0;
            mState = ReadingHeader;
        }
    }
}
