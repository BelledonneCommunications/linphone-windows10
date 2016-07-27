/*
VoipCallController.cpp
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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#include "Call.h"
#include "VoipCallController.h"

using namespace BelledonneCommunications::Linphone::Native;
using namespace Platform;


VoipCallController::VoipCallController() :
	callCoordinator(VoipCallCoordinator::GetDefault()),
	voipServiceName(ref new String(L"Linphone"))
{
}

VoipCallController::~VoipCallController()
{
}

VoipPhoneCall^ VoipCallController::NewIncomingCall(Platform::String^ contactUri)
{
	// Why RequestNewOutgoingCall() instead of RequestNewIncomingCall()?
	// Because we are using a custom incoming call view so just register the call to the system.
	return this->callCoordinator->RequestNewOutgoingCall(
		/*this->callInProgressPageUri + "?sip=" + number*/ "Linphone", // TODO
		contactUri,
		this->voipServiceName,
		VoipPhoneCallMedia::Audio | VoipPhoneCallMedia::Video);
}

VoipPhoneCall^ VoipCallController::NewOutgoingCall(Platform::String^ contactUri)
{
	return this->callCoordinator->RequestNewOutgoingCall(
		/*this->callInProgressPageUri + "?sip=" + number*/ "Linphone", // TODO
		contactUri,
		this->voipServiceName,
		VoipPhoneCallMedia::Audio | VoipPhoneCallMedia::Video);
}
