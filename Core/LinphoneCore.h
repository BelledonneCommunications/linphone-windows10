#pragma once

#include "Enums.h"
#include "LinphoneCoreListener.h"
#include "Utils.h"
#include "coreapi\linphonecore.h"

namespace Linphone
{
	namespace Core
	{
		ref class LinphoneCoreFactory;
		ref class LinphoneProxyConfig;
		ref class LinphoneAuthInfo;
		ref class LinphoneAddress;
		ref class LinphoneCall;
		ref class LinphoneCallParams;
		ref class LinphoneCallLog;
		ref class LinphoneCallStats;
		ref class PayloadType;
		ref class LpConfig;

		/// <summary>
		/// Signaling transports ports
		/// </summary>
		public ref class Transports sealed
		{
		public:
			Transports();
			Transports(Transports^ t);
			Platform::String^ ToString();

			property int UDP
            {
                int get();
            }
			property int TCP
            {
                int get();
            }
			property int TLS
            {
                int get();
            }
		private:
			int udp;
			int tcp;
			int tls;
		};
		
		/// <summary>
		/// Main object created by LinphoneCoreFactory::CreateLinphoneCore.
		/// </summary>
		public ref class LinphoneCore sealed
		{
		public:
			void SetContext(Platform::Object^ object);
			void ClearProxyConfigs();

			/// <summary>
			/// Adds a proxy config.
			/// This will start the registration of the proxy if registration is enabled.
			/// </summary>
			void AddProxyConfig(LinphoneProxyConfig^ proxyCfg);

			/// <summary>
			/// Sets the defaukt proxy.
			/// This default proxy must be part of the list of already entered LinphoneProxyConfig.
			/// Toggling it as default will make LinphoneCore use this identity associated with the proxy config in all incoming and outgoing calls.
			/// </summary>
			void SetDefaultProxyConfig(LinphoneProxyConfig^ proxyCfg);

			/// <summary>
			/// Gets the default proxy config, the one used to determine current identity.
			/// </summary>
			/// <returns>
			/// null if no default proxy config.
			/// </returns>
			LinphoneProxyConfig^ GetDefaultProxyConfig();

			/// <summary>
			/// Creates an empty proxy config.
			/// </summary>
			LinphoneProxyConfig^ CreateEmptyProxyConfig();

			Windows::Foundation::Collections::IVector<LinphoneProxyConfig^>^ GetProxyConfigList();

			void ClearAuthInfos();

			/// <summary>
			/// Adds authentication information to the LinphoneCore.
			/// This information will be used during all SIP transactions which requieres authentication.
			/// </summary>
			void AddAuthInfo(LinphoneAuthInfo^ info);

			/// <summary>
			/// Creates an empty auth info.
			/// </summary>
			LinphoneAuthInfo^ CreateAuthInfo(Platform::String^ username, Platform::String^ userid, Platform::String^ password, Platform::String^ ha1, Platform::String^ realm);
			
			/// <summary>
			/// Destroys LinphoneCore and free all underlying resources.
			/// </summary>
			void Destroy();

			/// <summary>
			/// Builds an address according to the current proxy config.
			/// In case destination is not a sip address, the default proxy domain is automatically appended.
			/// </summary>
			LinphoneAddress^ InterpretURL(Platform::String^ destination);

			/// <summary>
			/// Starts a call given a destination.
			/// Internally calls InterpretURL then Invite
			/// </summary>
			LinphoneCall^ Invite(Platform::String^ destination);

			/// <summary>
			/// Initiates an outgoing call given a destination LinphoneAddress.
			/// The LinphoneAddress can be constructed directly using LinphoneCoreFactory::CreateLinphoneAddress or InterpretURL.
			/// </summary>
			LinphoneCall^ InviteAddress(LinphoneAddress^ to);
			LinphoneCall^ InviteAddressWithParams(LinphoneAddress^ destination, LinphoneCallParams^ params);
			void TerminateCall(LinphoneCall^ call);

			/// <returns>
			/// nullptr if no one is in call.
			/// </returns>
			LinphoneCall^ GetCurrentCall();

			/// <returns>
			/// nullptr if no call engaged yet. 
			/// </returns>
			LinphoneAddress^ GetRemoteAddress();
			Platform::Boolean IsInCall();
			Platform::Boolean IsIncomingInvitePending();

			/// <summary>
			/// Accepts an incoming call.
			/// Basically the app is notified of incoming calls within the LinphoneCoreListener::CallState listener method.
			/// The application can later respond positively to the call using this method.
			/// </summary>
			void AcceptCall(LinphoneCall^ call);

			/// <summary>
			/// Accepts an incoming call.
			/// Basically the app is notified of incoming calls within the LinphoneCoreListener::CallState listener method.
			/// The application can later respond positively to the call using this method.
			/// </summary>
			void AcceptCallWithParams(LinphoneCall^ call, LinphoneCallParams^ params);

			/// <summary>
			/// Accepts call modifications initiated by other end.
			/// </summary>
			void AcceptCallUpdate(LinphoneCall^ call, LinphoneCallParams^ params);

			/// <summary>
			/// Prevent LinphoneCore from performing an automatic answer.
			/// </summary>
			void DeferCallUpdate(LinphoneCall^ call);
			void UpdateCall(LinphoneCall^ call, LinphoneCallParams^ params);
			LinphoneCallParams^ CreateDefaultCallParameters();
			
			/// <summary>
			/// This is a temporary workaround since the call to this method crash if it returns a IVector&lt;LinphoneCallLog^&gt;^.
			/// Returns a IList&lt;Object&gt; where each object is a LinphoneCallLog.
			/// </summary>
			Windows::Foundation::Collections::IVector<Platform::Object^>^ GetCallLogs();
			void ClearCallLogs();
			void RemoveCallLog(LinphoneCallLog^ log);

			/// <summary>
			/// This method is called by the application to notify the Linphone.Core library when network is reachable.
			/// Calling this method with true triggers Linphone to initiate a registration process for all proxy configs with parameter register set to enable.
			/// This method disables the automatic registration mode. It means you must call this method after each network state changes.
			/// </summary>
			void SetNetworkReachable(Platform::Boolean isReachable);
			Platform::Boolean IsNetworkReachable();

			/// <summary>
			/// Allow to control play level before entering the sound card.
			/// </summary>
			/// <param name="gain">level in db</param>
			void SetPlaybackGain(float gain);

			/// <summary>
			/// Returns the play level before entering the sound card (in db).
			/// </summary>
			float GetPlaybackGain();

			/// <summary>
			/// Sets the play level [0..100]
			/// </summary>
			void SetPlayLevel(int level);

			/// <summary>
			/// Gets playback level [0..100]
			/// </summary>
			/// <returns>
			/// -1 if it can't be determined
			/// </returns>
			int GetPlayLevel();

			/// <summary>
			/// Mutes or unmutes the local microphone.
			/// </summary>
			void MuteMic(Platform::Boolean isMuted);
			Platform::Boolean IsMicMuted();

			/// <summary>
			/// Initiates a dtmf signal if in call
			/// </summary>
			void SendDTMF(char16 number);

			/// <summary>
			/// Initiates a dtmf signal to the speaker if not in call.
			/// Sending of the dtmf is done in another function.
			/// </summary>
			/// <param name="duration">duration in ms, -1 for unlimited</param>
			void PlayDTMF(char16 number, int duration);

			/// <summary>
			/// Stops the current dtmf.
			/// </summary>
			void StopDTMF();

			PayloadType^ FindPayloadType(Platform::String^ mime, int clockRate, int channels);
			PayloadType^ FindPayloadType(Platform::String^ mime, int clockRate);

			/// <summary>
			/// Not implemented yet.
			/// </summary>
			void EnablePayloadType(PayloadType^ pt, Platform::Boolean enable);

			/// <summary>
			/// Returns the currently supported audio codecs, as PayloadType elements.
			/// </summary>
			Windows::Foundation::Collections::IVector<PayloadType^>^ GetAudioCodecs();

			void EnableEchoCancellation(Platform::Boolean enable);
			Platform::Boolean IsEchoCancellationEnabled();
			Platform::Boolean IsEchoLimiterEnabled();

			/// <summary>
			/// Starts an echo calibration of the sound devices, in order to find adequate settings for the echo canceller automatically.
			/// Status is notified to LinphoneCoreListener::EcCalibrationStatus.
			/// </summary>
			void StartEchoCalibration(Platform::Object^ data);
			void EnableEchoLimiter(Platform::Boolean enable);

			void SetSignalingTransportsPorts(Transports^ transports);
			Transports^ GetSignalingTransportsPorts();
			void EnableIPv6(Platform::Boolean enable);

			/// <summary>
			/// Sets the user presence status.
			/// </summary>
			/// <param name="minuteAway">how long in away</param>
			/// <param name="alternative_contact">SIP URI to redirect call if in state LinphoneStatusMoved.</param>
			void SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status);

