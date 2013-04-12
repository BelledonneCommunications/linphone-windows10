#ifndef SERVER_H
#define SERVER_H

#include <QTcpServer>

#include "connection.h"

class Server : public QTcpServer
{
    Q_OBJECT
public:
    explicit Server(QObject *parent = 0);
    
signals:
    void newConnection(Connection *connection);
    void disconnected();
    void newMessage(Connection::LogLevel level, const QString &message);
    
protected:
    void incomingConnection(qintptr socketDescriptor);

private:
    static const int Port = 38954;
};

#endif // SERVER_H
