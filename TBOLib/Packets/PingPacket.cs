using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [Serializable()]
    [StructLayout(LayoutKind.Sequential)]
    public struct PingPacket : IPacket
    {
        #region Fields
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
