#pragma once

#include "ApiLock.h"

namespace Linphone
{
	namespace BackEnd
	{
		ref class Globals;
		ref class LinphoneCore;

		public ref class LinphoneCoreFactory sealed
		{
		public:
			property Linphone::BackEnd::LinphoneCore^ LinphoneCore
            {
                Linphone::BackEnd::LinphoneCore^ get();
            }
			
			void CreateLinphoneCore();

		private:
			friend ref class Linphone::BackEnd::Globals;

			Linphone::BackEnd::LinphoneCore^ linphoneCore;

			LinphoneCoreFactory();
			~LinphoneCoreFactory();
		};
	}
}