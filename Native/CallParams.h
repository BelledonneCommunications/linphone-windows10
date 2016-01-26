/*
CallParams.h
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

#include "ApiLock.h"
#include "Core.h"


namespace BelledonneCommunications
{
	namespace Linphone
	{
		namespace Native
		{
			ref class PayloadType;

			/// <summary>
			/// This object contains various call related parameters.
			/// It can be used to retrieve parameters from a currently running call or modify the call's caracteristics dynamically.
			/// </summary>
			public ref class CallParams sealed
			{
			public:
				/// <summary>
				/// Sets the audio bandwidth in kbits/s (0 to disable limitation).
				/// </summary>
				property int AudioBandwidthLimit
				{
					int get();
					void set(int value);
				}

				/// <summary>
				/// Set the audio stream direction.
				/// </summary>
				property MediaDirection AudioDirection
				{
					MediaDirection get();
					void set(MediaDirection value);
				}

				/// <summary>
				/// Indicates low bandwidth mode.
				/// Configuring a call to low bandwidth mode will result in the core to activate several settings for the call in order to ensure that bitrate usage is lowered to the minimum possible.
				/// Tyically, ptime (packetization time) will be increased, audio codecs's output bitrate will be targetted to 20kbits/s provided that it is achievable by the codec selected after SDP handshake.
				/// Video is automatically disabled.
				/// </summary>
				property Platform::Boolean IsLowBandwidthEnabled
				{
					Platform::Boolean get();
					void set(Platform::Boolean value);
				}

				/// <summary>
				/// Enable or disable video.
				/// </summary>
				property Platform::Boolean IsVideoEnabled
				{
					Platform::Boolean get();
					void set(Platform::Boolean value);
				}

				/// <summary>
				/// Returns the MediaEncryption of the call (None, SRTP or ZRTP).
				/// </summary>
				property MediaEncryption MediaEncryption
				{
					BelledonneCommunications::Linphone::Native::MediaEncryption get();
					void set(BelledonneCommunications::Linphone::Native::MediaEncryption value);
				}

				/// <summary>
				/// Returns the size of the video being received.
				/// </summary>
				property VideoSize^ ReceivedVideoSize
				{
					VideoSize^ get();
				}

				/// <summary>
				/// Returns the size of the video being sent.
				/// </summary>
				property VideoSize^ SentVideoSize
				{
					VideoSize^ get();
				}

				/// <summary>
				/// Returns the PayloadType currently in use for the audio stream.
				/// </summary>
				property PayloadType^ UsedAudioCodec
				{
					PayloadType^ get();
				}

				/// <summary>
				/// Returns the PayloadType currently in use for the video stream.
				/// </summary>
				property PayloadType^ UsedVideoCodec
				{
					PayloadType^ get();
				}

				/// <summary>
				/// Set the video stream direction.
				/// </summary>
				property MediaDirection VideoDirection
				{
					MediaDirection get();
					void set(MediaDirection value);
				}

				/// <summary>
				/// Copy an existing CallParams object to a new CallParams object.
				/// </summary>
				CallParams^ Copy();

			private:
				friend class Utils;
				friend ref class Core;

				CallParams(::LinphoneCallParams* params);
				~CallParams();

				::LinphoneCallParams *params;
			};
		}
	}
}