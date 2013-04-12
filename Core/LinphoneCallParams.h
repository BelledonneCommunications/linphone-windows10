#pragma once

#include "LinphoneCore.h"

namespace Linphone
{
	namespace Core
	{
		ref class PayloadType;

		/// <summary>
		/// This object contains various call related parameters.
		/// It can be used to retrieve parameters from a currently running call or modify the call's caracteristics dynamically.
		/// </summary>
		public ref class LinphoneCallParams sealed
		{
		public:
			/// <summary>
			/// Sets the audio bandwidth in kbits/s.
			/// </summary>
			/// <param name="value">0 to disable limitation</param>
			void SetAudioBandwidth(int value);

			/// <summary>
			/// Returns the MediaEncryption of the call (None, SRTP or ZRTP).
			/// </summary>
			MediaEncryption GetMediaEncryption();

			/// <summary>
			/// Sets the MediaEncryption (None, SRTP or ZRTP).
			/// </summary>
			void SetMediaEncryption(MediaEncryption menc);

			/// <summary>
			/// Returns the PayloadType currently in use for the audio stream.
			/// </summary>
			PayloadType^ GetUsedAudioCodec();

			/// <summary>
			/// Indicates low bandwidth mode.
			/// Configuring a call to low bandwidth mode will result in the core to activate several settings for the call in order to ensure that bitrate usage is lowered to the minimum possible.
			/// Tyically, ptime (packetization time) will be increased, audio codecs's output bitrate will be targetted to 20kbits/s provided that it is achievable by the codec selected after SDP handshake.
			/// Video is automatically disabled.
			/// </summary>
			void EnableLowBandwidth(Platform::Boolean enable);

			/// <summary>
			/// Gets if the low bandwidth mode is enabled.
			/// See EnableLowBandwidth(boolean enable).
			/// </summary>
			Platform::Boolean IsLowBandwidthEnabled();
		};
	}
}