/*
PayloadType.h
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

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		/// <summary>
		/// Object representing a media payload type.
		/// </summary>
		public ref class PayloadType sealed
		{
		public:
			/// <summary>
			/// Gets the MIME type of the payload type.
			/// </summary>
			property Platform::String^ MimeType
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Gets the clock rate of the payload type.
			/// </summary>
			property int ClockRate
			{
				int get();
			}

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			PayloadType(::PayloadType *payload);
			~PayloadType();

			::PayloadType *payload;
		};
	}
}