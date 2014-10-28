#pragma once

#include "LinphoneCore.h"
#include "ApiLock.h"

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
			/// <returns>The media encryption of the call</returns>
			MediaEncryption GetMediaEncryption();

			/// <summary>
			/// Sets the MediaEncryption (None, SRTP or ZRTP).
			/// </summary>
			/// <param name="menc">The media encryption to use for the call</param>
			void SetMediaEncryption(MediaEncryption menc);

			/// <summary>
			/// Returns the PayloadType currently in use for the audio stream.
			/// </summary>
			/// <returns>The payload type currently in use for the audio stream</returns>
			PayloadType^ GetUsedAudioCodec();

			/// <summary>
			/// Indicates low bandwidth mode.
			/// Configuring a call to low bandwidth mode will result in the core to activate several settings for the call in order to ensure that bitrate usage is lowered to the minimum possible.
			/// Tyically, ptime (packetization time) will be increased, audio codecs's output bitrate will be targetted to 20kbits/s provided that it is achievable by the codec selected after SDP handshake.
			/// Video is automatically disabled.
			/// </summary>
			/// <param name="enable">A boolean value telling whether to enable the low bandwidth mode</param>
			void EnableLowBandwidth(Platform::Boolean enable);

			/// <summary>
			/// Gets if the low bandwidth mode is enabled.
			/// See EnableLowBandwidth(boolean enable).
			/// </summary>
			/// <returns>A boolean value telling whether the low bandwidth mode is enabled or not</returns>
			Platform::Boolean IsLowBandwidthEnabled();

			/// <summary>
			/// Tells whether video is enabled.
			/// </summary>
			/// <returns>A boolean value telling whether video is enabled or not</returns>
			Platform::Boolean IsVideoEnabled();

			/// <summary>
			/// Enable or disable video.
			/// </summary>
			/// <param name="enable">A boolean value telling whether video should be enabled or disabled</param>
			void EnableVideo(Platform::Boolean enable);

			/// <summary>
			/// Returns the PayloadType currently in use for the video stream.
			/// </summary>
			/// <returns>The payload type currently in use for the video stream</returns>
			PayloadType^ GetUsedVideoCodec();

			/// <summary>
			/// Returns the size of the video being sent.
			/// </summary>
			/// <returns>The size of the video being sent</returns>
			Windows::Foundation::Size GetSentVideoSize();

			/// <summary>
			/// Returns the size of the video being received.
			/// </summary>
			/// <returns>The size of the video being received</returns>
			Windows::Foundation::Size GetReceivedVideoSize();

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneCallParams(::LinphoneCallParams* params);
			~LinphoneCallParams();

			::LinphoneCallParams *params;
		};
	}
}