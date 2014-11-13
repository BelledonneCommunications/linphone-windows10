#pragma once

#include "Enums.h"
#include "LinphoneCoreListener.h"
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
	namespace Core
	{
		ref class LinphoneCoreFactory;
		ref class LinphoneCore;
		ref class LinphoneProxyConfig;
		ref class LinphoneAuthInfo;
		ref class LinphoneAddress;
		ref class LinphoneCall;
		ref class LinphoneCallParams;
		ref class LinphoneCallLog;
		ref class LinphoneCallStats;
		ref class LinphoneChatRoom;
		ref class PayloadType;
		ref class LpConfig;
		ref class Transports;
		ref class Tunnel;
		ref class VideoPolicy;
		ref class VideoSize;
		
		/// <summary>
		/// Main object created by LinphoneCoreFactory::CreateLinphoneCore.
		/// </summary>
		public ref class LinphoneCore sealed
		{
		public:
			/// <summary>
			/// Sets the global log level.
			/// </summary>
			/// <param name="logLevel">The log level to be set</param>
			static void SetLogLevel(OutputTraceLevel logLevel);

			/// <summary>
			/// Resets the log collection by removing the log files.
			/// </summary>
			static void ResetLogCollection();

			/// <summary>
			/// Removes all the proxy configs from LinphoneCore.
			/// </summary>
			void ClearProxyConfigs();

			/// <summary>
			/// Adds a proxy config.
			/// This will start the registration of the proxy if registration is enabled.
			/// </summary>
			/// <param name="proxyCfg">The proxy config to be added</param>
			void AddProxyConfig(LinphoneProxyConfig^ proxyCfg);

			/// <summary>
			/// Sets the default proxy.
			/// This default proxy must be part of the list of already entered LinphoneProxyConfig.
			/// Toggling it as default will make LinphoneCore use this identity associated with the proxy config in all incoming and outgoing calls.
			/// </summary>
			/// <param name="proxyCfg">The proxy config to set as default</param>
			void SetDefaultProxyConfig(LinphoneProxyConfig^ proxyCfg);

			/// <summary>
			/// Gets the default proxy config, the one used to determine current identity.
			/// </summary>
			/// <returns>The default proxy config or null if no default proxy config has been set</returns>
			LinphoneProxyConfig^ GetDefaultProxyConfig();

			/// <summary>
			/// Creates an empty proxy config.
			/// </summary>
			/// <returns>An empty proxy config</returns>
			LinphoneProxyConfig^ CreateEmptyProxyConfig();

			/// <summary>
			/// Gets the list of the current proxy configs.
			/// </summary>
			/// <returns>The list of proxy configs</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetProxyConfigList();

			/// <summary>
			/// Removes all the auth infos from LinphoneCore.
			/// </summary>
			void ClearAuthInfos();

			/// <summary>
			/// Adds authentication information to the LinphoneCore.
			/// This information will be used during all SIP transactions which requieres authentication.
			/// </summary>
			/// <param name="info">The authentication information to be added</param>
			void AddAuthInfo(LinphoneAuthInfo^ info);

			/// <summary>
			/// Creates an empty auth info.
			/// </summary>
			/// <param name="username">The authentication username</param>
			/// <param name="userid">The authentication userid</param>
			/// <param name="password">The authentication password</param>
			/// <param name="ha1">The authentication ha1</param>
			/// <param name="realm">The authentication realm</param>
			/// <param name="domain">The authentication domain</param>
			LinphoneAuthInfo^ CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm, Platform::String^ domain);

			/// <summary>
			/// Gets the current auth infos.
			/// </summary>
			/// <returns>The list of authentication informations</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetAuthInfos();
			
			/// <summary>
			/// Destroys LinphoneCore and free all underlying resources.
			/// </summary>
			void Destroy();

			/// <summary>
			/// Builds an address according to the current proxy config.
			/// In case destination is not a sip address, the default proxy domain is automatically appended.
			/// </summary>
			/// <param name="destination">Either a SIP address or a username to which the default proxy domain will be appended</param>
			/// <returns>The created LinphoneAddress</returns>
			LinphoneAddress^ InterpretURL(Platform::String^ destination);

			/// <summary>
			/// Initiates an outgoing call given a destination as a string.
			/// Internally calls InterpretURL then Invite
			/// </summary>
			/// <param name="destination">Either a SIP address or a username to which the default proxy domain will be appended</param>
			/// <returns>The LinphoneCall that has just been initiated</returns>
			/// <seealso cref="InviteAddress(LinphoneAddress^)"/>
			/// <seealso cref="InviteAddressWithParams(LinphoneAddress^, LinphoneCallParams^)"/>
			LinphoneCall^ Invite(Platform::String^ destination);

			/// <summary>
			/// Initiates an outgoing call given a destination LinphoneAddress.
			/// The LinphoneAddress can be constructed directly using LinphoneCoreFactory::CreateLinphoneAddress or InterpretURL.
			/// </summary>
			/// <param name="destination">The LinphoneAddress of the destination to call</param>
			/// <returns>The LinphoneCall that has just been initiated</returns>
			/// <seealso cref="Invite(String^)"/>
			/// <seealso cref="InviteAddressWithParams(LinphoneAddress^, LinphoneCallParams^)"/>
			LinphoneCall^ InviteAddress(LinphoneAddress^ destination);

			/// <summary>
			/// Initiates an outgoing call given a destination LinphoneAddress and the LinphoneCallParams to be used.
			/// The LinphoneAddress can be constructed directly using LinphoneCoreFactory::CreateLinphoneAddress or InterpretURL.
			/// </summary>
			/// <param name="destination">The LinphoneAddress of the destination to call</param>
			/// <param name="params">The LinphoneCallParams to be used</param>
			/// <returns>The LinphoneCall that has just been initiated</returns>
			/// <seealso cref="Invite(String^)"/>
			/// <seealso cref="InviteAddress(LinphoneAddress^)"/>
			LinphoneCall^ InviteAddressWithParams(LinphoneAddress^ destination, LinphoneCallParams^ params);

			/// <summary>
			/// Terminates the given call if running.
			/// </summary>
			/// <param name="call">The call to be terminated</param>
			void TerminateCall(LinphoneCall^ call);

			/// <summary>
			/// Declines an incoming call with a specific reason.
			/// </summary>
			void DeclineCall(LinphoneCall^ call, Reason reason);

			/// <summary>
			/// Gets the current active call.
			/// If there is only one ongoing call that is in the paused state, then there is no current call.
			/// </summary>
			/// <returns>The current activate call or null if there is no active call</returns>
			LinphoneCall^ GetCurrentCall();

			/// <summary>
			/// Tells whether there is at least one call running.
			/// </summary>
			/// <returns>true if at least a call is running, else returns false</returns>
			Platform::Boolean IsInCall();

			/// <summary>
			/// Tells whether there is an incoming call invite pending.
			/// </summary>
			/// <returns>true if there is an incoming call invite pending, else returns false</returns>
			Platform::Boolean IsIncomingInvitePending();

			/// <summary>
			/// Accepts an incoming call.
			/// Basically the app is notified of incoming calls within the LinphoneCoreListener::CallState listener method.
			/// The application can later respond positively to the call using this method.
			/// </summary>
			/// <param name="call">The incoming call to accept</param>
			/// <seealso cref="AcceptCallWithParams(LinphoneCall^, LinphoneCallParams^)"/>
			void AcceptCall(LinphoneCall^ call);

			/// <summary>
			/// Accepts an incoming call.
			/// Basically the app is notified of incoming calls within the LinphoneCoreListener::CallState listener method.
			/// The application can later respond positively to the call using this method.
			/// </summary>
			/// <param name="call">The incoming call to accept</param>
			/// <param name="params">The LinphoneCallParams to use for the accepted call</param>
			/// <seealso cref="AcceptCall(LinphoneCall^)"/>
			void AcceptCallWithParams(LinphoneCall^ call, LinphoneCallParams^ params);

			/// <summary>
			/// Accepts call modifications initiated by other end.
			/// </summary>
			/// <param name="call">The incoming call to accept</param>
			/// <param name="params">The new local LinphoneCallParams to use</param>
			void AcceptCallUpdate(LinphoneCall^ call, LinphoneCallParams^ params);

			/// <summary>
			/// Prevent LinphoneCore from performing an automatic answer when receiving call modifications from the other end of the call.
			/// </summary>
			/// <param name="call">The call for which a modification from the other end has been notified</param>
			void DeferCallUpdate(LinphoneCall^ call);

			/// <summary>
			/// Updates the given call with the given params if the remote agrees.
			/// </summary>
			/// <param name="call">The call to update</param>
			/// <param name="params">The new LinphoneCallParams to propose to the remote peer</param>
			void UpdateCall(LinphoneCall^ call, LinphoneCallParams^ params);

			/// <summary>
			/// Gets a default set of LinphoneCallParams.
			/// </summary>
			/// <returns>The default set of LinphoneCallParams</returns>
			LinphoneCallParams^ CreateDefaultCallParameters();
			
			/// <summary>
			/// Gets the list of call logs of the LinphoneCore.
			/// </summary>
			/// <returns>A list of LinphoneCallLog objects as Platform::Object</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetCallLogs();

			/// <summary>
			/// Removes all call logs from the LinphoneCore.
			/// </summary>
			void ClearCallLogs();

			/// <summary>
			/// Removes a specific log from the LinphoneCore.
			/// </summary>
			/// <param name="log">The call log to be removed</param>
			void RemoveCallLog(LinphoneCallLog^ log);

			/// <summary>
			/// This method is called by the application to notify the Linphone.Core library when network is reachable.
			/// Calling this method with true triggers Linphone to initiate a registration process for all proxy configs with parameter register set to enable.
			/// This method disables the automatic registration mode. It means you must call this method after each network state changes.
			/// </summary>
			/// <param name="isReachable">A boolean value telling whether the network is reachable</param>
			void SetNetworkReachable(Platform::Boolean isReachable);

			/// <summary>
			/// Tells whether the network has been set as reachable or not.
			/// </summary>
			/// <returns>true if the network has been set as reachable, else returns false</returns>
			Platform::Boolean IsNetworkReachable();

			/// <summary>
			/// Sets the microphone gain.
			/// </summary>
			/// <param name="gain">The microphone gain to set in dB</param>
			void SetMicrophoneGain(float gain);

			/// <summary>
			/// Allow to control play level before entering the sound card.
			/// </summary>
			/// <param name="gain">Level in db</param>
			void SetPlaybackGain(float gain);

			/// <summary>
			/// Gets the play level before entering the sound card (in dB).
			/// </summary>
			/// <returns>The play level in dB</returns>
			float GetPlaybackGain();

			/// <summary>
			/// Sets the playback level.
			/// </summary>
			/// <param name="level">The playback level in a scale from 0 to 100</param>
			void SetPlayLevel(int level);

			/// <summary>
			/// Gets playback level.
			/// </summary>
			/// <returns>The playback level in a scale from 0 to 100 or -1 if it can't be determined</returns>
			int GetPlayLevel();

			/// <summary>
			/// Mutes or unmutes the local microphone.
			/// </summary>
			/// <param name="isMuted">A boolean value telling whether to mute or unmute the microphone</param>
			void MuteMic(Platform::Boolean isMuted);

			/// <summary>
			/// Tells whether the microphone is muted or not.
			/// </summary>
			/// <returns>true if the microphone is muted, false otherwise</returns>
			Platform::Boolean IsMicMuted();

			/// <summary>
			/// Sends a DTMF signal to the remote party if in call.
			/// Playing the DTMF locally is done with PlayDTMF(char16, int).
			/// </summary>
			/// <param name="number">The DTMF digit to be sent</param>
			/// <seealso cref="PlayDTMF(char16, int)"/>
			void SendDTMF(char16 number);

			/// <summary>
			/// Plays a DTMF signal to the speaker if not in call.
			/// Sending of the DTMF is done with SendDTMF(char16).
			/// </summary>
			/// <param name="number">The DTMF digit to be played</param>
			/// <param name="duration">The duration of the DTMF digit in ms, -1 for unlimited</param>
			/// <seealso cref="SendDTMF(char16)"/>
			void PlayDTMF(char16 number, int duration);

			/// <summary>
			/// Stops the current playing DTMF.
			/// </summary>
			void StopDTMF();

			/// <summary>
			/// Tries to return the PayloadType matching the given MIME type, clock rate and number of channels.
			/// </summary>
			/// <param name="mime">The MIME type to search a payload type for</param>
			/// <param name="clockRate">The clock rate to search a payload type for</param>
			/// <param name="channels">The number of channels to search a payload type for</param>
			/// <returns>The PayloadType matching the parameters or null if not found</returns>
			PayloadType^ FindPayloadType(Platform::String^ mime, int clockRate, int channels);

			/// <summary>
			/// Tries to return the PayloadType matching the given MIME type and clock rate.
			/// </summary>
			/// <param name="mime">The MIME type to search a payload type for</param>
			/// <param name="clockRate">The clock rate to search a payload type for</param>
			/// <returns>The PayloadType matching the parameters or null if not found</returns>
			PayloadType^ FindPayloadType(Platform::String^ mime, int clockRate);

			/// <summary>
			/// Tells whether a payload type is enabled or not.
			/// </summary>
			/// <param name="pt">The PayloadType that is to be checked</param>
			/// <returns>A boolean value telling whether the PayloadType is enabled</returns>
			bool PayloadTypeEnabled(PayloadType^ pt);

			/// <summary>
			/// Enables or disables a payload type.
			/// The payload type to enable/disable can be retrieved using FindPayloadType(String, int, int).
			/// </summary>
			/// <param name="pt">The PayloadType to enable/disable</param>
			/// <param name="enable">A boolean value telling whether to enable or disable the PayloadType</param>
			void EnablePayloadType(PayloadType^ pt, Platform::Boolean enable);

			/// <summary>
			/// Gets the currently supported audio codecs, as PayloadType elements.
			/// </summary>
			/// <returns>A list of PayloadType objects as Platform::Object</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetAudioCodecs();

			/// <summary>
			/// Enables or disables the echo cancellation.
			/// </summary>
			/// <param name="enable">A boolean value telling whether to enable or disable the echo cancellation</param>
			void EnableEchoCancellation(Platform::Boolean enable);

			/// <summary>
			/// Tells whether the echo cancellation is enabled or not.
			/// </summary>
			/// <returns>A boolean value telling whether the echo cancellation is enabled</returns>
			Platform::Boolean IsEchoCancellationEnabled();

			/// <summary>
			/// Tells whether the echo limiter is enabled or not.
			/// </summary>
			/// <returns>A boolean value telling whether the echo limiter is enabled</returns>
			Platform::Boolean IsEchoLimiterEnabled();

			/// <summary>
			/// Starts an echo calibration of the sound devices, in order to find adequate settings for the echo canceller automatically.
			/// Status is notified to LinphoneCoreListener::EcCalibrationStatus.
			/// </summary>
			void StartEchoCalibration();

			/// <summary>
			/// Enables or disables the echo limiter.
			/// </summary>
			/// <param name="enable">A boolean value telling whether to enable the echo limiter</param>
			void EnableEchoLimiter(Platform::Boolean enable);

			/// <summary>
			/// Sets the ports to be used for each transport (UDP, TCP, TLS).
			/// </summary>
			/// <param name="transports">The transports to be used</param>
			void SetSignalingTransportsPorts(Transports^ transports);

			/// <summary>
			/// Gets the currently used ports for each transport (UDP, TCP, TLS).
			/// </summary>
			/// <returns>The transports that are currently used</returns>
			Transports^ GetSignalingTransportsPorts();

			/// <summary>
			/// Enables or disables IPv6 support.
			/// </summary>
			/// <param name="enable">A boolean value telling whether to enable IPv6 support</param>
			void EnableIPv6(Platform::Boolean enable);

			/// <summary>
			/// Sets the user presence status.
			/// </summary>
			/// <param name="minuteAway">How long in away</param>
			/// <param name="alternativeContact">SIP URI to redirect call if the status is OnlineStatus.StatusMoved</param>
			/// <param name="status">The new presence status</param>
			void SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status);

			/// <summary>
			/// Specifies a STUN server to help firewall traversal.
			/// </summary>
			/// <param name="stun">STUN server address and port, such as stun.linphone.org or stun.linphone.org:3478</param>
			void SetStunServer(Platform::String^ stun);

			/// <summary>
			/// Gets the address of the configured STUN server if any.
			/// </summary>
			/// <returns>The currently used STUN server</returns>
			Platform::String^ GetStunServer();

			/// <summary>
			/// Sets policy regarding workarounding NATs.
			/// </summary>
			/// <param name="policy">The policy to be set</param>
			void SetFirewallPolicy(FirewallPolicy policy);

			/// <summary>
			/// Gets the policy regarding workarounding NATs.
			/// </summary>
			/// <returns>The current policy</returns>
			FirewallPolicy GetFirewallPolicy();

			/// <summary>
			/// Sets the file or folder containing trusted root CAs.
			/// </summary>
			/// <param name="path">The path of the file or folder containing the root CAs</param>
			void SetRootCA(Platform::String^ path);

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
			/// <param name="ptime">The audio packetization interval to be set</param>
			void SetDownloadPTime(int ptime);

			/// <summary>
			/// Sets audio packetization interval sent to remote end (in milliseconds).
			/// A value of zero means that ptime is not specified.
			/// </summary>
			/// <param name="ptime">The audio packetization interval to be set</param>
			void SetUploadPTime(int ptime);

			/// <summary>
			/// Enables signaling keep alive. Small UDP packet sent periodically to keep UDP NAT association.
			/// </summary>
			/// <param name="enable">A boolean value telling whether to enable keep alive</param>
			void EnableKeepAlive(Platform::Boolean enable);

			/// <summary>
			/// Tells whether to send keep alives or not.
			/// </summary>
			/// <returns>true if the signaling keep alive is enabled, else returns false</returns>
			Platform::Boolean IsKeepAliveEnabled();

			/// <summary>
			/// Sets the file to be played when putting a call on hold.
			/// </summary>
			/// <param name="path">The path to the WAV file to be played</param>
			void SetPlayFile(Platform::String^ path);

			/// <summary>
			/// Pauses a currently active call.
			/// </summary>
			/// <param name="call">The call to be paused</param>
			/// <returns>A boolean value telling whether the call has successfully been paused</returns>
			Platform::Boolean PauseCall(LinphoneCall^ call);

			/// <summary>
			/// Resumes a currently paused call.
			/// </summary>
			/// <param name="call">The call to bbe resumed</param>
			/// <returns>A boolean value telling whether the call has successfully been resumed</returns>
			Platform::Boolean ResumeCall(LinphoneCall^ call);

			/// <summary>
			/// Pauses all the calls.
			/// </summary>
			/// <returns>A boolean value telling whether the calls have successfully been paused</returns>
			Platform::Boolean PauseAllCalls();

			/// <summary>
			/// Tells whether a conference is currently ongoing.
			/// </summary>
			/// <returns>true if a conference is ongoing, false otherwise</returns>
			Platform::Boolean IsInConference();

			/// <summary>
			/// Makes the local participant to join the conference. 
			/// Typically, the local participant is by default always part of the conference when joining an active call into a conference.
			/// However, by calling LeaveConference() and EnterConference() the application can decide to temporarily
			/// move out and in the local participant from the conference.
			/// </summary>
			/// <returns>A boolean value telling whether the conference has successfully been entered</returns>
			Platform::Boolean EnterConference();

			/// <summary>
			/// Moves the local participant out of the conference.
			/// When the local participant is out of the conference, the remote participants can continue to talk normally.
			/// </summary>
			void LeaveConference();

			/// <summary>
			/// Merges a call into a conference.
			/// If this is the first call that enters the conference, the virtual conference will be created automatically.
			/// If the local user was actively part of the call (ie not in paused state), then the local user is automatically entered into the conference.
			/// If the call was in paused state, then it is automatically resumed when entering into the conference.
			/// </summary>
			/// <param name="call">An established call, either in StreamsRunning or Paused state</param>
			void AddToConference(LinphoneCall^ call);

			/// <summary>
			/// Adds all calls into a conference.
			/// Merge all established calls (either in StreamsRunning or Paused) into a conference.
			/// </summary>
			void AddAllToConference();

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
			void RemoveFromConference(LinphoneCall^ call);

			/// <summary>
			/// Terminates the conference and the calls associated with it.
			/// All the calls that were merged to the conference are terminated, and the conference resources are destroyed.
			/// </summary>
			void TerminateConference();

			/// <summary>
			/// Returns the number of participants to the conference, including the local participant.
			/// Typically, after merging two calls into the conference, there is total of 3 participants:
			/// the local participant (or local user), and two remote participants that were the destinations of the two previously establised calls.
			/// </summary>
			/// <returns>The number of participants to the conference</returns>
			int GetConferenceSize();

			/// <summary>
			/// Terminates all the calls.
			/// </summary>
			void TerminateAllCalls();

			/// <summary>
			/// Gets the current list of calls.
			/// </summary>
			/// <returns>A list of LinphoneCall objects as Platform::Object</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetCalls();

			/// <summary>
			/// Gets the number of calls.
			/// </summary>
			/// <returns>The current number of calls</returns>
			int GetCallsNb();

			/// <summary>
			/// Searches from the list of current calls if a remote address match uri.
			/// </summary>
			/// <param name="uri">URI to search for in the list of current calls</param>
			/// <returns>The matching LinphoneCall if found, null otherwise</returns>
			LinphoneCall^ FindCallFromUri(Platform::String^ uri);

			/// <summary>
			/// Gets the maximum number of simultaneous calls the LinphoneCore can manage at a time. All new call above this limit are declined with a busy answer.
			/// </summary>
			/// <returns>The maximum number of simultaneous calls</returns>
			int GetMaxCalls();

			/// <summary>
			/// Sets the maximum number of simultaneous calls the LinphoneCore can manage at a time. All new call above this limit are declined with a busy answer.
			/// </summary>
			/// <param name="max">The new maximum number of simultaneous calls</param>
			void SetMaxCalls(int max);

			/// <summary>
			/// Tells whether a URI corresponds to my identity.
			/// </summary>
			/// <returns>true if the URI corresponds to my identity, false otherwise</returns>
			Platform::Boolean IsMyself(Platform::String^ uri);

			/// <summary>
			/// Use this method to check calls state and forbid proposing actions which could result in an active call.
			/// Eg: don't start a new call if one is in outgoing ringing.
			/// Eg: don't merge to conference either as it could result in two active calls (conference and accepted call).
			/// </summary>
			/// <returns>true if sound resources are currently being used, false otherwise</returns>
			Platform::Boolean IsSoundResourcesLocked();

			/// <summary>
			/// Tells whether a media encryption scheme is supported by the LinphoneCore engine.
			/// </summary>
			/// <param name="menc">The media encryption to check</param>
			/// <returns>true if supported, false otherwise</returns>
			Platform::Boolean IsMediaEncryptionSupported(MediaEncryption menc);

			/// <summary>
			/// Chooses the media encryption policy to be used for RTP packets.
			/// </summary>
			/// <param name="menc">The media encryption to use</param>
			void SetMediaEncryption(MediaEncryption menc);

			/// <summary>
			/// Gets the currently used media encryption.
			/// </summary>
			/// <returns>The currently used media encryption</returns>
			MediaEncryption GetMediaEncryption();

			/// <summary>
			/// Defines Linphone behaviour when encryption parameters negociation fails on outgoing call.
			/// </summary>
			/// <param name="yesno">If set to true call will fail; if set to false will resend an INVITE with encryption disabled</param>
			void SetMediaEncryptionMandatory(Platform::Boolean yesno);

			/// <summary>
			/// Tells the current behaviour when encryption parameters negociation fails on outgoing call.
			/// </summary>
			/// <returns>true means that call will fail; false means that will resend an INVITE with encryption disabled</returns>
			Platform::Boolean IsMediaEncryptionMandatory();

			/// <summary>
			/// Tells whether tunnel support is available.
			/// </summary>
			/// <returns>true if tunnel support is available, false otherwise</returns>
			Platform::Boolean IsTunnelAvailable();

			/// <summary>
			/// Gets the tunnel instance if available.
			/// </summary>
			/// <returns>The tunnel instance if available, null if not</returns>
			Tunnel^ GetTunnel();

			/// <summary>
			/// Sets the user agent string used in SIP messages.
			/// </summary>
			/// <param name="name">The user agent name to set</param>
			/// <param name="version">The user agent version to set</param>
			void SetUserAgent(Platform::String^ name, Platform::String^ version);

			/// <summary>
			/// Declares how many CPUs (cores) are available on the platform.
			/// </summary>
			/// <param name="count">The number of available CPUs</param>
			void SetCPUCount(int count);

			/// <summary>
			/// Gets the number of missed calls since last reset.
			/// </summary>
			/// <returns>The number of missed calls</returns>
			int GetMissedCallsCount();

			/// <summary>
			/// Reset the count of missed calls.
			/// </summary>
			void ResetMissedCallsCount();

			/// <summary>
			/// Re-initiates registration if network is up.
			/// </summary>
			void RefreshRegisters();

			/// <summary>
			/// Gets liblinphone's version as a string.
			/// </summary>
			/// <returns>liblinphone's version as a string</returns>
			Platform::String^ GetVersion();

			/// <summary>
			/// Sets the UDP port used for audio streaming.
			/// </summary>
			/// <param name="port">The UDP port to be used for audio streaming</param>
			void SetAudioPort(int port);

			/// <summary>
			/// Sets the UDP port range from which to randomly select the port used for audio streaming.
			/// </summary>
			/// <param name="minP">The lower value of the UDP port range</param>
			/// <param name="maxP">The upper value of the UDP port range</param>
			void SetAudioPortRange(int minP, int maxP);

			/// <summary>
			/// Sets the incoming call timeout in seconds.
			/// If an incoming call isn't answered for this timeout period, it is automatically declined.
			/// </summary>
			/// <param name="timeout">The new incoming call timeout in seconds</param>
			void SetIncomingTimeout(int timeout);

			/// <summary>
			/// Sets the call timeout in seconds.
			/// Once this time is elapsed (ringing included), the call is automatically hung up.
			/// </summary>
			/// <param name="timeout">The new call timeout in seconds</param>
			void SetInCallTimeout(int timeout);

			/// <summary>
			/// Set username and display name to use if no LinphoneProxyConfig is configured.
			/// </summary>
			/// <param name="displayName">The display name to use</param>
			/// <param name="userName">The username to use</param>
			void SetPrimaryContact(Platform::String^ displayName, Platform::String^ userName);

			/// <summary>
			/// Tells whether SIP INFO is used to send DTMFs.
			/// </summary>
			/// <returns>true if SIP INFO is used, false otherwise</returns>
			Platform::Boolean GetUseSipInfoForDTMFs();

			/// <summary>
			/// Tells whether RFC2833 is used to send DTMFs.
			/// </summary>
			/// <returns>true, if RFC2833 is used, false otherwise</returns>
			Platform::Boolean GetUseRFC2833ForDTMFs();

			/// <summary>
			/// Enables/Disables the use of SIP INFO to send DTMFs.
			/// </summary>
			/// <param name="use">A boolean value telling whether to use SIP INFO to send DTMFs</param>
			void SetUseSipInfoForDTMFs(Platform::Boolean use);

			/// <summary>
			/// Enables/Disables the use of RFC2833 to send DTMFs.
			/// </summary>
			/// <param name="use">A boolean value telling whether to use RFC2833 to send DTMFs</param>
			void SetUseRFC2833ForDTMFs(Platform::Boolean use);

			/// <summary>
			/// Gets the LpConfig object to read/write to the config file: useful if you wish to extend the config file with your own sections.
			/// </summary>
			/// <returns>The LpConfig used by the LinphoneCore</returns>
			LpConfig^ GetConfig();

			/// <summary>
			/// Tells whether video support has been compiled.
			/// </summary>
			/// <returns> true, if video is supported, false if it is not supported</returns>
			Platform::Boolean IsVideoSupported();

			/// <summary>
			/// Gets the current video policy.
			/// </summary>
			/// <returns>The current video policy</returns>
			VideoPolicy^ GetVideoPolicy();

			/// <summary>
			/// Sets the policy for video.
			/// </summary>
			/// <param name="policy">The policy to use for video</param>
			void SetVideoPolicy(VideoPolicy^ policy);

			/// <summary>
			/// Gets the list of video sizes that are supported.
			/// </summary>
			/// <returns>A list of VideoSize objects as Platform::Object</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetSupportedVideoSizes();

			/// <summary>
			/// Gets the preferred video size.
			/// </summary>
			/// <returns>The preferred video size</returns>
			VideoSize^ GetPreferredVideoSize();

			/// <summary>
			/// Gets the preferred video size name.
			/// </summary>
			/// <returns>The preferred video size name</returns>
			Platform::String^ GetPreferredVideoSizeName();

			/// <summary>
			/// Sets the preferred video size.
			/// This applies only to the stream that is captured and sent to the remote party,
			/// since we accept all standard video size on the receive path.
			/// </summary>
			/// <param name="size">The VideoSize to set as preferred (one of the VideoSize from GetSupportedVideoSizes())</param>
			void SetPreferredVideoSize(VideoSize^ size);

			/// <summary>
			/// Sets the preferred video size.
			/// This applies only to the stream that is captured and sent to the remote party,
			/// since we accept all standard video size on the receive path.
			/// </summary>
			/// <param name="width">The width of the preferred video size</param>
			/// <param name="height">The height of the preferred video size</param>
			void SetPreferredVideoSize(int width, int height);

			/// <summary>
			/// Sets the preferred video size by telling its name.
			/// This applies only to the stream that is captured and sent to the remote party,
			/// since we accept all standard video size on the receive path.
			/// </summary>
			/// <param name="sizeName">The name of the preferred video size (eg. "vga")</param>
			void SetPreferredVideoSizeByName(Platform::String^ sizeName);

			/// <summary>
			/// Gets the list of video devices.
			/// </summary>
			/// <returns>A list of device names as Platform::String^</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetVideoDevices();

			/// <summary>
			/// Gets the name of the currently active video device.
			/// </summary>
			/// <returns>The name of the currently active video device</returns>
			Platform::String^ GetVideoDevice();

			/// <summary>
			/// Sets the active video device.
			/// </summary>
			/// <param name="device">The name of the device to set as active</param>
			void SetVideoDevice(Platform::String^ device);

			/// <summary>
			/// Gets the currently supported video codecs, as PayloadType elements.
			/// </summary>
			/// <returns>A list of PayloadType objects as Platform::Object</returns>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetVideoCodecs();

			/// <summary>
			/// Deprecated! 
			/// Tells whether video is enabled or not.
			/// </summary>
			/// <returns>true if video is enabled, false otherwise</returns>
			Platform::Boolean IsVideoEnabled();

			/// <summary>
			/// Deprecated!
			/// Enables video.
			/// This method does not have any effect during calls. It just indicates LinphoneCore to
			/// initiate future calls with video or not. The two boolean parameters indicate in which
			/// direction video is enabled. Setting both to false disables video entirely.
			/// </summary>
			/// <param name="enableCapture">Indicates whether video capture is enabled</param>
			/// <param name="enableDisplay">Indicates whether video display should be shown</param>
			void EnableVideo(Platform::Boolean enableCapture, Platform::Boolean enableDisplay);

			/// <summary>
			/// Tells whether video capture is enabled.
			/// </summary>
			/// <returns>TRUE if video capture is enabled, FALSE if disabled.</returns>
			Platform::Boolean IsVideoCaptureEnabled();

			/// <summary>
			/// Tells whether video display is enabled.
			/// </summary>
			/// <returns>TRUE if video capture is enabled, FALSE if disabled.</returns>
			Platform::Boolean IsVideoDisplayEnabled();

			/// <summary>
			/// Enable or disable video capture.
			///This function does not have any effect during calls. It just indicates the LinphoneCore to initiate future calls with video capture or not.
			/// </summary>
			/// <param name="enable">TRUE to enable video capture, FALSE to disable it.</param>
			void EnableVideoCapture(Platform::Boolean enable);

			/// <summary>
			/// Enable or disable video display.
			/// This function does not have any effect during calls. It just indicates the LinphoneCore to initiate future calls with video display or not.
			/// </summary>
			/// <param name="enable">TRUE to enable video display, FALSE to disable it.</param>
			void EnableVideoDisplay(Platform::Boolean enable);

			/// <summary>
			/// Gets the native video window id.
			/// </summary>
			/// <returns>The native video window id</returns>
			int GetNativeVideoWindowId();

			/// <summary>
			/// Gets the camera sensor rotation in degrees.
			/// </summary>
			/// <returns>The number of degrees the camera sensor is rotated related to the screen  (0 to 360) or -1 if it could not be retrieved</returns>
			int GetCameraSensorRotation();

			/// <summary>
			/// Tells whether self view is enabled or not.
			/// </summary>
			/// <returns>true if self view is enabled, false otherwise</returns>
			Platform::Boolean IsSelfViewEnabled();

			/// <summary>
			/// Enables or disables self view during calls.
			/// Self-view refers to having local webcam image inserted in corner of the video window during calls.
			/// </summary>
			/// <param name="enable">true to enable self view, false to disable it</param>
			void EnableSelfView(Platform::Boolean enable);

			/// <summary>
			/// Get a chat room whose peer is the supplied address. If it does not exist yet, it will be created.
			/// </summary>
			/// <param name="address">A LinphoneAddress</param>
			Linphone::Core::LinphoneChatRoom^ GetChatRoom(Linphone::Core::LinphoneAddress^ address);

			/// <summary>
			/// Get a chat room for messaging from a sip uri like sip:joe@sip.linphone.org. If it does not exist yet, it will be created.
			/// </summary>
			/// <param name="to">The destination address for messages</param>
			Linphone::Core::LinphoneChatRoom^ GetChatRoomFromUri(Platform::String^ to);

			/// <summary>
			/// Sets the log collection upload server URL.
			/// </summary>
			/// <param name="url">The URL of the log collection upload server</param>
			void SetLogCollectionUploadServerUrl(Platform::String^ url);

			/// <summary>
			/// Starts the upload of the log collection.
			/// </summary>
			void UploadLogCollection();

			/// <summary>
			/// Tells the core the device current orientation. This can be used by capture filters
			/// on mobile devices to select between portrait / landscape mode and to produce properly
			///	oriented images.The exact meaning of the value in rotation if left to each device
			///	specific implementations.
			/// </summary>
			void SetDeviceRotation(int rotation);

			/// <summary>
			/// Notifies the system that the call needs to be muted/unmuted.
			/// </summary>
			/// <param name="isMuted">The new mute state</param>
			void NotifyMute(bool isMuted);

			/// <summary>
			/// Sets the path to the database file used to store chat messages
			/// </summary>
			/// <param name="chatDatabasePath">The path to the database file</param>
			void SetChatDatabasePath(Platform::String^ chatDatabasePath);

			/// <summary>
			/// Gets the list of the created chatrooms
			/// </summary>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetChatRooms();
			
			/// <summary>
			/// The LinphoneCoreListener that handles the events coming from the core.
			/// </summary>
			property LinphoneCoreListener^ CoreListener
            {
                LinphoneCoreListener^ get();
                void set(LinphoneCoreListener^ listener);
            }

			/// <summary>
			/// Set it to true to start the iterate, set it to false to stop it. 
			/// Is disabled by default.
			/// </summary>
			property Platform::Boolean IterateEnabled
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

		private:
			friend ref class Linphone::Core::LinphoneCoreFactory;
			friend class Linphone::Core::Utils;

			LinphoneCore(LinphoneCoreListener^ coreListener);
			LinphoneCore(LinphoneCoreListener^ coreListener, LpConfig^ config);
			void Init();
			~LinphoneCore();

			::LinphoneCore *lc;
			LinphoneCoreListener^ listener;
			LpConfig^ config;
			Windows::System::Threading::ThreadPoolTimer ^IterateTimer;
			Platform::Boolean isIterateEnabled;
		};
	}
}