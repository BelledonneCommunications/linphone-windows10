#-------------------------------------------------
#
# Project created by QtCreator 2013-04-06T17:51:40
#
#-------------------------------------------------

QT       += core gui network

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = LinphoneTCPLogger
TEMPLATE = app


SOURCES += main.cpp\
        logger.cpp \
    server.cpp \
    connection.cpp

HEADERS  += logger.h \
    server.h \
    connection.h

FORMS    += logger.ui

RESOURCES += \
    silk.qrc
