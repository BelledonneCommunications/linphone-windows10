#pragma once
#include <windows.h>
#include "Globals.h"

namespace Linphone
{
    namespace Core
    {
        namespace OutOfProcess
        {
            /// <summary>
			/// A remotely activatable class that is used by the UI process and managed code within the VoIP background process to get access to native objects that exist in the VoIP background process.
			/// </summary>
            public ref class Server sealed
            {
            public:
                Server()
                {
                }

                virtual ~Server()
                {
                }

				property CallController^ CallController
                {
                    Linphone::Core::CallController^ get()
                    {
                        return Globals::Instance->CallController;
                    };
                }

				property LinphoneCoreFactory^ LinphoneCoreFactory
                {
                    Linphone::Core::LinphoneCoreFactory^ get()
                    {
                        return Globals::Instance->LinphoneCoreFactory;
                    };
                }

                property LinphoneCore^ LinphoneCore
                {
                    Linphone::Core::LinphoneCore^ get()
                    {
                        return Globals::Instance->LinphoneCore;
                    };
                }
            };
        }
    }
}
