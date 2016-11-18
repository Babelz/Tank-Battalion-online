using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TBOLib;
using TBOLib.Packets;

namespace TBOServer
{
    public sealed class Match
    {
        #region Player class 
        public sealed class Player
        {
            #region Fields
            public readonly Client client;

            public Body body;

            public int health;

            public int vOrientation;
            public int hOrientation;

            public int timeFromLastPacket;
            #endregion

            public Player(Client client)
            {
                this.client = client;

                health       = 3;
                vOrientation = 0;
                hOrientation = 0;
            }
        }

        public sealed class Projectile
        {
            #region Fields
            public readonly Player sender;
            #endregion

            public Projectile(Player sender)
            {
                this.sender = sender;
            }
        }
        #endregion

        #region Constant fields
        public const float UnitToPixel = 32.0f;
        public const float PixelToUnit = 1.0f / UnitToPixel;
        public const float TimeStep    = 1.0f / 30.0f;          // Fix from RPG, 60 causes shaking, 30 is just fine!
        #endregion

        #region Fields
        private readonly List<Player> players;
        private readonly List<Body> entitites;
        private readonly World world;        
        #endregion

        #region Properties

        #endregion

        public Match()
        {
            world     = new World(Vector2.Zero);
            players   = new List<Player>();
            entitites = new List<Body>();
        }

        #region Event handlers
        private void Client_Received(Client client, IPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.Unknown:
                    break;
                case PacketType.Ping:
                    break;
                case PacketType.PlayerData:
                    break;
                case PacketType.Input:
                    break;
                case PacketType.MapData:
                    break;
                case PacketType.GameStateSync:
                    break;
                case PacketType.RoundStatus:
                    break;
                case PacketType.GameStatus:
                    break;
                case PacketType.Authentication:
                    break;
                case PacketType.ServerStatus:
                    break;
                case PacketType.Message:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Event handlers
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            BroadcastPlayerData();
        }
        #endregion

        private Body CreatePlayerBody(int x, int y, Player player)
        {
            // Create dynamic rectangle for the player.
            var body = BodyFactory.CreateRectangle(world,
                                                   ConvertUnits.ToSimUnits(Tiles.Width),
                                                   ConvertUnits.ToSimUnits(Tiles.Height),
                                                   10.0f,
                                                   player);
            body.Position       = new Vector2(ConvertUnits.ToSimUnits(x), ConvertUnits.ToSimUnits(y));
            body.IsStatic       = false;
            body.Mass           = 80.0f;
            body.Friction       = 0.2f;
            body.Restitution    = 0.2f;
            body.BodyType       = BodyType.Dynamic;
            body.FixedRotation  = true;

            // Store body.
            player.body         = body;

            return body;
        }
        private Body CreateTileBody(int x, int y)
        {
            // Create static tiles for the walls.
            var body = BodyFactory.CreateRectangle(world,
                                                   ConvertUnits.ToSimUnits(Tiles.Width),
                                                   ConvertUnits.ToSimUnits(Tiles.Height),
                                                   10.0f,
                                                   null);

            body.Position       = new Vector2(ConvertUnits.ToSimUnits(x), ConvertUnits.ToSimUnits(y));
            body.IsStatic       = true;
            body.Mass           = 500.0f;
            body.Friction       = 0.2f;
            body.Restitution    = 0.2f;
            body.BodyType       = BodyType.Static;
            body.FixedRotation  = true;

            return body;
        }

        private void CreatePlayers(params Client[] clients)
        {
            // Init players.
            foreach (var client in clients)
            {
                players.Add(new Player(client));

                client.Received += Client_Received;
            }
        }

        private void SendMapData(MapData map)
        {
            var packet = new MapDataPacket(map.width, map.height, map.tiles);

            for (var i = 0; i < players.Count; i++) players[i].client.Send(packet);
        }
        private void CreateMapEntitites(MapData map)
        {   
            // Init map.
            var tileIndex = 0;
            var playerIndex = 0;

            for (var i = 0; i < map.height; i++)
            {
                for (var j = 0; j < map.width; j++)
                {
                    var tile = map.tiles[tileIndex++];

                    if (tile == Tiles.Empty) continue;

                    if (tile == Tiles.Blocked)      entitites.Add(CreateTileBody(j * Tiles.Width, i * Tiles.Height));
                    else if (tile == Tiles.Spawn)   entitites.Add(CreatePlayerBody(j * Tiles.Width, i * Tiles.Height, players[playerIndex++]));
                }
            }
        }

        private void BroadcastPlayerData()
        {
            var packets = new IPacket[players.Count];
            var pindex  = 0;

            foreach (var player in players)
            {
                PlayerDataPacket packet;
                packet.name         = player.client.Name;
                packet.x            = ConvertUnits.ToDisplayUnits(player.body.Position.X);
                packet.y            = ConvertUnits.ToDisplayUnits(player.body.Position.Y);
                packet.vOrientation = player.vOrientation;
                packet.hOrientation = player.hOrientation;
                packet.health       = player.health;
                packet.guid         = player.client.Guid.ToString();

                packets[pindex++] = packet;
            }

            for (var i = 0; i < players.Count; i++) players[i].client.Send(packets);
        }
        
        private void StartMatch()
        {
            var timer       = new Timer();
            timer.Elapsed   += Timer_Elapsed;
            timer.Enabled   = true;
            timer.AutoReset = true;

            timer.Start();
        }

        public void Start(MapData map, params Client[] clients)
        {
            CreatePlayers(clients);

            SendMapData(map);

            CreateMapEntitites(map);

            StartMatch();
        }
    }
}
