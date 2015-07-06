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
			virtual Platform::String^ ToString() override;

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