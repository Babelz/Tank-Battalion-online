using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PingPacket : IPacket
    {
        #region Fields 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string contents;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.Ping;
            }
        }
        #endregion

        public PingPacket(string contents)
        {
            this.contents = contents;
        }
    }
}
