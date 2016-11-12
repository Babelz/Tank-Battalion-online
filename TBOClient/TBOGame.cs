using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Sockets;
using TBOLib;

namespace TBOClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TBOGame : Game
    {
        #region Fields
        private readonly GraphicsDeviceManager graphics;

        private readonly IniFile config;

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

            config                              = new IniFile("cfg.ini");
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
            var address = config.Read("address", "server");
            var port    = int.Parse(config.Read("port", "server"));

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
                        case TBOLib.Packets.PacketType.Unknown:
                            break;
                        case TBOLib.Packets.PacketType.Ping:
                            break;
                        case TBOLib.Packets.PacketType.ClientJoined:
                            break;
                        case TBOLib.Packets.PacketType.ClientJoinedLobby:
                            break;
                        case TBOLib.Packets.PacketType.GameData:
                            break;
                        case TBOLib.Packets.PacketType.PlayerData:
                            break;
                        case TBOLib.Packets.PacketType.Input:
                            break;
                        case TBOLib.Packets.PacketType.MapData:
                            break;
                        case TBOLib.Packets.PacketType.GameStateSync:
                            break;
                        case TBOLib.Packets.PacketType.RoundStatus:
                            break;
                        case TBOLib.Packets.PacketType.GameStatus:
                            break;
                        case TBOLib.Packets.PacketType.Authentication:
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
