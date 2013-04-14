#ifndef LOGGER_H
#define LOGGER_H

#include <QMainWindow>
#include <QTextTableFormat>

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
    void appendMessage(quint64 time, Connection::LogLevel level, const QString &message);
    
private:
    Ui::Logger *mUi;

    Server mServer;
    QTextTableFormat mTableFormat;
};

#endif // LOGGER_H
