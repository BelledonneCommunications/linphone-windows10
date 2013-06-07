#include "CallController.h"
#include "LinphoneCall.h"
#include "LinphoneCallParams.h"
#include "LinphoneCore.h"
#include "Server.h"

using namespace Linphone::Core;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Phone::Networking::Voip;

VoipPhoneCall^ CallController::OnIncomingCallReceived(Linphone::Core::LinphoneCall^ call, Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallViewDismissedCallback^ incomingCallViewDismissedCallback)
{
	gApiLock.Lock();

	VoipPhoneCall^ incomingCall = nullptr;
	this->call = call;

	VoipCallMedia media = VoipCallMedia::Audio;
	if (Globals::Instance->LinphoneCore->IsVideoSupported()	&& Globals::Instance->LinphoneCore->IsVideoEnabled()) {
		LinphoneCallParams^ remoteParams = call->GetRemoteParams();
		LinphoneCallParams^ localParams = call->GetCurrentParamsCopy();
		if ((remoteParams != nullptr) && remoteParams->IsVideoEnabled() && (localParams != nullptr) && localParams->IsVideoEnabled()) {
			media = VoipCallMedia::Audio | VoipCallMedia::Video;
		}
	}

	if (this->customIncomingCallView) {
		this->callCoordinator->RequestNewOutgoingCall(
			this->callInProgressPageUri + "?sip=" + contactNumber,
			contactNumber,
			this->voipServiceName,
			media,
			&incomingCall);
	} else {
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
	}

	gApiLock.Unlock();
    return incomingCall;
}

void CallController::OnAcceptCallRequested(VoipPhoneCall^ incomingCall, CallAnswerEventArgs^ args)
{
	gApiLock.Lock();

	incomingCall->NotifyCallActive();

	if (this->onIncomingCallViewDismissed != nullptr)
		this->onIncomingCallViewDismissed();

	if (this->call != nullptr)
		Globals::Instance->LinphoneCore->AcceptCall(this->call);

	gApiLock.Unlock();
} 
 
void CallController::OnRejectCallRequested(VoipPhoneCall^ incomingCall, CallRejectEventArgs^ args)
{
	gApiLock.Lock();

	incomingCall->NotifyCallEnded();

	if (this->onIncomingCallViewDismissed != nullptr)
		this->onIncomingCallViewDismissed();

	if (this->call != nullptr)
		Globals::Instance->LinphoneCore->TerminateCall(this->call);

	gApiLock.Unlock();
} 

void CallController::EndCall(VoipPhoneCall^ call)
{
	gApiLock.Lock();
	call->NotifyCallEnded();
	gApiLock.Unlock();
}

VoipPhoneCall^ CallController::NewOutgoingCall(Platform::String^ number)
{
	gApiLock.Lock();

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

IncomingCallViewDismissedCallback^ CallController::IncomingCallViewDismissed::get()
{
	gApiLock.Lock();
	IncomingCallViewDismissedCallback^ cb = this->onIncomingCallViewDismissed;
	gApiLock.Unlock();
	return cb;
}

void CallController::IncomingCallViewDismissed::set(IncomingCallViewDismissedCallback^ cb)
{
	gApiLock.Lock();
	this->onIncomingCallViewDismissed = cb;
	gApiLock.Unlock();
}

Platform::Boolean CallController::CustomIncomingCallView::get()
{
	gApiLock.Lock();
	Platform::Boolean value = this->customIncomingCallView;
	gApiLock.Unlock();
	return value;
}

void CallController::CustomIncomingCallView::set(Platform::Boolean value)
{
	gApiLock.Lock();
	this->customIncomingCallView = value;
	gApiLock.Unlock();
}

CallController::CallController() :
		callInProgressPageUri(L"/Views/InCall.xaml"),
		voipServiceName(nullptr),
		defaultContactImageUri(nullptr),
		linphoneImageUri(nullptr),
		ringtoneUri(nullptr),
		callCoordinator(VoipCallCoordinator::GetDefault())
{
	// URIs required for interactions with the VoipCallCoordinator
    String^ installFolder = String::Concat(Windows::ApplicationModel::Package::Current->InstalledLocation->Path, "\\");
    this->defaultContactImageUri = ref new Uri(installFolder, "Assets\\unknown.png");
    this->voipServiceName = ref new String(L"Linphone");
	this->linphoneImageUri = ref new Uri(installFolder, "Assets\\pnicon.png");
	this->ringtoneUri = ref new Uri(installFolder, "Assets\\Sounds\\Ringtone.wma");

	this->acceptCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallAnswerEventArgs^>(this, &CallController::OnAcceptCallRequested);
    this->rejectCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallRejectEventArgs^>(this, &CallController::OnRejectCallRequested);
}

CallController::~CallController()
{

}
