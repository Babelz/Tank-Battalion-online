﻿using System;
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string version;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string time;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string response;
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

        public AuthenticationPacket(string version, string time)
        {
            this.version = version;
            this.time    = time;

            response     = string.Empty;
        }
    }
}
