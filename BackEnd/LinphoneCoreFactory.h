#pragma once

#include "LinphoneCore.h";

namespace Linphone
{
	namespace BackEnd
	{
		ref class LinphoneCore;
		
		public ref class LinphoneCoreFactory sealed
		{
		public:
			property Linphone::BackEnd::LinphoneCore^ LinphoneCore
            {
                Linphone::BackEnd::LinphoneCore^ get();
            }

			LinphoneCoreFactory();
			void CreateLinphoneCore();

		private:
			Linphone::BackEnd::LinphoneCore^ linphoneCore;

			~LinphoneCoreFactory();
		};
	}
}