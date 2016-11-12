using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ServerStatusPacket : IPacket
    {
        #region Fields
        public int playersInLobby;

        public int playersPlaying;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.ServerStatus;
            }
        }
        #endregion

        public ServerStatusPacket(int playersInLobby, int playersPlaying)
        {
            this.playersInLobby = playersInLobby;
            this.playersPlaying = playersPlaying;
        }
    }
}
