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
        /// Client has joined the server.
        /// </summary>
        ClientJoined,

        /// <summary>
        /// Client has joined a lobby.
        /// </summary>
        ClientJoinedLobby,
        
        /// <summary>
        /// Packet that contains game data.
        /// </summary>
        GameData,

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
        Authentication
    }

    public static class Packet
    {
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
                null, // typeof(ClientJoinedPacket),
                null, // typeof(ClientJoinedLobbyPacket),
                null, // typeof(PlayerDataPacket),
                null, // typeof(InputPacket),
                null, // typeof(MapDataPacket), 
                null, // typeof(GameStateSyncPacket),
                null, // typeof(RoundStatusPacket),
                null, // typeof(GameStatusPacket),
                null,
                typeof(AuthenticationPacket)
            };

            sizes = new int[]
            {
                0,
                Marshal.SizeOf(typeof(PingPacket)),
                0, // Marshal.SizeOf(typeof(ClientJoinedPacket)),
                0, // Marshal.SizeOf(typeof(ClientJoinedLobbyPacket)),
                0, // Marshal.SizeOf(typeof(PlayerDataPacket)),
                0, // Marshal.SizeOf(typeof(InputPacket)),
                0, // Marshal.SizeOf(typeof(MapDataPacket)), 
                0, // Marshal.SizeOf(typeof(GameStateSyncPacket)),
                0, // Marshal.SizeOf(typeof(RoundStatusPacket)),
                0, // Marshal.SizeOf(typeof(GameStatusPacket)),
                0,
                Marshal.SizeOf(typeof(AuthenticationPacket))
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
