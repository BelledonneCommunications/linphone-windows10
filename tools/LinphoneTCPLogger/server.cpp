#include "connection.h"
#include "server.h"

Server::Server(QObject *parent) :
    QTcpServer(parent)
{
    listen(QHostAddress::Any, Port);
}

void Server::incomingConnection(qintptr socketDescriptor)
{
    Connection *connection = new Connection(this);
    connection->setSocketDescriptor(socketDescriptor);
    connect(connection, SIGNAL(disconnected()), this, SIGNAL(disconnected()));
    connect(connection, SIGNAL(newMessage(Connection::LogLevel, QString)),
            this, SIGNAL(newMessage(Connection::LogLevel, QString)));
    emit newConnection(connection);
}
