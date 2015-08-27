/*
HistoryDataModel.cs
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
    class HistoryDataModel
    {
        private static HistoryDataModel _dataModel = new HistoryDataModel();
        private ObservableCollection<HistoryGroup> _history = new ObservableCollection<HistoryGroup>();

        public ObservableCollection<HistoryGroup> History
        {
            get { return this._history; }
        }

        public static IEnumerable<HistoryGroup> GetHistory()
        {
            return _dataModel.Fill();
        }

        private IEnumerable<HistoryGroup> Fill()
        {
            if (History.Count != 0) return History;

            // TODO: Fill with real data
            DateTime now = DateTime.Now;
            AddEntry(new HistoryEntry(now, HistoryEntryState.Incoming, "Gautier PELLOUX-PRAYER"));
            AddEntry(new HistoryEntry(now, HistoryEntryState.Outgoing, "François GRISEZ"));
            AddEntry(new HistoryEntry(now, HistoryEntryState.Incoming, "Margaux CLERC"));
            DateTime yesterday = now.AddDays(-1);
            AddEntry(new HistoryEntry(yesterday, HistoryEntryState.Missed, "Sylvain BERFINI"));
            AddEntry(new HistoryEntry(yesterday, HistoryEntryState.Outgoing, "Guillaume BIENKOWSKI"));
            DateTime threedaysago = now.AddDays(-3);
            AddEntry(new HistoryEntry(threedaysago, HistoryEntryState.Missed, "Simon MORLAT"));
            AddEntry(new HistoryEntry(threedaysago, HistoryEntryState.Missed, "Jehan MONNIER"));
            AddEntry(new HistoryEntry(threedaysago, HistoryEntryState.Incoming, "Marielle RELLIER"));
            AddEntry(new HistoryEntry(threedaysago, HistoryEntryState.Outgoing, "Ghislain MARY"));
            return History;
        }

        private void AddEntry(HistoryEntry entry)
        {
            HistoryGroup group = null;
            string entryDate = entry.Date.ToString("dd/MM/yyyy");
            foreach (HistoryGroup hg in History)
            {
                if (hg.Name == entryDate)
                {
                    group = hg;
                }
            }
            if (group == null)
            {
                group = new HistoryGroup(entryDate, entry.Date.Year, entry.Date.Month, entry.Date.Day);
                History.Add(group);
            }
            group.Entries.Add(entry);
        }
    }

    public class HistoryGroup
    {
        public HistoryGroup(string name, int year, int month, int day)
        {
            Name = name;
            Date = new DateTime(year, month, day);
            Entries = new ObservableCollection<HistoryEntry>();
        }

        public string Name { get; private set; }
        public DateTime Date { get; private set; }
        public ObservableCollection<HistoryEntry> Entries { get; private set; }
    }

    public enum HistoryEntryState
    {
        Incoming,
        Outgoing,
        Missed
    }

    public class HistoryEntry
    {
        public HistoryEntry(DateTime date, HistoryEntryState state, string name)
        {
            Date = date;
            State = state;
            Name = name;
        }

        public DateTime Date { get; private set; }
        public HistoryEntryState State { get; private set; }
        public string Name { get; private set; }
    }
}
