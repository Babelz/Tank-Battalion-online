using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    public struct PingPacket : IPacket
    {
        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.Ping;
            }
        }

        public string Time
        {
            get;
            set;
        }
        #endregion
    }
}
