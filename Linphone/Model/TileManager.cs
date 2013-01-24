using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Model
{
    public class TileManager
    {
        private static TileManager singleton;
        public static TileManager Instance
        {
            get
            {
                if (TileManager.singleton == null)
                    TileManager.singleton = new TileManager();

                return TileManager.singleton;
            }
        }

        public TileManager()
        {

        }

        /// <summary>
        /// Displays the number of missed call on the live tile and on the lock screen
        /// </summary>
        /// <param name="missedCalls">Number of missed calls</param>
        public void UpdateTileWithMissedCalls(int missedCalls)
        {
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
            var data = new StandardTileData();
            data.Count = missedCalls;
            tile.Update(data);
        }

        /// <summary>
        /// Delete the missed call display from the live tile and the lock screen
        /// </summary>
        public void RemoveMissedCallsTile()
        {
            UpdateTileWithMissedCalls(0);
        }
    }
}
