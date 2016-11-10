using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AuthenticationPacket : IPacket
    {
        #region Fields
        public readonly string contents;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.Authentication;
            }
        }
        #endregion

        public AuthenticationPacket(string contents)
        {
            this.contents = contents;
        }

        public int Size()
        {
            return contents.Length;
        }
    }
}
