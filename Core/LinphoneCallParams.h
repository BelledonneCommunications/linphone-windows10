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
			/// Sets the audio bandwidth in kbits/s (0 to disable limitation).
			/// </summary>
			property int AudioBandwidthLimit
			{
				void set(int value);
			}

			/// <summary>
			/// Returns the MediaEncryption of the call (None, SRTP or ZRTP).
			/// </summary>
			property Linphone::Core::MediaEncryption MediaEncryption
			{
				Linphone::Core::MediaEncryption get();
				void set(Linphone::Core::MediaEncryption value);
			}

			/// <summary>
			/// Returns the PayloadType currently in use for the audio stream.
			/// </summary>
			property PayloadType^ UsedAudioCodec
			{
				PayloadType^ get();
			}

			/// <summary>
			/// Indicates low bandwidth mode.
			/// Configuring a call to low bandwidth mode will result in the core to activate several settings for the call in order to ensure that bitrate usage is lowered to the minimum possible.
			/// Tyically, ptime (packetization time) will be increased, audio codecs's output bitrate will be targetted to 20kbits/s provided that it is achievable by the codec selected after SDP handshake.
			/// Video is automatically disabled.
			/// </summary>
			property Platform::Boolean LowBandwidthEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Enable or disable video.
			/// </summary>
			property Platform::Boolean VideoEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Returns the PayloadType currently in use for the video stream.
			/// </summary>
			property PayloadType^ UsedVideoCodec
			{
				PayloadType^ get();
			}

			/// <summary>
			/// Returns the size of the video being sent.
			/// </summary>
			property Windows::Foundation::Size SentVideoSize
			{
				Windows::Foundation::Size get();
			}

			/// <summary>
			/// Returns the size of the video being received.
			/// </summary>
			property Windows::Foundation::Size ReceivedVideoSize
			{
				Windows::Foundation::Size get();
			}

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneCallParams(::LinphoneCallParams* params);
			~LinphoneCallParams();

			::LinphoneCallParams *params;
		};
	}
}