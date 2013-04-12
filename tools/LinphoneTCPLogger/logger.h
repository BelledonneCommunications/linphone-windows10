#ifndef LOGGER_H
#define LOGGER_H

#include <QMainWindow>

#include "server.h"

namespace Ui {
class Logger;
}

class Logger : public QMainWindow
{
    Q_OBJECT
    
public:
    explicit Logger(QWidget *parent = 0);
    ~Logger();

public slots:
    void notifyConnection(Connection *connection);
    void notifyDisconnection();
    void appendMessage(Connection::LogLevel level, const QString &message);
    
private:
    Ui::Logger *mUi;

    Server mServer;
};

#endif // LOGGER_H
