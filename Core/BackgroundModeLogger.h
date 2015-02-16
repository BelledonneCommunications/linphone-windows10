/*
BackgroundModeLogger.h
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

#include "LinphoneCoreFactory.h"
#include <mutex>

// Do not treat doxygen documentation as XML
#pragma warning(push)
#pragma warning(disable : 4635)
#include "coreapi\linphonecore.h"
#pragma warning(pop)

namespace Linphone
{
	namespace Core
	{
		ref class Globals;

		/// <summary>
		/// Object to log the application traces while in background mode.
		/// </summary>
		public ref class BackgroundModeLogger sealed : OutputTraceListener
		{
		public:
			/// <summary>
			/// Method called to output a trace to the logs.
			/// </summary>
			/// <param name="level">The level of the trace to output</param>
			/// <param name="msg">The message to ouput</param>
			virtual void OutputTrace(OutputTraceLevel level, Platform::String^ msg);

		private:
			friend ref class Linphone::Core::Globals;

			BackgroundModeLogger();
			~BackgroundModeLogger();
		};
	}
}