using Linphone.Core;
using Linphone.Core.OutOfProcess;
using Mediastreamer2.WP8Video;

namespace Linphone.Agents
{
    public static class AgentHost
    {
        static AgentHost()
        {
            AgentHost.videoRenderer = new VideoRenderer();
            Globals.Instance.VideoRenderer = AgentHost.videoRenderer;
        }

        internal static void OnAgentStarted()
        {
            Globals.Instance.StartServer(RegistrationHelper.OutOfProcServerClassNames);
        }

        static VideoRenderer videoRenderer;
    }
}
