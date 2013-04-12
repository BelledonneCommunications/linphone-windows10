#pragma once

#include <roapi.h>

namespace Linphone
{
    namespace Core
    {
        ref class CallController;
		ref class LinphoneCoreFactory;
		ref class LinphoneCore;
		ref class BackgroundModeLogger;
        
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

			/// <summary>
			///Returns the id of the background process (HeadlessHost)
			/// </summary>
            static unsigned int GetCurrentProcessId();

            static Platform::String^ GetUiDisconnectedEventName(unsigned int backgroundProcessId);

            static Platform::String^ GetBackgroundProcessReadyEventName(unsigned int backgroundProcessId);

			/// <summary>
			/// Gets the instance of the Globals class, used to directly access any C++/CX objects from C#
			/// </summary>
            static property Globals^ Instance
            {
                Globals^ get();
            }

			/// <summary>
			/// Gets the current instance of LinphoneCoreFactory
			/// </summary>
			property LinphoneCoreFactory^ LinphoneCoreFactory
            {
                Linphone::Core::LinphoneCoreFactory^ get();
            }

			/// <summary>
			/// Gets the current instance of LinphoneCore from LinphoneCoreFactory
			/// </summary>
			property LinphoneCore^ LinphoneCore
            {
                Linphone::Core::LinphoneCore^ get();
            }

			/// <summary>
			/// Gets the current instance of the logger
			/// </summary>
			property BackgroundModeLogger^ BackgroundModeLogger
			{
				Linphone::Core::BackgroundModeLogger^ get();
			}
 
			/// <summary>
			/// Gets the current instance of the native call controller
			/// </summary>
            property CallController^ CallController 
            { 
                Linphone::Core::CallController^ get(); 
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

			Linphone::Core::BackgroundModeLogger^ backgroundModeLogger;

            Linphone::Core::CallController^ callController; 
        };
    }
}
