using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBOLib.Packets
{
    public enum PacketType : byte
    {
        /// <summary>
        /// Packet type is not specified, packets
        /// of this type should be handled as errors.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Packet of type that has data related to 
        /// the new client that is attempting to connect
        /// the server.
        /// </summary>
        ClientConnected,

        /// <summary>
        /// Client has joined the server.
        /// </summary>
        ClientJoined,

        /// <summary>
        /// Client has joined a lobby.
        /// </summary>
        ClientJoinedLobby,

        /// <summary>
        /// Server is asking the client for the client data.
        /// </summary>
        GetClientData,

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
        /// Player got hit by a projectile.
        /// </summary>
        PlayerHit,

        /// <summary>
        /// Round has ended.
        /// </summary>
        RoundEnd,

        /// <summary>
        /// Round has started.
        /// </summary>
        RoundStart,

        /// <summary>
        /// Game has ended.
        /// </summary>
        GameEnd
    }
}
