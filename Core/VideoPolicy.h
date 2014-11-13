#pragma once

#include "Utils.h"

namespace Linphone
{
    namespace Core
	{
		ref class LinphoneCoreFactory;
		ref class LinphoneCore;

		/// <summary>
		/// Class describing policy regarding video streams establishments.
		/// </summary>
		public ref class VideoPolicy sealed
		{
		public:
			/// <summary>
			/// Whether video shall be automatically proposed for outgoing calls.
			/// </summary>
			property bool AutomaticallyInitiate
			{
				bool get();
				void set(bool value);
			}

			/// <summary>
			/// Whether video shall be automatically accepted for incoming calls.
			/// </summary>
			property bool AutomaticallyAccept
			{
				bool get();
				void set(bool value);
			}

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCoreFactory;
			friend ref class Linphone::Core::LinphoneCore;

			VideoPolicy();
			VideoPolicy(bool automaticallyInitiate, bool automaticallyAccept);

			bool automaticallyInitiate;
			bool automaticallyAccept;
		};
	}
}