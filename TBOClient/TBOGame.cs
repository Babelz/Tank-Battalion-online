using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using TBOLib;
using TBOLib.Packets;

namespace TBOClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TBOGame : Game
    {
        #region Fields
        private readonly GraphicsDeviceManager graphics;
        
        private SpriteBatch spriteBatch;

        private GameInfoLog infoLog;

        private Client client;
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
        }
        #endregion

        private void InitializeClient()
        {
            client = new Client();

            client.Connected += Client_Connected;
        }

        private void Connect()
        {
            var address = Configuration.ServerAddress;
            var port    = Configuration.Port;

            infoLog.AddEntry(EntryType.Message, string.Format("connecting to {0}:{1}", address, port));

            client.Connect(address, port);
        }

        private void ProcessIncomingPackets()
        {
            if (client.HasIncomingPackets())
            {
                var packets = client.Receive();

                for (var i = 0; i < packets.Length; i++)
                {
                    var packet = packets[i];

                    switch (packet.Type)
                    {
                        case PacketType.Unknown:
                            break;
                        case PacketType.Ping:
                            break;
                        case PacketType.ClientJoined:
                            break;
                        case PacketType.ClientJoinedLobby:
                            break;
                        case PacketType.GameData:
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
                            var authentication = (AuthenticationPacket)packet;

                            authentication.response = string.Format("NAME:{0}", Configuration.Name);

                            client.Send(authentication);
                            break;
                        default:
                            break;
                    }
                }
            }
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
        }

        protected override void LoadContent()
        {
        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            ProcessIncomingPackets();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
