#include "connection.h"
#include <qendian.h>

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

        memcpy(&mLogTime, mBuffer.constData(), 8);
#ifdef Q_LITTLE_ENDIAN
        //Convert to LittleEndian
        mLogTime = qbswap(mLogTime);
#endif
        mLogLevel = static_cast<LogLevel>(static_cast<unsigned int>(mBuffer[8]));
        mMessageLength = ((static_cast<unsigned int>(mBuffer[9]) & 0xFF) << 8)
                | (static_cast<unsigned int>(mBuffer[10]) & 0xFF);
        mBuffer.clear();
        mState = ReadingMessage;
    }

    if (mState == ReadingMessage) {
        if (hasEnoughData()) {
            mBuffer = read(mMessageLength);
            emit newMessage(mLogTime, mLogLevel, QString::fromUtf8(mBuffer));
            mBuffer.clear();
            mMessageLength = 0;
            mState = ReadingHeader;
        }
    }
}
