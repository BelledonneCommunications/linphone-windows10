#pragma once

namespace Linphone
{
	namespace Core
	{
		public interface class CallControllerListener
		{
			void NewCallStarted(Platform::String^ callerNumber);

			void CallEnded();
		};
	}
}