			/// <summary>
			/// Specifies a STUN server to help firewall trasversal.
			/// </summary>
			/// <param name="stun">STUN server address and port, such as stun.linphone.org or stun.linphone.org:3478</param>
			void SetStunServer(Platform::String^ stun);
			Platform::String^ GetStunServer();

			/// <summary>
			/// Sets policy regarding workarounding NATs.
			/// </summary>
			void SetFirewallPolicy(FirewallPolicy policy);
			FirewallPolicy GetFirewallPolicy();

			/// <summary>
			/// Sets the file or folder containing trusted root CAs
			/// </summary>
			void SetRootCA(Platform::String^ path);
			void SetUploadBandwidth(int bw);
			void SetDownloadBandwidth(int bw);

			/// <summary>
			/// Sets audio packetization interval suggested for remote end (in milliseconds).
			/// </summary>
			void SetDownloadPTime(int ptime);

			/// <summary>
			/// Sets audio packetization interval sent to remote end (in milliseconds).
			/// </summary>
			void SetUploadPTime(int ptime);

			/// <summary>
			/// Enables signaling keep alive. Small UDP packet sent periodically to keep UDP NAT association.
			/// </summary>
			void EnableKeepAlive(Platform::Boolean enable);
			Platform::Boolean IsKeepAliveEnabled();

