#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneCore;

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
			/// Gets a boolean configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="defaultValue">The value to return if the configuration item is not found</param>
			/// <returns>The value of the configuration item or defaultValue if the item is not found</returns>
			bool GetBool(Platform::String^ section, Platform::String^ key, bool defaultValue);

			/// <summary>
			/// Sets a boolean configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="value">The value to set for the configuration item</param>
			void SetBool(Platform::String^ section, Platform::String^ key, bool value);

			/// <summary>
			/// Gets an integer configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="defaultValue">The value to return if the configuration item is not found</param>
			/// <returns>The value of the configuration item or defaultValue if the item is not found</returns>
			int GetInt(Platform::String^ section, Platform::String^ key, int defaultValue);

			/// <summary>
			/// Sets an integer configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="value">The value to set for the configuration item</param>
			void SetInt(Platform::String^ section, Platform::String^ key, int value);

			/// <summary>
			/// Gets a 64 bit integer configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="defaultValue">The value to return if the configuration item is not found</param>
			/// <returns>The value of the configuration item or defaultValue if the item is not found</returns>
			int64 GetInt64(Platform::String^ section, Platform::String^ key, int64 defaultValue);

			/// <summary>
			/// Sets a 64 bit integer configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="value">The value to set for the configuration item</param>
			void SetInt64(Platform::String^ section, Platform::String^ key, int64 value);

			/// <summary>
			/// Gets a float configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="defaultValue">The value to return if the configuration item is not found</param>
			/// <returns>The value of the configuration item or defaultValue if the item is not found</returns>
			float GetFloat(Platform::String^ section, Platform::String^ key, float defaultValue);

			/// <summary>
			/// Sets a float configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="value">The value to set for the configuration item</param>
			void SetFloat(Platform::String^ section, Platform::String^ key, float value);

			/// <summary>
			/// Gets a string configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="defaultValue">The value to return if the configuration item is not found</param>
			/// <returns>The value of the configuration item or defaultValue if the item is not found</returns>
			Platform::String^ GetString(Platform::String^ section, Platform::String^ key, Platform::String^ defaultValue);

			/// <summary>
			/// Sets a string configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="value">The value to set for the configuration item</param>
			void SetString(Platform::String^ section, Platform::String^ key, Platform::String^ value);

			/// <summary>
			/// Gets an integer range configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="defaultValue">The value to return if the configuration item is not found</param>
			/// <returns>The value of the configuration item or defaultValue if the item is not found</returns>
			Platform::Array<int>^ GetRange(Platform::String^ section, Platform::String^ key, const Platform::Array<int>^ defaultValue);

			/// <summary>
			/// Sets an integer range configuration item.
			/// </summary>
			/// <param name="section">The name of the configuration section where the item is located</param>
			/// <param name="key">The configuration item name</param>
			/// <param name="value">The value to set for the configuration item</param>
			void SetRange(Platform::String^ section, Platform::String^ key, const Platform::Array<int>^ value);

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LpConfig(::LpConfig *config);
			LpConfig(Platform::String^ configPath, Platform::String^ factoryConfigPath);
			~LpConfig();

			::LpConfig *config;
		};
	}
}