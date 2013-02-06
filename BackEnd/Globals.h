#pragma once

#include <roapi.h>

namespace Linphone
{
    namespace BackEnd
    {
        ref class CallController;
		ref class LinphoneCoreFactory;
		ref class LinphoneCore;
        
        // A singleton container that is used to hold other global singletons and background process-wide static state.
        // Another purpose of this class is to start the out-of-process WinRT server, so that the UI process
        // managed code can instantiate WinRT objects in this process.
        public ref class Globals sealed
        {
        public:
            // Start the out-of-process WinRT server, so that the UI process can instantiate WinRT objects in this process.
            void StartServer(const Platform::Array<Platform::String^>^ outOfProcServerClassNames);

            static unsigned int GetCurrentProcessId();

            // Get the name of the event that indicates if the UI is connected to the background process or not
            static Platform::String^ GetUiDisconnectedEventName(unsigned int backgroundProcessId);

            // Get the name of the event that indicates if the background process is ready or not
            static Platform::String^ GetBackgroundProcessReadyEventName(unsigned int backgroundProcessId);

            static property Globals^ Instance
            {
                Globals^ get();
            }

            property Linphone::BackEnd::CallController^ CallController
            {
                Linphone::BackEnd::CallController^ get();
            }

			property Linphone::BackEnd::LinphoneCoreFactory^ LinphoneCoreFactory
            {
                Linphone::BackEnd::LinphoneCoreFactory^ get();
            }

			property Linphone::BackEnd::LinphoneCore^ LinphoneCore
            {
                Linphone::BackEnd::LinphoneCore^ get();
            }

        private:
            Globals();
            ~Globals();

            // Name of the event that indicates if another instance of the VoIP background process exists or not
            static const LPCWSTR noOtherBackgroundProcessEventName;

            // Name of the event that indicates if the UI is connected to the background process or not
            static const LPCWSTR uiDisconnectedEventName;

            // Name of the event that indicates if the background process is ready or not
            static const LPCWSTR backgroundProcessReadyEventName;

            static Globals^ singleton;

            bool started;

            // A cookie that is used to unregister remotely activatable objects in this process
            RO_REGISTRATION_COOKIE serverRegistrationCookie;

            // An event that indicates if another instance of the background process exists or not
            HANDLE noOtherBackgroundProcessEvent;

            // An event that indicates that the background process is ready
            HANDLE backgroundReadyEvent;

            Linphone::BackEnd::CallController^ callController;

			Linphone::BackEnd::LinphoneCoreFactory^ linphoneCoreFactory;
        };
    }
}
