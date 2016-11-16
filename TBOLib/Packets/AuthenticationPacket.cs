using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct AuthenticationPacket : IPacket
    {
        #region Fields
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string version;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string time;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string response;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string guid;
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

        public AuthenticationPacket(string version, string time, string guid)
        {
            this.version = version;
            this.time    = time;
            this.guid    = guid;

            response     = string.Empty;
        }
    }
}
