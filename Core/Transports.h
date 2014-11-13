#pragma once

#include "Utils.h"

namespace Linphone
{
    namespace Core
	{
		ref class LinphoneCoreFactory;
		ref class LinphoneCore;

		/// <summary>
		/// Signaling transports ports
		/// </summary>
		public ref class Transports sealed
		{
		public:
			/// <summary>
			/// Gets a string representation of the Transports object.
			/// </summary>
			/// <returns>A string representation of the Transports object</returns>
			Platform::String^ ToString();

			/// <summary>
			/// UDP port of the Transports object.
			/// </summary>
			property int UDP
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// TCP port of the Transports object.
			/// </summary>
			property int TCP
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// TLS port of the Transports object.
			/// </summary>
			property int TLS
			{
				int get();
				void set(int value);
			}
		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCoreFactory;
			friend ref class Linphone::Core::LinphoneCore;

			Transports();
			Transports(int udp_port, int tcp_port, int tls_port);
			Transports(Transports^ t);

			int udp;
			int tcp;
			int tls;
		};
	}
}