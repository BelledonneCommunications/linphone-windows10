#pragma once

#include <windows.phone.networking.voip.h>
#include "CallControllerListener.h"
#include "ApiLock.h"

namespace Linphone
{
    namespace BackEnd
    {
        ref class Globals;
 
        // A method that is called back when the incoming call dialog has been dismissed. 
        // This callback is used to complete the incoming call agent. 
        public delegate void IncomingCallViewDismissedCallback(); 

        // A class that provides methods and properties related to VoIP calls.
        // It wraps Windows.Phone.Networking.Voip.VoipCallCoordinator, and provides app-specific call functionality.
        public ref class CallController sealed
        {
        public:
			// Start processing an incoming call. Called by managed code in this process (the VoIP agent host process). 
            // Returns true if the incoming call processing was started, false otherwise. 
            bool OnIncomingCallReceived(Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallViewDismissedCallback^ incomingCallViewDismissedCallback); 
 
			void EndCurrentCall();

			Platform::Boolean NewOutgoingCall(Platform::String^ number, Platform::String^ name);

			void SetCallControllerListener(CallControllerListener^ listener);
        private:
            friend ref class Linphone::BackEnd::Globals;

			CallControllerListener^ callControllerListener;
 
            Platform::String^ voipServiceName; 

			Platform::String^ callerNumber;
			
            // The relative URI to the call-in-progress page 
            Platform::String^ callInProgressPageUri; 
			
            // The URI to the contact image, jpg or png, 1mb max ! 
            Windows::Foundation::Uri^ defaultContactImageUri; 
 
            // The URI to the linphone icon, 128kb max ! 
            Windows::Foundation::Uri^ linphoneImageUri; 

            Windows::Foundation::Uri^ ringtoneUri; 

            Windows::Phone::Networking::Voip::VoipCallCoordinator^ callCoordinator; 

			IncomingCallViewDismissedCallback^ onIncomingCallViewDismissed;

			Windows::Phone::Networking::Voip::VoipPhoneCall^ currentCall;

			 // Called by the VoipCallCoordinator when the user accepts an incoming call. 
            void OnAcceptCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallAnswerEventArgs^ args); 
 
            // Called by the VoipCallCoordinator when the user rejects an incoming call. 
            void OnRejectCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallRejectEventArgs^ args); 
 
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallAnswerEventArgs^>^ acceptCallRequestedHandler; 
            Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallRejectEventArgs^>^ rejectCallRequestedHandler; 
            
            CallController();
            ~CallController();
        };
    }
}
