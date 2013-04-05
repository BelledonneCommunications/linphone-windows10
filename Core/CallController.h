#pragma once

#include <windows.phone.networking.voip.h>
#include "ApiLock.h"

namespace Linphone
{
    namespace Core
    {
        ref class Globals;
		ref class LinphoneCall;
 
        public delegate void IncomingCallViewDismissedCallback(); 

		/// <summary>
		/// Provides methods and properties related to Windows.Phone.Networking.Voip.VoipPhoneCall calls.
		/// </summary>
        public ref class CallController sealed
        {
        public:
            Windows::Phone::Networking::Voip::VoipPhoneCall^ OnIncomingCallReceived(LinphoneCall^ call, Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallViewDismissedCallback^ incomingCallViewDismissedCallback); 
 
			void EndCall(Windows::Phone::Networking::Voip::VoipPhoneCall^ call);

			Windows::Phone::Networking::Voip::VoipPhoneCall^ NewOutgoingCall(Platform::String^ number, Platform::String^ name);

        private:
            friend ref class Linphone::Core::Globals;
 
            Platform::String^ voipServiceName; 

			Platform::String^ callerNumber;
			
            Platform::String^ callInProgressPageUri; 
			
            // The URI to the contact image, jpg or png, 1mb max ! 
            Windows::Foundation::Uri^ defaultContactImageUri; 
 
            // The URI to the linphone icon, 128kb max ! 
            Windows::Foundation::Uri^ linphoneImageUri; 

            Windows::Foundation::Uri^ ringtoneUri; 

            Windows::Phone::Networking::Voip::VoipCallCoordinator^ callCoordinator; 

			Linphone::Core::LinphoneCall^ call;

			IncomingCallViewDismissedCallback^ onIncomingCallViewDismissed;

            void OnAcceptCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallAnswerEventArgs^ args); 
 
            void OnRejectCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallRejectEventArgs^ args); 
 
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallAnswerEventArgs^>^ acceptCallRequestedHandler; 
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallRejectEventArgs^>^ rejectCallRequestedHandler; 
            
            CallController();
            ~CallController();
        };
    }
}
