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
	namespace Core
	{
		ref class LinphoneCore;

		/// <summary>
		/// Tunnel server configuration.
		/// </summary>
		public ref class TunnelConfig sealed
		{
		public:
			TunnelConfig(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay);
			Platform::String^ ToString();

			property Platform::String^ Host
            {
                Platform::String^ get();
				void set(Platform::String^ value);
            }
			property int Port
			{
				int get();
				void set(int value);
			}
			property int UdpMirrorPort
			{
				int get();
				void set(int value);
			}
			property int RoundTripDelay
			{
				int get();
				void set(int value);
			}

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
			/// Tells whether the tunnel is enabled or not.
			/// </summary>
			bool IsEnabled();

			/// <summary>
			/// Enable/disable the tunnel.
			/// </summary>
			void Enable(Platform::Boolean enable);

			/// <summary>
			/// Start tunnel need detection.
			/// </summary>
			void AutoDetect();

			/// <summary>
			/// Returns an IList&lt;Object&gt; where each object is a TunnelConfig.
			/// </summary>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetServers();

			/// <summary>
			/// Removes all tunnel server addresses previously entered with AddServer().
			/// </summary>
			void CleanServers();

			/// <summary>
			/// Set an optional http proxy to go through when connecting to tunnel server.
			/// <param name="host">Http proxy host</param>
			/// <param name="port">Http proxy port</param>
			/// <param name="username">Optional http proxy username if the proxy request authentication. Currently only basic authentication is supported. Use null if not needed.</param>
			/// <param name="password">Optional http proxy password. Use null if not needed.</param>
			/// </summary>
			void SetHttpProxy(Platform::String^ host, int port, Platform::String^ username, Platform::String^ password);

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

		private:
			friend ref class Linphone::Core::LinphoneCore;

			Tunnel(::LinphoneTunnel *tunnel);
			~Tunnel();

			::LinphoneTunnel *lt;
		};
	}
}