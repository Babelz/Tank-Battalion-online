using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            #endregion

            public Player(Client client)
            {
                this.client = client;

                health = 3;
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

        private Body CreatePlayerBody(int x, int y, Player player)
        {
            var body = BodyFactory.CreateRectangle(world,
                                                   ConvertUnits.ToSimUnits(Tiles.Width),
                                                   ConvertUnits.ToSimUnits(Tiles.Height),
                                                   10.0f,
                                                   player);
            body.IsStatic       = false;
            body.Mass           = 80.0f;
            body.Friction       = 0.2f;
            body.Restitution    = 0.2f;
            body.BodyType       = BodyType.Dynamic;
            body.FixedRotation  = true;

            return body;
        }
        private Body CreateTileBody(int x, int y)
        {
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

        public void Initialize(MapData map, params Client[] clients)
        {
            var data = new MapDataPacket(map.width, map.height, map.tiles);

            // Init players.
            for (var i = 0; i < clients.Length; i++) players.Add(new Player(clients[i]));
            
            // Init map.
            var tileIndex   = 0;
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
    }
}
