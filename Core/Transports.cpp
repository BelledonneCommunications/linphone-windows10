#include "Transports.h"

Linphone::Core::Transports::Transports() :
udp(5060),
tcp(0),
tls(0)
{
}

Linphone::Core::Transports::Transports(int udp_port, int tcp_port, int tls_port) :
udp(udp_port),
tcp(tcp_port),
tls(tls_port)
{
}

Linphone::Core::Transports::Transports(Linphone::Core::Transports^ t) :
udp(t->UDP),
tcp(t->TCP),
tls(t->TLS)
{
}

int Linphone::Core::Transports::UDP::get()
{
	return udp;
}

void Linphone::Core::Transports::UDP::set(int value)
{
	this->udp = value;
	this->tcp = 0;
	this->tls = 0;
}

int Linphone::Core::Transports::TCP::get()
{
	return tcp;
}

void Linphone::Core::Transports::TCP::set(int value)
{
	this->udp = 0;
	this->tcp = value;
	this->tls = 0;
}

int Linphone::Core::Transports::TLS::get()
{
	return tls;
}

void Linphone::Core::Transports::TLS::set(int value)
{
	this->udp = 0;
	this->tcp = 0;
	this->tls = value;
}

Platform::String^ Linphone::Core::Transports::ToString()
{
	return "udp[" + udp + "] tcp[" + tcp + "] tls[" + tls + "]";
}
