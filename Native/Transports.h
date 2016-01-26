/*
Transports.h
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

#include "Utils.h"

namespace BelledonneCommunications
{
	namespace Linphone
	{
		namespace Native
		{
			ref class LinphoneCore;

			/// <summary>
			/// Signaling transports ports
			/// </summary>
			public ref class Transports sealed
			{
			public:
				/// <summary>
				/// Creates a default Transports object (using the UDP 5060 port).
				/// </summary>
				/// <returns>The created Transports</returns>
				Transports();

				/// <summary>
				/// Creates a Transports object specifying the ports to use.
				/// </summary>
				/// <param name="udp_port">The UDP port to use (0 to disable)</param>
				/// <param name="tcp_port">The TCP port to use (0 to disable)</param>
				/// <param name="tls_port">The TLS port to use (0 to disable)</param>
				/// <returns>The created Transports</returns>
				Transports(int udp_port, int tcp_port, int tls_port);

				/// <summary>
				/// Clones a Transports object.
				/// </summary>
				/// <param name="t">The Transports object to clone</param>
				/// <returns>The cloned Transports</returns>
				Transports(Transports^ t);

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

				/// <summary>
				/// Gets a string representation of the Transports object.
				/// </summary>
				/// <returns>A string representation of the Transports object</returns>
				virtual Platform::String^ ToString() override;

			private:
				friend class Utils;
				friend ref class LinphoneCore;

				int udp;
				int tcp;
				int tls;
			};
		}
	}
}