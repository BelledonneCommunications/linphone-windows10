#include "connection.h"
#include "logger.h"
#include "ui_logger.h"

#include <QScrollBar>

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

    connect(&mServer, SIGNAL(newConnection(Connection*)),
            this, SLOT(notifyConnection(Connection*)));
    connect(&mServer, SIGNAL(disconnected()),
            this, SLOT(notifyDisconnection()));
    connect(&mServer, SIGNAL(newMessage(Connection::LogLevel, QString)),
            this, SLOT(appendMessage(Connection::LogLevel, QString)));
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

void Logger::appendMessage(Connection::LogLevel level, const QString &message)
{
    if (message.isEmpty())
        return;

    QTextCursor cursor(mUi->textEdit->textCursor());
    cursor.movePosition(QTextCursor::End);
    QColor color = mUi->textEdit->textColor();
    if (level == Connection::Debug) {
        mUi->textEdit->setTextColor(Qt::gray);
    } else if (level == Connection::Warning) {
        mUi->textEdit->setTextColor(Qt::darkYellow);
    } else if (level == Connection::Error) {
        mUi->textEdit->setTextColor(Qt::red);
    }
    mUi->textEdit->append(message);
    mUi->textEdit->setTextColor(color);
    QScrollBar *bar = mUi->textEdit->verticalScrollBar();
    bar->setValue(bar->maximum());
}
