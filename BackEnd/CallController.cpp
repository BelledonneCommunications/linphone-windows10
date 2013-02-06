#include "CallController.h"
#include "Server.h"

using namespace Linphone::BackEnd;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Phone::Networking::Voip;

bool CallController::OnIncomingCallReceived(Platform::String^ contactName, Platform::String^ contactNumber, IncomingCallDialogDismissedCallback^ incomingCallDialogDismissedCallback) 
{ 
	std::lock_guard<std::recursive_mutex> lock(g_apiLock); 

	VoipPhoneCall^ incomingCall = nullptr; 
	try 
    { 
        TimeSpan ringingTimeout; 
        ringingTimeout.Duration = 90 * 10 * 1000 * 1000; // in 100ns units 
 
        ::OutputDebugString(L"[CallController::OnIncomingCallReceived] => Will time out in 90 seconds\n"); 
 
        // Ask the Phone Service to start a new incoming call 
        this->callCoordinator->RequestNewIncomingCall( 
            this->callInProgressPageUri, 
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
 
    // Register for events about this incoming call. 
    incomingCall->AnswerRequested += this->acceptCallRequestedHandler; 
    incomingCall->RejectRequested += this->rejectCallRequestedHandler; 
 
    return true; 
}

void CallController::OnAcceptCallRequested(VoipPhoneCall^ sender, CallAnswerEventArgs^ args) 
{ 

} 
 
void CallController::OnRejectCallRequested(VoipPhoneCall^ sender, CallRejectEventArgs^ args) 
{ 

} 

CallController::CallController() :
		callInProgressPageUri(L"/Views/InCall.xaml"), 
		voipServiceName(nullptr), 
		defaultContactImageUri(nullptr), 
		linphoneImageUri(nullptr),
		ringtoneUri(nullptr)
{
	this->callCoordinator = VoipCallCoordinator::GetDefault(); 

	// URIs required for interactions with the VoipCallCoordinator 
    String^ installFolder = String::Concat(Windows::ApplicationModel::Package::Current->InstalledLocation->Path, "\\"); 
    this->defaultContactImageUri = ref new Uri(installFolder, "Assets\\unknown.png"); 
    this->voipServiceName = ref new String(L"Linphone"); 
	this->linphoneImageUri = ref new Uri(installFolder, "Assets\\AppIcon.png"); 
    this->ringtoneUri = ref new Uri(installFolder, "Assets\\Sounds\\Ringtone.wma"); 

	this->acceptCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallAnswerEventArgs^>(this, &CallController::OnAcceptCallRequested); 
    this->rejectCallRequestedHandler = ref new TypedEventHandler<VoipPhoneCall^, CallRejectEventArgs^>(this, &CallController::OnRejectCallRequested); 
}

CallController::~CallController()
{

}
