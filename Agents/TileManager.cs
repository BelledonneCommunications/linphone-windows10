/*
TileManager.cs
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

using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Linphone.Agents
{
    /// <summary>
    /// Utility class used to handle everything that is Tile related
    /// </summary>
    public class TileManager
    {
        private string TileCountKey = "TileCount";
        private IsolatedStorageSettings settings;

        private static TileManager singleton;
        /// <summary>
        /// Static instance to access the class.
        /// </summary>
        public static TileManager Instance
        {
            get
            {
                if (TileManager.singleton == null)
                    TileManager.singleton = new TileManager();

                return TileManager.singleton;
            }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public TileManager()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Update the count to display on the Tile, and update the Tile with the new count
        /// </summary>
        /// <param name="toAdd"></param>
        public void UpdateCount(int toAdd)
        {
            int count = 0;
            try
            {
                count = (int)settings[TileCountKey];
                count += toAdd;
                settings[TileCountKey] = count;
            }
            catch (KeyNotFoundException)
            {
                settings.Add(TileCountKey, toAdd);
                count = toAdd;
            }
            settings.Save();
            UpdateTile(count);
        }

        private void UpdateTile(int count)
        {
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
            if (tile != null)
            {
                var data = new IconicTileData();
                data.Count = count;
                tile.Update(data);
            }
        }

        /// <summary>
        /// Delete the missed call display from the live tile and the lock screen
        /// </summary>
        public void RemoveCountOnTile()
        {
            try
            {
                settings[TileCountKey] = 0;
            }
            catch (KeyNotFoundException)
            {
                settings.Add(TileCountKey, 0);
            }
            settings.Save();
            UpdateTile(0);
        }
    }
}
