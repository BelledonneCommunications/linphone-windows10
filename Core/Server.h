/*
Server.h
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
#include <windows.h>
#include "Globals.h"
#include "LinphoneCore.h"

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

				property BackgroundModeLogger^ BackgroundModeLogger
				{
					Linphone::Core::BackgroundModeLogger^ get()
					{
						return Globals::Instance->BackgroundModeLogger;
					};
				}

				property CallController^ CallController
				{
					Linphone::Core::CallController^ get()
					{
						return Globals::Instance->CallController;
					};
				}

				/// <summary>
				/// Returns the install location of the app, used to get the uri of resources needed for the incoming call view
				/// </summary>
				property Platform::String^ Path
				{
					Platform::String^ get()
					{
						return Windows::ApplicationModel::Package::Current->InstalledLocation->Path;
					};
				}
            };
        }
    }
}
