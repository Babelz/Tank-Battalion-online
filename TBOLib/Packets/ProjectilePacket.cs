using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ProcjectilePacket : IPacket
    {
        #region Fields
        public float x;
        public float y;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ownerGuid;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.ProjectilePacket;
            }
        }
        #endregion
    }
}
