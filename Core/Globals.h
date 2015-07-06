/*
Globals.h
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

#if 0
			/// <summary>
			/// Access the video renderer instance
			/// </summary>
			property Mediastreamer2::WP8Video::IVideoRenderer^ VideoRenderer
			{
				Mediastreamer2::WP8Video::IVideoRenderer^ get();
				void set(Mediastreamer2::WP8Video::IVideoRenderer^ value);
			}
#endif

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

			//Mediastreamer2::WP8Video::IVideoRenderer^ videoRenderer;
        };
    }
}
