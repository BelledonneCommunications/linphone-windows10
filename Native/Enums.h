/*
Enums.h
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

namespace BelledonneCommunications
{
	namespace Linphone
	{
		namespace Native
		{
			/// <summary>
			/// Direction of a call, either Outgoing or Incoming.
			/// </summary>
			public enum class CallDirection : int
			{
				Outgoing,
				Incoming
			};

			/// <summary>
			/// Call states
			/// </summary>
			public enum class CallState : int
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
				End = 13,
				PausedByRemote = 14,
				UpdatedByRemote = 15,
				IncomingEarlyMedia = 16,
				Updating = 17,
				Released = 18,
				EarlyUpdatedByRemote = 19,
				EarlyUpdating = 20
			};

			/// <summary>
			/// Represents a call status
			/// </summary>
			public enum class CallStatus : int
			{
				Success = 0,
				Aborted = 1,
				Missed = 2,
				Declined = 3
			};

			/// <summary>
			/// Chat message states
			/// </summary>
			public enum class ChatMessageState : int
			{
				Idle = 0,
				InProgress = 1,
				Delivered = 2,
				NotDelivered = 3,
				FileTransferError = 4,
				FileTransferDone = 5
			};

			/// <summary>
			/// Statuses of the echo canceller calibration process.
			/// </summary>
			public enum class EcCalibratorStatus : int
			{
				InProgress = 0,
				Done = 1,
				Failed = 2,
				DoneNoEcho = 3
			};

			/// <summary>
			/// Policy to use to work around the issues caused by NAT (Network Address Translation).
			/// </summary>
			public enum class FirewallPolicy : int
			{
				NoFirewall = 0,
				UseNatAddress = 1,
				UseStun = 2,
				UseIce = 3,
				UseUpnp = 4
			};

			/// <summary>
			/// Core global states
			/// </summary>
			public enum class GlobalState : int
			{
				Off = 0,
				Startup = 1,
				On = 2,
				Shutdown = 3,
				Configuring = 4
			};

			/// <summary>
			/// State of the ICE processing.
			/// </summary>
			public enum class IceState : int
			{
				NotActivated = 0,
				Failed = 1,
				InProgress = 2,
				HostConnection = 3,
				ReflexiveConnection = 4,
				RelayConnection = 5
			};

			/// <summary>
			/// Log collection states
			/// </summary>
			public enum class LogCollectionState : int
			{
				Disabled = 0,
				Enabled = 1,
				EnabledWithoutPreviousLogHandler = 2
			};

			/// <summary>
			/// Used to notify if log collection upload have been succesfully delivered or not
			/// </summary>
			public enum class LogCollectionUploadState : int
			{
				InProgress = 0, /**< Delivery in progress */
				Delivered = 1, /**< Log collection upload successfully delivered and acknowledged by remote end point */
				NotDelivered = 2 /**< Log collection upload was not delivered */
			};

			/// <summary>
			/// Indicates for a given media the stream direction.
			/// </summary>
			public enum class MediaDirection : int
			{
				Inactive = 0,
				SendOnly = 1,
				RecvOnly = 2,
				SendRecv = 3
			};

			/// <summary>
			/// Types of media encryption.
			/// </summary>
			public enum class MediaEncryption : int
			{
				None = 0,
				SRTP = 1,
				ZRTP = 2,
				DTLS = 3
			};

			/// <summary>
			/// Media type of the statistics (audio or video).
			/// </summary>
			public enum class MediaType : int
			{
				Audio = 0,
				Video = 1
			};

			/// <summary>
			/// Presence statuses.
			/// </summary>
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
				Moved = 8,
				AltService = 9,
				Pending = 10,
				Vacation = 11
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
			/// Failure reasons or contextual information for some events
			/// </summary>
			public enum class Reason : int
			{
				None = 0,
				NoResponse = 1, /**<No response received from remote*/
				Forbidden = 2, /**<Authentication failed due to bad credentials or resource forbidden*/
				Declined = 3, /**<The call has been declined*/
				NotFound = 4, /**<Destination of the call was not found.*/
				NotAnswered = 5, /**<The call was not answered in time (request timeout)*/
				Busy = 6, /**<Phone line was busy */
				UnsupportedContent = 7, /**<Unsupported content */
				IOError = 8, /**<Transport error: connection failures, disconnections etc...*/
				DoNotDisturb = 9, /**<Do not disturb reason*/
				Unauthorized = 10, /**<Operation is unauthorized because missing credential*/
				NotAcceptable = 11, /**<Operation like call update rejected by peer*/
				NoMatch = 12, /**<Operation could not be executed by server or remote client because it didn't have any context for it*/
				MovedPermanently = 13, /**<Resource moved permanently*/
				Gone = 14, /**<Resource no longer exists*/
				TemporarilyUnavailable = 15, /**<Temporarily unavailable*/
				AddressIncomplete = 16, /**<Address incomplete*/
				NotImplemented = 17, /**<Not implemented*/
				BadGateway = 18, /**<Bad gateway*/
				ServerTimeout = 19, /**<Server timeout*/
				Unknown = 20 /**Unknown reason*/
			};

			/// <summary>
			/// Proxy registration states
			/// </summary>
			public enum class RegistrationState : int
			{
				None = 0,
				Progress = 1,
				Ok = 2,
				Cleared = 3,
				Failed = 4
			};

			/// <summary>
			/// Transport types
			/// </summary>
			public enum class Transport : int
			{
				UDP = 0,
				TCP = 1,
				TLS = 2,
				DTLS = 3
			};
		}
	}
}