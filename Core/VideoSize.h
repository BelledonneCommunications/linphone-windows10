#pragma once

#include "Utils.h"

namespace Linphone
{
    namespace Core
	{
		ref class LinphoneCoreFactory;
		ref class LinphoneCore;

		/// <summary>
		/// Class describing a video size.
		/// </summary>
		public ref class VideoSize sealed
		{
		public:
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
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCoreFactory;
			friend ref class Linphone::Core::LinphoneCore;

			VideoSize(int width, int height);
			VideoSize(int width, int height, Platform::String^ name);

			int width;
			int height;
			Platform::String^ name;
		};
	}
}