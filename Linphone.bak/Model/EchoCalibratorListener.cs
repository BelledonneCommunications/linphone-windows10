/*
EchoCalibratorListener.cs
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
