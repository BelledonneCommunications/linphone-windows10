#pragma once

#include "LinphoneChatMessage.h"

namespace Linphone
{
	namespace Core
	{
		/// <summary>
		/// Listener to be called when the state of the message changes.
		/// </summary>
		public interface class LinphoneChatMessageListener
		{
		public:
			void MessageStateChanged(LinphoneChatMessage^ message, LinphoneChatMessageState state);
		};
	}
}