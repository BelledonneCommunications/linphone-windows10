#include "connection.h"
#include "logger.h"
#include "ui_logger.h"

#include <QScrollBar>
#include <QTextTable>

Logger::Logger(QWidget *parent) :
    QMainWindow(parent),
    mUi(new Ui::Logger)
{
    static const char * GENERIC_ICON_TO_CHECK = "document-open";
    static const char * FALLBACK_ICON_THEME = "silk";
    if (!QIcon::hasThemeIcon(GENERIC_ICON_TO_CHECK)) {
        //If there is no default working icon theme then we should
        //use an icon theme that we provide via a .qrc file
        //This case happens under Windows and Mac OS X
        //This does not happen under GNOME or KDE
        QIcon::setThemeName(FALLBACK_ICON_THEME);
    }
    mUi->setupUi(this);

    mUi->textEdit->setReadOnly(true);
    mTableFormat.setBorder(0);

    connect(&mServer, SIGNAL(newConnection(Connection*)),
            this, SLOT(notifyConnection(Connection*)));
    connect(&mServer, SIGNAL(disconnected()),
            this, SLOT(notifyDisconnection()));
    connect(&mServer, SIGNAL(newMessage(quint64, Connection::LogLevel, QString)),
            this, SLOT(appendMessage(quint64, Connection::LogLevel, QString)));
}

Logger::~Logger()
{
    delete mUi;
}

void Logger::notifyConnection(Connection *connection)
{
    mUi->statusBar->showMessage(QString("Connected to %1").arg(connection->peerAddress().toString()));
}

void Logger::notifyDisconnection()
{
    mUi->statusBar->showMessage(QString("Disconnected"), 5000);
}

void Logger::appendMessage(quint64 time, Connection::LogLevel level, const QString &message)
{
    if (message.isEmpty())
        return;

    QScrollBar *bar = mUi->textEdit->verticalScrollBar();
    int barpos = bar->value();
    int barmax = bar->maximum();
    QTextCursor cursor(mUi->textEdit->textCursor());
    cursor.movePosition(QTextCursor::End);
    QTextTable *table = cursor.insertTable(1, 2, mTableFormat);
    QTextCharFormat format;
    format.setForeground(mUi->textEdit->textColor());
    if (level == Connection::Debug) {
        format.setForeground(Qt::gray);
    } else if (level == Connection::Warning) {
        format.setForeground(Qt::darkYellow);
    } else if (level == Connection::Error) {
        format.setForeground(Qt::red);
    }
    table->cellAt(0, 0).firstCursorPosition().insertText('[' + QString("%1.%2").arg(time / 1000).arg(time % 1000) + "] ");
    table->cellAt(0, 1).firstCursorPosition().insertText(message, format);
    if (barpos == barmax) {
        bar->setValue(bar->maximum());
    } else {
        bar->setValue(barpos);
    }
}
