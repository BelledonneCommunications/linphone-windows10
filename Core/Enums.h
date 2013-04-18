#pragma once

namespace Linphone
{
	namespace Core
	{
		public enum class OnlineStatus : int
		{
			Offline = 0,
			Online = 1,
			Busy = 2,
			BeRightBack = 3,
			Away = 4,
			OnThePhone = 5,
			OutToLunch = 6,
			DoNotDisturb = 7,
			StatusMoved = 8,
			StatusAltService = 9,
			Pending = 10
		};

		public enum class CallDirection : int
		{
			Outgoing, 
			Incoming
		};
		
		/// <summary>
		/// Linphone core states
		/// </summary>
		public enum class GlobalState : int
		{
			GlobalOff = 0,
			GlobalStartup = 1,
			GlobalOn = 2,
			GlobalShutdown = 3
		};

		/// <summary>
		/// Proxy registration states
		/// </summary>
		public enum class RegistrationState : int
		{
			RegistrationNone = 0,
			RegistrationInProgress = 1,
			RegistrationOk = 2,
			RegistrationCleared = 3,
			RegistrationFailed = 4
		};

		public enum class MediaEncryption : int
		{
			None = 0,
			SRTP = 1,
			ZRTP = 2
		};

		public enum class FirewallPolicy : int
		{
			NoFirewall = 0,
			UseNatAddress = 1,
			UseStun = 2,
			UseIce = 3
		};

		public enum class EcCalibratorStatus : int
		{
			InProgress = 0,
			Done = 1,
			Failed = 2,
			DoneNoEcho = 3
		};

		/// <summary>
		/// Linphone call states
		/// </summary>
		public enum class LinphoneCallState : int
		{
			Idle = 0,
			IncomingReceived = 1,
			OutgoingInit = 2,
			OutgoingProgress = 3,
			OutgoingRinging = 4,
			OutgoingEarlyMedia = 5,
			Connected = 6,
			StreamsRunning = 7,
			Pausing = 8,
			Paused = 9,
			Resuming = 10,
			Refered = 11,
			Error = 12,
			CallEnd = 13,
			PausedByRemote = 14,
			UpdatedByRemote = 15,
			IncomingEarlyMedia = 16,
			Udating = 17,
			Released = 18
		};

		/// <summary>
		/// Represents a call status
		/// </summary>
		public enum class LinphoneCallStatus : int
		{
			Success = 0,
			Aborted = 1,
			Missed = 2,
			Declined = 3
		};

		/// <summary>
		/// Levels for trace output
		/// </summary>
		public enum class OutputTraceLevel : int
		{
			Debug = 0,
			Message = 1,
			Warning = 2,
			Error = 3,
			None = 255
		};

		/// <summary>
		/// The output destinations for logging
		/// </summary>
		public enum class OutputTraceDest : int
		{
			None = 0,
			Debugger = 1,
			File = 2,
			TCPRemote = 3
		};

		/// <summary>
		/// Linphone chat message states
		/// </summary>
		public enum class LinphoneChatMessageState : int
		{
			Idle = 0,
			InProgress = 1,
			Delivered = 2,
			NotDelivered = 3
		};
	}
}