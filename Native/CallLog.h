/*
CallLog.h
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

#include "Core.h"
#include "Enums.h"


namespace Linphone
{
	namespace Native
	{
		ref class Address;
		ref class Core;

		/// <summary>
		/// Call data records object
		/// </summary>
		public ref class CallLog sealed
		{
		public:
			/// <summary>
			/// Returns the call id from signaling.
			/// </summary>
			property Platform::String^ CallId
			{
				Platform::String^ get();
			}

			/// <summary>
			/// Gets the CallDirection of the call (Incoming or Outgoing).
			/// </summary>
			property CallDirection Direction
			{
				CallDirection get();
			}

			/// <summary>
			/// Returns the call duration in seconds.
			/// </summary>
			property int Duration
			{
				int get();
			}

			/// <summary>
			/// Gets the Address of the caller.
			/// </summary>
			property Address^ FromAddress
			{
				Address^ get();
			}

			/// <summary>
			/// Tells whether video was enabled at the end of the call.
			/// </summary>
			property Platform::Boolean IsVideoEnabled
			{
				Platform::Boolean get();
			}

			/// <summary>
			/// Returns the start date/time of the call in seconds elpsed since January first 1970.
			/// </summary>
			property int64 StartDate
			{
				int64 get();
			}

			/// <summary>
			/// Gets the CallStatus of the call (Success, Aborted, Missed or Declined).
			/// </summary>
			property CallStatus Status
			{
				CallStatus get();
			}

			/// <summary>
			/// Gets the Address of the callee.
			/// </summary>
			property Address^ ToAddress
			{
				Address^ get();
			}

		private:
			friend class Linphone::Native::Utils;
			friend ref class Linphone::Native::Core;

			CallLog(::LinphoneCallLog *cl);
			~CallLog();

			::LinphoneCallLog *callLog;
		};
	}
}