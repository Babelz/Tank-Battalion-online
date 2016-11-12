using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBOLib;

namespace TBOServer
{
    public sealed class Match
    {
        #region Player class 
        public sealed class Player
        {
            #region Fields
            public readonly Client client;
            #endregion

            public Player(Client client)
            {
                this.client = client;
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
        private readonly List<Body> map;
        private readonly World world;        
        #endregion

        #region Properties

        #endregion

        public Match()
        {
            world   = new World(Vector2.Zero);
            players = new List<Player>();
            map     = new List<Body>();
        }

        public void Initialize(MapData map, List<Client> clients)
        {
            for (var i = 0; i < clients.Count; i++) players.Add(new Player(clients[i]));

            var tileIndex = 0;

            for (var i = 0; i < map.height; i++)
            {
                for (var j = 0; j < map.width; j++)
                {
                    var tile = map.tiles[tileIndex++];

                    if (tile == 0) continue;

                    if (tile == 1)
                    {
                    }
                    else if (tile == 2)
                    {
                    }
                }
            }
        }
    }
}
