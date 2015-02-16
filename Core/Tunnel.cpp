/*
Tunnel.cpp
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

#include "Tunnel.h"
#include "Server.h"
#include "Enums.h"
#include "ApiLock.h"
#include <collection.h>

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;


Linphone::Core::TunnelConfig::TunnelConfig(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay) :
	host(host),
	port(port),
	udpMirrorPort(udpMirrorPort),
	roundTripDelay(roundTripDelay)
{
}

Platform::String^ Linphone::Core::TunnelConfig::Host::get()
{
	return this->host;
}

void Linphone::Core::TunnelConfig::Host::set(Platform::String^ value)
{
	API_LOCK;
	this->host = value;
}

int Linphone::Core::TunnelConfig::Port::get()
{
	return this->port;
}

void Linphone::Core::TunnelConfig::Port::set(int value)
{
	API_LOCK;
	this->port = value;
}

int Linphone::Core::TunnelConfig::UdpMirrorPort::get()
{
	return this->udpMirrorPort;
}

void Linphone::Core::TunnelConfig::UdpMirrorPort::set(int value)
{
	API_LOCK;
	this->udpMirrorPort = value;
}

int Linphone::Core::TunnelConfig::RoundTripDelay::get()
{
	return this->roundTripDelay;
}

void Linphone::Core::TunnelConfig::RoundTripDelay::set(int value)
{
	API_LOCK;
	this->roundTripDelay = value;
}

Platform::String^ Linphone::Core::TunnelConfig::ToString()
{
	return "host[" + this->host + "] port[" + this->port + "] udpMirrorPort[" + this->udpMirrorPort + "] roundTripDelay[" + this->roundTripDelay + "]";
}



Linphone::Core::Tunnel::Tunnel(::LinphoneTunnel *tunnel) :
	lt(tunnel)
{
}

Linphone::Core::Tunnel::~Tunnel()
{
}


Platform::Boolean Linphone::Core::Tunnel::IsEnabled()
{
	API_LOCK;
	return (linphone_tunnel_enabled(this->lt) == TRUE);
}

void Linphone::Core::Tunnel::Enable(Platform::Boolean enable)
{
	API_LOCK;
	linphone_tunnel_enable(this->lt, enable);
}

void Linphone::Core::Tunnel::AutoDetect()
{
	API_LOCK;
	linphone_tunnel_auto_detect(this->lt);
}

static void AddServerConfigToVector(void *vServerConfig, void *vector)
{
	::LinphoneTunnelConfig *pc = (LinphoneTunnelConfig *)vServerConfig;
	Linphone::Core::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Core::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ serverconfigs = (list) ? list->Ref() : nullptr;

	const char *chost = linphone_tunnel_config_get_host(pc);
	int port = linphone_tunnel_config_get_port(pc);
	int udpMirrorPort = linphone_tunnel_config_get_remote_udp_mirror_port(pc);
	int roundTripDelay = linphone_tunnel_config_get_delay(pc);
	Linphone::Core::TunnelConfig^ serverConfig = ref new Linphone::Core::TunnelConfig(Linphone::Core::Utils::cctops(chost), port, udpMirrorPort, roundTripDelay);
	serverconfigs->Append(serverConfig);
}

IVector<Object^>^ Linphone::Core::Tunnel::GetServers()
{
	API_LOCK;
	IVector<Object^>^ serverconfigs = ref new Vector<Object^>();
	const MSList *configList = linphone_tunnel_get_servers(this->lt);
	RefToPtrProxy<IVector<Object^>^> *serverConfigPtr = new RefToPtrProxy<IVector<Object^>^>(serverconfigs);
	ms_list_for_each2(configList, AddServerConfigToVector, serverConfigPtr);
	return serverconfigs;
}

void Linphone::Core::Tunnel::CleanServers()
{
	API_LOCK;
	linphone_tunnel_clean_servers(this->lt);
}

void Linphone::Core::Tunnel::SetHttpProxy(Platform::String^ host, int port, Platform::String^ username, Platform::String^ password)
{
	API_LOCK;
	const char* h = Linphone::Core::Utils::pstoccs(host);
	const char* u = Linphone::Core::Utils::pstoccs(username);
	const char* pwd = Linphone::Core::Utils::pstoccs(password);
	linphone_tunnel_set_http_proxy(this->lt, h, port, u, pwd);
	delete(h);
	delete(u);
	delete(pwd);
}

void Linphone::Core::Tunnel::AddServer(Platform::String^ host, int port)
{
	API_LOCK;
	const char* h = Linphone::Core::Utils::pstoccs(host);
	LinphoneTunnelConfig* config = linphone_tunnel_config_new();
	linphone_tunnel_config_set_host(config, h);
	linphone_tunnel_config_set_port(config, port);
	linphone_tunnel_add_server(this->lt, config);
	delete(h);
}

void Linphone::Core::Tunnel::AddServer(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay)
{
	API_LOCK;
	const char* h = Linphone::Core::Utils::pstoccs(host);
	LinphoneTunnelConfig* config = linphone_tunnel_config_new();
	linphone_tunnel_config_set_host(config, h);
	linphone_tunnel_config_set_port(config, port);
	linphone_tunnel_config_set_delay(config, roundTripDelay);
	linphone_tunnel_config_set_remote_udp_mirror_port(config, udpMirrorPort);
	linphone_tunnel_add_server(this->lt, config);
	delete(h);
}
