#include "VideoSize.h"

Linphone::Core::VideoSize::VideoSize(int width, int height) :
width(width), height(height), name("")
{
}

Linphone::Core::VideoSize::VideoSize(int width, int height, Platform::String^ name) :
width(width), height(height), name(name)
{
}

int Linphone::Core::VideoSize::Width::get()
{
	return width;
}

void Linphone::Core::VideoSize::Width::set(int value)
{
	width = value;
}

int Linphone::Core::VideoSize::Height::get()
{
	return height;
}

void Linphone::Core::VideoSize::Height::set(int value)
{
	height = value;
}

Platform::String^ Linphone::Core::VideoSize::Name::get()
{
	return name;
}

void Linphone::Core::VideoSize::Name::set(Platform::String^ value)
{
	name = value;
}