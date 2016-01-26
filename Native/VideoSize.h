/*
VideoSize.h
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
			/// Class describing a video size.
			/// </summary>
			public ref class VideoSize sealed
			{
			public:
				/// <summary>
				/// Creates a VideoSize object.
				/// </summary>
				/// <param name="width">The video width</param>
				/// <param name="height">The video height</param>
				/// <returns>The created VideoSize</returns>
				VideoSize(int width, int height);

				/// <summary>
				/// Creates a named VideoSize object.
				/// </summary>
				/// <param name="width">The video width</param>
				/// <param name="height">The video height</param>
				/// <param name="name">The video size name</param>
				/// <returns>The created VideoSize</returns>
				VideoSize(int width, int height, Platform::String^ name);

				/// <summary>
				/// The video size width (eg. 640).
				/// </summary>
				property int Width
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// The video size height (eg. 480).
				/// </summary>
				property int Height
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// The video size name (eg. vga).
				/// </summary>
				property Platform::String^ Name
				{
					Platform::String^ get();
					void set(Platform::String^ value);
				}

			private:
				friend class Utils;
				friend ref class LinphoneCore;

				int width;
				int height;
				Platform::String^ name;
			};
		}
	}
}