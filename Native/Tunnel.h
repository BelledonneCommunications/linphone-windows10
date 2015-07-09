/*
Tunnel.h
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

#pragma once

#include "Enums.h"
#include "Utils.h"


// Do not treat doxygen documentation as XML
#pragma warning(push)
#pragma warning(disable : 4635)
#include "coreapi\linphone_tunnel.h"
#pragma warning(pop)

namespace Linphone
{
	namespace Native
	{
		ref class Core;

		/// <summary>
		/// Tunnel server configuration.
		/// </summary>
		public ref class TunnelConfig sealed
		{
		public:
			/// <summary>
			/// Constructs a TunnelConfig.
			/// </summary>
			/// <param name="host">The tunnel server host</param>
			/// <param name="port">The tunnel server port</param>
			/// <param name="udpMirrorPort">The tunnel server UDP mirror port</param>
			/// <param name="roundTripDelay">The round trip delay</param>
			TunnelConfig(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay);

			/// <summary>
			/// The tunnel server host.
			/// </summary>
			property Platform::String^ Host
            {
                Platform::String^ get();
				void set(Platform::String^ value);
            }

			/// <summary>
			/// The tunnel server port.
			/// </summary>
			property int Port
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// The round trip delay.
			/// </summary>
			property int RoundTripDelay
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// The tunnel server UDP mirror port.
			/// </summary>
			property int UdpMirrorPort
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Gets a string representation of the TunnelConfig.
			/// </summary>
			/// <returns>The string representation of the tunnel config</returns>
			virtual Platform::String^ ToString() override;

		private:
			Platform::String^ host;
			int port;
			int udpMirrorPort;
			int roundTripDelay;
		};


		/// <summary>
		/// Main object obtained by LinphoneCore.GetTunnel()
		/// </summary>
		public ref class Tunnel sealed
		{
		public:
			/// <summary>
			/// Enables the tunnel.
			/// </summary>
			property bool IsEnabled
			{
				bool get();
				void set(bool value);
			}

			/// <summary>
			/// Add a server to the tunnel configuration
			/// </summary>
			/// <param name="host">Tunnel server IP address</param>
			/// <param name="port">Tunnel server TLS port, recommended value is 443</param>
			void AddServer(Platform::String^ host, int port);

			/// <summary>
			/// Add a server to the tunnel configuration
			/// </summary>
			/// <param name="host">Tunnel server IP address</param>
			/// <param name="port">Tunnel server TLS port, recommended value is 443</param>
			/// <param name="udpMirrorPort">Remote port on the tunnel server side used to test UDP reachability</param>
			/// <param name="roundTripDelay">UDP packet round trip delay in ms considered as acceptable. Recommended value is 1000 ms.</param>
			void AddServer(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay);

			/// <summary>
			/// Start tunnel need detection.
			/// </summary>
			void AutoDetect();

			/// <summary>
			/// Removes all tunnel server addresses previously entered with AddServer().
			/// </summary>
			void CleanServers();

			/// <summary>
			/// Returns an IList&lt;Object&gt; where each object is a TunnelConfig.
			/// </summary>
			/// <returns>A list of TunnelConfig</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetServers();

			/// <summary>
			/// Set an optional http proxy to go through when connecting to tunnel server.
			/// </summary>
			/// <param name="host">Http proxy host</param>
			/// <param name="port">Http proxy port</param>
			/// <param name="username">Optional http proxy username if the proxy request authentication. Currently only basic authentication is supported. Use null if not needed.</param>
			/// <param name="password">Optional http proxy password. Use null if not needed.</param>
			void SetHttpProxy(Platform::String^ host, int port, Platform::String^ username, Platform::String^ password);

		private:
			friend ref class Linphone::Native::Core;

			Tunnel(::LinphoneTunnel *tunnel);
			~Tunnel();

			::LinphoneTunnel *lt;
		};
	}
}