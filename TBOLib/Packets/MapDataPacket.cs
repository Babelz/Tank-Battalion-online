using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MapDataPacket : IPacket
    {
        #region Fields
        public int width;

        public int height;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] tiles;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.MapData;
            }
        }
        #endregion

        public MapDataPacket(int width, int height, byte[] tiles)
        {
            this.width  = width;
            this.height = height;

            this.tiles = new byte[1024];

            Array.Copy(tiles, this.tiles, tiles.Length);
        }
    }
}
