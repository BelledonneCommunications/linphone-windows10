
namespace Linphone.Model
{
    /// <summary>
    /// Interface for a basic call (start, end) listener.
    /// </summary>
    public interface CallControllerListener
    {
        /// <summary>
        /// Called when a new call is started.
        /// </summary>
        void NewCallStarted(string callerNumber);

        /// <summary>
        /// Called when a call is ended.
        /// </summary>
        void CallEnded();

        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        void MuteStateChanged(bool isMicMuted);

        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        void PauseStateChanged(bool isCallPaused); 
    }
}
