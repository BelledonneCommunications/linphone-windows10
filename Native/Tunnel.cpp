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
#include "Enums.h"
#include "ApiLock.h"
#include <collection.h>

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;


Linphone::Native::TunnelConfig::TunnelConfig(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay)
	: host(host), port(port), udpMirrorPort(udpMirrorPort), roundTripDelay(roundTripDelay)
{
}

Platform::String^ Linphone::Native::TunnelConfig::Host::get()
{
	return this->host;
}

void Linphone::Native::TunnelConfig::Host::set(Platform::String^ value)
{
	API_LOCK;
	this->host = value;
}

int Linphone::Native::TunnelConfig::Port::get()
{
	return this->port;
}

void Linphone::Native::TunnelConfig::Port::set(int value)
{
	API_LOCK;
	this->port = value;
}

int Linphone::Native::TunnelConfig::RoundTripDelay::get()
{
	return this->roundTripDelay;
}

void Linphone::Native::TunnelConfig::RoundTripDelay::set(int value)
{
	API_LOCK;
	this->roundTripDelay = value;
}

int Linphone::Native::TunnelConfig::UdpMirrorPort::get()
{
	return this->udpMirrorPort;
}

void Linphone::Native::TunnelConfig::UdpMirrorPort::set(int value)
{
	API_LOCK;
	this->udpMirrorPort = value;
}

Platform::String^ Linphone::Native::TunnelConfig::ToString()
{
	return "host[" + this->host + "] port[" + this->port + "] udpMirrorPort[" + this->udpMirrorPort + "] roundTripDelay[" + this->roundTripDelay + "]";
}



static void AddServerConfigToVector(void *vServerConfig, void *vector)
{
	::LinphoneTunnelConfig *pc = (LinphoneTunnelConfig *)vServerConfig;
	Linphone::Native::RefToPtrProxy<IVector<Object^>^> *list = reinterpret_cast< Linphone::Native::RefToPtrProxy<IVector<Object^>^> *>(vector);
	IVector<Object^>^ serverconfigs = (list) ? list->Ref() : nullptr;

	const char *chost = linphone_tunnel_config_get_host(pc);
	int port = linphone_tunnel_config_get_port(pc);
	int udpMirrorPort = linphone_tunnel_config_get_remote_udp_mirror_port(pc);
	int roundTripDelay = linphone_tunnel_config_get_delay(pc);
	Linphone::Native::TunnelConfig^ serverConfig = ref new Linphone::Native::TunnelConfig(Linphone::Native::Utils::cctops(chost), port, udpMirrorPort, roundTripDelay);
	serverconfigs->Append(serverConfig);
}

Linphone::Native::Tunnel::Tunnel(::LinphoneTunnel *tunnel)
	: lt(tunnel)
{
}

Linphone::Native::Tunnel::~Tunnel()
{
}

Platform::Boolean Linphone::Native::Tunnel::Enabled::get()
{
	API_LOCK;
	return (linphone_tunnel_enabled(this->lt) == TRUE);
}

void Linphone::Native::Tunnel::Enabled::set(Platform::Boolean enable)
{
	API_LOCK;
	linphone_tunnel_enable(this->lt, enable);
}

void Linphone::Native::Tunnel::AddServer(Platform::String^ host, int port)
{
	API_LOCK;
	const char* h = Linphone::Native::Utils::pstoccs(host);
	LinphoneTunnelConfig* config = linphone_tunnel_config_new();
	linphone_tunnel_config_set_host(config, h);
	linphone_tunnel_config_set_port(config, port);
	linphone_tunnel_add_server(this->lt, config);
	delete(h);
}

void Linphone::Native::Tunnel::AddServer(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay)
{
	API_LOCK;
	const char* h = Linphone::Native::Utils::pstoccs(host);
	LinphoneTunnelConfig* config = linphone_tunnel_config_new();
	linphone_tunnel_config_set_host(config, h);
	linphone_tunnel_config_set_port(config, port);
	linphone_tunnel_config_set_delay(config, roundTripDelay);
	linphone_tunnel_config_set_remote_udp_mirror_port(config, udpMirrorPort);
	linphone_tunnel_add_server(this->lt, config);
	delete(h);
}

void Linphone::Native::Tunnel::AutoDetect()
{
	API_LOCK;
	linphone_tunnel_auto_detect(this->lt);
}

void Linphone::Native::Tunnel::CleanServers()
{
	API_LOCK;
	linphone_tunnel_clean_servers(this->lt);
}

IVector<Object^>^ Linphone::Native::Tunnel::GetServers()
{
	API_LOCK;
	IVector<Object^>^ serverconfigs = ref new Vector<Object^>();
	const MSList *configList = linphone_tunnel_get_servers(this->lt);
	RefToPtrProxy<IVector<Object^>^> *serverConfigPtr = new RefToPtrProxy<IVector<Object^>^>(serverconfigs);
	ms_list_for_each2(configList, AddServerConfigToVector, serverConfigPtr);
	return serverconfigs;
}

void Linphone::Native::Tunnel::SetHttpProxy(Platform::String^ host, int port, Platform::String^ username, Platform::String^ password)
{
	API_LOCK;
	const char* h = Linphone::Native::Utils::pstoccs(host);
	const char* u = Linphone::Native::Utils::pstoccs(username);
	const char* pwd = Linphone::Native::Utils::pstoccs(password);
	linphone_tunnel_set_http_proxy(this->lt, h, port, u, pwd);
	delete(h);
	delete(u);
	delete(pwd);
}
