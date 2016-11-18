using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using TBOLib;
using TBOLib.Packets;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TBOClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TBOGame : Game
    {
        #region Game state enum
        private enum GameState
        {
            Connecting,
            Lobby,
            WaitingForGameplay,
            Gameplay,
            Disconnected
        }
        #endregion

        #region Fields
        private readonly GraphicsDeviceManager graphics;
        
        private SpriteBatch spriteBatch;

        private GameInfoLog infoLog;

        private Client client;

        private ServerStatusPacket? serverState;
        
        private string[] waitDisplayStrings;
        private int waitElapsed;

        private GameState gameState;

        private const int SpriteWidth  = 32;
        private const int SpriteHeight = 32;

        private Texture2D sprites;
        private Rectangle wallSrc;
        private Rectangle playerSrc;
        private Rectangle projectileSrc;

        private MapDataPacket map;

        private List<PlayerDataPacket?> players;
        #endregion

        public TBOGame()
        {
            graphics                            = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth   = 1280;
            graphics.PreferredBackBufferHeight  = 720;
            graphics.PreferMultiSampling        = true;
            graphics.ApplyChanges();

            Content.RootDirectory               = "Content";
        }

        #region Event handlers
        private void Client_Connected(Client client)
        {
            infoLog.AddEntry(EntryType.Message, "connected!");

            gameState       = GameState.Lobby;
            client.Received += Client_Received;

            client.BeginListen();
        }
        private void Client_Received(Client client, IPacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.Unknown:
                    break;
                case PacketType.Ping:
                    var pong = (PingPacket)packet;

                    pong.contents = "PONG";

                    client.Send(pong);

                    infoLog.AddEntry(EntryType.Message, string.Format("responding to ping at {0}", DateTime.Now.ToLongTimeString()));
                    break;
                case PacketType.PlayerData:
                    if (players.Count == 2) players.Clear();

                    var playerData = (PlayerDataPacket)packet;
                    
                    players.Add(playerData);
                    break;
                case PacketType.MapData:
                    map = (MapDataPacket)packet;

                    infoLog.AddEntry(EntryType.Message, "got map data from server...");
                    break;
                case PacketType.GameStateSync:
                    break;
                case PacketType.RoundStatus:
                    break;
                case PacketType.GameStatus:
                    break;
                case PacketType.Authentication:
                    var authentication = (AuthenticationPacket)packet;

                    client.Guid             = Guid.Parse(authentication.guid);
                    authentication.response = string.Format("NAME:{0}", Configuration.Name);

                    infoLog.AddEntry(EntryType.Message, "responding to authentication request...");

                    client.Send(authentication);
                    break;
                case PacketType.ServerStatus:
                    serverState = (ServerStatusPacket)packet;
                    break;
                case PacketType.Message:
                    var message = (MessagePacket)packet;

                    if (message.contents.StartsWith("found an opponent"))
                    {
                        gameState           = GameState.WaitingForGameplay;
                        waitDisplayStrings  = message.contents.Split('\n');
                        waitElapsed         = 3000;
                    }
                    
                    infoLog.AddEntry(EntryType.Message, "joined match!");
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void InitializeClient()
        {
            client = new Client(Configuration.Name);

            client.Connected += Client_Connected;
        }

        private void Connect()
        {
            var address = Configuration.ServerAddress;
            var port    = Configuration.Port;

            infoLog.AddEntry(EntryType.Message, string.Format("connecting to {0}:{1}", address, port));

            client.Connect(address, port);
        }

        private void ReadInput()
        {
            var state = Keyboard.GetState();

            InputPacket? moveInputPacket    = null;
            InputPacket? shootInputPacket   = null;

            if (state.IsKeyDown(Keys.W))      moveInputPacket = new InputPacket(InputCommand.Up);
            else if (state.IsKeyDown(Keys.A)) moveInputPacket = new InputPacket(InputCommand.Left);
            else if (state.IsKeyDown(Keys.S)) moveInputPacket = new InputPacket(InputCommand.Down);
            else if (state.IsKeyDown(Keys.D)) moveInputPacket = new InputPacket(InputCommand.Right);

            if (state.IsKeyDown(Keys.Space)) shootInputPacket = new InputPacket(InputCommand.Shoot);
            
            if (!client.IsConnected())
            {
                gameState = GameState.Disconnected;
            }

            if (moveInputPacket != null)  client.Send(moveInputPacket);
            if (shootInputPacket != null) client.Send(shootInputPacket);
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Services.AddService(spriteBatch);

            infoLog = new GameInfoLog(this);
            Components.Add(infoLog);

            base.Initialize();

            InitializeClient();
            Connect();
            
            wallSrc       = new Rectangle(0, 0, SpriteWidth, SpriteHeight);
            playerSrc     = new Rectangle(SpriteWidth, 0, SpriteWidth, SpriteHeight);
            projectileSrc = new Rectangle(SpriteWidth * 2, 0, SpriteWidth, SpriteHeight);

            players       = new List<PlayerDataPacket?>();
        }

        protected override void LoadContent()
        {
            sprites = Content.Load<Texture2D>("sprites");
        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            if (gameState == GameState.Gameplay) ReadInput();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // Draw lobby state...
            if (gameState == GameState.Lobby && serverState != null)
            {
                var font = Content.Load<SpriteFont>("info log");

                var online = string.Format("online: {0}", serverState?.playersPlaying + serverState?.playersInLobby);
                var lobby = string.Format("in lobby: {0}", serverState?.playersInLobby);
                var playing = string.Format("playing: {0}", serverState?.playersPlaying);

                var position = new Vector2(graphics.PreferredBackBufferWidth / 2.0f, graphics.PreferredBackBufferHeight / 2.0f);

                const float Offset = 32.0f;

                var onlinePos = position;
                onlinePos.X -= font.MeasureString(online).X / 2.0f;
                onlinePos.Y += font.MeasureString(online).Y / 2.0f + Offset;

                var lobbyPos = position;
                lobbyPos.X -= font.MeasureString(lobby).X / 2.0f;

                var playingPos = position;
                playingPos.X -= font.MeasureString(playing).X / 2.0f;
                playingPos.Y -= font.MeasureString(playing).Y / 2.0f + Offset;

                spriteBatch.DrawString(font, online, onlinePos, Color.White);
                spriteBatch.DrawString(font, lobby, lobbyPos, Color.White);
                spriteBatch.DrawString(font, playing, playingPos, Color.White);
            }
            else if (gameState == GameState.WaitingForGameplay)
            {
                waitElapsed -= gameTime.ElapsedGameTime.Milliseconds;

                if (waitElapsed <= 0)
                {
                    gameState = GameState.Gameplay;
                }
                else
                {
                    var font = Content.Load<SpriteFont>("info log");
                    
                    var first  = waitDisplayStrings[0];
                    var second = waitDisplayStrings[1].Replace("{s}", ((int)Math.Round(waitElapsed / 1000.0f, 0)).ToString());
                    var third  = waitDisplayStrings[2];

                    var center = new Vector2(graphics.PreferredBackBufferWidth / 2.0f, graphics.PreferredBackBufferHeight / 2.0f);

                    const float Offset = 32.0f;

                    var firstSize = font.MeasureString(first);
                    var secondSize = font.MeasureString(second);
                    var thirdSize = font.MeasureString(third);

                    var firstPos = center;
                    firstPos.X -= firstSize.X / 2.0f;
                    firstPos.Y -= firstSize.Y + Offset;

                    var secondPos = center;
                    secondPos.X -= secondSize.X / 2.0f;

                    var thirdPos = center;
                    thirdPos.X -= firstSize.X / 2.0f;
                    thirdPos.Y += thirdSize.Y + Offset;

                    spriteBatch.DrawString(font, first, firstPos, Color.White);
                    spriteBatch.DrawString(font, second, secondPos, Color.White);
                    spriteBatch.DrawString(font, third, thirdPos, Color.White);
                }
            }
            else if (gameState == GameState.Gameplay)
            {
                // Draw map.
                var tileIndex = 0;

                const int WallType = 1;

                for (var i = 0; i < map.height; i++)
                {
                    for (var j = 0; j < map.width; j++)
                    {
                        var tileType = map.tiles[tileIndex++];

                        if (tileType == WallType) spriteBatch.Draw(sprites, new Rectangle(j * SpriteWidth, i * SpriteHeight, SpriteWidth, SpriteHeight), wallSrc, Color.White);
                    }
                }

                // Draw players.
                for (var i = 0; i < players.Count; i++)
                {
                    if (players[i] == null) continue;

                    var player       = players[i].Value;
                    var pos          = new Vector2(player.x, player.y);
                    var health       = player.health;
                    var color        = Guid.Parse(player.guid) == client.Guid ? Color.Green : Color.Red;
                    var name         = player.name;
                    var vOrientation = player.vOrientation;
                    var hOrientation = player.hOrientation;
                    var font         = Content.Load<SpriteFont>("info log");
                    var textOffset   = new Vector2(32.0f);
                    var effects      = SpriteEffects.None;
                    var rot          = 0.0f;
                    var orig         = Vector2.Zero;
                    
                    if (vOrientation == -1) effects |= SpriteEffects.FlipVertically;

                    if (hOrientation == 0)       rot = 0.0f;
                    else if (hOrientation == -1) rot = MathHelper.ToRadians(90.0f);
                    else if (hOrientation == 1)  rot = MathHelper.ToRadians(-90.0f);

                    if (hOrientation == -1) orig = new Vector2(0.0f, 32.0f);
                    if (hOrientation == 1)  orig = new Vector2(32.0f, 0.0f);

                    if (color == Color.Green)
                    {
                        // Draw our hud.
                        var text     = string.Format("{0} - health: {1}", name, health);
                        var textSize = font.MeasureString(text);

                        spriteBatch.DrawString(font, 
                                               text, 
                                               new Vector2(textOffset.X, 
                                                           graphics.PreferredBackBufferHeight - textSize.Y - textOffset.Y), 
                                               color);
                    }
                    else if (color == Color.Red)
                    {
                        // Draw enemy hud.
                        var text     = string.Format("{0} - health: {1}", name, health);
                        var textSize = font.MeasureString(text);

                        spriteBatch.DrawString(font, 
                                               text, new Vector2(graphics.PreferredBackBufferWidth - textSize.X - textOffset.X, 
                                                                 graphics.PreferredBackBufferHeight - textSize.Y - textOffset.Y),
                                               color);
                    }
                    
                    spriteBatch.Draw(sprites, pos, playerSrc, color, rot, orig, 1.0f, effects, 0.0f);
                }
            }
            else if (gameState == GameState.Disconnected)
            {
                var font = Content.Load<SpriteFont>("info log");

                var str = "disconnected from server, server shutdown...";
                var siz = font.MeasureString(str);
                var pos = new Vector2(graphics.PreferredBackBufferWidth / 2.0f - )

                spriteBatch.DrawString(font, str, pos, Color.Red);
            }

            spriteBatch.End();

            // Draw gameplay state...

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.Close();

            base.OnExiting(sender, args);
        }
    }
}
