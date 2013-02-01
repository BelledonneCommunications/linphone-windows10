#pragma once
#include <windows.phone.networking.voip.h>
#include "ApiLock.h"

namespace Linphone
{
    namespace BackEnd
    {
        // Forward declaration
        ref class Globals;

        // A class that provides methods and properties related to VoIP calls.
        // It wraps Windows.Phone.Networking.Voip.VoipCallCoordinator, and provides app-specific call functionality.
        public ref class CallController sealed
        {
        public:

        private:
            // Only the server can create an instance of this object
            friend ref class Linphone::BackEnd::Globals;

            // Constructor and destructor
            CallController();
            ~CallController();
        };
    }
}
