/*
CallController.h
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

//#include <windows.phone.networking.voip.h>
#include "Enums.h"
#include "ApiLock.h"

namespace Linphone
{
    namespace Native
    {
        ref class Globals;
		ref class LinphoneCall;
 
		/// <summary>
		/// Callback to be called when the PushNotification Agent has to be dismissed, i.e. after a call has been accepted, denied or stopped by the caller.
		/// </summary>
        public delegate void IncomingCallViewDismissedCallback(); 

		/// <summary>
		/// Provides methods and properties related to Windows.Phone.Networking.Voip.VoipPhoneCall calls.
		/// </summary>
        public ref class CallController sealed
        {
        public:
			/// <summary>
			/// Starts the system incoming call view.
			/// </summary>
			/// <param name="call">The incoming LinphoneCall to notify</param>
			/// <param name="contactName">The display name of the caller</param>
			/// <param name="contactNumber">The number or SIP URI of the caller</param>
			/// <param name="incomingCallViewDismissedCallback">The callback to be called if the notified incoming call is dismissed by the user</param>
			/// <returns>The system VoipPhoneCall that has been notified</returns>
            //Windows::Phone::Networking::Voip::VoipPhoneCall^ OnIncomingCallReceived(LinphoneCall^ call, Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallViewDismissedCallback^ incomingCallViewDismissedCallback); 

			/// <summary>
			/// Starts an outgoing call using native VoipPhoneCall.
			/// </summary>
			/// <param name="number">The number of SIP URI to call</param>
			/// <returns>The system VoipPhoneCall that has been initiated</returns>
			//Windows::Phone::Networking::Voip::VoipPhoneCall^ NewOutgoingCall(Platform::String^ number);

			/// <summary>
			/// Starts an incoming call for custom incoming call view.
			/// </summary>
			/// <param name="contactNumber">The number or SIP URI of the caller</param>
			/// <returns>The system VoipPhoneCall that has been created</returns>
			//Windows::Phone::Networking::Voip::VoipPhoneCall^ NewIncomingCallForCustomIncomingCallView(Platform::String^ contactNumber);

			/// <summary>
			/// Notifies the system that the call needs to be muted/unmuted.
			/// </summary>
			/// <param name="isMuted">The new mute state</param>
			void NotifyMute(bool isMuted);

			/// <summary>
			/// Changes the reason used when declining an incoming call.
			/// </summary>
			property Linphone::Native::Reason DeclineReason
            {
                Linphone::Native::Reason get();
				void set(Linphone::Native::Reason cb);
            }

			/// <summary>
			/// Callback to be called when the PushNotification Agent has to be dismissed, i.e. after a call has been accepted, denied or stopped by the caller.
			/// </summary>
			property IncomingCallViewDismissedCallback^ IncomingCallViewDismissed
            {
                IncomingCallViewDismissedCallback^ get();
				void set(IncomingCallViewDismissedCallback^ cb);
            }

			/// <summary>
			/// Property to tell that we will use our custom incoming call view instead of the one provided by the OS.
			/// </summary>
			property Platform::Boolean CustomIncomingCallView
			{
				Platform::Boolean get();
				void set(Platform::Boolean value);
			}

        private:
            friend ref class Linphone::Native::Globals;
 
            Platform::String^ voipServiceName;
			
            Platform::String^ callInProgressPageUri; 

			Linphone::Native::Reason declineReason;
			
            // The URI to the contact image, jpg or png, 1mb max ! 
            Windows::Foundation::Uri^ defaultContactImageUri; 
 
            // The URI to the linphone icon, 128kb max ! 
            Windows::Foundation::Uri^ linphoneImageUri; 

            Windows::Foundation::Uri^ ringtoneUri; 

            //Windows::Phone::Networking::Voip::VoipCallCoordinator^ callCoordinator; 

			Linphone::Native::LinphoneCall^ call;

			IncomingCallViewDismissedCallback^ onIncomingCallViewDismissed;

			Platform::Boolean customIncomingCallView;

            //void OnAcceptCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallAnswerEventArgs^ args); 
 
            //void OnRejectCallRequested(Windows::Phone::Networking::Voip::VoipPhoneCall^ sender, Windows::Phone::Networking::Voip::CallRejectEventArgs^ args); 
 
            //Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallAnswerEventArgs^>^ acceptCallRequestedHandler; 
            //Windows::Foundation::TypedEventHandler<Windows::Phone::Networking::Voip::VoipPhoneCall^, Windows::Phone::Networking::Voip::CallRejectEventArgs^>^ rejectCallRequestedHandler; 
            
            CallController();
            ~CallController();
        };
    }
}
