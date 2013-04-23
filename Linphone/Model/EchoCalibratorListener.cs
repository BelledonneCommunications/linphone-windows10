using Linphone.Core;

namespace Linphone.Model
{
    /// <summary>
    /// Interface for an echo canceller calibrator listener.
    /// </summary>
    public interface EchoCalibratorListener
    {
        /// <summary>
        /// Called when a new call is started.
        /// </summary>
        void ECStatusNotified(EcCalibratorStatus status, int delayMs);
    }
}
