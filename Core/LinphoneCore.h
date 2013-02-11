#pragma once

#include "OnlineStatus.h"

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
		ref class PayloadType;
		ref class LpConfig;

		public enum class GlobalState : int
		{
			GlobalOff = 0,
			GlobalStartup = 1,
			GlobalOn = 2,
			GlobalShutdown = 3
		};

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

		public ref class LinphoneCore sealed
		{
		public:
			void ClearProxyConfigs();
			void AddProxyConfig(LinphoneProxyConfig^ proxyCfg);
			void SetDefaultProxyConfig(LinphoneProxyConfig^ proxyCfg);
			LinphoneProxyConfig^ GetDefaultProxyConfig();
			Windows::Foundation::Collections::IVector<LinphoneProxyConfig^>^ GetProxyConfigList();

			void ClearAuthInfos();
			void AddAuthInfo(LinphoneAuthInfo^ info);
			
			void Iterate();
			void Destroy();

			LinphoneAddress^ InterpretURL(Platform::String^ destination);
			LinphoneCall^ Invite(Platform::String^ destination);
			LinphoneCall^ InviteAddress(LinphoneAddress^ to);
			LinphoneCall^ InviteAddressWithParams(LinphoneAddress^ destination, LinphoneCallParams^ params);
			void TerminateCall(LinphoneCall^ call);
			LinphoneCall^ GetCurrentCall();
			LinphoneAddress^ GetRemoteAddress();
			Platform::Boolean IsInCall();
			Platform::Boolean IsIncomingInvitePending();
			void AcceptCall(LinphoneCall^ call);
			void AcceptCallWithParams(LinphoneCall^ call, LinphoneCallParams^ params);
			void AcceptCallUpdate(LinphoneCall^ call, LinphoneCallParams^ params);
			void DeferCallUpdate(LinphoneCall^ call);
			void UpdateCall(LinphoneCall^ call, LinphoneCallParams^ params);
			LinphoneCallParams^ CreateDefaultCallParameters();

			Windows::Foundation::Collections::IVector<LinphoneCallLog^>^ GetCallLogs();
			void ClearCallLogs();
			void RemoveCallLog(LinphoneCallLog^ log);

			void SetNetworkReachable(Platform::Boolean isReachable);
			Platform::Boolean IsNetworkReachable();

			void SetPlaybackGain(float gain);
			float GetPlaybackGain();
			void SetPlayLevel(int level);
			int GetPlayLevel();
			void MuteMic(Platform::Boolean isMuted);
			Platform::Boolean IsMicMuted();
			void EnableSpeaker(Platform::Boolean enable);
			Platform::Boolean IsSpeakerEnabled();

			void SendDTMF(char16 number);
			void PlayDTMF(char16 number, int duration);
			void StopDTMF();

			PayloadType^ FindPayloadType(Platform::String^ mime, int clockRate, int channels);
			PayloadType^ FindPayloadType(Platform::String^ mime, int clockRate);
			void EnablePayloadType(PayloadType^ pt, Platform::Boolean enable);
			Windows::Foundation::Collections::IVector<PayloadType^>^ GetAudioCodecs();

			void EnableEchoCancellation(Platform::Boolean enable);
			Platform::Boolean IsEchoCancellationEnabled();
			Platform::Boolean IsEchoLimiterEnabled();
			void StartEchoCalibration(Platform::Object^ data);
			void EnableEchoLimiter(Platform::Boolean enable);

			void SetSignalingTransportsPorts(Transports^ transports);
			Transports^ GetSignalingTransportsPorts();
			void EnableIPv6(Platform::Boolean enable);

			void SetPresenceInfo(int minuteAway, Platform::String^ alternativeContact, OnlineStatus status);

			void SetStunServer(Platform::String^ stun);
			Platform::String^ GetStunServer();

			void SetFirewallPolicy(FirewallPolicy policy);
			FirewallPolicy GetFirewallPolicy();

			void SetRootCA(Platform::String^ path);
			void SetUploadBandwidth(int bw);
			void SetDownloadBandwidth(int bw);
			void SetDownloadPTime(int ptime);
			void SetUploadPTime(int ptime);

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
			void TunnelAddServerAndMirror(Platform::String^ host, int port, int udpMirrorPort, int roundTripDelay);
			Platform::Boolean IsTunnelAvailable();

			void SetUserAgent(Platform::String^ name, Platform::String^ version);
			void SetCPUCount(int count);
			int GetMissedCallsCount();
			void ResetMissedCallsCount();
			void RefreshRegisters();
			Platform::String^ GetVersion();

			void SetAudioPort(int port);
			void SetAudioPortRange(int minP, int maxP);
			void SetIncomingTimeout(int timeout);
			void SetInCallTimeout(int timeout);
			void SetMicrophoneGain(float gain);
			void SetPrimaryContact(Platform::String^ displayName, Platform::String^ userName);
			void SetUseSipInfoForDTMFs(Platform::Boolean use);
			void SetUseRFC2833ForDTMFs(Platform::Boolean use);

			LpConfig^ GetConfig();
		private:
			friend ref class Linphone::Core::LinphoneCoreFactory;

			LinphoneCore();
			~LinphoneCore();
		};
	}
}