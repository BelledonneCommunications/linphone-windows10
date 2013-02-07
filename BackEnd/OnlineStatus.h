#pragma once

namespace Linphone
{
	namespace BackEnd
	{
		public enum class OnlineStatus : int
		{
			Offline = 0,
			Online = 1,
			Busy = 2,
			BeRightBack = 3,
			Away = 4,
			OnThePhone = 5,
			OutToLunch = 6,
			DoNotDisturb = 7,
			StatusMoved = 8,
			StatusAltService = 9,
			Pending = 10
		};
	}
}