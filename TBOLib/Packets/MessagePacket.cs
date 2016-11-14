using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MessagePacket : IPacket
    {
        #region Fields 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string contents;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.Message;
            }
        }
        #endregion

        public MessagePacket(string contents)
        {
            this.contents = contents;
        }
    }
}
