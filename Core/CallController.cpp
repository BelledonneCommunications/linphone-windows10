#include "CallController.h"
#include "LinphoneCall.h"
#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "Server.h"

using namespace Linphone::Core;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Phone::Networking::Voip;

//#define ACCEPT_WITH_VIDEO_OR_WITH_AUDIO_ONLY

VoipPhoneCall^ CallController::OnIncomingCallReceived(Linphone::Core::LinphoneCall^ call, Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallViewDismissedCallback^ incomingCallViewDismissedCallback)
{
	TRACE; gApiLock.Lock();

	VoipPhoneCall^ incomingCall = nullptr;
	this->call = call;

	VoipCallMedia media = VoipCallMedia::Audio;
#ifdef ACCEPT_WITH_VIDEO_OR_WITH_AUDIO_ONLY
	if (Globals::Instance->LinphoneCore->IsVideoSupported()	&& Globals::Instance->LinphoneCore->IsVideoEnabled()) {
		bool automatically_accept = false;
		LinphoneCallParams^ remoteParams = call->GetRemoteParams();
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
			gApiLock.Unlock();
			return nullptr;
	    }

		incomingCall->AnswerRequested += this->acceptCallRequestedHandler;
		incomingCall->RejectRequested += this->rejectCallRequestedHandler;
	} else {
		// When using the custom incoming call view, the VoipPhoneCall will be created when getting in the
		// StreamsRunning state by calling NewIncomingCallForCustomIncomingCallView()
	}

	gApiLock.Unlock();
    return incomingCall;
}

void CallController::OnAcceptCallRequested(VoipPhoneCall^ incomingCall, CallAnswerEventArgs^ args)
{
	TRACE; gApiLock.Lock();

	incomingCall->NotifyCallActive();

	if (this->onIncomingCallViewDismissed != nullptr) {
		this->onIncomingCallViewDismissed();
		this->onIncomingCallViewDismissed = nullptr;
	}

	if (this->call != nullptr) {
#ifdef ACCEPT_WITH_VIDEO_OR_WITH_AUDIO_ONLY
		LinphoneCallParams^ params = call->GetCurrentParamsCopy();
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

	gApiLock.Unlock();
} 
 
void CallController::OnRejectCallRequested(VoipPhoneCall^ incomingCall, CallRejectEventArgs^ args)
{
	TRACE; gApiLock.Lock();

	if (this->onIncomingCallViewDismissed != nullptr) {
		this->onIncomingCallViewDismissed();
		this->onIncomingCallViewDismissed = nullptr;
	}

	//This will call notifyCallEnded on the call state changed callback
	if (this->call != nullptr)
		Globals::Instance->LinphoneCore->DeclineCall(this->call, this->declineReason);

	gApiLock.Unlock();
}

VoipPhoneCall^ CallController::NewOutgoingCall(Platform::String^ number)
{
	TRACE; gApiLock.Lock();

	VoipPhoneCall^ outgoingCall = nullptr;
	this->call = call;

	VoipCallMedia media = VoipCallMedia::Audio;
	if (Globals::Instance->LinphoneCore->IsVideoSupported() && Globals::Instance->LinphoneCore->IsVideoEnabled()) {
		VideoPolicy^ policy = Globals::Instance->LinphoneCore->GetVideoPolicy();
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

	gApiLock.Unlock();
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

IncomingCallViewDismissedCallback^ CallController::IncomingCallViewDismissed::get()
{
	TRACE; gApiLock.Lock();
	IncomingCallViewDismissedCallback^ cb = this->onIncomingCallViewDismissed;
	gApiLock.Unlock();
	return cb;
}

void CallController::IncomingCallViewDismissed::set(IncomingCallViewDismissedCallback^ cb)
{
	TRACE; gApiLock.Lock();
	this->onIncomingCallViewDismissed = cb;
	gApiLock.Unlock();
}

Platform::Boolean CallController::CustomIncomingCallView::get()
{
	TRACE; gApiLock.Lock();
	Platform::Boolean value = this->customIncomingCallView;
	gApiLock.Unlock();
	return value;
}

void CallController::CustomIncomingCallView::set(Platform::Boolean value)
{
	TRACE; gApiLock.Lock();
	this->customIncomingCallView = value;
	gApiLock.Unlock();
}

DeclineReason CallController::DeclineReason::get()
{
	TRACE; gApiLock.Lock();
	Linphone::Core::DeclineReason value = this->declineReason;
	gApiLock.Unlock();
	return value;
}

void CallController::DeclineReason::set(Linphone::Core::DeclineReason value)
{
	TRACE; gApiLock.Lock();
	this->declineReason = value;
	gApiLock.Unlock();
}

CallController::CallController() :
		callInProgressPageUri(L"/Views/InCall.xaml"),
		voipServiceName(nullptr),
		defaultContactImageUri(nullptr),
		linphoneImageUri(nullptr),
		ringtoneUri(nullptr),
		declineReason(Linphone::Core::DeclineReason::LinphoneReasonDeclined),
		callCoordinator(VoipCallCoordinator::GetDefault())
{
	// URIs required for interactions with the VoipCallCoordinator
    String^ installFolder = String::Concat(Windows::ApplicationModel::Package::Current->InstalledLocation->Path, "\\");
    this->defaultContactImageUri = ref new Uri(installFolder, "Assets\\unknown.png");
    this->voipServiceName = ref new String(L"Linphone");
	this->linphoneImageUri = ref new Uri(installFolder, "Assets\\pnicon.png");
	this->ringtoneUri = ref new Uri(installFolder, "Assets\\Sounds\\oldphone.wma");

	this->acceptCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallAnswerEventArgs^>(this, &CallController::OnAcceptCallRequested);
    this->rejectCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallRejectEventArgs^>(this, &CallController::OnRejectCallRequested);
}

CallController::~CallController()
{

}
