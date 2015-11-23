/*
Core.h
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

#include "Enums.h"
#include "CoreListener.h"
#include "Utils.h"


// Do not treat doxygen documentation as XML
#pragma warning(push)
#pragma warning(disable : 4635)
#include "coreapi\linphonecore.h"
#include "coreapi\linphonecore_utils.h"
#include "coreapi\linphone_tunnel.h"
#include "coreapi\lpconfig.h"
#pragma warning(pop)

namespace Linphone
{
	namespace Native
	{
		ref class Address;
		ref class AuthInfo;
		ref class Call;
		ref class CallLog;
		ref class CallParams;
		ref class CallStats;
		ref class ChatRoom;
		ref class Core;
		ref class LpConfig;
		ref class PayloadType;
		ref class ProxyConfig;
		ref class Transports;
		ref class Tunnel;
		ref class VideoPolicy;
		ref class VideoSize;
		
		/// <summary>
		/// Main object.
		/// </summary>
		public ref class Core sealed
		{
		public:
			Core(CoreListener^ coreListener);
			Core(CoreListener^ coreListener, LpConfig^ config);
			virtual ~Core();


			/// <summary>
			/// Declares how many CPUs (cores) are available on the platform.
			/// </summary>
			static property int CPUCount
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Enables log collection to upload logs on a server.
			/// </summary>
			static property Linphone::Native::LogCollectionState LogCollectionEnabled
			{
				Linphone::Native::LogCollectionState get();
				void set(Linphone::Native::LogCollectionState value);
			}

			/// <summary>
			/// Sets the path where the log files will be written for log collection.
			/// </summary>
			static property Platform::String^ LogCollectionPath
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Sets the global log level.
			/// </summary>
			static property OutputTraceLevel LogLevel
			{
				OutputTraceLevel get();
				void set(OutputTraceLevel value);
			}

			/// <summary>
			/// Tells whether tunnel support is available.
			/// </summary>
			static property Platform::Boolean TunnelAvailable
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Gets liblinphone's version as a string.
			/// </summary>
			static property Platform::String^ Version
			{
				Platform::String^ get();
			}




			/// <summary>
			/// Resets the log collection by removing the log files.
			/// </summary>
			static void ResetLogCollection();




			/// <summary>
			/// Gets the currently supported audio codecs, as PayloadType elements.
			/// </summary>
			/// <returns>A list of PayloadType objects</returns>
			property Windows::Foundation::Collections::IVector<Linphone::Native::PayloadType^>^ AudioCodecs
			{
				Windows::Foundation::Collections::IVector<Linphone::Native::PayloadType^>^ get();
			}

			/// <summary>
			/// Sets the UDP port used for audio streaming.
			/// </summary>
			property int AudioPort
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Gets the current auth infos.
			/// </summary>
			/// <returns>The list of authentication informations</returns>
			property Windows::Foundation::Collections::IVector<Linphone::Native::AuthInfo^>^ AuthInfoList
			{
				Windows::Foundation::Collections::IVector<Linphone::Native::AuthInfo^>^ get();
			}

			/// <summary>
			/// Gets the list of call logs of the Core.
			/// </summary>
			/// <returns>A list of CallLog objects</returns>
			property Windows::Foundation::Collections::IVector<Linphone::Native::CallLog^>^ CallLogs
			{
				Windows::Foundation::Collections::IVector<Linphone::Native::CallLog^>^ get();
			}

			/// <summary>
			/// Gets the current list of calls.
			/// </summary>
			property Windows::Foundation::Collections::IVector<Linphone::Native::Call^>^ Calls
			{
				Windows::Foundation::Collections::IVector<Linphone::Native::Call^>^ get();
			}

			/// <summary>
			/// Gets the number of calls.
			/// </summary>
			property int CallsNb
			{
				int get();
			}

			/// <summary>
			/// Gets the camera sensor rotation in degrees.
			/// </summary>
			property int CameraSensorRotation
			{
				int get();
			}

			/// <summary>
			/// Sets the path to the database file used to store chat messages
			/// </summary>
			property Platform::String^ ChatDatabasePath
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Gets the list of the created chatrooms
			/// </summary>
			property Windows::Foundation::Collections::IVector<Linphone::Native::ChatRoom^>^ ChatRooms
			{
				Windows::Foundation::Collections::IVector<Linphone::Native::ChatRoom^>^ get();
			}

			/// <summary>
			/// Returns the number of participants to the conference, including the local participant.
			/// Typically, after merging two calls into the conference, there is total of 3 participants:
			/// the local participant (or local user), and two remote participants that were the destinations of the two previously establised calls.
			/// </summary>
			property int ConferenceSize
			{
				int get();
			}

			/// <summary>
			/// Gets the LpConfig object to read/write to the config file: useful if you wish to extend the config file with your own sections.
			/// </summary>
			property Linphone::Native::LpConfig^ Config
			{
				Linphone::Native::LpConfig^ get();
			}

			/// <summary>
			/// The CoreListener that handles the events coming from the core.
			/// </summary>
			property Linphone::Native::CoreListener^ CoreListener
			{
				Linphone::Native::CoreListener^ get();
				void set(Linphone::Native::CoreListener^ listener);
			}

			/// <summary>
			/// Gets the current active call.
			/// If there is only one ongoing call that is in the paused state, then there is no current call.
			/// </summary>
			property Call^ CurrentCall
			{
				Call^ get();
			}

			/// <summary>
			/// Sets the default proxy.
			/// This default proxy must be part of the list of already entered ProxyConfig.
			/// Toggling it as default will make Core use this identity associated with the proxy config in all incoming and outgoing calls.
			/// </summary>
			property ProxyConfig^ DefaultProxyConfig
			{
				ProxyConfig^ get();
				void set(ProxyConfig^ value);
			}

			/// <summary>
			/// Tells the core the device current orientation. This can be used by capture filters
			/// on mobile devices to select between portrait / landscape mode and to produce properly
			///	oriented images.The exact meaning of the value in rotation if left to each device
			///	specific implementations.
			/// </summary>
			property int DeviceRotation
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Maximum available download bandwidth.
			/// This is IP bandwidth, in kbit/s (0 for infinite).
			/// This information is used signaled to other parties during calls (within SDP messages) so that the remote end
			/// can have sufficient knowledge to properly configure its audio and video codec output bitrate to not overflow available bandwidth.
			/// </summary>
			property int DownloadBandwidth
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Sets audio packetization interval suggested for remote end (in milliseconds).
			/// A value of zero means that ptime is not specified.
			/// </summary>
			property int DownloadPtime
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Sets policy regarding workarounding NATs.
			/// </summary>
			property Linphone::Native::FirewallPolicy FirewallPolicy
			{
				Linphone::Native::FirewallPolicy get();
				void set(Linphone::Native::FirewallPolicy value);
			}

			/// <summary>
			/// Sets the call timeout in seconds.
			/// Once this time is elapsed (ringing included), the call is automatically hung up.
			/// </summary>
			property int InCallTimeout
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Sets the incoming call timeout in seconds.
			/// If an incoming call isn't answered for this timeout period, it is automatically declined.
			/// </summary>
			property int IncTimeout
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Enables or disables the echo cancellation.
			/// </summary>
			property Platform::Boolean IsEchoCancellationEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Enables or disables the echo limiter.
			/// </summary>
			property Platform::Boolean IsEchoLimiterEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Tells whether there is at least one call running.
			/// </summary>
			property Platform::Boolean IsInCall
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Tells whether there is an incoming call invite pending.
			/// </summary>
			/// <returns>true if there is an incoming call invite pending, else returns false</returns>
			property Platform::Boolean IsIncomingInvitePending
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Tells whether a conference is currently ongoing.
			/// </summary>
			property Platform::Boolean IsInConference
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Enables signaling keep alive. Small UDP packet sent periodically to keep UDP NAT association.
			/// </summary>
			property Platform::Boolean IsKeepAliveEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Enables or disables IPv6 support.
			/// </summary>
			property Platform::Boolean IsIpv6Enabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Set it to true to start the iterate, set it to false to stop it. 
			/// Is disabled by default.
			/// </summary>
			property Platform::Boolean IsIterateEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Defines Linphone behaviour when encryption parameters negociation fails on outgoing call.
			/// </summary>
			property Platform::Boolean IsMediaEncryptionMandatory
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Enables or disables the local microphone.
			/// </summary>
			property Platform::Boolean IsMicEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// This method is called by the application to notify the Linphone.Core library when network is reachable.
			/// Calling this method with true triggers Linphone to initiate a registration process for all proxy configs with parameter register set to enable.
			/// This method disables the automatic registration mode. It means you must call this method after each network state changes.
			/// </summary>
			property Platform::Boolean IsNetworkReachable
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Enables or disables self view during calls.
			/// Self-view refers to having local webcam image inserted in corner of the video window during calls.
			/// </summary>
			property Platform::Boolean IsSelfViewEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Use this method to check calls state and forbid proposing actions which could result in an active call.
			/// Eg: don't start a new call if one is in outgoing ringing.
			/// Eg: don't merge to conference either as it could result in two active calls (conference and accepted call).
			/// </summary>
			property Platform::Boolean IsSoundResourcesLocked
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Enable or disable video capture.
			///This function does not have any effect during calls. It just indicates the Core to initiate future calls with video capture or not.
			/// </summary>
			property Platform::Boolean IsVideoCaptureEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Enable or disable video display.
			/// This function does not have any effect during calls. It just indicates the Core to initiate future calls with video display or not.
			/// </summary>
			property Platform::Boolean IsVideoDisplayEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Tells whether video support has been compiled.
			/// </summary>
			property Platform::Boolean IsVideoSupported
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Sets the log collection upload server URL.
			/// </summary>
			property Platform::String^ LogCollectionUploadServerUrl
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Sets the maximum number of simultaneous calls the Core can manage at a time. All new call above this limit are declined with a busy answer.
			/// </summary>
			property int MaxCalls
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Chooses the media encryption policy to be used for RTP packets.
			/// </summary>
			property Linphone::Native::MediaEncryption MediaEncryption
			{
				Linphone::Native::MediaEncryption get();
				void set(Linphone::Native::MediaEncryption value);
			}

			/// <summary>
			/// Sets the microphone gain in dB.
			/// </summary>
			property float MicGainDb
			{
				float get();
				void set(float value);
			}

			/// <summary>
			/// Gets the number of missed calls since last reset.
			/// </summary>
			property int MissedCallsCount
			{
				int get();
			}

			/// <summary>
			/// Sets the native preview window id (a Windows::UI::Xaml::Controls::CaptureElement as a Platform::Object).
			/// </summary>
			property Platform::Object^ NativePreviewWindowId
			{
				Platform::Object^ get();
				void set(Platform::Object^ value);
			}

			/// <summary>
			/// Sets the native video window id (a Windows::UI::Xaml::Controls::MediaElement as a Platform::Object).
			/// </summary>
			property Platform::Object^ NativeVideoWindowId
			{
				Platform::Object^ get();
				void set(Platform::Object^ value);
			}

			/// <summary>
			/// Allow to control play level before entering the sound card in dB.
			/// </summary>
			property float PlaybackGainDb
			{
				float get();
				void set(float value);
			}

			/// <summary>
			/// Sets the file to be played when putting a call on hold.
			/// </summary>
			property Platform::String^ PlayFile
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Sets the playback level in a scale from 0 to 100.
			/// </summary>
			property int PlayLevel
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Sets the preferred video size.
			/// This applies only to the stream that is captured and sent to the remote party,
			/// since we accept all standard video size on the receive path.
			/// </summary>
			property Linphone::Native::VideoSize^ PreferredVideoSize
			{
				Linphone::Native::VideoSize^ get();
				void set(Linphone::Native::VideoSize^ value);
			}

			/// <summary>
			/// Gets the preferred video size name.
			/// </summary>
			property Platform::String^ PreferredVideoSizeName
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Gets the list of the current proxy configs.
			/// </summary>
			property Windows::Foundation::Collections::IVector<Linphone::Native::ProxyConfig^>^ ProxyConfigList
			{
				Windows::Foundation::Collections::IVector<Linphone::Native::ProxyConfig^>^ get();
			}

			/// <summary>
			/// Sets the file or folder containing trusted root CAs.
			/// </summary>
			property Platform::String^ RootCa
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Sets the ports to be used for each transport (UDP, TCP, TLS).
			/// </summary>
			property Transports^ SipTransports
			{
				Transports^ get();
				void set(Transports^ value);
			}

			/// <summary>
			/// Specifies a STUN server to help firewall traversal, such as stun.linphone.org or stun.linphone.org:3478
			/// </summary>
			property Platform::String^ StunServer
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Gets the list of video sizes that are supported.
			/// </summary>
			property Windows::Foundation::Collections::IVector<Linphone::Native::VideoSize^>^ SupportedVideoSizes
			{
				Windows::Foundation::Collections::IVector<Linphone::Native::VideoSize^>^ get();
			}

			/// <summary>
			/// Gets the tunnel instance if available.
			/// </summary>
			property Linphone::Native::Tunnel^ Tunnel
			{
				Linphone::Native::Tunnel^ get();
			}

			/// <summary>
			/// Sets maximum available upload bandwidth.
			/// This is IP bandwidth, in kbit/s (0 for infinite).
			/// This information is used by liblinphone together with remote side available bandwidth signaled in SDP messages
			/// to properly configure audio and video codec's output bitrate.
			/// </summary>
			property int UploadBandwidth
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Sets audio packetization interval sent to remote end (in milliseconds).
			/// A value of zero means that ptime is not specified.
			/// </summary>
			property int UploadPtime
			{
				int get();
				void set(int value);
			}

			/// <summary>
			/// Enables/Disables the use of SIP INFO to send DTMFs.
			/// </summary>
			property Platform::Boolean UseInfoForDtmf
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Enables/Disables the use of RFC2833 to send DTMFs.
			/// </summary>
			property Platform::Boolean UseRfc2833ForDtmf
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

			/// <summary>
			/// Gets the currently supported video codecs, as PayloadType elements.
			/// </summary>
			property Windows::Foundation::Collections::IVector<Linphone::Native::PayloadType^>^ VideoCodecs
			{
				Windows::Foundation::Collections::IVector<Linphone::Native::PayloadType^>^ get();
			}

			/// <summary>
			/// Sets the active video device.
			/// </summary>
			property Platform::String^ VideoDevice
			{
				Platform::String^ get();
				void set(Platform::String^ value);
			}

			/// <summary>
			/// Gets the list of video devices.
			/// </summary>
			property Windows::Foundation::Collections::IVector<Platform::String^>^ VideoDevices
			{
				Windows::Foundation::Collections::IVector<Platform::String^>^ get();
			}

			/// <summary>
			/// Sets the policy for video.
			/// </summary>
			property Linphone::Native::VideoPolicy^ VideoPolicy
			{
				Linphone::Native::VideoPolicy^ get();
				void set(Linphone::Native::VideoPolicy^ value);
			}




			/// <summary>
			/// Accepts an incoming call.
			/// Basically the app is notified of incoming calls within the CoreListener::CallState listener method.
			/// The application can later respond positively to the call using this method.
			/// </summary>
			/// <param name="call">The incoming call to accept</param>
			/// <seealso cref="AcceptCallWithParams(Call^, CallParams^)"/>
			void AcceptCall(Call^ call);

			/// <summary>
			/// Accepts call modifications initiated by other end.
			/// </summary>
			/// <param name="call">The incoming call to accept</param>
			/// <param name="params">The new local CallParams to use</param>
			void AcceptCallUpdate(Call^ call, CallParams^ params);

			/// <summary>
			/// Accepts an incoming call.
			/// Basically the app is notified of incoming calls within the CoreListener::CallState listener method.
			/// The application can later respond positively to the call using this method.
			/// </summary>
			/// <param name="call">The incoming call to accept</param>
			/// <param name="params">The CallParams to use for the accepted call</param>
			/// <seealso cref="AcceptCall(Call^)"/>
			void AcceptCallWithParams(Call^ call, CallParams^ params);

			/// <summary>
			/// Adds all calls into a conference.
			/// Merge all established calls (either in StreamsRunning or Paused) into a conference.
			/// </summary>
			void AddAllToConference();

			/// <summary>
			/// Adds authentication information to the Core.
			/// This information will be used during all SIP transactions which requieres authentication.
			/// </summary>
			/// <param name="info">The authentication information to be added</param>
			void AddAuthInfo(AuthInfo^ info);

			/// <summary>
			/// Adds a proxy config.
			/// This will start the registration of the proxy if registration is enabled.
			/// </summary>
			/// <param name="proxyCfg">The proxy config to be added</param>
			void AddProxyConfig(ProxyConfig^ proxyCfg);

			/// <summary>
			/// Merges a call into a conference.
			/// If this is the first call that enters the conference, the virtual conference will be created automatically.
			/// If the local user was actively part of the call (ie not in paused state), then the local user is automatically entered into the conference.
			/// If the call was in paused state, then it is automatically resumed when entering into the conference.
			/// </summary>
			/// <param name="call">An established call, either in StreamsRunning or Paused state</param>
			void AddToConference(Call^ call);

			/// <summary>
			/// Removes all the auth infos from Core.
			/// </summary>
			void ClearAllAuthInfo();

			/// <summary>
			/// Removes all call logs from the Core.
			/// </summary>
			void ClearCallLogs();

			/// <summary>
			/// Removes all the proxy configs from Core.
			/// </summary>
			void ClearProxyConfig();

			/// <summary>
			/// Creates an Address object using an URI.
			/// </summary>
			/// <param name="uri">The URI from which to create a Linphone::Native::Address</param>
			/// <returns>The created Linphone::Native::Address</returns>
			Address^ CreateAddress(Platform::String^ uri);

			/// <summary>
			/// Creates an AuthInfo.
			/// </summary>
			/// <param name="username">The authentication username</param>
			/// <param name="userid">The authentication userid</param>
			/// <param name="password">The authentication password</param>
			/// <param name="ha1">The authentication ha1</param>
			/// <param name="realm">The authentication realm</param>
			/// <param name="domain">The authentication domain</param>
			AuthInfo^ CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain);

			/// <summary>
			/// Gets a set of CallParams.
			/// </summary>
			/// <returns>The set of CallParams for the specified call</returns>
			CallParams^ CreateCallParams(Call^ call);

			/// <summary>
			/// Creates an empty proxy config.
			/// </summary>
			/// <returns>An empty proxy config</returns>
			ProxyConfig^ CreateProxyConfig();

			/// <summary>
			/// Declines an incoming call with a specific reason.
			/// </summary>
			void DeclineCall(Call^ call, Reason reason);

			/// <summary>
			/// Prevent Core from performing an automatic answer when receiving call modifications from the other end of the call.
			/// </summary>
			/// <param name="call">The call for which a modification from the other end has been notified</param>
			void DeferCallUpdate(Call^ call);

			/// <summary>
			/// Enables or disables a payload type.
			/// The payload type to enable/disable can be retrieved using FindPayloadType(String, int, int).
			/// </summary>
			/// <param name="pt">The PayloadType to enable/disable</param>
			/// <param name="enable">A boolean value telling whether to enable or disable the PayloadType</param>
			void EnablePayloadType(PayloadType^ pt, Platform::Boolean enable);

			/// <summary>
			/// Makes the local participant to join the conference. 
			/// Typically, the local participant is by default always part of the conference when joining an active call into a conference.
			/// However, by calling LeaveConference() and EnterConference() the application can decide to temporarily
			/// move out and in the local participant from the conference.
			/// </summary>
			/// <returns>A boolean value telling whether the conference has successfully been entered</returns>
			Platform::Boolean EnterConference();

			/// <summary>
			/// Searches from the list of current calls if a remote address match uri.
			/// </summary>
			/// <param name="uri">URI to search for in the list of current calls</param>
			/// <returns>The matching Call if found, null otherwise</returns>
			Call^ FindCallFromUri(Platform::String^ uri);

			/// <summary>
			/// Tries to return the PayloadType matching the given MIME type, clock rate and number of channels.
			/// </summary>
			/// <param name="mime">The MIME type to search a payload type for</param>
			/// <param name="clockRate">The clock rate to search a payload type for</param>
			/// <param name="channels">The number of channels to search a payload type for (can be -1 to ignore the number of channels in the search)</param>
			/// <returns>The PayloadType matching the parameters or null if not found</returns>
			PayloadType^ FindPayloadType(Platform::String^ mime, int clockRate, int channels);

			/// <summary>
			/// Get a chat room whose peer is the supplied address. If it does not exist yet, it will be created.
			/// </summary>
			/// <param name="address">An Address</param>
			Linphone::Native::ChatRoom^ GetChatRoom(Linphone::Native::Address^ address);

			/// <summary>
			/// Get a chat room for messaging from a sip uri like sip:joe@sip.linphone.org. If it does not exist yet, it will be created.
			/// </summary>
			/// <param name="to">The destination address for messages</param>
			Linphone::Native::ChatRoom^ GetChatRoomFromUri(Platform::String^ to);

			/// <summary>
			/// Builds an address according to the current proxy config.
			/// In case destination is not a sip address, the default proxy domain is automatically appended.
			/// </summary>
			/// <param name="destination">Either a SIP address or a username to which the default proxy domain will be appended</param>
			/// <returns>The created Address</returns>
			Address^ InterpretURL(Platform::String^ destination);

			/// <summary>
			/// Initiates an outgoing call given a destination as a string.
			/// Internally calls InterpretURL then Invite
			/// </summary>
			/// <param name="destination">Either a SIP address or a username to which the default proxy domain will be appended</param>
			/// <returns>The Call that has just been initiated</returns>
			/// <seealso cref="InviteAddress(Address^)"/>
			/// <seealso cref="InviteAddressWithParams(Address^, CallParams^)"/>
			Call^ Invite(Platform::String^ destination);

			/// <summary>
			/// Initiates an outgoing call given a destination Address.
			/// The Address can be constructed directly using CreateAddress or InterpretURL.
			/// </summary>
			/// <param name="destination">The Address of the destination to call</param>
			/// <returns>The Call that has just been initiated</returns>
			/// <seealso cref="Invite(String^)"/>
			/// <seealso cref="InviteAddressWithParams(Address^, CallParams^)"/>
			Call^ InviteAddress(Address^ destination);

			/// <summary>
			/// Initiates an outgoing call given a destination Address and the CallParams to be used.
			/// The Address can be constructed directly using CreateAddress or InterpretURL.
			/// </summary>
			/// <param name="destination">The Address of the destination to call</param>
			/// <param name="params">The CallParams to be used</param>
			/// <returns>The Call that has just been initiated</returns>
			/// <seealso cref="Invite(String^)"/>
			/// <seealso cref="InviteAddress(Address^)"/>
			Call^ InviteAddressWithParams(Address^ destination, CallParams^ params);

			/// <summary>
			/// Tells whether a media encryption scheme is supported by the Core engine.
			/// </summary>
			/// <param name="menc">The media encryption to check</param>
			/// <returns>true if supported, false otherwise</returns>
			Platform::Boolean IsMediaEncryptionSupported(Linphone::Native::MediaEncryption menc);

			/// <summary>
			/// Moves the local participant out of the conference.
			/// When the local participant is out of the conference, the remote participants can continue to talk normally.
			/// </summary>
			void LeaveConference();

			/// <summary>
			/// Pauses all the calls.
			/// </summary>
			/// <returns>A boolean value telling whether the calls have successfully been paused</returns>
			Platform::Boolean PauseAllCalls();

			/// <summary>
			/// Pauses a currently active call.
			/// </summary>
			/// <param name="call">The call to be paused</param>
			/// <returns>A boolean value telling whether the call has successfully been paused</returns>
			Platform::Boolean PauseCall(Call^ call);

			/// <summary>
			/// Tells whether a payload type is enabled or not.
			/// </summary>
			/// <param name="pt">The PayloadType that is to be checked</param>
			/// <returns>A boolean value telling whether the PayloadType is enabled</returns>
			bool PayloadTypeEnabled(PayloadType^ pt);

			/// <summary>
			/// Plays a DTMF signal to the speaker if not in call.
			/// Sending of the DTMF is done with SendDTMF(char16).
			/// </summary>
			/// <param name="number">The DTMF digit to be played</param>
			/// <param name="duration">The duration of the DTMF digit in ms, -1 for unlimited</param>
			/// <seealso cref="SendDTMF(char16)"/>
			void PlayDtmf(char16 number, int duration);

			/// <summary>
			/// Re-initiates registration if network is up.
			/// </summary>
			void RefreshRegisters();

			/// <summary>
			/// Removes a specific log from the Core.
			/// </summary>
			/// <param name="log">The call log to be removed</param>
			void RemoveCallLog(CallLog^ log);

			/// <summary>
			/// Removes a call from the conference.
			/// After removing the remote participant belonging to the supplied call, the call becomes a normal call in paused state.
			/// If one single remote participant is left alone together with the local user in the conference after the removal, then the conference is
			/// automatically transformed into a simple call in StreamsRunning state.
			/// The conference's resources are then automatically destroyed.
			/// In other words, unless LeaveConference() is explicitely called, the last remote participant of a conference is automatically
			/// put in a simple call in running state.
			/// </summary>
			/// <param name="call">A call that has been previously merged into the conference.</param>
			void RemoveFromConference(Call^ call);

			/// <summary>
			/// Reset the count of missed calls.
			/// </summary>
			void ResetMissedCallsCount();

			/// <summary>
			/// Resumes a currently paused call.
			/// </summary>
			/// <param name="call">The call to bbe resumed</param>
			/// <returns>A boolean value telling whether the call has successfully been resumed</returns>
			Platform::Boolean ResumeCall(Call^ call);

			/// <summary>
			/// Sends a DTMF signal to the remote party if in call.
			/// Playing the DTMF locally is done with PlayDTMF(char16, int).
			/// </summary>
			/// <param name="number">The DTMF digit to be sent</param>
			/// <seealso cref="PlayDTMF(char16, int)"/>
			void SendDtmf(char16 number);

			/// <summary>
			/// Sets the UDP port range from which to randomly select the port used for audio streaming.
			/// </summary>
			/// <param name="minP">The lower value of the UDP port range</param>
			/// <param name="maxP">The upper value of the UDP port range</param>
			void SetAudioPortRange(int minP, int maxP);

			/// <summary>
			/// Sets the preferred video size by telling its name.
			/// This applies only to the stream that is captured and sent to the remote party,
			/// since we accept all standard video size on the receive path.
			/// </summary>
			/// <param name="sizeName">The name of the preferred video size (eg. "vga")</param>
			void SetPreferredVideoSizeByName(Platform::String^ sizeName);

			/// <summary>
			/// Sets the user presence status.
			/// </summary>
			/// <param name="minuteAway">How long in away</param>
			/// <param name="alternativeContact">SIP URI to redirect call if the status is OnlineStatus.StatusMoved</param>
			/// <param name="status">The new presence status</param>
			void SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status);

			/// <summary>
			/// Set the contact to use if no ProxyConfig is configured.
			/// </summary>
			/// <param name="contact">The contact to use (a valid SIP uri)</param>
			void SetPrimaryContact(Platform::String^ contact);

			/// <summary>
			/// Sets the user agent string used in SIP messages.
			/// </summary>
			/// <param name="name">The user agent name to set</param>
			/// <param name="version">The user agent version to set</param>
			void SetUserAgent(Platform::String^ name, Platform::String^ version);

			/// <summary>
			/// Stops the current playing DTMF.
			/// </summary>
			void StopDtmf();

			/// <summary>
			/// Terminates all the calls.
			/// </summary>
			void TerminateAllCalls();

			/// <summary>
			/// Terminates the given call if running.
			/// </summary>
			/// <param name="call">The call to be terminated</param>
			void TerminateCall(Call^ call);

			/// <summary>
			/// Terminates the conference and the calls associated with it.
			/// All the calls that were merged to the conference are terminated, and the conference resources are destroyed.
			/// </summary>
			void TerminateConference();

			/// <summary>
			/// Updates the given call with the given params if the remote agrees.
			/// </summary>
			/// <param name="call">The call to update</param>
			/// <param name="params">The new CallParams to propose to the remote peer</param>
			void UpdateCall(Call^ call, CallParams^ params);

			/// <summary>
			/// Starts the upload of the log collection.
			/// </summary>
			void UploadLogCollection();

#if 0
			/// <summary>
			/// Starts an echo calibration of the sound devices, in order to find adequate settings for the echo canceller automatically.
			/// Status is notified to CoreListener::EcCalibrationStatus.
			/// </summary>
			void StartEchoCalibration();
#endif

		private:
			friend ref class Linphone::Native::Address;
			friend class Linphone::Native::Utils;

			void Init();

			::LinphoneCore *lc;
			Linphone::Native::CoreListener^ listener;
			LpConfig^ config;
			Windows::Foundation::IAsyncAction^ IterateWorkItem;
			Platform::Boolean isIterateEnabled;

			static OutputTraceLevel logLevel;
		};
	}
}