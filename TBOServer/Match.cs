using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
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
        #region Constant fields
        private const float Velocity            = 1.0f;
        private const int ShootingCooldown      = 2500;
        private const int VelocityIterations    = 2;
        #endregion

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

            public int shootingCooldown;
            public int velocitySimSteps;
            #endregion

            public Player(Client client)
            {
                this.client = client;

                health       = 3;
                vOrientation = 0;
                hOrientation = 0;
            }
        }
        #endregion

        #region Projectile class
        public sealed class Projectile
        {
            #region Fields
            public readonly Player sender;

            public readonly int pid;
            #endregion

            public Projectile(Player sender, int pid)
            {
                this.sender = sender;
                this.pid    = pid;
            }
        }
        #endregion

        #region Constant fields
        public const float UnitToPixel = 32.0f;
        public const float PixelToUnit = 1.0f / UnitToPixel;
        public const float TimeStep    = 1.0f / 60.0f;          // Fix from RPG, 60 causes shaking, 30 is just fine!
        #endregion

        #region Fields
        private readonly List<Player> players;
        private readonly List<Body> entitites;
        private readonly World world;

        private int pidGenerator;

        private bool running;
        #endregion

        #region Events
        public event MatchEventHandler Started;
        public event MatchEventHandler Ended;
        #endregion

        #region Properties
        public string this[int index]
        {
            get
            {
                return players[index].client.Name;
            }
        }
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
            if (!running) return;

            var player = players.First(p => p.client == client);

            player.timeFromLastPacket = 0;

            switch (packet.Type)
            {
                case PacketType.Unknown:
                    break;
                case PacketType.Ping:
                    break;
                case PacketType.PlayerData:
                    break;
                case PacketType.Input:
                    var inputPacket = (InputPacket)packet;
                    
                    switch (inputPacket.command)
                    {
                        case InputCommand.Up:
                            player.velocitySimSteps     = VelocityIterations;
                            player.body.LinearVelocity  = new Vector2(0.0f, -Velocity);
                            player.vOrientation         = -1;
                            player.hOrientation         = 0;
                            break;
                        case InputCommand.Left:
                            player.velocitySimSteps     = VelocityIterations;
                            player.body.LinearVelocity  = new Vector2(-Velocity, 0.0f);
                            player.hOrientation         = -1;
                            player.vOrientation         = 0;
                            break;
                        case InputCommand.Down:
                            player.velocitySimSteps     = VelocityIterations;
                            player.body.LinearVelocity  = new Vector2(0.0f, Velocity);
                            player.vOrientation         = 1;
                            player.hOrientation         = 0;
                            break;
                        case InputCommand.Right:
                            player.velocitySimSteps     = VelocityIterations;
                            player.body.LinearVelocity  = new Vector2(Velocity, 0.0f);
                            player.hOrientation         = 1;
                            player.vOrientation         = 0;
                            break;
                        case InputCommand.Shoot:
                            if (player.shootingCooldown <= 0)
                            {
                                player.shootingCooldown = ShootingCooldown;

                                SendProjectile(player);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case PacketType.MapData:
                    break;
                case PacketType.ProjectilePacket:
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

        private bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            var projectile = fixtureA.Body.UserData as Projectile;
            var player     = fixtureB.Body.UserData as Player;
            
            if (projectile != null && player != null)
            {
                player.health -= 1;

                BroadcastPlayerData();
            }

            var packet       = new ProjectilePacket();
            packet.pid       = projectile.pid;
            packet.destroyed = true;

            for (var i = 0; i < players.Count; i++) players[i].client.Send(packet);

            entitites.Remove(fixtureA.Body);
            world.RemoveBody(fixtureA.Body); 

            return true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var player in players)
            {
                player.timeFromLastPacket   += 16;
                player.shootingCooldown     -= 16;

                if (player.client.Available()) player.client.ListenOnce();
            }

            BroadcastPlayerData();

            SimulatePhysics();

            if (!running) (sender as Timer).AutoReset = false; 
        }
        #endregion
        
        private void SendProjectile(Player player)
        {
            const float ProjectileSpawnOffset = 32.0f;

            var px           = ConvertUnits.ToDisplayUnits(player.body.Position.X);
            var py           = ConvertUnits.ToDisplayUnits(player.body.Position.Y);
            var hOrientation = player.hOrientation;
            var vOrientation = player.vOrientation;
            var velocity     = new Vector2(hOrientation * 2.0f, vOrientation * 2.0f);

            // Add offset x.
            if      (hOrientation == -1)     px -= ProjectileSpawnOffset;
            else if (hOrientation == 1)      px += ProjectileSpawnOffset;

            // Add offset y.
            if      (vOrientation == -1)    py -= ProjectileSpawnOffset;
            else if (vOrientation == 1)     py += ProjectileSpawnOffset;

            var npid = pidGenerator++;

            // Create dynamic rectangle for the projectile.
            var body = BodyFactory.CreateRectangle(world,
                                                   ConvertUnits.ToSimUnits(Tiles.Width - 16),    // Leave 8-pixels from size so players
                                                   ConvertUnits.ToSimUnits(Tiles.Height - 16),   // Can move more smoothly.
                                                   10.0f,
                                                   player);

            body.Position            = new Vector2(ConvertUnits.ToSimUnits(px), ConvertUnits.ToSimUnits(py));
            body.IsStatic            = false;
            body.Mass                = 80.0f;
            body.Friction            = 0.2f;
            body.Restitution         = 0.2f;
            body.BodyType            = BodyType.Dynamic;
            body.FixedRotation       = true;
            body.LinearVelocity      = velocity;
            body.UserData            = new Projectile(player, npid);
            body.CollidesWith        = Category.Cat2 | Category.Cat1;
            body.CollisionCategories = Category.Cat2;
            
            body.OnCollision         += Body_OnCollision;
            
            entitites.Add(body);

            var projectilePacket       = new ProjectilePacket();
            projectilePacket.x         = px;
            projectilePacket.y         = py;
            projectilePacket.velx      = velocity.X;
            projectilePacket.vely      = velocity.Y;
            projectilePacket.destroyed = false;
            projectilePacket.pid       = npid;
            projectilePacket.ownerGuid = player.client.Guid.ToString();
            
            for (var i = 0; i < players.Count; i++) players[i].client.Send(projectilePacket);
        }

        private Body CreatePlayerBody(int x, int y, Player player)
        {
            // Create dynamic rectangle for the player.
            var body = BodyFactory.CreateRectangle(world,
                                                   ConvertUnits.ToSimUnits(Tiles.Width - 8),    // Leave 8-pixels from size so players
                                                   ConvertUnits.ToSimUnits(Tiles.Height - 8),   // Can move more smoothly.
                                                   10.0f,
                                                   player);
            body.Position               = new Vector2(ConvertUnits.ToSimUnits(x), ConvertUnits.ToSimUnits(y));
            body.IsStatic               = false;
            body.Mass                   = 80.0f;
            body.Friction               = 0.2f;
            body.Restitution            = 0.2f;
            body.BodyType               = BodyType.Dynamic;
            body.FixedRotation          = true;
            body.CollidesWith           = Category.Cat2 | Category.Cat1;
            body.CollisionCategories    = Category.Cat2;
            
            // Store body.
            player.body         = body;

            return body;
        }
        private Body CreateTileBody(int x, int y)
        {
            // Create static tiles for the walls.
            var body = BodyFactory.CreateRectangle(world,
                                                   ConvertUnits.ToSimUnits(Tiles.Width - 4),
                                                   ConvertUnits.ToSimUnits(Tiles.Height - 4),
                                                   10.0f,
                                                   null);

            body.Position            = new Vector2(ConvertUnits.ToSimUnits(x), ConvertUnits.ToSimUnits(y));
            body.IsStatic            = true;
            body.Mass                = 500.0f;
            body.Friction            = 0.0f;
            body.Restitution         = 0.2f;
            body.BodyType            = BodyType.Static;
            body.FixedRotation       = true;
            body.CollisionCategories = Category.Cat1;
            body.CollidesWith        = Category.Cat2;

            return body;
        }

        private void CreatePlayers(params Client[] clients)
        {
            // Init players.
            foreach (var client in clients)
            {
                var player = new Player(client);

                player.vOrientation = 1;
                player.hOrientation = 0;

                players.Add(player);

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

            if (!running) return;

            // Check for dead players.
            var deadPlayer = players.FirstOrDefault(p => p.health <= 0);
            
            if (deadPlayer != null)
            {
                BroadcastGameEndPacket(deadPlayer);

                running = false;
            }
        }

        private void BroadcastGameEndPacket(Player deadPlayer)
        {
            var contents = string.Format("Player \"{0}\" won!", players.FirstOrDefault(p => !ReferenceEquals(deadPlayer, p)).client.Name);

            var packet      = new GameEndPacket();
            packet.contents = contents;

            for (var i = 0; i < players.Count; i++) players[i].client.Send(packet);

            Ended?.Invoke(this);
        }
        
        private void SimulatePhysics()
        {
            world.Step(TimeStep);

            foreach (var player in players)
            {
                if (player.velocitySimSteps < 0)
                {
                    player.body.LinearVelocity = Vector2.Zero;

                    continue;
                }
                
                player.velocitySimSteps--;
            }
        }

        private void StartMatch()
        {
            var timer       = new Timer();
            timer.Elapsed   += Timer_Elapsed;
            timer.Enabled   = true;
            timer.AutoReset = true;
            timer.Interval  = 16;

            timer.Start();

            Started?.Invoke(this);
        }

        public void Start(MapData map, params Client[] clients)
        {
            if (running) return;

            running = true;

            CreatePlayers(clients);

            SendMapData(map);

            CreateMapEntitites(map);

            StartMatch();
        }

        public delegate void MatchEventHandler(Match match);
    }
}
