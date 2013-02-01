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
                // Constructor
                Server()
                {
                }

                // Destructor
                virtual ~Server()
                {
                }

                // Called by the UI process to get the call controller object
                property CallController^ CallController
                {
                    Linphone::BackEnd::CallController^ get()
                    {
                        return Globals::Instance->CallController;
                    };
                }

                // Add methods and properties to get other objects here, as required.
            };
        }
    }
}
