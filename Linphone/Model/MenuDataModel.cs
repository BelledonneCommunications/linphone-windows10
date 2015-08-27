/*
MenuDataModel.cs
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public sealed class MenuDataModel
    {
        public static ObservableCollection<MenuEntry> GetEntries()
        {
            ObservableCollection<MenuEntry> entries = new ObservableCollection<MenuEntry>();
            entries.Add(new MenuEntry("Assistant", typeof(AssistantPage)));
            entries.Add(new MenuEntry("Settings", null));
            entries.Add(new MenuEntry("About", null));
            entries.Add(new MenuEntry("License", null));
            return entries;
        }
    }

    public class MenuEntry
    {
        public MenuEntry(String title, Type type)
        {
            Title = title;
            Type = type;
        }
        public String Title { get; private set; }
        public Type Type { get; private set; }
    }
}
