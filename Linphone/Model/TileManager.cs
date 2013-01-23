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

        public void UpdateTileWithMissedCalls(int missedCalls)
        {
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
            var data = new StandardTileData();
            data.Count = missedCalls;
            tile.Update(data);
        }

        public void RemoveMissedCallsTile()
        {
            UpdateTileWithMissedCalls(0);
        }
    }
}
