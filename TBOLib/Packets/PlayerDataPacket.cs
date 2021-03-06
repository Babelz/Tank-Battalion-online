﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PlayerDataPacket : IPacket
    {
        #region Fields 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;

        public float x;
        public float y;

        public int vOrientation;
        public int hOrientation;

        public int health;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string guid;
        #endregion

        #region Properties
        public PacketType Type
        {
            get
            {
                return PacketType.PlayerData;
            }
        }
        #endregion

        public static bool operator !=(PlayerDataPacket lhs, PlayerDataPacket rhs)
        {
            return !(lhs == rhs);
        }
        public static bool operator ==(PlayerDataPacket lhs, PlayerDataPacket rhs)
        {
            var lhsGuid = Guid.Parse(lhs.guid);
            var rhsGuid = Guid.Parse(rhs.guid);

            return lhsGuid == rhsGuid;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }
    }
}
