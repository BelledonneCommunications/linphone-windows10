#include "CallController.h"
#include "Server.h"

using namespace Linphone::BackEnd;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Phone::Networking::Voip;

void CallController::SetCallControllerListener(CallControllerListener^ listener)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	this->callControllerListener = listener;
}

bool CallController::OnIncomingCallReceived(Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallViewDismissedCallback^ incomingCallViewDismissedCallback) 
{ 
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 

	VoipPhoneCall^ incomingCall = nullptr; 
	this->callerNumber = contactNumber;

	try 
    { 
        TimeSpan ringingTimeout; 
        ringingTimeout.Duration = 90 * 10 * 1000 * 1000; // in 100ns units 
 
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
            VoipCallMedia::Audio, 
            ringingTimeout,
            &incomingCall);
    } 
    catch(...) 
    {
        return false; 
    } 
 
    incomingCall->AnswerRequested += this->acceptCallRequestedHandler; 
    incomingCall->RejectRequested += this->rejectCallRequestedHandler; 
 
    return true; 
}

void CallController::OnAcceptCallRequested(VoipPhoneCall^ incomingCall, CallAnswerEventArgs^ args) 
{ 
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	incomingCall->NotifyCallActive();
	this->currentCall = incomingCall;

	if (this->onIncomingCallViewDismissed != nullptr)
		this->onIncomingCallViewDismissed();

	if (this->callControllerListener != nullptr)
		this->callControllerListener->NewCallStarted(this->callerNumber);
} 
 
void CallController::OnRejectCallRequested(VoipPhoneCall^ incomingCall, CallRejectEventArgs^ args) 
{ 
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	incomingCall->NotifyCallEnded();

	if (this->onIncomingCallViewDismissed != nullptr)
		this->onIncomingCallViewDismissed();

	this->callerNumber = nullptr;
} 

void CallController::EndCurrentCall()
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	if (this->currentCall == nullptr)
		return;

	this->currentCall->NotifyCallEnded();

	if (this->callControllerListener != nullptr)
		this->callControllerListener->CallEnded();

	this->currentCall = nullptr;
}

Platform::Boolean CallController::NewOutgoingCall(Platform::String^ number, Platform::String^ name)
{
	std::lock_guard<std::recursive_mutex> lock(g_apiLock);

	if (this->currentCall != nullptr)
		return false;
	
	VoipPhoneCall^ outgoingCall = nullptr;

	this->callCoordinator->RequestNewOutgoingCall(
		this->callInProgressPageUri + "?sip=" + number, 
        name, 
        this->voipServiceName, 
        VoipCallMedia::Audio,
		&outgoingCall);

	outgoingCall->NotifyCallActive();
	this->currentCall = outgoingCall;
	this->callerNumber = number;

	if (this->callControllerListener != nullptr)
		this->callControllerListener->NewCallStarted(number);

	return true;
}

CallController::CallController() :
		callInProgressPageUri(L"/Views/InCall.xaml"), 
		voipServiceName(nullptr), 
		defaultContactImageUri(nullptr), 
		linphoneImageUri(nullptr),
		ringtoneUri(nullptr),
		callerNumber(nullptr),
		callControllerListener(nullptr)
{
	this->callCoordinator = VoipCallCoordinator::GetDefault(); 

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
