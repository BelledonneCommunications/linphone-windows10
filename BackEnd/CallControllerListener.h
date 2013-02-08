#pragma once

namespace Linphone
{
	namespace BackEnd
	{
		public interface class CallControllerListener
		{
			void NewCallStarted(Platform::String^ callerNumber);

			void CallEnded();
		};
	}
}