			void SetPlayFile(Platform::String^ path);
			Platform::Boolean PauseCall(LinphoneCall^ call);
			Platform::Boolean ResumeCall(LinphoneCall^ call);
			Platform::Boolean PauseAllCalls();
			Platform::Boolean IsInConference();
			Platform::Boolean EnterConference();
			void LeaveConference();
			void AddToConference(LinphoneCall^ call);
			void AddAllToConference();
			void RemoveFromConference();
			void TerminateConference();
			int GetConferenceSize();
			void TerminateAllCalls();
			Windows::Foundation::Collections::IVector<LinphoneCall^>^ GetCalls();
			int GetCallsNb();
			LinphoneCall^ FindCallFromUri(Platform::String^ uri);
			int GetMaxCalls();
			void SetMaxCalls(int max);

			Platform::Boolean IsMyself(Platform::String^ uri);

			/// <summary>
			/// Use this method to check calls state and forbid proposing actions which could result in an active call.
			/// Eg: don't start a new call if one is in outgoing ringing.
			/// Eg: don't merge to conference either as it could result in two active calls (conference and accepted call).
			/// </summary>
			Platform::Boolean IsSoundResourcesLocked();
			Platform::Boolean IsMediaEncryptionSupported(MediaEncryption menc);
			void SetMediaEncryption(MediaEncryption menc);
			MediaEncryption GetMediaEncryption();
			void SetMediaEncryptionMandatory(Platform::Boolean yesno);
			Platform::Boolean IsMediaEncryptionMandatory();

			void EnableTunnel(Platform::Boolean enable);
			void TunnelAutoDetect();
			void TunnelCleanServers();
			void TunnelSetHttpProxy(Platform::String^ host, int port, Platform::String^ username, Platform::String^ password);

			/// <param name="host">Tunnel server IP address</param>
			/// <param name="port">Tunnel server TLS port, recommended value is 443</param>
			/// <param name="udpMirrorPort">Remote port on the tunnel server side used to test UDP reachability</param>
			/// <param name="roundTripDelay">UDP packet round trip delay in ms considered as acceptable. Recommended value is 1000 ms.</param>
			void TunnelAddServerAndMirror(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay);
			Platform::Boolean IsTunnelAvailable();

			void SetUserAgent(Platform::String^ name, Platform::String^ version);
			void SetCPUCount(int count);
			int GetMissedCallsCount();
			void ResetMissedCallsCount();

			/// <summary>
			/// Re-initiates registration if network is up.
			/// </summary>
			void RefreshRegisters();
			Platform::String^ GetVersion();

			/// <summary>
			/// Sets the UDP port used for audio streaming.
			/// </summary>
			void SetAudioPort(int port);

			/// <summary>
			/// Sets the UDP port range from which to randomly select the port used for audio streaming.
			/// </summary>
			void SetAudioPortRange(int minP, int maxP);

			/// <summary>
			/// Sets the incoming call timeout in seconds.
			/// If an incoming call isn't answered for this timeout period, it is automatically declined.
			/// </summary>
			void SetIncomingTimeout(int timeout);

			/// <summary>
			/// Sets the call timeout in seconds.
			/// Once this time is elapsed (ringing included), the call is automatically hung up.
			/// </summary>
			void SetInCallTimeout(int timeout);
			void SetMicrophoneGain(float gain);

			/// <summary>
			/// Set username and display name to use if no LinphoneProxyConfig is configured.
			/// </summary>
			void SetPrimaryContact(Platform::String^ displayName, Platform::String^ userName);

			/// <summary>
			/// Enables/Disables the use of SIP INFO for DTMFs.
			/// </summary>
			void SetUseSipInfoForDTMFs(Platform::Boolean use);

			/// <summary>
			/// Enables/Disables the use of inband DTMFs.
			/// </summary>
			void SetUseRFC2833ForDTMFs(Platform::Boolean use);

			/// <summary>
			/// Returns the LpConfig object to read/write to the config file: useful if you wish to extend the config file with your own sections.
			/// </summary>
			LpConfig^ GetConfig();

			property LinphoneCoreListener^ CoreListener
            {
                LinphoneCoreListener^ get();
                void set(LinphoneCoreListener^ listener);
            }

		private:
			friend ref class Linphone::Core::LinphoneCoreFactory;

			LinphoneCoreListener^ listener;

			LinphoneCore(LinphoneCoreListener^ coreListener);
			void Init();
			~LinphoneCore();

			::LinphoneCore *lc;
			Windows::System::Threading::ThreadPoolTimer ^IterateTimer;
		};
	}
}