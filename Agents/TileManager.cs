using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linphone.Agents
{
    /// <summary>
    /// Utility class used to handle everything that is Tile related
    /// </summary>
    public class TileManager
    {
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

        }

        /// <summary>
        /// Displays the number of missed call and unread messages on the live tile and on the lock screen
        /// </summary>
        /// <param name="missedCalls">Number of missed calls + number of unread messages</param>
        public void UpdateTileWithMissedCallsAndUnreadMessages(int count)
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
            UpdateTileWithMissedCallsAndUnreadMessages(0);
        }
    }
}
