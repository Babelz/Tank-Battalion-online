using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    public enum PacketType : int
    {
        /// <summary>
        /// Packet type is not specified, packets
        /// of this type should be handled as errors.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Ping packet.
        /// </summary>
        Ping,

        /// <summary>
        /// Packet that contains player data.
        /// </summary>
        PlayerData,

        /// <summary>
        /// Player input packet.
        /// </summary>
        Input,

        /// <summary>
        /// Packet containing data of the current map.
        /// </summary>
        MapData,

        /// <summary>
        /// Contains the whole game state that lives inside the server.
        /// </summary>
        GameStateSync,

        /// <summary>
        /// Contains data about the status of the round.
        /// </summary>
        RoundStatus,
        
        /// <summary>
        /// Contains data about the game status.
        /// </summary>
        GameStatus,

        /// <summary>
        /// Packet used to authenticate/validate the clients.
        /// </summary>
        Authentication,

        /// <summary>
        /// Packet that contains information about the server.
        /// </summary>
        ServerStatus,

        /// <summary>
        /// Generic message packet.
        /// </summary>
        Message
    }

    public static class Packet
    {
        #region Constant fields
        public const int HeaderSize     = sizeof(int);
        public const int PacketTypeSize = sizeof(byte);
        #endregion

        #region Fields
        private static readonly Type[] types;

        private static readonly int[] sizes;

        private static readonly int maxSize;
        #endregion

        static Packet()
        {
            types = new Type[]
            {
                null,
                typeof(PingPacket),
                typeof(PlayerDataPacket),
                typeof(InputPacket),
                typeof(MapDataPacket), 
                null, // typeof(GameStateSyncPacket),
                null, // typeof(RoundStatusPacket),
                null, // typeof(GameStatusPacket),
                typeof(AuthenticationPacket),
                typeof(ServerStatusPacket),
                typeof(MessagePacket)
            };

            sizes = new int[]
            {
                0,
                Marshal.SizeOf(typeof(PingPacket)),
                Marshal.SizeOf(typeof(PlayerDataPacket)),
                Marshal.SizeOf(typeof(InputPacket)),
                Marshal.SizeOf(typeof(MapDataPacket)), 
                0, // Marshal.SizeOf(typeof(GameStateSyncPacket)),
                0, // Marshal.SizeOf(typeof(RoundStatusPacket)),
                0, // Marshal.SizeOf(typeof(GameStatusPacket)),
                Marshal.SizeOf(typeof(AuthenticationPacket)),
                Marshal.SizeOf(typeof(ServerStatusPacket)),
                Marshal.SizeOf(typeof(MessagePacket))
            };

            maxSize = sizes.Max();
        }

        public static Type GetType(PacketType type)
        {
            var index = (int)type;

            return types[index];
        }

        public static int GetSize(PacketType type)
        {
            var index = (int)type;

            return sizes[index];
        }

        public static int GetMaxSize()
        {
            return maxSize;
        }
    }
}
