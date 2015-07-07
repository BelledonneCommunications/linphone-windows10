/*
CallController.cpp
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

#include "Call.h"
#include "CallController.h"
#include "CallParams.h"
#include "Core.h"
#include "VideoPolicy.h"

using namespace Linphone::Native;
using namespace Platform;
using namespace Windows::Foundation;
//using namespace Windows::Phone::Networking::Voip;

//#define ACCEPT_WITH_VIDEO_OR_WITH_AUDIO_ONLY

#if 0
VoipPhoneCall^ CallController::OnIncomingCallReceived(Linphone::Native::Call^ call, Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallViewDismissedCallback^ incomingCallViewDismissedCallback)
{
	VoipPhoneCall^ incomingCall = nullptr;
	this->call = call;

	VoipCallMedia media = VoipCallMedia::Audio;
#ifdef ACCEPT_WITH_VIDEO_OR_WITH_AUDIO_ONLY
	if (Globals::Instance->LinphoneCore->IsVideoSupported()	&& Globals::Instance->LinphoneCore->IsVideoEnabled()) {
		bool automatically_accept = false;
		CallParams^ remoteParams = call->GetRemoteParams();
		VideoPolicy^ policy = Globals::Instance->LinphoneCore->GetVideoPolicy();
		if (policy != nullptr) {
			automatically_accept = policy->AutomaticallyAccept;
		}
		if ((remoteParams != nullptr) && remoteParams->IsVideoEnabled() && automatically_accept) {
			media = VoipCallMedia::Audio | VoipCallMedia::Video;
		}
	}
#endif

	if (!this->customIncomingCallView) {
		TimeSpan ringingTimeout;
		ringingTimeout.Duration = 90 * 10 * 1000 * 1000; // in 100ns units

		try {
			if (incomingCallViewDismissedCallback != nullptr)
				this->onIncomingCallViewDismissed = incomingCallViewDismissedCallback;

			// Ask the Phone Service to start a new incoming call
			this->callCoordinator->RequestNewIncomingCall(
				this->callInProgressPageUri + "?sip=" + contactNumber,
				contactName,
				contactNumber,
				this->defaultContactImageUri,
				this->voipServiceName,
				this->linphoneImageUri,
				"",
				this->ringtoneUri,
				media,
				ringingTimeout,
				&incomingCall);
		}
		catch(...) {
			return nullptr;
	    }

		incomingCall->AnswerRequested += this->acceptCallRequestedHandler;
		incomingCall->RejectRequested += this->rejectCallRequestedHandler;
	} else {
		// When using the custom incoming call view, the VoipPhoneCall will be created when getting in the
		// StreamsRunning state by calling NewIncomingCallForCustomIncomingCallView()
	}

    return incomingCall;
}

void CallController::OnAcceptCallRequested(VoipPhoneCall^ incomingCall, CallAnswerEventArgs^ args)
{
	incomingCall->NotifyCallActive();

	if (this->onIncomingCallViewDismissed != nullptr) {
		this->onIncomingCallViewDismissed();
		this->onIncomingCallViewDismissed = nullptr;
	}

	if (this->call != nullptr) {
#ifdef ACCEPT_WITH_VIDEO_OR_WITH_AUDIO_ONLY
		CallParams^ params = call->GetCurrentParamsCopy();
		if ((args->AcceptedMedia & VoipCallMedia::Video) == VoipCallMedia::Video) {
			params->EnableVideo(true);
		} else {
			params->EnableVideo(false);
		}
		Globals::Instance->LinphoneCore->AcceptCallWithParams(this->call, params);
#else
		Globals::Instance->LinphoneCore->AcceptCall(this->call);
#endif
	}
} 
 
void CallController::OnRejectCallRequested(VoipPhoneCall^ incomingCall, CallRejectEventArgs^ args)
{
	if (this->onIncomingCallViewDismissed != nullptr) {
		this->onIncomingCallViewDismissed();
		this->onIncomingCallViewDismissed = nullptr;
	}

	//This will call notifyCallEnded on the call state changed callback
	if (this->call != nullptr)
		Globals::Instance->LinphoneCore->DeclineCall(this->call, this->declineReason);
}

VoipPhoneCall^ CallController::NewOutgoingCall(Platform::String^ number)
{
	VoipPhoneCall^ outgoingCall = nullptr;
	this->call = call;

	VoipCallMedia media = VoipCallMedia::Audio;
	if (Globals::Instance->LinphoneCore->VideoSupported && Globals::Instance->LinphoneCore->VideoEnabled) {
		VideoPolicy^ policy = Globals::Instance->LinphoneCore->VideoPolicy;
		if ((policy != nullptr) && policy->AutomaticallyInitiate) {
			media = VoipCallMedia::Audio | VoipCallMedia::Video;
		}
	}

	this->callCoordinator->RequestNewOutgoingCall(
		this->callInProgressPageUri + "?sip=" + number,
        number,
        this->voipServiceName,
        media,
		&outgoingCall);

	outgoingCall->NotifyCallActive();
	return outgoingCall;
}

VoipPhoneCall^ CallController::NewIncomingCallForCustomIncomingCallView(Platform::String^ contactNumber)
{
	VoipPhoneCall^ incomingCall = nullptr;
	VoipCallMedia media = VoipCallMedia::Audio;

	this->callCoordinator->RequestNewOutgoingCall(
			this->callInProgressPageUri + "?sip=" + contactNumber,
			contactNumber,
			this->voipServiceName,
			media,
			&incomingCall);

	return incomingCall;
}
#endif

void CallController::NotifyMute(bool isMuted)
{
#if 0
	if (isMuted)
		this->callCoordinator->NotifyMuted();
	else
		this->callCoordinator->NotifyUnmuted();
#endif
}

IncomingCallViewDismissedCallback^ CallController::IncomingCallViewDismissed::get()
{
	return this->onIncomingCallViewDismissed;
}

void CallController::IncomingCallViewDismissed::set(IncomingCallViewDismissedCallback^ cb)
{
	API_LOCK;
	this->onIncomingCallViewDismissed = cb;
}

Platform::Boolean CallController::CustomIncomingCallView::get()
{
	return this->customIncomingCallView;
}

void CallController::CustomIncomingCallView::set(Platform::Boolean value)
{
	API_LOCK;
	this->customIncomingCallView = value;
}

Reason CallController::DeclineReason::get()
{
	return this->declineReason;
}

void CallController::DeclineReason::set(Linphone::Native::Reason value)
{
	API_LOCK;
	this->declineReason = value;
}

CallController::CallController() :
		callInProgressPageUri(L"/Views/InCall.xaml"),
		voipServiceName(nullptr),
		defaultContactImageUri(nullptr),
		linphoneImageUri(nullptr),
		ringtoneUri(nullptr),
		declineReason(Linphone::Native::Reason::Declined) //,
		//callCoordinator(VoipCallCoordinator::GetDefault())
{
	// URIs required for interactions with the VoipCallCoordinator
    String^ installFolder = String::Concat(Windows::ApplicationModel::Package::Current->InstalledLocation->Path, "\\");
    this->defaultContactImageUri = ref new Uri(installFolder, "Assets\\unknown.png");
    this->voipServiceName = ref new String(L"Linphone");
	this->linphoneImageUri = ref new Uri(installFolder, "Assets\\pnicon.png");
	this->ringtoneUri = ref new Uri(installFolder, "Assets\\Sounds\\oldphone.wma");

	//this->acceptCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallAnswerEventArgs^>(this, &CallController::OnAcceptCallRequested);
    //this->rejectCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallRejectEventArgs^>(this, &CallController::OnRejectCallRequested);
}

CallController::~CallController()
{
}
