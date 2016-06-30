/*
CallControllerListener.cs
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

using BelledonneCommunications.Linphone.Native;

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
        void CallEnded(Call call);

        /// <summary>
        /// Called when receiving an incoming call 
        /// </summary>
        void CallIncoming(Call call);

        /// <summary>
        /// Called when the mute status of the microphone changes.
        /// </summary>
        void MuteStateChanged(bool isMicMuted);

        /// <summary>
        /// Called when the call changes its state to paused or resumed.
        /// </summary>
        void PauseStateChanged(Call call, bool isCallPaused, bool isCallPausedByRemote);

        /// <summary>
        /// Called when the call is updated by the remote party.
        /// </summary>
        /// <param name="call">The call that has been updated</param>
        /// <param name="isVideoAdded">A boolean telling whether the remote party added video</param>
        void CallUpdatedByRemote(Call call, bool isVideoAdded);
    }
}
