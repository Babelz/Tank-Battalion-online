using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    public enum InputCommand : byte
    {
        None = 0,
        Up,
        Left,
        Down,
        Right,
        Shoot
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct InputPacket : IPacket
    {
        #region Fields
        public InputCommand command;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.Input;
            }
        }
        #endregion

        public InputPacket(InputCommand command)
        {
            this.command = command;
        }
    }
}
