#pragma once

#include "LinphoneCore.h"
#include "Enums.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneCore;

		/// <summary>
		/// Listener to be called when the state of the message changes.
		/// </summary>
		public interface class LinphoneChatMessageListener
		{
		public:
			void MessageStateChanged(LinphoneChatMessage^ message, LinphoneChatMessageState state);
		};

		/// <summary>
		/// Represents a chat message shared between two users.
		/// </summary>
		public ref class LinphoneChatMessage sealed
		{
		public:
			/// <summary>
			/// Returns the text associated to this message.
			/// </summary>
			Platform::String^ GetText();
			
			/// <summary>
			/// Get peer address as LinphoneAddress associated to this message.
			/// </summary>
			Linphone::Core::LinphoneAddress^ GetPeerAddress();
			
			/// <summary>
			/// Get from address as LinphoneAddress associated to this message.
			/// </summary>
			Linphone::Core::LinphoneAddress^ GetFrom();
			
			/// <summary>
			/// Linphone message can carry external body as defined by rfc2017.
			/// </summary>
			Platform::String^ GetExternalBodyUrl();
			
			/// <summary>
			/// Linphone message can carry external body as defined by rfc2017.
			/// </summary>
			void SetExternalBodyUrl(Platform::String^ url);
			
			/// <summary>
			/// Gets the time at which the message was sent (in milliseconds since 01/01/1970).
			/// </summary>
			int64 GetTime();

			/// <summary>
			/// Gets the state of the message (Idle, InProgress, Delivered or NotDelivered).
			/// </summary>
			Linphone::Core::LinphoneChatMessageState GetState();

		private:
			friend class Linphone::Core::Utils;
			friend ref class Linphone::Core::LinphoneCore;

			LinphoneChatMessage(::LinphoneChatMessage *cm);
			~LinphoneChatMessage();

			::LinphoneChatMessage *message;
		};
	}
}