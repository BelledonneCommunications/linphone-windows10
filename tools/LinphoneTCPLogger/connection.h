#ifndef CONNECTION_H
#define CONNECTION_H

#include <QTcpSocket>

static const int HeaderLogTimeSize = 8;
static const int HeaderLogLevelSize = 1;
static const int HeaderMessageLengthSize = 2;
static const int HeaderSize = HeaderLogTimeSize + HeaderLogLevelSize + HeaderMessageLengthSize;

class Connection : public QTcpSocket
{
    Q_OBJECT
public:
    enum LogLevel {
        Debug,
        Message,
        Warning,
        Error
    };
    enum ParseState {
        ReadingHeader,
        ReadingMessage
    };

    explicit Connection(QObject *parent = 0);
    
signals:
    void newMessage(quint64 time, Connection::LogLevel level, const QString &message);
    
private slots:
    void processReadyRead();
    void notifyConnectionActive();

private:
    bool hasEnoughData();
    void processData();

    QByteArray mBuffer;
    ParseState mState;
    quint64 mLogTime;
    LogLevel mLogLevel;
    unsigned int mMessageLength;
};

#endif // CONNECTION_H
