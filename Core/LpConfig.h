#pragma once

namespace Linphone
{
	namespace Core
	{
		/// <summary>
		/// Object used to manipulate a configuration file.
		/// The format of the configuration file is a .ini like format:
		/// - sections are defined in []
		/// - each section contains a sequence of key=value pairs.
		/// <example>
		/// [sound]
		/// echocanceler=1
		/// playback_dev=ALSA: Default device
		/// 
		/// [video]
		/// enabled=1
		/// </example>
		/// </summary>
		public ref class LpConfig sealed
		{
		public:
			/// <summary>
			/// Sets an integer configuration item.
			/// </summary>
			void SetInt(Platform::String^ section, Platform::String^ key, int value);
		};
	}
}