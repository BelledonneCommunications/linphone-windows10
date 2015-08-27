/*
AssistantModel.cs
Copyright(C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public enum AssistantState
    {
        Welcome,
        CreateLinphoneAccount,
        CreateLinphoneAccountValidationPending,
        UseLinphoneAccount,
        UseSipAccount,
        DownloadConfiguration
    }

    public class AssistantModel
    {
        public AssistantModel()
        {
            State = AssistantState.Welcome;
        }

        private List<AssistantState> _states_history = new List<AssistantState>();

        private AssistantState _state;
        public AssistantState State
        {
            get
            {
                return _state;
            }
            set
            {
                _states_history.Add(_state);
                _state = value;
            }
        }

        public AssistantState PreviousState
        {
            get
            {
                _state = _states_history.Last<AssistantState>();
                _states_history.Remove(_state);
                return _state;
            }
        }
    }
}
