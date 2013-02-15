#pragma once

#include <roapi.h>

namespace Linphone
{
    namespace Core
    {
        ref class CallController;
		ref class LinphoneCoreFactory;
		ref class LinphoneCore;
        
		/// <summary>
		/// Singleton container used to hold global singletons and background process-wide objects.
		/// Also used to start out of process server, allowing the UI process managed code to instantiate WinRT objects in this process.
		/// </summary>
        public ref class Globals sealed
        {
        public:
			/// <summary>
			/// Starts the out of process server, allowing the UI process to instantiate WinRT objects in this process.
			/// </summary>
            void StartServer(const Platform::Array<Platform::String^>^ outOfProcServerClassNames);

            static unsigned int GetCurrentProcessId();

            static Platform::String^ GetUiDisconnectedEventName(unsigned int backgroundProcessId);

            static Platform::String^ GetBackgroundProcessReadyEventName(unsigned int backgroundProcessId);

            static property Globals^ Instance
            {
                Globals^ get();
            }

			property LinphoneCoreFactory^ LinphoneCoreFactory
            {
                Linphone::Core::LinphoneCoreFactory^ get();
            }

			property LinphoneCore^ LinphoneCore
            {
                Linphone::Core::LinphoneCore^ get();
            }

        private:
            Globals();
            ~Globals();

            static const LPCWSTR noOtherBackgroundProcessEventName;

            static const LPCWSTR uiDisconnectedEventName;

            static const LPCWSTR backgroundProcessReadyEventName;

            static Globals^ singleton;

            bool started;

            // A cookie that is used to unregister remotely activatable objects in this process
            RO_REGISTRATION_COOKIE serverRegistrationCookie;

            HANDLE noOtherBackgroundProcessEvent;

            HANDLE backgroundReadyEvent;

			Linphone::Core::LinphoneCoreFactory^ linphoneCoreFactory;
        };
    }
}
