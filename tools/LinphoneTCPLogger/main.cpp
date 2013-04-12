#include "logger.h"
#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication app(argc, argv);
    Logger l;
    l.show();
    
    return app.exec();
}
