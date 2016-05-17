/*
AlphaKeyGroup.xaml.cs
Copyright (C) 2016  Belledonne Communications, Grenoble, France
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

using System;
using System.Collections.Generic;
using Windows.Globalization.Collation;

namespace Linphone.Model
{

    public class AlphaKeyGroup<T>
    {
        const string GlobeGroupKey = "\uD83C\uDF10";

        public string Key { get; private set; }
        public List<T> InternalList { get; private set; }

        public AlphaKeyGroup(string key)
        {
            Key = key;
            InternalList = new List<T>();
        }

        private static List<AlphaKeyGroup<T>> CreateDefaultGroups(CharacterGroupings slg)
        {
            List<AlphaKeyGroup<T>> list = new List<AlphaKeyGroup<T>>();

            foreach (CharacterGrouping cg in slg)
            {
                if (cg.Label == "") continue;
                if (cg.Label == "...")
                {
                    list.Add(new AlphaKeyGroup<T>(GlobeGroupKey));
                }
                else
                {
                    list.Add(new AlphaKeyGroup<T>(cg.Label));
                }
            }

            return list;
        }

        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, Func<T, string> keySelector, bool sort)
        {
            CharacterGroupings slg = new CharacterGroupings();
            List<AlphaKeyGroup<T>> list = CreateDefaultGroups(slg);

            foreach (T item in items)
            {
                int index = 0;
                {
                    string label = slg.Lookup(keySelector(item));
                    index = list.FindIndex(alphakeygroup => (alphakeygroup.Key.Equals(label, StringComparison.CurrentCulture)));
                }

                if (index >= 0 && index < list.Count)
                {
                    list[index].InternalList.Add(item);
                }
            }

            if (sort)
            {
                foreach (AlphaKeyGroup<T> group in list)
                {
                    group.InternalList.Sort((c0, c1) => { return keySelector(c0).CompareTo(keySelector(c0)); });
                }
            }

            return list;
        }
    }
}