using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PlayerDataPacket : IPacket
    {
        #region Fields 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;

        public float x;
        public float y;

        public int vOrientation;
        public int hOrientation;

        public int health;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.PlayerData;
            }
        }
        #endregion
    }
}
