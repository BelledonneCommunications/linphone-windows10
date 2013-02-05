#pragma once
#include <windows.h>
#include "Globals.h"

namespace Linphone
{
    namespace BackEnd
    {
        namespace OutOfProcess
        {
            // A remotely activatable class that is used by the UI process and managed code within
            // the VoIP background process to get access to native objects that exist in the VoIP background process.
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
                    Linphone::BackEnd::CallController^ get()
                    {
                        return Globals::Instance->CallController;
                    };
                }

				property LinphoneCoreFactory^ LinphoneCoreFactory
                {
                    Linphone::BackEnd::LinphoneCoreFactory^ get()
                    {
                        return Globals::Instance->LinphoneCoreFactory;
                    };
                }

                property LinphoneCore^ LinphoneCore
                {
                    Linphone::BackEnd::LinphoneCore^ get()
                    {
                        return Globals::Instance->LinphoneCore;
                    };
                }
            };
        }
    }
}
