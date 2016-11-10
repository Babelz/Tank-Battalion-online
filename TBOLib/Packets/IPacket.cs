using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    /// <summary>
    /// Interface that all packets need to implement.
    /// </summary>
    public interface IPacket
    {
        #region Properties
        /// <summary>
        /// Returns the type of the packet.
        /// </summary>
        PacketType Type
        {
            get;
        }
        #endregion
    }
}
