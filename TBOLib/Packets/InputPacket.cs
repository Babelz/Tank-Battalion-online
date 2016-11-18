using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct InputPacket : IPacket
    {
        #region Properties
        public PacketType Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
