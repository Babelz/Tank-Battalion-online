using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBOServer
{
    // 0 - empty
    // 1 - blocked tile
    // 2 - spawn

    public struct MapData
    {
        public readonly int width;
        public readonly int height;

        public readonly int maxPlayers;

        public readonly byte[] tiles;

        public MapData(int width, int height, byte[] tiles)
        {
            this.width  = width;
            this.height = height;
            this.tiles  = tiles;

            maxPlayers  = tiles.Count(t => t == 2);
        }
    }

    public static class Maps
    {
        #region Map objects
        public static readonly MapData Map1v1;
        #endregion

        static Maps()
        {
            // Map1v1 init.
            const int Map1v1Width  = 16;
            const int Map1v1Height = 16;

            var tiles = new string[Map1v1Height]
            {
                "################",
                "#x             #",
                "# ############ #",
                "#      #       #",
                "#      #       #",
                "#      #  ###  #",
                "#              #",
                "#  ####### ##  #",
                "#  #           #",
                "#  #  ####     #",
                "#  #  #      # #",
                "#            # #",
                "#  #  #  #   # #",
                "# ## ### ##### #",
                "#             x#",
                "################",
            };

            var bytes = new List<byte>(Map1v1Width * Map1v1Height);

            Array.ForEach(tiles, 
                s => bytes.AddRange(s.Select(
                    c => (byte)(c == '#' ? 1 : c == 'x' ? 2 : 0))));

            Map1v1 = new MapData(Map1v1Width, Map1v1Height, bytes.ToArray());
        }
    }
}
