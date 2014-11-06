using Linphone.Core;
using Linphone.Core.OutOfProcess;
using Mediastreamer2.WP8Video;
using System;
using System.IO;
using Windows.Storage;

namespace Linphone.Agents
{
    public static class AgentHost
    {
        static VideoRenderer videoRenderer = new VideoRenderer();

        static AgentHost()
        {
            Globals.Instance.VideoRenderer = AgentHost.videoRenderer;
        }

        internal static void OnAgentStarted()
        {
            Globals.Instance.StartServer(RegistrationHelper.OutOfProcServerClassNames);
        }
    }
}
